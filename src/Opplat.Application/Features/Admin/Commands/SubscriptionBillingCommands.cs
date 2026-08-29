using Microsoft.EntityFrameworkCore;
using Opplat.Application.Abstractions.Invoicing;
using Opplat.Application.Abstractions.Messaging;
using Opplat.Application.Abstractions.Services;
using Opplat.Application.Dtos;
using Opplat.Domain.Entities.Administration;
using Opplat.Infrastructure.Persistance.Data.Administration;

namespace Opplat.Application.Features.Admin.Commands;

public sealed record CancelSubscriptionCommand(string TenantIdentifier) : IRequest;
public sealed record CreateBillingPortalSessionCommand(string TenantIdentifier, string ReturnUrl) : IRequest<PaymentGatewayPortalResult>;
public sealed record GetSubscriptionDetailsQuery(string TenantIdentifier) : IRequest<TenantSubscriptionDetailsDto>;
public sealed record ChangeSubscriptionPlanCommand(string TenantIdentifier, Guid SubscriptionPlanId, string BillingInterval) : IRequest<TenantSubscriptionDetailsDto>;
public sealed record GetSubscriptionPaymentHistoryQuery(string TenantIdentifier) : IRequest<IReadOnlyList<SubscriptionPaymentHistoryItem>>;
public sealed record DownloadSubscriptionInvoiceCommand(string TenantIdentifier, string InvoiceId) : IRequest<byte[]?>;
public sealed record SendSubscriptionInvoiceEmailCommand(string TenantIdentifier, string InvoiceId) : IRequest;

public sealed class CancelSubscriptionCommandHandler(
    AdminTenantCatalogDbContext db,
    IPaymentGatewayService paymentGatewayService) : IRequestHandler<CancelSubscriptionCommand>
{
    public async Task Handle(CancelSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var identifier = request.TenantIdentifier.Trim().ToLowerInvariant();
        var tenant = await db.Tenants.FirstOrDefaultAsync(t => t.Identifier == identifier, cancellationToken)
            ?? throw new KeyNotFoundException($"Tenant '{identifier}' was not found.");

        if (tenant.CancelAtPeriodEnd)
            return;

        if (!string.IsNullOrWhiteSpace(tenant.StripeSubscriptionId))
        {
            var result = await paymentGatewayService.CancelSubscriptionAsync(
                tenant.StripeSubscriptionId,
                atPeriodEnd: true,
                cancellationToken);
            if (!result.Succeeded)
                throw new InvalidOperationException(result.Error ?? "Failed to cancel the subscription.");
        }

        tenant.CancelAtPeriodEnd = true;
        tenant.ModifiedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(cancellationToken);
    }
}

public sealed class CreateBillingPortalSessionCommandHandler(
    AdminTenantCatalogDbContext db,
    IPaymentGatewayService paymentGatewayService) : IRequestHandler<CreateBillingPortalSessionCommand, PaymentGatewayPortalResult>
{
    public async Task<PaymentGatewayPortalResult> Handle(CreateBillingPortalSessionCommand request, CancellationToken cancellationToken)
    {
        var identifier = request.TenantIdentifier.Trim().ToLowerInvariant();
        var customerId = await db.Tenants
            .Where(t => t.Identifier == identifier)
            .Select(t => t.StripeCustomerId)
            .FirstOrDefaultAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(customerId))
            throw new InvalidOperationException("This tenant has no payment customer configured.");

        return await paymentGatewayService.CreateBillingPortalSessionAsync(
            customerId,
            request.ReturnUrl,
            cancellationToken);
    }
}

