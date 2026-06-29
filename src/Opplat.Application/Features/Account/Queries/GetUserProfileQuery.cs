using Finbuckle.MultiTenant.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Opplat.Application.Abstractions.Auth;
using Opplat.Application.Dtos;
using Opplat.Domain.Entities.Administration;
using Opplat.Domain.Models;
using Opplat.Infrastructure.Persistance.Data.Administration;

namespace Opplat.Application.Features.Account.Queries;

public record GetUserProfileQuery(string Username) : IRequest<AccountDto?>;

public class GetUserProfileQueryHandler(
    AdminTenantCatalogDbContext db,
    IMultiTenantContextAccessor<AppTenantInfo> tenantAccessor) : IRequestHandler<GetUserProfileQuery, AccountDto?>
{
    private readonly AdminTenantCatalogDbContext _db = db;
    private readonly IMultiTenantContextAccessor<AppTenantInfo> _tenantAccessor = tenantAccessor;

    public async Task<AccountDto?> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var tenantInfo = _tenantAccessor.MultiTenantContext?.TenantInfo;
        if (tenantInfo is null || !Guid.TryParse(tenantInfo.Id, out var tenantId))
            return null;

        // Match by email (username == email in this system) or by EntraOid
        var tenantUser = await _db.TenantUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(
                u => u.TenantId == tenantId &&
                     (u.Email == request.Username || u.EntraOid == request.Username),
                cancellationToken);

        if (tenantUser is null) return null;

        return new AccountDto
        {
            UserId   = tenantUser.Id,
            Name     = string.Empty,
            LastName = string.Empty,
            Username = tenantUser.Email,
            Email    = tenantUser.Email,
            Active   = tenantUser.IsActive,
            Roles    = [MapRole(tenantUser.Role)]
        };
    }

    private static string MapRole(TenantUserRole role) => role switch
    {
        TenantUserRole.PrimaryAdmin => AuthRoles.TenantAdmin,
        TenantUserRole.Admin        => AuthRoles.TenantAdmin,
        _                           => AuthRoles.TenantUser
    };
}
