using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Opplat.Application.Abstractions.Options;
using Opplat.Application.Abstractions.Services;
using Opplat.Domain.Entities.Administration;
using Opplat.Infrastructure.Persistance.Data.Administration;

namespace Opplat.Infrastructure.Services.Billing.Redsys;

/// <summary>
/// Implements the CIT (card-setup) redirect flow: builds the signed hosted-page form for the
/// browser to auto-submit, and processes the resulting asynchronous notification (webhook).
/// Recurring MIT charges are handled separately by <see cref="RedsysPaymentGatewayService"/>
/// since they are synchronous REST calls, not redirect-based.
/// </summary>
public sealed class RedsysCardSetupService(
    AdminTenantCatalogDbContext dbContext,
    RedsysOrderIdGenerator orderIdGenerator,
    RedsysMerchantParametersBuilder parametersBuilder,
    RedsysSignatureService signatureService,
    IOptions<RedsysOptions> options,
    ILogger<RedsysCardSetupService> logger) : IRedsysCardSetupService
{
    private readonly RedsysOptions _options = options.Value;

    public async Task<RedsysCardSetupRedirect> InitiateCardSetupAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var tenant = await dbContext.Tenants
            .Include(t => t.SubscriptionPlan)
            .FirstOrDefaultAsync(t => t.Id == tenantId, cancellationToken)
            ?? throw new InvalidOperationException($"Tenant '{tenantId}' was not found.");

        var plan = tenant.SubscriptionPlan
            ?? throw new InvalidOperationException($"Tenant '{tenantId}' has no subscription plan.");

        var amount = tenant.BillingInterval == BillingInterval.Annual ? plan.PricingAnnual : plan.PricingMonthly;
        var amountMinorUnits = RedsysMerchantParametersBuilder.FormatAmountMinorUnits(amount);
        var order = await orderIdGenerator.GenerateAsync(cancellationToken);

        var fields = new Dictionary<string, string>
        {
            ["Ds_Merchant_MerchantCode"] = _options.MerchantCode,
            ["Ds_Merchant_Terminal"] = _options.Terminal,
            ["Ds_Merchant_Order"] = order,
            ["Ds_Merchant_Amount"] = amountMinorUnits,
            ["Ds_Merchant_Currency"] = RedsysMerchantParametersBuilder.CurrencyNumericCode(plan.Currency),
            ["Ds_Merchant_TransactionType"] = "0",
            ["Ds_Merchant_MerchantURL"] = _options.MerchantUrl,
            ["Ds_Merchant_UrlOK"] = _options.UrlOk,
            ["Ds_Merchant_UrlKO"] = _options.UrlKo,
            ["Ds_Merchant_Identifier"] = "REQUIRED",
            ["Ds_Merchant_Cof_Ini"] = "S",
            ["Ds_Merchant_Cof_Type"] = "R",
            ["Ds_Merchant_ProductDescription"] = $"Opplat subscription ({plan.Name})"
        };

        var merchantParametersBase64 = parametersBuilder.Build(fields);
        var signature = signatureService.ComputeSignature(_options.SecretKeyBase64, order, merchantParametersBase64);

        dbContext.RedsysTransactions.Add(new RedsysTransaction
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Order = order,
            Kind = RedsysTransactionKind.CardSetup,
            Status = RedsysTransactionStatus.Pending,
            AmountCents = long.Parse(amountMinorUnits),
            Currency = plan.Currency,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow
        });
        await dbContext.SaveChangesAsync(cancellationToken);

        return new RedsysCardSetupRedirect(
            _options.HostedPaymentPageUrl,
            new Dictionary<string, string>
            {
                ["Ds_SignatureVersion"] = "HMAC_SHA256_V1",
                ["Ds_MerchantParameters"] = merchantParametersBase64,
                ["Ds_Signature"] = signature
            });
    }

    public async Task<RedsysNotificationResult> HandleNotificationAsync(
        IReadOnlyDictionary<string, string> notificationFields,
        CancellationToken cancellationToken = default)
    {
        if (!notificationFields.TryGetValue("Ds_MerchantParameters", out var merchantParametersBase64) ||
            !notificationFields.TryGetValue("Ds_Signature", out var receivedSignature))
        {
            logger.LogWarning("Redsys notification is missing Ds_MerchantParameters/Ds_Signature.");
            return new RedsysNotificationResult(false, false, null, "Missing required fields.");
        }

        var parameters = parametersBuilder.Decode(merchantParametersBase64);
        if (!parameters.TryGetValue("Ds_Order", out var order))
            return new RedsysNotificationResult(false, false, null, "Missing Ds_Order.");

        if (!signatureService.VerifySignature(_options.SecretKeyBase64, order, merchantParametersBase64, receivedSignature))
        {
            logger.LogWarning("Redsys notification signature verification failed for order {Order}.", order);
            return new RedsysNotificationResult(false, false, order, "Invalid signature.");
        }

        var transaction = await dbContext.RedsysTransactions.FirstOrDefaultAsync(t => t.Order == order, cancellationToken);
        if (transaction is null)
        {
            logger.LogWarning("Redsys notification references unknown order {Order}.", order);
            return new RedsysNotificationResult(false, false, order, "Unknown order.");
        }

        parameters.TryGetValue("Ds_Response", out var responseCode);
        var authorized = IsAuthorized(responseCode);

        transaction.ResponseCode = responseCode;
        transaction.RawResponsePayload = merchantParametersBase64;
        transaction.Status = authorized ? RedsysTransactionStatus.Authorized : RedsysTransactionStatus.Declined;

        if (authorized && transaction.Kind == RedsysTransactionKind.CardSetup)
            await ActivateTenantAfterCardSetupAsync(transaction, order, parameters, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
        return new RedsysNotificationResult(
            true, authorized, order, authorized ? null : $"Redsys declined (Ds_Response={responseCode}).");
    }

    private async Task ActivateTenantAfterCardSetupAsync(
        RedsysTransaction transaction,
        string order,
        IReadOnlyDictionary<string, string> parameters,
        CancellationToken cancellationToken)
    {
        parameters.TryGetValue("Ds_Merchant_Identifier", out var identifierToken);
        parameters.TryGetValue("Ds_Merchant_Cof_Txnid", out var cofTxnId);
        parameters.TryGetValue("Ds_Card_Number", out var maskedCard);
        parameters.TryGetValue("Ds_Card_Brand", out var cardBrand);

        transaction.IdentifierToken = identifierToken;
        transaction.CofTxnId = cofTxnId;

        if (string.IsNullOrWhiteSpace(identifierToken))
            return;

        var tenant = await dbContext.Tenants.FirstOrDefaultAsync(t => t.Id == transaction.TenantId, cancellationToken);
        if (tenant is null)
            return;

        var paymentMethod = await dbContext.TenantPaymentMethods
            .FirstOrDefaultAsync(p => p.TenantId == tenant.Id && p.IsDefault, cancellationToken);
        var last4 = string.IsNullOrWhiteSpace(maskedCard) || maskedCard.Length < 4 ? null : maskedCard[^4..];

        if (paymentMethod is null)
        {
            dbContext.TenantPaymentMethods.Add(new TenantPaymentMethod
            {
                Id = Guid.NewGuid(),
                TenantId = tenant.Id,
                StripePaymentMethodId = identifierToken,
                Brand = cardBrand,
                Last4 = last4,
                IsDefault = true,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            });
        }
        else
        {
            paymentMethod.StripePaymentMethodId = identifierToken;
            paymentMethod.Brand = cardBrand;
            paymentMethod.Last4 = last4;
            paymentMethod.ModifiedAt = DateTime.UtcNow;
        }

        var periodEnd = DateTime.UtcNow.Add(
            tenant.BillingInterval == BillingInterval.Annual ? TimeSpan.FromDays(365) : TimeSpan.FromDays(30));

        tenant.StripeSubscriptionId = identifierToken;
        tenant.BillingStatus = TenantBillingStatus.Active;
        tenant.RenewalRetryCount = 0;
        tenant.NextBillingDate = periodEnd;
        tenant.ModifiedAt = DateTime.UtcNow;

        dbContext.SubscriptionInvoices.Add(new SubscriptionInvoice
        {
            Id = Guid.NewGuid(),
            TenantId = tenant.Id,
            StripeInvoiceId = $"redsys_{order}",
            AmountDue = transaction.AmountCents / 100m,
            Currency = transaction.Currency,
            Status = SubscriptionInvoiceStatus.Paid,
            PeriodStart = DateTime.UtcNow,
            PeriodEnd = periodEnd,
            PaidAt = DateTime.UtcNow,
            HostedInvoiceUrl = $"/billing/invoices/redsys_{order}/pdf",
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow
        });
    }

    private static bool IsAuthorized(string? responseCode) =>
        int.TryParse(responseCode, out var code) && code is >= 0 and <= 99;
}