public sealed class GetSubscriptionDetailsQueryHandler(AdminTenantCatalogDbContext db)
    : IRequestHandler<GetSubscriptionDetailsQuery, TenantSubscriptionDetailsDto>
{
    public async Task<TenantSubscriptionDetailsDto> Handle(GetSubscriptionDetailsQuery request, CancellationToken cancellationToken)
    {
        var identifier = request.TenantIdentifier.Trim().ToLowerInvariant();
        var tenant = await db.Tenants
            .AsNoTracking()
            .Include(item => item.SubscriptionPlan)
            .FirstOrDefaultAsync(item => item.Identifier == identifier, cancellationToken)
            ?? throw new KeyNotFoundException($"Tenant '{identifier}' was not found.");

        return new TenantSubscriptionDetailsDto
        {
            TenantIdentifier = tenant.Identifier,
            SubscriptionPlanId = tenant.SubscriptionPlanId,
            SubscriptionPlanName = tenant.SubscriptionPlan?.Name ?? "Sin plan",
            BillingInterval = tenant.BillingInterval.ToString(),
            PricingMonthly = tenant.SubscriptionPlan?.PricingMonthly ?? 0,
            PricingAnnual = tenant.SubscriptionPlan?.PricingAnnual ?? 0,
            Currency = tenant.SubscriptionPlan?.Currency ?? "EUR",
            BillingStatus = tenant.BillingStatus.ToString(),
            NextBillingDate = tenant.NextBillingDate,
            CancelAtPeriodEnd = tenant.CancelAtPeriodEnd
        };
    }
}

public sealed class ChangeSubscriptionPlanCommandHandler(
    AdminTenantCatalogDbContext db,
    IPaymentGatewayService paymentGatewayService)
    : IRequestHandler<ChangeSubscriptionPlanCommand, TenantSubscriptionDetailsDto>
{
    public async Task<TenantSubscriptionDetailsDto> Handle(ChangeSubscriptionPlanCommand request, CancellationToken cancellationToken)
    {
        var identifier = request.TenantIdentifier.Trim().ToLowerInvariant();
        var tenant = await db.Tenants
            .Include(item => item.SubscriptionPlan)
            .FirstOrDefaultAsync(item => item.Identifier == identifier, cancellationToken)
            ?? throw new KeyNotFoundException($"Tenant '{identifier}' was not found.");

        var interval = ParseBillingInterval(request.BillingInterval);
        var plan = await db.SubscriptionPlans
            .FirstOrDefaultAsync(item => item.Id == request.SubscriptionPlanId && item.IsActive, cancellationToken)
            ?? throw new KeyNotFoundException("Subscription plan was not found.");

        var priceId = interval == BillingInterval.Annual
            ? plan.StripePriceIdAnnual
            : plan.StripePriceIdMonthly;

        if (!string.IsNullOrWhiteSpace(tenant.StripeSubscriptionId) && !string.IsNullOrWhiteSpace(priceId))
        {
            var result = await paymentGatewayService.ChangeSubscriptionPriceAsync(tenant.StripeSubscriptionId, priceId, cancellationToken);
            if (!result.Succeeded)
                throw new InvalidOperationException(result.Error ?? "Failed to update the subscription plan.");
        }

        tenant.SubscriptionPlanId = plan.Id;
        tenant.BillingInterval = interval;
        tenant.NextBillingDate ??= DateTime.UtcNow.Add(interval == BillingInterval.Annual ? TimeSpan.FromDays(365) : TimeSpan.FromDays(30));
        tenant.ModifiedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(cancellationToken);

        return new TenantSubscriptionDetailsDto
        {
            TenantIdentifier = tenant.Identifier,
            SubscriptionPlanId = tenant.SubscriptionPlanId,
            SubscriptionPlanName = tenant.SubscriptionPlan?.Name ?? plan.Name,
            BillingInterval = tenant.BillingInterval.ToString(),
            PricingMonthly = plan.PricingMonthly,
            PricingAnnual = plan.PricingAnnual,
            Currency = plan.Currency,
            BillingStatus = tenant.BillingStatus.ToString(),
            NextBillingDate = tenant.NextBillingDate,
            CancelAtPeriodEnd = tenant.CancelAtPeriodEnd
        };
    }

    private static BillingInterval ParseBillingInterval(string value)
        => Enum.TryParse<BillingInterval>(value, true, out var interval)
            ? interval
            : BillingInterval.Monthly;
}

