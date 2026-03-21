using Finbuckle.MultiTenant.Abstractions;
using MediatR;
using Opplat.MainApp.Models;

namespace Opplat.MainApp.Features.Admin.Queries;

public record GetTenantsQuery() : IRequest<List<AdminTenantDto>>;

public sealed class GetTenantsQueryHandler : IRequestHandler<GetTenantsQuery, List<AdminTenantDto>>
{
    private readonly IMultiTenantStore<AppTenantInfo> _tenantStore;

    public GetTenantsQueryHandler(IMultiTenantStore<AppTenantInfo> tenantStore)
    {
        _tenantStore = tenantStore;
    }

    public async Task<List<AdminTenantDto>> Handle(GetTenantsQuery request, CancellationToken cancellationToken)
    {
        var tenants = await _tenantStore.GetAllAsync();

        return tenants
            .OrderBy(tenant => tenant.Name)
            .Select(tenant => new AdminTenantDto
            {
                Id = tenant.Id ?? string.Empty,
                Identifier = tenant.Identifier ?? string.Empty,
                Name = tenant.Name ?? string.Empty,
                ConnectionString = tenant.ConnectionString ?? string.Empty,
                IsActive = tenant.IsActive
            })
            .ToList();
    }
}
