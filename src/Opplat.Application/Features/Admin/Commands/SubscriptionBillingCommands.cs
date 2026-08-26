using Microsoft.EntityFrameworkCore;
using Opplat.Application.Abstractions.Messaging;
using Opplat.Application.Abstractions.Services;
using Opplat.Domain.Entities.Administration;
using Opplat.Infrastructure.Persistance.Data.Administration;

namespace Opplat.Application.Features.Admin.Commands;

public sealed record CancelSubscriptionCommand(string TenantIdentifier) : IRequest;
public sealed record CreateBillingPortalSessionCommand(string TenantIdentifier, string ReturnUrl) : IRequest<PaymentGatewayPortalResult>;
public sealed record GetSubscriptionPaymentHistoryQuery(string TenantIdentifier) : IRequest<IReadOnlyList<SubscriptionPaymentHistoryItem>>;

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
