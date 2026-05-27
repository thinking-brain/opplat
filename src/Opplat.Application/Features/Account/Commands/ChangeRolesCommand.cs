using Finbuckle.MultiTenant.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Opplat.Application.Abstractions.Auth;
using Opplat.Application.Abstractions.Identity;
using Opplat.Domain.Entities.Administration;
using Opplat.Domain.Models;
using Opplat.Infrastructure.Persistance.Data.Administration;

namespace Opplat.Application.Features.Account.Commands;

public record ChangeRolesCommand(string UserId, List<string> Roles) : IRequest<ChangeRolesResult>;

public record ChangeRolesResult(bool Success, string? ErrorMessage);

public class ChangeRolesCommandHandler(
    AdminTenantCatalogDbContext db,
    IMultiTenantContextAccessor<AppTenantInfo> tenantAccessor,
    IUserManagementService userManagementService,
    ILogger<ChangeRolesCommandHandler> logger) : IRequestHandler<ChangeRolesCommand, ChangeRolesResult>
{
    private readonly AdminTenantCatalogDbContext _db = db;
    private readonly IMultiTenantContextAccessor<AppTenantInfo> _tenantAccessor = tenantAccessor;
    private readonly IUserManagementService _userManagementService = userManagementService;
    private readonly ILogger<ChangeRolesCommandHandler> _logger = logger;

    public async Task<ChangeRolesResult> Handle(ChangeRolesCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate and normalise roles
        var normalizedRoles = request.Roles
            .Where(role => !string.IsNullOrWhiteSpace(role))
            .Select(role => role.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (normalizedRoles.Count == 0)
            return new ChangeRolesResult(false, "At least one tenant role must be assigned.");

        var invalidRoles = normalizedRoles
            .Where(role => !AuthRoles.TenantAssignable.Contains(role, StringComparer.OrdinalIgnoreCase))
            .ToList();

        if (invalidRoles.Count > 0)
            return new ChangeRolesResult(
                false,
                $"Only tenant roles are allowed ({string.Join(", ", AuthRoles.TenantAssignable)}). Invalid roles: {string.Join(", ", invalidRoles)}.");

        // 2. Resolve tenant
        var tenantInfo = _tenantAccessor.MultiTenantContext?.TenantInfo;
        if (tenantInfo is null || !Guid.TryParse(tenantInfo.Id, out var tenantId))
            return new ChangeRolesResult(false, "No valid tenant context available.");

        if (!Guid.TryParse(request.UserId, out var userId))
            return new ChangeRolesResult(false, "Invalid user ID format.");

        // 3. Find TenantUser scoped to this tenant
        var tenantUser = await _db.TenantUsers
            .FirstOrDefaultAsync(u => u.Id == userId && u.TenantId == tenantId, cancellationToken);

        if (tenantUser is null)
            return new ChangeRolesResult(false, "User not found in this tenant.");

        // 4. Map highest-privilege role to TenantUserRole enum
        tenantUser.Role       = normalizedRoles.Contains(AuthRoles.TenantAdmin, StringComparer.OrdinalIgnoreCase)
            ? TenantUserRole.Admin
            : TenantUserRole.User;
        tenantUser.ModifiedAt = DateTime.UtcNow;

        // 5. Sync realm roles with identity provider when an OID is present
        if (!string.IsNullOrWhiteSpace(tenantUser.EntraOid))
        {
            var userManagementResult = await _userManagementService.AssignRolesAsync(
                tenantUser.EntraOid, normalizedRoles, cancellationToken);

            if (!userManagementResult.Succeeded)
                _logger.LogWarning(
                    "ChangeRolesCommand: Role sync failed for user {UserId} (OID: {Oid}): {Error}.",
                    userId, tenantUser.EntraOid, userManagementResult.Error);
        }

        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Roles for user {UserId} in tenant {TenantId} changed to: {Roles}.",
            userId, tenantId, string.Join(", ", normalizedRoles));

        return new ChangeRolesResult(true, null);
    }
}
