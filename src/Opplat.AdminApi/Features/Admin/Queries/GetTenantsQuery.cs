using MediatR;
using Microsoft.EntityFrameworkCore;
using Opplat.AdminApi.Data;
using Opplat.AdminApi.Endpoints;

namespace Opplat.AdminApi.Features.Admin.Queries;

public sealed record GetTenantsQuery() : IRequest<List<AdminTenantDto>>;

public sealed class GetTenantsQueryHandler : IRequestHandler<GetTenantsQuery, List<AdminTenantDto>>
{
    private readonly AdminTenantCatalogDbContext _db;

    public GetTenantsQueryHandler(AdminTenantCatalogDbContext db)
    {
        _db = db;
    }

    public async Task<List<AdminTenantDto>> Handle(GetTenantsQuery request, CancellationToken cancellationToken)
    {
        return await _db.Tenants
            .AsNoTracking()
            .OrderBy(tenant => tenant.Name)
            .ThenBy(tenant => tenant.Identifier)
            .Select(tenant => AdminPortalMappings.ToDto(tenant))
            .ToListAsync(cancellationToken);
    }
}
