using Microsoft.EntityFrameworkCore;
using Opplat.Domain.Entities.Administration;
using Opplat.Infrastructure.Persistance.Data.Administration;

namespace Opplat.Application.Services;

/// <summary>
/// Enforces that the PrimaryAdmin of a tenant cannot be deleted or demoted
/// by non-SuperAdmin actors.
/// </summary>
public static class PrimaryAdminGuard
{
    /// <summary>
    /// Throws <see cref="InvalidOperationException"/> if the target user is the tenant's PrimaryAdmin
    /// and the actor does not have the SuperAdmin role.
    /// </summary>
    public static async Task EnsureCanModifyUserAsync(
        AdminTenantCatalogDbContext db,
        Guid targetUserId,
        bool actorIsSuperAdmin,
        CancellationToken cancellationToken = default)
    {
        var user = await db.TenantUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == targetUserId, cancellationToken);

        if (user is null)
            return; // Will be caught as not found elsewhere

        if (user.IsPrimaryAdmin && !actorIsSuperAdmin)
            throw new InvalidOperationException(
                "The PrimaryAdmin of a tenant cannot be modified or removed. " +
                "Only a SuperAdmin can perform this operation.");
    }

    /// <summary>
    /// Throws <see cref="InvalidOperationException"/> if attempting to remove the last PrimaryAdmin.
    /// </summary>
    public static async Task EnsurePrimaryAdminRemainsAsync(
        AdminTenantCatalogDbContext db,
        Guid tenantId,
        Guid targetUserId,
        CancellationToken cancellationToken = default)
    {
        var targetUser = await db.TenantUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == targetUserId, cancellationToken);

        if (targetUser is null || !targetUser.IsPrimaryAdmin)
            return; // Not a primary admin, no protection needed

        var primaryAdminCount = await db.TenantUsers
            .CountAsync(u => u.TenantId == tenantId && u.IsPrimaryAdmin && u.IsActive, cancellationToken);

        if (primaryAdminCount <= 1)
            throw new InvalidOperationException(
                "Cannot remove the last PrimaryAdmin from a tenant. " +
                "Assign a new PrimaryAdmin before removing this user.");
    }
}
