
using Opplat.Domain.Entities.Administration;
using Opplat.Domain.Models.Administration;

namespace Opplat.Application.Abstractions.Services;

public interface ITenantProvisioningReporter
{
    Task ReportAsync(TenantProvisioningResult result, CancellationToken cancellationToken);
}

public interface ITenantProvisioningCoordinator
{

    public Task<TenantProvisioningResult> EnsureTenantProvisionedAsync(
        string tenantIdentifier,
        CancellationToken cancellationToken = default);

    public Task<TenantProvisioningResult> EnsureTenantProvisionedAsync(
        Tenant tenant,
        CancellationToken cancellationToken = default);
}
