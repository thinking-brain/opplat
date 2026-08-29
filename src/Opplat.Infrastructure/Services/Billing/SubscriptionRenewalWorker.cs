using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Opplat.Application.Abstractions.Services;
using Opplat.Domain.Entities.Administration;
using Opplat.Infrastructure.Persistance.Data.Administration;

namespace Opplat.Infrastructure.Services.Billing;

/// <summary>
/// Polls tenants whose billing period has ended and renews their subscription,
/// recording a new <see cref="SubscriptionInvoice"/> for each successful charge
/// so that the "download invoice" flow has something to serve for recurring payments.
/// </summary>
public sealed class SubscriptionRenewalWorker(
    IServiceScopeFactory scopeFactory,
    IOptions<SubscriptionRenewalWorkerOptions> options,
    ILogger<SubscriptionRenewalWorker> logger)
    : BackgroundService
{
    private readonly SubscriptionRenewalWorkerOptions _opts = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation(
            "SubscriptionRenewalWorker started (interval={Interval}, batch={Batch})",
            _opts.Interval, _opts.BatchSize);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessBatchAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled error in SubscriptionRenewalWorker tick");
            }

            await Task.Delay(_opts.Interval, stoppingToken);
        }

        logger.LogInformation("SubscriptionRenewalWorker stopped");
    }

    internal async Task ProcessBatchAsync(CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AdminTenantCatalogDbContext>();
        var paymentGatewayService = scope.ServiceProvider.GetRequiredService<IPaymentGatewayService>();

        var now = DateTime.UtcNow;
        var dueTenants = await db.Tenants
            .Where(tenant => tenant.NextBillingDate != null
                && tenant.NextBillingDate <= now
                && tenant.BillingStatus != TenantBillingStatus.Cancelled)
            .Take(_opts.BatchSize)
            .ToListAsync(cancellationToken);

        foreach (var tenant in dueTenants)
        {
            try
            {
                await ProcessTenantAsync(tenant, db, paymentGatewayService, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process subscription renewal for tenant {Identifier}", tenant.Identifier);
            }
        }
    }

    internal async Task ProcessTenantAsync(
        Tenant tenant,
        AdminTenantCatalogDbContext db,
        IPaymentGatewayService paymentGatewayService,
        CancellationToken cancellationToken)
    {
        if (tenant.CancelAtPeriodEnd)
        {
            tenant.BillingStatus = TenantBillingStatus.Cancelled;
            tenant.Status = TenantStatus.Inactive;
            tenant.InactivatedAt = DateTime.UtcNow;
            tenant.ModifiedAt = DateTime.UtcNow;
            await db.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Tenant {Identifier} subscription cancelled at period end", tenant.Identifier);
            return;
        }

        if (string.IsNullOrWhiteSpace(tenant.StripeSubscriptionId))
        {
            logger.LogWarning("Tenant {Identifier} is due for renewal but has no subscription configured; skipping.", tenant.Identifier);
            return;
        }

        var result = await paymentGatewayService.RenewSubscriptionAsync(
            tenant.StripeSubscriptionId, tenant.BillingInterval, cancellationToken);

        if (!result.Succeeded || string.IsNullOrWhiteSpace(result.InvoiceId))
        {
            tenant.BillingStatus = TenantBillingStatus.PastDue;
            tenant.ModifiedAt = DateTime.UtcNow;
            await db.SaveChangesAsync(cancellationToken);
            logger.LogWarning("Subscription renewal failed for tenant {Identifier}: {Error}", tenant.Identifier, result.Error);
            return;
        }

        var plan = await db.SubscriptionPlans
            .Where(p => p.Id == tenant.SubscriptionPlanId)
            .Select(p => new { p.PricingMonthly, p.PricingAnnual, p.Currency })
            .FirstOrDefaultAsync(cancellationToken);

        var periodStart = tenant.NextBillingDate!.Value;
        var periodEnd = result.CurrentPeriodEnd
            ?? periodStart.Add(tenant.BillingInterval == BillingInterval.Annual ? TimeSpan.FromDays(365) : TimeSpan.FromDays(30));

        db.SubscriptionInvoices.Add(new SubscriptionInvoice
        {
            Id = Guid.NewGuid(),
            TenantId = tenant.Id,
            StripeInvoiceId = result.InvoiceId,
            AmountDue = tenant.BillingInterval == BillingInterval.Annual ? plan?.PricingAnnual ?? 0 : plan?.PricingMonthly ?? 0,
            Currency = plan?.Currency ?? "EUR",
            Status = SubscriptionInvoiceStatus.Paid,
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            PaidAt = DateTime.UtcNow,
            HostedInvoiceUrl = $"/billing/invoices/{result.InvoiceId}/pdf",
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow
        });

        tenant.NextBillingDate = periodEnd;
        tenant.BillingStatus = TenantBillingStatus.Active;
        tenant.ModifiedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Renewed subscription for tenant {Identifier}, new invoice {InvoiceId}", tenant.Identifier, result.InvoiceId);
    }
}
