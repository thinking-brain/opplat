using Microsoft.EntityFrameworkCore;
using Opplat.Application.Abstractions.Services;
using Opplat.Infrastructure.Persistance.Data.Administration;

namespace Opplat.Infrastructure.Services;

internal sealed class UserTenantResolver(AdminTenantCatalogDbContext db) : IUserTenantResolver
{
    public async Task<UserTenantInfo?> ResolveAsync(string userEmail, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(userEmail))
            return null;

        var result = await db.TenantUsers
            .Where(u => u.Email == userEmail && u.IsActive)
            .Select(u => new { TenantId = u.TenantId.ToString(), u.Tenant!.Identifier })
            .FirstOrDefaultAsync(ct);

        return result is null ? null : new UserTenantInfo(result.TenantId, result.Identifier);
    }
}