public sealed class GetSubscriptionPaymentHistoryQueryHandler(AdminTenantCatalogDbContext db)
    : IRequestHandler<GetSubscriptionPaymentHistoryQuery, IReadOnlyList<SubscriptionPaymentHistoryItem>>
{
    public async Task<IReadOnlyList<SubscriptionPaymentHistoryItem>> Handle(
        GetSubscriptionPaymentHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var identifier = request.TenantIdentifier.Trim().ToLowerInvariant();
        return await db.SubscriptionInvoices
            .AsNoTracking()
            .Where(invoice => invoice.Tenant != null && invoice.Tenant.Identifier == identifier)
            .OrderByDescending(invoice => invoice.PeriodEnd)
            .Select(invoice => new SubscriptionPaymentHistoryItem(
                invoice.StripeInvoiceId,
                invoice.AmountDue,
                invoice.Currency,
                invoice.Status,
                invoice.PeriodStart,
                invoice.PeriodEnd,
                invoice.PaidAt,
                invoice.HostedInvoiceUrl))
            .ToListAsync(cancellationToken);
    }
}

public sealed record SubscriptionPaymentHistoryItem(
    string InvoiceId,
    decimal Amount,
    string Currency,
    SubscriptionInvoiceStatus Status,
    DateTime PeriodStart,
    DateTime PeriodEnd,
    DateTime? PaidAt,
    string? HostedInvoiceUrl);

public sealed class DownloadSubscriptionInvoiceCommandHandler(
    AdminTenantCatalogDbContext db,
    IPaymentGatewayService paymentGatewayService) : IRequestHandler<DownloadSubscriptionInvoiceCommand, byte[]?>
{
    public async Task<byte[]?> Handle(DownloadSubscriptionInvoiceCommand request, CancellationToken cancellationToken)
    {
        var identifier = request.TenantIdentifier.Trim().ToLowerInvariant();
        var invoiceId = await db.SubscriptionInvoices
            .Where(invoice => invoice.StripeInvoiceId == request.InvoiceId && invoice.Tenant != null && invoice.Tenant.Identifier == identifier)
            .Select(invoice => invoice.StripeInvoiceId)
            .FirstOrDefaultAsync(cancellationToken);

        return invoiceId is null
            ? null
            : await paymentGatewayService.DownloadInvoicePdfAsync(invoiceId, cancellationToken);
    }
}

public sealed class SendSubscriptionInvoiceEmailCommandHandler(
    AdminTenantCatalogDbContext db,
    IPaymentGatewayService paymentGatewayService,
    ISubscriptionInvoiceEmailService emailService) : IRequestHandler<SendSubscriptionInvoiceEmailCommand>
{
    public async Task Handle(SendSubscriptionInvoiceEmailCommand request, CancellationToken cancellationToken)
    {
        var identifier = request.TenantIdentifier.Trim().ToLowerInvariant();
        var invoice = await db.SubscriptionInvoices
            .Include(item => item.Tenant)
            .ThenInclude(tenant => tenant!.TenantUsers)
            .FirstOrDefaultAsync(item => item.StripeInvoiceId == request.InvoiceId && item.Tenant != null && item.Tenant.Identifier == identifier, cancellationToken)
            ?? throw new KeyNotFoundException("Subscription invoice was not found.");

        var recipientEmail = invoice.Tenant!.TenantUsers
            .Where(user => user.IsPrimaryAdmin && user.IsActive)
            .Select(user => user.Email)
            .FirstOrDefault();
        if (string.IsNullOrWhiteSpace(recipientEmail))
            throw new InvalidOperationException("No active primary administrator email is configured for this tenant.");

        var pdfBytes = await paymentGatewayService.DownloadInvoicePdfAsync(invoice.StripeInvoiceId, cancellationToken);
        if (pdfBytes is null)
            throw new InvalidOperationException("The payment provider did not return an invoice PDF.");

        await emailService.SendAsync(invoice, pdfBytes, recipientEmail, cancellationToken);
    }
}
