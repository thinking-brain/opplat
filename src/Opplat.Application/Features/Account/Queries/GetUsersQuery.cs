using Finbuckle.MultiTenant.Abstractions;
using Opplat.Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using Opplat.Application.Abstractions.Auth;
using Opplat.Application.Dtos;
using Opplat.Domain.Entities.Administration;
using Opplat.Domain.Models;
using Opplat.Infrastructure.Persistance.Data.Administration;

namespace Opplat.Application.Features.Account.Queries;

public record GetUsersQuery : IRequest<List<AccountDto>>;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, List<AccountDto>>
{
    private readonly AdminTenantCatalogDbContext _db;
    private readonly IMultiTenantContextAccessor<AppTenantInfo> _tenantAccessor;

    public GetUsersQueryHandler(
        AdminTenantCatalogDbContext db,
        IMultiTenantContextAccessor<AppTenantInfo> tenantAccessor)
    {
        _db             = db;
        _tenantAccessor = tenantAccessor;
    }

    public async Task<List<AccountDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var tenantInfo = _tenantAccessor.MultiTenantContext?.TenantInfo;
        if (tenantInfo is null || !Guid.TryParse(tenantInfo.Id, out var tenantId))
            return [];

        var tenantUsers = await _db.TenantUsers
            .AsNoTracking()
            .Where(u => u.TenantId == tenantId)
            .OrderBy(u => u.Email)
            .ToListAsync(cancellationToken);

        return tenantUsers
            .Select(u => new AccountDto
            {
                UserId   = u.Id,
                Name     = string.Empty,
                LastName = string.Empty,
                Username = u.Email,
                Email    = u.Email,
                Active   = u.IsActive,
                Roles    = [MapRole(u.Role)]
            })
            .ToList();
    }

    private static string MapRole(TenantUserRole role) => role switch
    {
        TenantUserRole.PrimaryAdmin => AuthRoles.TenantAdmin,
        TenantUserRole.Admin        => AuthRoles.TenantAdmin,
        _                           => AuthRoles.TenantUser
    };
}
