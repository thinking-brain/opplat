using MediatR;
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

public sealed class RegisterTenantCommandHandler : IRequestHandler<RegisterTenantCommand, TenantRegistrationResult>
{
    private readonly AdminTenantCatalogDbContext _db;
    private readonly IKeycloakUserService _keycloakUserService;
    private readonly ITenantProvisioningCoordinator _provisioningCoordinator;
    private readonly ILogger<RegisterTenantCommandHandler> _logger;

    public RegisterTenantCommandHandler(
        AdminTenantCatalogDbContext db,
        IKeycloakUserService keycloakUserService,
        ITenantProvisioningCoordinator provisioningCoordinator,
        ILogger<RegisterTenantCommandHandler> logger)
    {
        _db = db;
        _keycloakUserService = keycloakUserService;
        _provisioningCoordinator = provisioningCoordinator;
        _logger = logger;
    }

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

        // 4. Create user in Keycloak
        var keycloakResult = await _keycloakUserService.CreateUserAsync(new CreateKeycloakUserRequest
        {
            Username = request.Username,
            Email = normalizedEmail,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Password = request.Password,
            Enabled = true,
            EmailVerified = false
        }, cancellationToken);

        if (!keycloakResult.Succeeded)
        {
            _logger.LogWarning(
                "Keycloak user creation failed for {Email} during self-registration: {Error}",
                normalizedEmail, keycloakResult.ErrorMessage);
            return new TenantRegistrationResult(false, null, keycloakResult.ErrorMessage ?? "Failed to create user account.");
        }

        // 4.5. Assign TenantAdmin and TenantUser realm roles so the primary admin can access management features.
        var roleAssignResult = await _keycloakUserService.AssignRealmRolesAsync(
            keycloakResult.UserId!,
            [AuthRoles.TenantAdmin, AuthRoles.TenantUser],
            cancellationToken);

        if (!roleAssignResult.Succeeded)
        {
            _logger.LogWarning(
                "Keycloak role assignment failed for user {UserId} during self-registration: {Error}",
                keycloakResult.UserId, roleAssignResult.ErrorMessage);
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
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow
        };

        // 7. Create TenantUser entity
        var tenantUser = new TenantUser
        {
            Id = Guid.NewGuid(),
            EntraOid = keycloakResult.UserId ?? string.Empty,
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
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Self-registered tenant '{Identifier}' with primary admin '{Email}' (KeycloakId: {UserId}).",
            identifier, normalizedEmail, keycloakResult.UserId);

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
