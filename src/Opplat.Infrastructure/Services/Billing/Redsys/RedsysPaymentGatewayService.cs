using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Opplat.Application.Abstractions.Options;
using Opplat.Application.Abstractions.Services;
using Opplat.Domain.Entities.Administration;
using Opplat.Infrastructure.Persistance.Data.Administration;

namespace Opplat.Infrastructure.Services.Billing.Redsys;

/// <summary>
/// Real Redsys implementation of <see cref="IPaymentGatewayService"/>. Card tokenization
/// (CreatePaymentMethodAsync/AttachPaymentMethodAsync) is not supported here because Redsys
/// requires a browser redirect for that — use <see cref="IRedsysCardSetupService"/> instead.
/// Only <see cref="RenewSubscriptionAsync"/> makes a real (synchronous MIT REST) network call;
/// the other methods are local bookkeeping since Redsys has no customer/subscription objects.
/// </summary>
public sealed class RedsysPaymentGatewayService(
    AdminTenantCatalogDbContext dbContext,
    RedsysApiClient apiClient,
    RedsysOrderIdGenerator orderIdGenerator,
    RedsysMerchantParametersBuilder parametersBuilder,
    RedsysSignatureService signatureService,
    IOptions<RedsysOptions> redsysOptions,
    IOptions<OpplatLegalDataOptions> legalDataOptions,
    ILogger<RedsysPaymentGatewayService> logger) : IPaymentGatewayService
{
    private readonly RedsysOptions _options = redsysOptions.Value;
    private readonly OpplatLegalDataOptions _legalDataOptions = legalDataOptions.Value;

    public Task<PaymentGatewayCardResult> CreatePaymentMethodAsync(
        string cardNumber, int expMonth, int expYear, string cvc, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException(
            "Redsys requires the card-setup redirect flow; use IRedsysCardSetupService.InitiateCardSetupAsync instead of submitting raw card details.");

    public Task<PaymentGatewayResult> AttachPaymentMethodAsync(
        string customerId, string paymentMethodId, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException(
            "Redsys requires the card-setup redirect flow; use IRedsysCardSetupService.InitiateCardSetupAsync instead.");

    public Task<PaymentGatewayResult> CreateCustomerAsync(
        string email, string businessName, CancellationToken cancellationToken = default) =>
        Task.FromResult(new PaymentGatewayResult(true, $"local_customer_{Guid.NewGuid():N}"));

    public Task<PaymentGatewaySubscriptionResult> CreateSubscriptionAsync(
        string customerId, string priceId, BillingInterval interval, CancellationToken cancellationToken = default) =>
        Task.FromResult(new PaymentGatewaySubscriptionResult(true, null, TenantBillingStatus.Trialing, null, null));

    public Task<PaymentGatewayResult> CancelSubscriptionAsync(
        string subscriptionId, bool atPeriodEnd, CancellationToken cancellationToken = default) =>
        Task.FromResult(new PaymentGatewayResult(true, subscriptionId));

    public Task<PaymentGatewayResult> ChangeSubscriptionPriceAsync(
        string subscriptionId, string priceId, CancellationToken cancellationToken = default) =>
        Task.FromResult(new PaymentGatewayResult(true, subscriptionId));

    public Task<PaymentGatewayPortalResult> CreateBillingPortalSessionAsync(
        string customerId, string returnUrl, CancellationToken cancellationToken = default) =>
        Task.FromResult(new PaymentGatewayPortalResult(false, Error: "Redsys does not provide a hosted billing portal."));

    public async Task<byte[]?> DownloadInvoicePdfAsync(string invoiceId, CancellationToken cancellationToken = default)
    {
        var invoice = await dbContext.SubscriptionInvoices
            .AsNoTracking()
            .Include(item => item.Tenant)
            .FirstOrDefaultAsync(item => item.StripeInvoiceId == invoiceId, cancellationToken);

        if (invoice is null)
            return null;

        return SubscriptionInvoicePdfRenderer.Render(
            invoiceId,
            invoice.Tenant?.Name ?? "Opplat Tenant",
            invoice.Tenant?.Identifier ?? "tenant",
            invoice.PeriodStart,
            invoice.PeriodEnd,
            invoice.PaidAt ?? invoice.PeriodEnd,
            invoice.AmountDue,
            invoice.Currency,
            _legalDataOptions);
    }

    public async Task<PaymentGatewaySubscriptionResult> RenewSubscriptionAsync(
        string subscriptionId, BillingInterval interval, CancellationToken cancellationToken = default)
    {
        var tenant = await dbContext.Tenants
            .Include(t => t.SubscriptionPlan)
            .FirstOrDefaultAsync(t => t.StripeSubscriptionId == subscriptionId, cancellationToken);

        if (tenant?.SubscriptionPlan is null)
        {
            logger.LogWarning("RenewSubscriptionAsync: no tenant found for Redsys token {Token}.", subscriptionId);
            return new PaymentGatewaySubscriptionResult(false, subscriptionId, Error: "Tenant/subscription not found.");
        }

        var lastTransaction = await dbContext.RedsysTransactions
            .Where(t => t.TenantId == tenant.Id && t.Status == RedsysTransactionStatus.Authorized)
            .OrderByDescending(t => t.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (lastTransaction?.CofTxnId is null)
        {
            logger.LogWarning(
                "RenewSubscriptionAsync: tenant {Identifier} has no prior authorized COF transaction to chain from.",
                tenant.Identifier);
            return new PaymentGatewaySubscriptionResult(false, subscriptionId, Error: "No prior COF transaction to chain from.");
        }

        var amount = interval == BillingInterval.Annual ? tenant.SubscriptionPlan.PricingAnnual : tenant.SubscriptionPlan.PricingMonthly;
        var amountMinorUnits = RedsysMerchantParametersBuilder.FormatAmountMinorUnits(amount);
        var order = await orderIdGenerator.GenerateAsync(cancellationToken);

        var fields = new Dictionary<string, string>
        {
            ["Ds_Merchant_MerchantCode"] = _options.MerchantCode,
            ["Ds_Merchant_Terminal"] = _options.Terminal,
            ["Ds_Merchant_Order"] = order,
            ["Ds_Merchant_Amount"] = amountMinorUnits,
            ["Ds_Merchant_Currency"] = RedsysMerchantParametersBuilder.CurrencyNumericCode(tenant.SubscriptionPlan.Currency),
            ["Ds_Merchant_TransactionType"] = "0",
            ["Ds_Merchant_Identifier"] = subscriptionId,
            ["Ds_Merchant_Cof_Ini"] = "N",
            ["Ds_Merchant_Cof_Type"] = "R",
            ["Ds_Merchant_Cof_Txnid"] = lastTransaction.CofTxnId
        };

        var merchantParametersBase64 = parametersBuilder.Build(fields);
        var signature = signatureService.ComputeSignature(_options.SecretKeyBase64, order, merchantParametersBase64);

        var pendingTransaction = new RedsysTransaction
        {
            Id = Guid.NewGuid(),
            TenantId = tenant.Id,
            Order = order,
            Kind = RedsysTransactionKind.Recurring,
            Status = RedsysTransactionStatus.Pending,
            AmountCents = long.Parse(amountMinorUnits),
            Currency = tenant.SubscriptionPlan.Currency,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow
        };
        dbContext.RedsysTransactions.Add(pendingTransaction);
        await dbContext.SaveChangesAsync(cancellationToken);

        var result = await apiClient.PostRestTransactionAsync(merchantParametersBase64, signature, cancellationToken);
        var responseCode = result.MerchantParameters?.GetValueOrDefault("Ds_Response");
        var authorized = result.TransportSucceeded && IsAuthorized(responseCode);

        pendingTransaction.Status = authorized ? RedsysTransactionStatus.Authorized : RedsysTransactionStatus.Declined;
        pendingTransaction.ResponseCode = responseCode ?? result.ErrorCode;
        pendingTransaction.RawResponsePayload = result.RawResponse;
        if (authorized)
            pendingTransaction.CofTxnId = result.MerchantParameters?.GetValueOrDefault("Ds_Merchant_Cof_Txnid");
        await dbContext.SaveChangesAsync(cancellationToken);

        if (!authorized)
        {
            return new PaymentGatewaySubscriptionResult(
                false, subscriptionId, tenant.BillingStatus,
                Error: $"Redsys declined the recurring charge (Ds_Response={responseCode ?? result.ErrorCode}).");
        }

        var periodEnd = DateTime.UtcNow.Add(interval == BillingInterval.Annual ? TimeSpan.FromDays(365) : TimeSpan.FromDays(30));
        return new PaymentGatewaySubscriptionResult(true, subscriptionId, TenantBillingStatus.Active, periodEnd, $"redsys_{order}");
    }

    private static bool IsAuthorized(string? responseCode) =>
        int.TryParse(responseCode, out var code) && code is >= 0 and <= 99;
}
