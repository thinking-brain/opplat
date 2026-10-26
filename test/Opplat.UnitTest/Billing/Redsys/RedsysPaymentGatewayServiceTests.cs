using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Opplat.Application.Abstractions.Options;
using Opplat.Domain.Entities.Administration;
using Opplat.Infrastructure.Persistance.Data.Administration;
using Opplat.Infrastructure.Services.Billing.Redsys;

namespace Opplat.UnitTest.Billing.Redsys;

public sealed class RedsysPaymentGatewayServiceTests
{
    private const string SecretKeyBase64 = "Mk9m6qLKAxTKBQGb1RIkxsvCJcynv5oc";

    [Fact]
    public async Task RenewSubscriptionAsync_NoPriorAuthorizedTransaction_FailsWithoutCallingRedsys()
    {
        await using var db = CreateDbContext();
        var plan = CreatePlan();
        var tenant = CreateTenant(plan.Id, "token-abc");
        db.SubscriptionPlans.Add(plan);
        db.Tenants.Add(tenant);
        await db.SaveChangesAsync();

        var callCount = 0;
        var service = CreateService(db, _ => { callCount++; return SuccessResponse("0000", "cof-2"); });

        var result = await service.RenewSubscriptionAsync("token-abc", BillingInterval.Monthly, CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal(0, callCount);
        Assert.Empty(await db.RedsysTransactions.ToListAsync());
    }

    [Fact]
    public async Task RenewSubscriptionAsync_Authorized_ChainsCofTxnIdAndAdvancesPeriod()
    {
        await using var db = CreateDbContext();
        var plan = CreatePlan();
        var tenant = CreateTenant(plan.Id, "token-abc");
        db.SubscriptionPlans.Add(plan);
        db.Tenants.Add(tenant);
        db.RedsysTransactions.Add(new RedsysTransaction
        {
            Id = Guid.NewGuid(),
            TenantId = tenant.Id,
            Order = "000000000001",
            Kind = RedsysTransactionKind.CardSetup,
            Status = RedsysTransactionStatus.Authorized,
            CofTxnId = "cof-1",
            AmountCents = 1000,
            Currency = "EUR",
            CreatedAt = DateTime.UtcNow.AddDays(-30),
            ModifiedAt = DateTime.UtcNow.AddDays(-30)
        });
        await db.SaveChangesAsync();

        var service = CreateService(db, _ => SuccessResponse("0000", "cof-2"));

        var result = await service.RenewSubscriptionAsync("token-abc", BillingInterval.Monthly, CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Equal(TenantBillingStatus.Active, result.BillingStatus);
        Assert.NotNull(result.CurrentPeriodEnd);

        var newTransaction = await db.RedsysTransactions
            .SingleAsync(t => t.Kind == RedsysTransactionKind.Recurring);
        Assert.Equal(RedsysTransactionStatus.Authorized, newTransaction.Status);
        Assert.Equal("cof-2", newTransaction.CofTxnId);
    }

    [Fact]
    public async Task RenewSubscriptionAsync_Declined_ReturnsFailureAndMarksTransactionDeclined()
    {
        await using var db = CreateDbContext();
        var plan = CreatePlan();
        var tenant = CreateTenant(plan.Id, "token-abc");
        db.SubscriptionPlans.Add(plan);
        db.Tenants.Add(tenant);
        db.RedsysTransactions.Add(new RedsysTransaction
        {
            Id = Guid.NewGuid(),
            TenantId = tenant.Id,
            Order = "000000000001",
            Kind = RedsysTransactionKind.CardSetup,
            Status = RedsysTransactionStatus.Authorized,
            CofTxnId = "cof-1",
            AmountCents = 1000,
            Currency = "EUR",
            CreatedAt = DateTime.UtcNow.AddDays(-30),
            ModifiedAt = DateTime.UtcNow.AddDays(-30)
        });
        await db.SaveChangesAsync();

        var service = CreateService(db, _ => SuccessResponse("0180", null));

        var result = await service.RenewSubscriptionAsync("token-abc", BillingInterval.Monthly, CancellationToken.None);

        Assert.False(result.Succeeded);
        var newTransaction = await db.RedsysTransactions
            .SingleAsync(t => t.Kind == RedsysTransactionKind.Recurring);
        Assert.Equal(RedsysTransactionStatus.Declined, newTransaction.Status);
    }

    private static HttpResponseMessage SuccessResponse(string dsResponse, string? cofTxnId)
    {
        var builder = new RedsysMerchantParametersBuilder();
        var responseParameters = new Dictionary<string, string> { ["Ds_Response"] = dsResponse };
        if (cofTxnId is not null)
            responseParameters["Ds_Merchant_Cof_Txnid"] = cofTxnId;

        var payload = new
        {
            Ds_SignatureVersion = "HMAC_SHA256_V1",
            Ds_MerchantParameters = builder.Build(responseParameters),
            Ds_Signature = "irrelevant-for-outbound-response"
        };

        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            // Ds_* fields are case-sensitive; disable camelCase to match a real Redsys response.
            Content = JsonContent.Create(payload, options: new JsonSerializerOptions { PropertyNamingPolicy = null })
        };
    }

    private static RedsysPaymentGatewayService CreateService(
        AdminTenantCatalogDbContext db,
        Func<HttpRequestMessage, HttpResponseMessage> responder)
    {
        var httpClient = new HttpClient(new FakeHttpMessageHandler(responder));
        var options = Options.Create(new RedsysOptions
        {
            Enabled = true,
            MerchantCode = "999008881",
            Terminal = "001",
            SecretKeyBase64 = SecretKeyBase64,
            Environment = RedsysEnvironment.Test
        });

        return new RedsysPaymentGatewayService(
            db,
            new RedsysApiClient(httpClient, options),
            new RedsysOrderIdGenerator(db),
            new RedsysMerchantParametersBuilder(),
            new RedsysSignatureService(),
            options,
            Options.Create(new OpplatLegalDataOptions()),
            NullLogger<RedsysPaymentGatewayService>.Instance);
    }

    private static AdminTenantCatalogDbContext CreateDbContext()
    {
        var dbOptions = new DbContextOptionsBuilder<AdminTenantCatalogDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AdminTenantCatalogDbContext(dbOptions);
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

    private static Tenant CreateTenant(Guid planId, string redsysToken) => new()
    {
        Id = Guid.NewGuid(),
        Identifier = "tenant-1",
        Name = "Tenant 1",
        DatabaseSchema = "tenant_1",
        DatabaseInstanceId = Guid.NewGuid(),
        SubscriptionPlanId = planId,
        BillingInterval = BillingInterval.Monthly,
        BillingStatus = TenantBillingStatus.Active,
        StripeSubscriptionId = redsysToken,
        NextBillingDate = DateTime.UtcNow.AddMinutes(-1),
        CreatedAt = DateTime.UtcNow,
        ModifiedAt = DateTime.UtcNow
    };

    private sealed class FakeHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responder) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(responder(request));
    }
}
