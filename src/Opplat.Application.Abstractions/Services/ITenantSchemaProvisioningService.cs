
using Opplat.Domain.Entities.Administration;
using Opplat.Domain.Models.Administration;

namespace Opplat.Application.Abstractions.Services;

/// <summary>
/// Module 3.1: Schema Provisioning Service.
/// Creates dedicated schemas for tenants within assigned database instances.
/// Idempotent — multiple executions for the same tenant are safe.
/// </summary>
public interface ITenantSchemaProvisioningService
{
    /// <summary>
    /// Provisions a dedicated schema for the given tenant within its assigned database instance.
    /// Idempotent: if the schema already exists, this operation succeeds without error.
    /// </summary>
    public Task<TenantSchemaProvisioningOutcome> ProvisionTenantSchemaAsync(Tenant tenant, CancellationToken cancellationToken = default);

    public Task<bool> SchemaExistsAsync(Tenant tenant, CancellationToken cancellationToken = default);

    /// <summary>
    /// Drops a tenant's schema (cleanup/offboarding). Non-idempotent — fails if schema doesn't exist.
    /// </summary>
    public Task DropTenantSchemaAsync(Tenant tenant, CancellationToken cancellationToken = default);
}
