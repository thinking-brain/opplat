using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Opplat.Application.Abstractions.Options;
using Opplat.Domain.Entities.Administration;
using Opplat.Infrastructure.Persistance.Data.Administration;
using Opplat.Infrastructure.Services.Billing.Redsys;

namespace Opplat.UnitTest.Billing.Redsys;

public sealed class RedsysCardSetupServiceTests
{
    private const string SecretKeyBase64 = "Mk9m6qLKAxTKBQGb1RIkxsvCJcynv5oc";

    [Fact]
    public async Task InitiateCardSetupAsync_WritesPendingTransactionAndReturnsSignedForm()
    {
        await using var db = CreateDbContext();
        var plan = CreatePlan();
        var tenant = CreateTenant(plan.Id);
        db.SubscriptionPlans.Add(plan);
        db.Tenants.Add(tenant);
        await db.SaveChangesAsync();

        var service = CreateService(db);
        var redirect = await service.InitiateCardSetupAsync(tenant.Id, CancellationToken.None);

        Assert.Equal("https://sis-t.redsys.es:25443/sis/realizarPago", redirect.ActionUrl);
        Assert.Equal("HMAC_SHA256_V1", redirect.FormFields["Ds_SignatureVersion"]);
        Assert.True(redirect.FormFields.ContainsKey("Ds_MerchantParameters"));
        Assert.True(redirect.FormFields.ContainsKey("Ds_Signature"));

        var transaction = await db.RedsysTransactions.SingleAsync();
        Assert.Equal(RedsysTransactionKind.CardSetup, transaction.Kind);
        Assert.Equal(RedsysTransactionStatus.Pending, transaction.Status);
        Assert.Equal(tenant.Id, transaction.TenantId);
    }

    [Fact]
    public async Task HandleNotificationAsync_AuthorizedCardSetup_ActivatesTenantAndSavesPaymentMethod()
    {
        await using var db = CreateDbContext();
        var plan = CreatePlan();
        var tenant = CreateTenant(plan.Id);
        tenant.BillingStatus = TenantBillingStatus.Trialing;
        db.SubscriptionPlans.Add(plan);
        db.Tenants.Add(tenant);
        await db.SaveChangesAsync();

        var service = CreateService(db);
        var redirect = await service.InitiateCardSetupAsync(tenant.Id, CancellationToken.None);
        var order = DecodeOrder(redirect.FormFields["Ds_MerchantParameters"]);

        var notification = BuildAuthorizedNotification(order, "token-abc", "cof-123", "454881******0004", "visa");

        var result = await service.HandleNotificationAsync(notification, CancellationToken.None);

        Assert.True(result.Recognized);
        Assert.True(result.Authorized);

        var persistedTenant = await db.Tenants.SingleAsync();
        Assert.Equal(TenantBillingStatus.Active, persistedTenant.BillingStatus);
        Assert.Equal("token-abc", persistedTenant.StripeSubscriptionId);
        Assert.NotNull(persistedTenant.NextBillingDate);
        Assert.Equal(0, persistedTenant.RenewalRetryCount);

        var paymentMethod = await db.TenantPaymentMethods.SingleAsync();
        Assert.Equal("token-abc", paymentMethod.StripePaymentMethodId);
        Assert.Equal("0004", paymentMethod.Last4);
        Assert.True(paymentMethod.IsDefault);

        var transaction = await db.RedsysTransactions.SingleAsync();
        Assert.Equal(RedsysTransactionStatus.Authorized, transaction.Status);
        Assert.Equal("cof-123", transaction.CofTxnId);

        Assert.Single(await db.SubscriptionInvoices.ToListAsync());
    }

