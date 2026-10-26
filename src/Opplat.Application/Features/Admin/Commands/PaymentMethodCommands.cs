using Microsoft.EntityFrameworkCore;
using Opplat.Application.Abstractions.Messaging;
using Opplat.Application.Abstractions.Services;
using Opplat.Domain.Entities.Administration;
using Opplat.Infrastructure.Persistance.Data.Administration;

namespace Opplat.Application.Features.Admin.Commands;

/// <summary>
/// Starts the Redsys card-setup redirect flow for an authenticated tenant (used by the
/// "Payment Methods" settings page). No raw card details are ever submitted to this API —
/// the caller must POST the returned form fields to <see cref="RedsysCardSetupRedirectDto.ActionUrl"/>.
/// </summary>
public sealed record InitiateCardSetupCommand(string TenantIdentifier) : IRequest<RedsysCardSetupRedirectDto>;

/// <summary>
/// Public/anonymous variant used right after self-registration, before any auth session
/// exists — the tenant is identified by the id returned from registration, not by identity claims.
/// </summary>
public sealed record InitiateTenantCardSetupCommand(Guid TenantId) : IRequest<RedsysCardSetupRedirectDto>;

public sealed record RedsysCardSetupRedirectDto(string ActionUrl, IReadOnlyDictionary<string, string> FormFields);

public sealed record ListPaymentMethodsQuery(string TenantIdentifier) : IRequest<IReadOnlyList<TenantPaymentMethodDto>>;
public sealed record DeletePaymentMethodCommand(string TenantIdentifier, Guid PaymentMethodId) : IRequest;
public sealed record SetDefaultPaymentMethodCommand(string TenantIdentifier, Guid PaymentMethodId) : IRequest;

public sealed record TenantPaymentMethodDto(
    Guid Id,
    string? Brand,
    string? Last4,
    long? ExpMonth,
    long? ExpYear,
    bool IsDefault);

public sealed class InitiateCardSetupCommandHandler(
    AdminTenantCatalogDbContext db,
    IRedsysCardSetupService cardSetupService) : IRequestHandler<InitiateCardSetupCommand, RedsysCardSetupRedirectDto>
{
    public async Task<RedsysCardSetupRedirectDto> Handle(InitiateCardSetupCommand request, CancellationToken cancellationToken)
    {
        var tenant = await db.Tenants
            .FirstOrDefaultAsync(item => item.Identifier == Normalize(request.TenantIdentifier), cancellationToken)
            ?? throw new KeyNotFoundException("Tenant was not found.");

        var redirect = await cardSetupService.InitiateCardSetupAsync(tenant.Id, cancellationToken);
        return new RedsysCardSetupRedirectDto(redirect.ActionUrl, redirect.FormFields);
    }

    private static string Normalize(string identifier) => identifier.Trim().ToLowerInvariant();
}

public sealed class InitiateTenantCardSetupCommandHandler(IRedsysCardSetupService cardSetupService)
    : IRequestHandler<InitiateTenantCardSetupCommand, RedsysCardSetupRedirectDto>
{
    public async Task<RedsysCardSetupRedirectDto> Handle(InitiateTenantCardSetupCommand request, CancellationToken cancellationToken)
    {
        var redirect = await cardSetupService.InitiateCardSetupAsync(request.TenantId, cancellationToken);
        return new RedsysCardSetupRedirectDto(redirect.ActionUrl, redirect.FormFields);
    }
}

public sealed class ListPaymentMethodsQueryHandler(AdminTenantCatalogDbContext db)
    : IRequestHandler<ListPaymentMethodsQuery, IReadOnlyList<TenantPaymentMethodDto>>
{
    public async Task<IReadOnlyList<TenantPaymentMethodDto>> Handle(ListPaymentMethodsQuery request, CancellationToken cancellationToken) =>
        await db.TenantPaymentMethods
            .AsNoTracking()
            .Where(item => item.Tenant != null && item.Tenant.Identifier == request.TenantIdentifier.Trim().ToLowerInvariant())
            .OrderByDescending(item => item.IsDefault)
            .ThenByDescending(item => item.CreatedAt)
            .Select(item => new TenantPaymentMethodDto(item.Id, item.Brand, item.Last4, item.ExpMonth, item.ExpYear, item.IsDefault))
            .ToListAsync(cancellationToken);
}

public sealed class SetDefaultPaymentMethodCommandHandler(AdminTenantCatalogDbContext db)
    : IRequestHandler<SetDefaultPaymentMethodCommand>
{
    public async Task Handle(SetDefaultPaymentMethodCommand request, CancellationToken cancellationToken)
    {
        var identifier = request.TenantIdentifier.Trim().ToLowerInvariant();
        var methods = await db.TenantPaymentMethods
            .Where(item => item.Tenant != null && item.Tenant.Identifier == identifier)
            .ToListAsync(cancellationToken);
        var selected = methods.FirstOrDefault(item => item.Id == request.PaymentMethodId)
            ?? throw new KeyNotFoundException("Payment method was not found.");

        foreach (var method in methods)
            method.IsDefault = method.Id == selected.Id;
        await db.SaveChangesAsync(cancellationToken);
    }
}

public sealed class DeletePaymentMethodCommandHandler(AdminTenantCatalogDbContext db)
    : IRequestHandler<DeletePaymentMethodCommand>
{
    public async Task Handle(DeletePaymentMethodCommand request, CancellationToken cancellationToken)
    {
        var identifier = request.TenantIdentifier.Trim().ToLowerInvariant();
        var tenant = await db.Tenants
            .Include(item => item.SubscriptionPlan)
            .FirstOrDefaultAsync(item => item.Identifier == identifier, cancellationToken)
            ?? throw new KeyNotFoundException("Tenant was not found.");
        var methods = await db.TenantPaymentMethods
            .Where(item => item.TenantId == tenant.Id)
            .ToListAsync(cancellationToken);
        var selected = methods.FirstOrDefault(item => item.Id == request.PaymentMethodId)
            ?? throw new KeyNotFoundException("Payment method was not found.");

        var isPaidPlan = tenant.SubscriptionPlan?.PricingMonthly > 0 || tenant.SubscriptionPlan?.PricingAnnual > 0;
        if (tenant.BillingStatus == TenantBillingStatus.Active && isPaidPlan && methods.Count == 1)
            throw new InvalidOperationException("Add another payment method before deleting the only card on an active subscription.");

        var wasDefault = selected.IsDefault;
        db.TenantPaymentMethods.Remove(selected);
        if (wasDefault)
        {
            var replacement = methods
                .Where(item => item.Id != selected.Id)
                .OrderByDescending(item => item.CreatedAt)
                .FirstOrDefault();
            if (replacement is not null)
                replacement.IsDefault = true;
        }
        await db.SaveChangesAsync(cancellationToken);
    }
}
