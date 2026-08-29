using Microsoft.EntityFrameworkCore;
using Opplat.Application.Abstractions.Messaging;
using Opplat.Application.Abstractions.Services;
using Opplat.Domain.Entities.Administration;
using Opplat.Infrastructure.Persistance.Data.Administration;

namespace Opplat.Application.Features.Admin.Commands;

public sealed record AddPaymentMethodCommand(
    string TenantIdentifier,
    string CardNumber,
    int ExpMonth,
    int ExpYear,
    string Cvc,
    bool SetAsDefault = false) : IRequest<TenantPaymentMethodDto>;

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

public sealed class AddPaymentMethodCommandHandler(
    AdminTenantCatalogDbContext db,
    IPaymentGatewayService paymentGatewayService) : IRequestHandler<AddPaymentMethodCommand, TenantPaymentMethodDto>
{
    public async Task<TenantPaymentMethodDto> Handle(AddPaymentMethodCommand request, CancellationToken cancellationToken)
    {
        var tenant = await db.Tenants
            .FirstOrDefaultAsync(item => item.Identifier == Normalize(request.TenantIdentifier), cancellationToken)
            ?? throw new KeyNotFoundException("Tenant was not found.");
        var existingMethods = await db.TenantPaymentMethods
            .Where(item => item.TenantId == tenant.Id)
            .ToListAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(tenant.StripeCustomerId))
            throw new InvalidOperationException("This tenant has no payment customer configured.");

        var card = await paymentGatewayService.CreatePaymentMethodAsync(
            request.CardNumber, request.ExpMonth, request.ExpYear, request.Cvc, cancellationToken);
        if (!card.Succeeded || string.IsNullOrWhiteSpace(card.ExternalId))
            throw new InvalidOperationException(card.Error ?? "Failed to create payment method.");

        var attached = await paymentGatewayService.AttachPaymentMethodAsync(
            tenant.StripeCustomerId, card.ExternalId, cancellationToken);
        if (!attached.Succeeded)
            throw new InvalidOperationException(attached.Error ?? "Failed to attach payment method.");

        var isDefault = request.SetAsDefault || existingMethods.All(item => !item.IsDefault);
        if (isDefault)
            foreach (var existing in existingMethods)
                existing.IsDefault = false;

        var paymentMethod = new TenantPaymentMethod
        {
            Id = Guid.NewGuid(),
            TenantId = tenant.Id,
            StripePaymentMethodId = card.ExternalId,
            Brand = card.Brand,
            Last4 = card.Last4,
            ExpMonth = request.ExpMonth,
            ExpYear = request.ExpYear,
            IsDefault = isDefault,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow
        };

        db.TenantPaymentMethods.Add(paymentMethod);
        await db.SaveChangesAsync(cancellationToken);
        return ToDto(paymentMethod);
    }

    private static string Normalize(string identifier) => identifier.Trim().ToLowerInvariant();
    private static TenantPaymentMethodDto ToDto(TenantPaymentMethod item) =>
        new(item.Id, item.Brand, item.Last4, item.ExpMonth, item.ExpYear, item.IsDefault);
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