    [Fact]
    public async Task HandleNotificationAsync_InvalidSignature_IsRejectedAndTenantUntouched()
    {
        await using var db = CreateDbContext();
        var plan = CreatePlan();
        var tenant = CreateTenant(plan.Id);
        db.SubscriptionPlans.Add(plan);
        db.Tenants.Add(tenant);
        await db.SaveChangesAsync();

        var service = CreateService(db);
        var redirect = await service.InitiateCardSetupAsync(tenant.Id, CancellationToken.None);
        var order = DecodeOrder(redirect.FormFields["Ds_MerchantParameters"]);

        var builder = new RedsysMerchantParametersBuilder();
        var parameters = new Dictionary<string, string> { ["Ds_Order"] = order, ["Ds_Response"] = "0000" };
        var merchantParametersBase64 = builder.Build(parameters);

        var notification = new Dictionary<string, string>
        {
            ["Ds_MerchantParameters"] = merchantParametersBase64,
            ["Ds_Signature"] = "not-a-valid-signature"
        };

        var result = await service.HandleNotificationAsync(notification, CancellationToken.None);

        Assert.False(result.Authorized);
        var persistedTenant = await db.Tenants.SingleAsync();
        Assert.Equal(TenantBillingStatus.Trialing, persistedTenant.BillingStatus);
        Assert.Null(persistedTenant.StripeSubscriptionId);
    }

    private static IReadOnlyDictionary<string, string> BuildAuthorizedNotification(
        string order, string identifierToken, string cofTxnId, string maskedCard, string cardBrand)
    {
        var signatureService = new RedsysSignatureService();
        var builder = new RedsysMerchantParametersBuilder();

        var parameters = new Dictionary<string, string>
        {
            ["Ds_Order"] = order,
            ["Ds_Response"] = "0000",
            ["Ds_Merchant_Identifier"] = identifierToken,
            ["Ds_Merchant_Cof_Txnid"] = cofTxnId,
            ["Ds_Card_Number"] = maskedCard,
            ["Ds_Card_Brand"] = cardBrand
        };
        var merchantParametersBase64 = builder.Build(parameters);
        var signature = signatureService.ComputeSignature(SecretKeyBase64, order, merchantParametersBase64);

        return new Dictionary<string, string>
        {
            ["Ds_MerchantParameters"] = merchantParametersBase64,
            ["Ds_Signature"] = signature
        };
    }

    private static string DecodeOrder(string merchantParametersBase64)
    {
        var builder = new RedsysMerchantParametersBuilder();
        return builder.Decode(merchantParametersBase64)["Ds_Merchant_Order"];
    }

    private static RedsysCardSetupService CreateService(AdminTenantCatalogDbContext db) =>
        new(
            db,
            new RedsysOrderIdGenerator(db),
            new RedsysMerchantParametersBuilder(),
            new RedsysSignatureService(),
            Options.Create(new RedsysOptions
            {
                Enabled = true,
                MerchantCode = "999008881",
                Terminal = "001",
                SecretKeyBase64 = SecretKeyBase64,
                Environment = RedsysEnvironment.Test,
                MerchantUrl = "https://api.example.test/public/webhooks/redsys",
                UrlOk = "https://app.example.test/billing/card-setup/callback",
                UrlKo = "https://app.example.test/billing/card-setup/callback"
            }),
            NullLogger<RedsysCardSetupService>.Instance);

    private static AdminTenantCatalogDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AdminTenantCatalogDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AdminTenantCatalogDbContext(options);
    }

    private static SubscriptionPlan CreatePlan() => new()
    {
        Id = Guid.NewGuid(),
        Name = "Basic",
        PricingMonthly = 10,
        PricingAnnual = 100,
        Currency = "EUR",
        CreatedAt = DateTime.UtcNow,
        ModifiedAt = DateTime.UtcNow
    };

    private static Tenant CreateTenant(Guid planId) => new()
    {
        Id = Guid.NewGuid(),
        Identifier = "tenant-1",
        Name = "Tenant 1",
        DatabaseSchema = "tenant_1",
        DatabaseInstanceId = Guid.NewGuid(),
        SubscriptionPlanId = planId,
        BillingInterval = BillingInterval.Monthly,
        BillingStatus = TenantBillingStatus.Trialing,
        CreatedAt = DateTime.UtcNow,
        ModifiedAt = DateTime.UtcNow
    };
}
