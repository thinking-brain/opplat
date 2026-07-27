using Finbuckle.MultiTenant.Abstractions;
using Opplat.Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Opplat.Application.Abstractions.Auth;
using Opplat.Application.Abstractions.Identity;
using Opplat.Application.Dtos;
using Opplat.Domain.Entities.Administration;
using Opplat.Domain.Models;
using Opplat.Infrastructure.Persistance.Data.Administration;

namespace Opplat.Application.Features.Admin.Commands;

public record CreateTenantUserCommand(
    string Name,
    string LastName,
    string Username,
    string Email,
    IReadOnlyCollection<string> Roles) : IRequest<AdminUserDto?>;

public sealed class CreateTenantUserCommandHandler(
    AdminTenantCatalogDbContext db,
    IMultiTenantContextAccessor<AppTenantInfo> tenantAccessor,
    IUserManagementService userManagementService,
    ILogger<CreateTenantUserCommandHandler> logger) : IRequestHandler<CreateTenantUserCommand, AdminUserDto?>
{
    private readonly AdminTenantCatalogDbContext _db = db;
    private readonly IMultiTenantContextAccessor<AppTenantInfo> _tenantAccessor = tenantAccessor;
    private readonly IUserManagementService _userManagementService = userManagementService;
    private readonly ILogger<CreateTenantUserCommandHandler> _logger = logger;

    public async Task<AdminUserDto?> Handle(CreateTenantUserCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate roles
        var roles = request.Roles
            .Where(role => !string.IsNullOrWhiteSpace(role))
            .Select(role => role.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (roles.Count == 0)
        {
            _logger.LogWarning("Rejected tenant user creation for {UserName} because no tenant role was supplied.", request.Username);
            return null;
        }

        var invalidRoles = roles
            .Where(role => !AuthRoles.TenantAssignable.Contains(role, StringComparer.OrdinalIgnoreCase))
            .ToList();

        if (invalidRoles.Count > 0)
        {
            _logger.LogWarning(
                "Rejected tenant user creation for {UserName} because the request included invalid roles: {Roles}.",
                request.Username,
                string.Join(", ", invalidRoles));
            return null;
        }

        // 2. Get tenant context
        var tenantInfo = _tenantAccessor.MultiTenantContext?.TenantInfo;
        if (tenantInfo is null || !Guid.TryParse(tenantInfo.Id, out var tenantId))
        {
            _logger.LogWarning("CreateTenantUserCommand: no valid tenant context available for user {UserName}.", request.Username);
            return null;
        }

        // 3. Look up tenant with subscription plan and active users
        var tenant = await _db.Tenants
            .Include(t => t.SubscriptionPlan)
            .Include(t => t.TenantUsers)
            .FirstOrDefaultAsync(t => t.Id == tenantId, cancellationToken);

        if (tenant is null)
        {
            _logger.LogWarning("CreateTenantUserCommand: tenant {TenantId} not found in catalog.", tenantId);
            return null;
        }

        // 4. Check seat limit against subscription plan
        if (tenant.SubscriptionPlan is not null)
        {
            var activeCount = tenant.TenantUsers.Count(u => u.IsActive);
            if (activeCount >= tenant.SubscriptionPlan.MaxActiveUsers)
            {
                _logger.LogWarning(
                    "Tenant {TenantIdentifier} has reached the maximum of {Max} active users allowed by the subscription plan.",
                    tenant.Identifier, tenant.SubscriptionPlan.MaxActiveUsers);
                return null;
            }
        }

        // 5. Generate temporary password meeting Entra complexity requirements
        var temporaryPassword = $"Tmp!{Guid.NewGuid():N}1A";

        // 6. Create user in identity provider
        var userCreateResult = await _userManagementService.CreateUserAsync(new CreateUserRequest
        {
            Email = request.Email,
            UserName = $"{request.Name} {request.LastName}".Trim(),
            FirstName = request.Name,
            LastName = request.LastName,
            Password = temporaryPassword
        }, cancellationToken);

        if (!userCreateResult.Succeeded)
        {
            _logger.LogError(
                "Failed to create identity provider user for {Email}: {Error}",
                request.Email, userCreateResult.Error);
            return null;
        }

        // 7. Map role: TenantAdmin → Admin, otherwise → User
        var tenantUserRole = roles.Contains(AuthRoles.TenantAdmin, StringComparer.OrdinalIgnoreCase)
            ? TenantUserRole.Admin
            : TenantUserRole.User;

        // 8. Create TenantUser record in catalog
        var tenantUser = new TenantUser
        {
            Id = Guid.NewGuid(),
            EntraOid = userCreateResult.ObjectId ?? string.Empty,
            TenantId = tenantId,
            Email = request.Email,
            Role = tenantUserRole,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow
        };
        _db.TenantUsers.Add(tenantUser);

        // 9. Persist
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created tenant user {Email} (OID: {Oid}) for tenant {TenantIdentifier}.",
            request.Email, userCreateResult.ObjectId, tenant.Identifier);

        // 10. Return populated DTO
        return new AdminUserDto
        {
            TenantId = tenantInfo.Id,
            TenantIdentifier = tenantInfo.Identifier,
            TenantName = tenantInfo.Name ?? string.Empty,
            UserId = tenantUser.Id,
            Name = request.Name,
            LastName = request.LastName,
            Username = request.Username,
            Email = request.Email,
            Active = true,
            Roles = roles
        };
    }
}
