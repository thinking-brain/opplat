using MediatR;
using Microsoft.EntityFrameworkCore;
using Opplat.Application.Dtos;
using Opplat.Infrastructure.Persistance.Data.Administration;

namespace Opplat.Application.Features.Admin.Queries;

public sealed record GetTenantsQuery() : IRequest<List<AdminTenantDto>>;

public sealed class GetTenantsQueryHandler(AdminTenantCatalogDbContext db) : IRequestHandler<GetTenantsQuery, List<AdminTenantDto>>
{
    private readonly AdminTenantCatalogDbContext _db = db;

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
