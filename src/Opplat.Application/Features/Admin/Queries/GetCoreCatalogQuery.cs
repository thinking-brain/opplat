using MediatR;
using Microsoft.EntityFrameworkCore;
using Opplat.Application.Dtos;
using Opplat.Infrastructure.Persistance.Data.Administration;

namespace Opplat.Application.Features.Admin.Queries;

public sealed record GetCoreTenantsQuery() : IRequest<List<CoreTenantCatalogDto>>;
public sealed record GetSubscriptionPlansQuery() : IRequest<List<AdminSubscriptionPlanDto>>;
public sealed record GetDatabaseInstancesQuery() : IRequest<List<AdminDatabaseInstanceDto>>;

public sealed class GetCoreTenantsQueryHandler : IRequestHandler<GetCoreTenantsQuery, List<CoreTenantCatalogDto>>
{
    private readonly AdminTenantCatalogDbContext _db;

    public GetCoreTenantsQueryHandler(AdminTenantCatalogDbContext db)
    {
        _db = db;
    }

    public async Task<List<CoreTenantCatalogDto>> Handle(GetCoreTenantsQuery request, CancellationToken cancellationToken)
    {
        var tenants = await _db.Tenants
            .AsNoTracking()
            .Include(tenant => tenant.SubscriptionPlan)
            .Include(tenant => tenant.DatabaseInstance)
            .Include(tenant => tenant.TenantUsers)
            .OrderBy(tenant => tenant.Name)
            .ThenBy(tenant => tenant.Identifier)
            .ToListAsync(cancellationToken);

        return [.. tenants
            .Select(tenant => AdminPortalMappings.ToCoreDto(
                tenant,
                tenant.TenantUsers.Count(user => user.IsActive)))];
    }
}

public sealed class GetSubscriptionPlansQueryHandler(AdminTenantCatalogDbContext db) : IRequestHandler<GetSubscriptionPlansQuery, List<AdminSubscriptionPlanDto>>
{
    private readonly AdminTenantCatalogDbContext _db = db;

    public async Task<List<AdminSubscriptionPlanDto>> Handle(GetSubscriptionPlansQuery request, CancellationToken cancellationToken)
    {
        var plans = await _db.SubscriptionPlans
            .AsNoTracking()
            .OrderBy(plan => plan.Name)
            .ToListAsync(cancellationToken);

        return plans.Select(AdminPortalMappings.ToDto).ToList();
    }
}

public sealed class GetDatabaseInstancesQueryHandler(AdminTenantCatalogDbContext db) : IRequestHandler<GetDatabaseInstancesQuery, List<AdminDatabaseInstanceDto>>
{
    private readonly AdminTenantCatalogDbContext _db = db;

    public async Task<List<AdminDatabaseInstanceDto>> Handle(GetDatabaseInstancesQuery request, CancellationToken cancellationToken)
    {
        var instances = await _db.DatabaseInstances
            .AsNoTracking()
            .OrderBy(instance => instance.Identifier)
            .ToListAsync(cancellationToken);

        return instances.Select(AdminPortalMappings.ToDto).ToList();
    }
}
