using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Opplat.Application.Abstractions.Services;
using Opplat.Domain.Entities.Administration;
using Opplat.Infrastructure.Persistance.Data.Administration;
using Opplat.Infrastructure.Services.Billing;

namespace Opplat.UnitTest.Billing;

public sealed class SubscriptionRenewalWorkerTests
{
    [Fact]
    public async Task DueTenant_GeneratesInvoiceAndAdvancesBillingDate()
    {
        var oldPeriodEnd = DateTime.UtcNow.AddMinutes(-1);
        var newPeriodEnd = oldPeriodEnd.AddDays(30);
        var gateway = new TestPaymentGateway(new PaymentGatewaySubscriptionResult(
            true, "sub_1", TenantBillingStatus.Active, newPeriodEnd, "inv_2"));
        await using var provider = await CreateProviderAsync(gateway, CreateTenant(oldPeriodEnd), CreatePlan());

        await RunBatchAsync(provider);

        await using var scope = provider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AdminTenantCatalogDbContext>();
        var tenant = await db.Tenants.SingleAsync();
        var invoice = await db.SubscriptionInvoices.SingleAsync();
        Assert.Equal(newPeriodEnd, tenant.NextBillingDate);
        Assert.Equal(TenantBillingStatus.Active, tenant.BillingStatus);
        Assert.Equal("inv_2", invoice.StripeInvoiceId);
        Assert.Equal(oldPeriodEnd, invoice.PeriodStart);
        Assert.Equal(newPeriodEnd, invoice.PeriodEnd);
        Assert.Equal(SubscriptionInvoiceStatus.Paid, invoice.Status);
    }

    [Fact]
    public async Task TenantCancelledAtPeriodEnd_IsInactivatedWithoutRenewal()
    {
        var tenant = CreateTenant(DateTime.UtcNow.AddMinutes(-1));
        tenant.CancelAtPeriodEnd = true;
        var gateway = new TestPaymentGateway();
        await using var provider = await CreateProviderAsync(gateway, tenant, CreatePlan());

        await RunBatchAsync(provider);

        await using var scope = provider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AdminTenantCatalogDbContext>();
        var persistedTenant = await db.Tenants.SingleAsync();
        Assert.Equal(TenantBillingStatus.Cancelled, persistedTenant.BillingStatus);
        Assert.Equal(TenantStatus.Inactive, persistedTenant.Status);
        Assert.Empty(await db.SubscriptionInvoices.ToListAsync());
        Assert.False(gateway.RenewalCalled);
    }

    [Fact]
    public async Task RenewalFailure_MarksTenantPastDueWithoutInvoice()
    {
        var tenant = CreateTenant(DateTime.UtcNow.AddMinutes(-1));
        var gateway = new TestPaymentGateway(new PaymentGatewaySubscriptionResult(
            false, "sub_1", TenantBillingStatus.PastDue, Error: "card declined"));
        await using var provider = await CreateProviderAsync(gateway, tenant, CreatePlan());

        await RunBatchAsync(provider);

        await using var scope = provider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AdminTenantCatalogDbContext>();
        var persistedTenant = await db.Tenants.SingleAsync();
        Assert.Equal(TenantBillingStatus.PastDue, persistedTenant.BillingStatus);
        Assert.Empty(await db.SubscriptionInvoices.ToListAsync());
    }

    [Fact]
    public async Task TenantNotDue_IsLeftUntouched()
    {
        var tenant = CreateTenant(DateTime.UtcNow.AddHours(1));
        await using var provider = await CreateProviderAsync(new TestPaymentGateway(), tenant, CreatePlan());

        await RunBatchAsync(provider);

        await using var scope = provider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AdminTenantCatalogDbContext>();
        Assert.Empty(await db.SubscriptionInvoices.ToListAsync());
        Assert.False(provider.GetRequiredService<TestPaymentGateway>().RenewalCalled);
    }

    private static async Task<ServiceProvider> CreateProviderAsync(
        TestPaymentGateway gateway,
        Tenant tenant,
        SubscriptionPlan plan)
    {
        var databaseName = Guid.NewGuid().ToString();
        var services = new ServiceCollection();
        services.AddSingleton(gateway);
        services.AddScoped<IPaymentGatewayService>(sp => sp.GetRequiredService<TestPaymentGateway>());
        services.AddDbContext<AdminTenantCatalogDbContext>(options =>
            options.UseInMemoryDatabase(databaseName));
        var provider = services.BuildServiceProvider();
        await using (var scope = provider.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AdminTenantCatalogDbContext>();
            db.SubscriptionPlans.Add(plan);
            tenant.SubscriptionPlanId = plan.Id;
            db.Tenants.Add(tenant);
            await db.SaveChangesAsync();
        }
        return provider;
    }

    private static async Task RunBatchAsync(ServiceProvider provider)
    {
        var worker = new SubscriptionRenewalWorker(
            provider.GetRequiredService<IServiceScopeFactory>(),
            Options.Create(new SubscriptionRenewalWorkerOptions { BatchSize = 10 }),
            NullLogger<SubscriptionRenewalWorker>.Instance);
        await worker.ProcessBatchAsync(CancellationToken.None);
    }

    private static Tenant CreateTenant(DateTime nextBillingDate) => new()
    {
        Id = Guid.NewGuid(),
        Identifier = "tenant-1",
        Name = "Tenant 1",
        DatabaseSchema = "tenant_1",
        DatabaseInstanceId = Guid.NewGuid(),
        StripeSubscriptionId = "sub_1",
        BillingInterval = BillingInterval.Monthly,
        BillingStatus = TenantBillingStatus.Active,
        NextBillingDate = nextBillingDate,
        CreatedAt = DateTime.UtcNow,
        ModifiedAt = DateTime.UtcNow
    };

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

    private sealed class TestPaymentGateway(PaymentGatewaySubscriptionResult? renewalResult = null) : IPaymentGatewayService
    {
        private readonly PaymentGatewaySubscriptionResult _renewalResult = renewalResult
            ?? new PaymentGatewaySubscriptionResult(false, Error: "not configured");

        public bool RenewalCalled { get; private set; }

        public Task<PaymentGatewaySubscriptionResult> RenewSubscriptionAsync(string subscriptionId, BillingInterval interval, CancellationToken cancellationToken = default)
        {
            RenewalCalled = true;
            return Task.FromResult(_renewalResult);
        }

        public Task<PaymentGatewayCardResult> CreatePaymentMethodAsync(string cardNumber, int expMonth, int expYear, string cvc, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<PaymentGatewayResult> CreateCustomerAsync(string email, string businessName, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<PaymentGatewayResult> AttachPaymentMethodAsync(string customerId, string paymentMethodId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<PaymentGatewaySubscriptionResult> CreateSubscriptionAsync(string customerId, string priceId, BillingInterval interval, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<PaymentGatewayResult> CancelSubscriptionAsync(string subscriptionId, bool atPeriodEnd, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<PaymentGatewayResult> ChangeSubscriptionPriceAsync(string subscriptionId, string priceId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<PaymentGatewayPortalResult> CreateBillingPortalSessionAsync(string customerId, string returnUrl, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<byte[]?> DownloadInvoicePdfAsync(string invoiceId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }
}
