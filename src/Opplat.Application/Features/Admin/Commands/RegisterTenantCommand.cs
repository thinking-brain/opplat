using Opplat.Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Opplat.Application.Abstractions.Auth;
using Opplat.Application.Abstractions.Identity;
using Opplat.Application.Abstractions.Services;
using Opplat.Application.Dtos;
using Opplat.Domain.Entities.Administration;
using Opplat.Infrastructure.Persistance.Data.Administration;

namespace Opplat.Application.Features.Admin.Commands;

public sealed record RegisterTenantCommand(TenantRegistrationRequest Request) : IRequest<TenantRegistrationResult>;

public sealed class RegisterTenantCommandHandler(
    AdminTenantCatalogDbContext db,
    IUserManagementService userManagementService,
    ITenantProvisioningCoordinator provisioningCoordinator,
    IPaymentGatewayService paymentGatewayService,
    ILogger<RegisterTenantCommandHandler> logger) : IRequestHandler<RegisterTenantCommand, TenantRegistrationResult>
{
    private readonly AdminTenantCatalogDbContext _db = db;
    private readonly IUserManagementService _userManagementService = userManagementService;
    private readonly ITenantProvisioningCoordinator _provisioningCoordinator = provisioningCoordinator;
    private readonly IPaymentGatewayService _paymentGatewayService = paymentGatewayService;
    private readonly ILogger<RegisterTenantCommandHandler> _logger = logger;

    public async Task<TenantRegistrationResult> Handle(RegisterTenantCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        var identifier = AdminPortalMappings.NormalizeTenantIdentifier(request.TenantIdentifier);

        // 1. Validate TenantIdentifier uniqueness
        if (await _db.Tenants.AnyAsync(t => t.Identifier == identifier, cancellationToken))
            return new TenantRegistrationResult(false, null, $"Tenant identifier '{identifier}' is already taken.");

        // 2. Validate Email uniqueness
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        if (await _db.TenantUsers.AnyAsync(u => u.Email == normalizedEmail, cancellationToken))
            return new TenantRegistrationResult(false, null, "An account with this email address already exists.");

        // 3. Resolve SubscriptionPlan
        SubscriptionPlan? plan;
        if (request.SubscriptionPlanId.HasValue && request.SubscriptionPlanId.Value != Guid.Empty)
        {
            plan = await _db.SubscriptionPlans.FindAsync([request.SubscriptionPlanId.Value], cancellationToken);
            if (plan is null)
                return new TenantRegistrationResult(false, null, $"Subscription plan '{request.SubscriptionPlanId}' not found.");
        }
        else
        {
            plan = await _db.SubscriptionPlans
                .Where(p => p.IsActive)
                .OrderBy(p => p.PricingMonthly)
                .FirstOrDefaultAsync(cancellationToken);
            if (plan is null)
                return new TenantRegistrationResult(false, null, "No active subscription plans are available.");
        }

        var priceId = request.BillingInterval == BillingInterval.Annual
            ? plan.StripePriceIdAnnual
            : plan.StripePriceIdMonthly;
        var hasPaymentMethod = !string.IsNullOrWhiteSpace(request.CardNumber)
            || request.CardExpMonth.HasValue
            || request.CardExpYear.HasValue
            || !string.IsNullOrWhiteSpace(request.CardCvc);
        PaymentGatewayCardResult? cardResult = null;
        var customerResult = await _paymentGatewayService.CreateCustomerAsync(
            normalizedEmail,
            request.BusinessName.Trim(),
            cancellationToken);
        if (!customerResult.Succeeded || string.IsNullOrWhiteSpace(customerResult.ExternalId))
            return new TenantRegistrationResult(false, null, customerResult.Error ?? "Failed to create billing customer.");

        if (hasPaymentMethod)
        {
            if (string.IsNullOrWhiteSpace(request.CardNumber)
                || !request.CardExpMonth.HasValue
                || !request.CardExpYear.HasValue
                || string.IsNullOrWhiteSpace(request.CardCvc))
                return new TenantRegistrationResult(false, null, "Complete all payment method fields or skip payment setup.");

            cardResult = await _paymentGatewayService.CreatePaymentMethodAsync(
                request.CardNumber,
                request.CardExpMonth.Value,
                request.CardExpYear.Value,
                request.CardCvc,
                cancellationToken);
            if (!cardResult.Succeeded || string.IsNullOrWhiteSpace(cardResult.ExternalId))
                return new TenantRegistrationResult(false, null, cardResult.Error ?? "Failed to create payment method.");

            var paymentMethodResult = await _paymentGatewayService.AttachPaymentMethodAsync(
                customerResult.ExternalId,
                cardResult.ExternalId,
                cancellationToken);
            if (!paymentMethodResult.Succeeded)
                return new TenantRegistrationResult(false, null, paymentMethodResult.Error ?? "Failed to attach payment method.");
        }

        var subscriptionResult = await _paymentGatewayService.CreateSubscriptionAsync(
            customerResult.ExternalId,
            priceId ?? string.Empty,
            request.BillingInterval,
            cancellationToken);
        if (!subscriptionResult.Succeeded)
            return new TenantRegistrationResult(false, null, subscriptionResult.Error ?? "Failed to create subscription.");

        // 4. Create user in Keycloak
        var createUserResult = await _userManagementService.CreateUserAsync(new CreateUserRequest
        {
            UserName = request.Username,
            Email = normalizedEmail,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Password = request.Password,
        }, cancellationToken);

        if (!createUserResult.Succeeded)
        {
            _logger.LogWarning(
                "Keycloak user creation failed for {Email} during self-registration: {Error}",
                normalizedEmail, createUserResult.Error);
            return new TenantRegistrationResult(false, null, createUserResult.Error ?? "Failed to create user account.");
        }

        // 4.5. Assign TenantAdmin and TenantUser realm roles so the primary admin can access management features.
        var roleAssignResult = await _userManagementService.AssignRolesAsync(
            createUserResult.ObjectId!,
            [AuthRoles.TenantAdmin, AuthRoles.TenantUser],
            cancellationToken);

        if (!roleAssignResult.Succeeded)
        {
            _logger.LogWarning(
                "Keycloak role assignment failed for user {UserId} during self-registration: {Error}",
                createUserResult.ObjectId, roleAssignResult.Error);
            return new TenantRegistrationResult(false, null, "Failed to assign tenant roles. Please contact support.");
        }

        // 5. Pick the DatabaseInstance with lowest CurrentTenantSchemaCount
        var dbInstance = await _db.Set<DatabaseInstance>()
            .Where(d => d.Status == DatabaseInstanceStatus.Active)
            .OrderBy(d => d.CurrentTenantSchemaCount)
            .FirstOrDefaultAsync(cancellationToken);

        if (dbInstance is null)
            return new TenantRegistrationResult(false, null, "No active database instances are available.");

        // 6. Create Tenant entity
        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Identifier = identifier,
            Name = request.BusinessName.Trim(),
            DatabaseSchema = $"tenant_{identifier}",
            Status = TenantStatus.Active,
            SubscriptionPlanId = plan.Id,
            DatabaseInstanceId = dbInstance.Id,
            BillingStatus = subscriptionResult.BillingStatus,
            BillingInterval = request.BillingInterval,
            NextBillingDate = subscriptionResult.CurrentPeriodEnd,
            StripeCustomerId = customerResult.ExternalId,
            StripeSubscriptionId = subscriptionResult.SubscriptionId,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow
        };

        // 7. Create TenantUser entity
        var tenantUser = new TenantUser
        {
            Id = Guid.NewGuid(),
            EntraOid = createUserResult.ObjectId ?? string.Empty,
            TenantId = tenant.Id,
            Email = normalizedEmail,
            Role = TenantUserRole.Admin,
            IsPrimaryAdmin = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow
        };

        // 8. Persist both
        _db.Tenants.Add(tenant);
        _db.TenantUsers.Add(tenantUser);
        if (cardResult is not null)
        {
            _db.TenantPaymentMethods.Add(new TenantPaymentMethod
            {
                Id = Guid.NewGuid(),
                TenantId = tenant.Id,
                StripePaymentMethodId = cardResult.ExternalId!,
                Brand = cardResult.Brand,
                Last4 = cardResult.Last4,
                ExpMonth = request.CardExpMonth,
                ExpYear = request.CardExpYear,
                IsDefault = true,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            });
        }

        if (!string.IsNullOrWhiteSpace(subscriptionResult.InvoiceId))
        {
            var periodEnd = subscriptionResult.CurrentPeriodEnd ?? DateTime.UtcNow;
            _db.SubscriptionInvoices.Add(new SubscriptionInvoice
            {
                Id = Guid.NewGuid(),
                TenantId = tenant.Id,
                StripeInvoiceId = subscriptionResult.InvoiceId,
                AmountDue = request.BillingInterval == BillingInterval.Annual ? plan.PricingAnnual : plan.PricingMonthly,
                Currency = plan.Currency,
                Status = cardResult is null ? SubscriptionInvoiceStatus.Open : SubscriptionInvoiceStatus.Paid,
                PeriodStart = periodEnd.Add(request.BillingInterval == BillingInterval.Annual ? TimeSpan.FromDays(-365) : TimeSpan.FromDays(-30)),
                PeriodEnd = periodEnd,
                PaidAt = cardResult is null ? null : DateTime.UtcNow,
                HostedInvoiceUrl = $"/billing/invoices/{subscriptionResult.InvoiceId}/pdf",
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            });
        }
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Self-registered tenant '{Identifier}' with primary admin '{Email}' (KeycloakId: {UserId}).",
            identifier, normalizedEmail, createUserResult.ObjectId);

        // 9. Provision tenant schema (non-fatal if it fails)
        try
        {
            await _provisioningCoordinator.EnsureTenantProvisionedAsync(tenant, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "Schema provisioning failed for tenant '{Identifier}' after self-registration. Tenant and user records were saved.",
                identifier);
        }

        // 10. Return success
        return new TenantRegistrationResult(true, identifier, "Registration successful.");
    }
}
