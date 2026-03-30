using Opplat.Domain.Entities.Administration;
using Opplat.Domain.Models.Administration;

namespace Opplat.Application.Abstractions.Services;

/// <summary>
/// Module 3.3: Schema Migration Runner.
/// Executes database schema changes across tenant schemas.
/// Supports per-tenant execution, bulk execution, phased/rolling deployments, and rollback.
/// </summary>
public interface ITenantSchemaMigrationRunner
{
    public Task<TenantSchemaMigrationRunReport> ExecuteMigrationForTenantAsync(
        string tenantIdentifier,
        TenantSchemaMigrationDefinition definition,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a migration against a single tenant's schema.
    /// Logs success/failure per tenant.
    /// </summary>
    public Task<TenantMigrationResult> ExecuteMigrationForTenantAsync(
        Tenant tenant,
        string migrationName,
        string migrationSql,
        string? rollbackSql = null,
        CancellationToken cancellationToken = default);

    public Task<TenantSchemaMigrationRunReport> RunMigrationBulkAsync(
        TenantSchemaMigrationDefinition definition,
        int? batchSize = null,
        TimeSpan? delayBetweenBatches = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a migration in bulk across all active tenants.
    /// Uses phased execution to minimize downtime: can pause between batches.
    /// Returns results for all tenants, with detailed success/failure tracking.
    /// </summary>
    public Task<IReadOnlyList<TenantMigrationResult>> ExecuteMigrationBulkAsync(
        string migrationName,
        string migrationSql,
        string? rollbackSql = null,
        int? batchSize = null,
        TimeSpan? delayBetweenBatches = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs a migration execution record to enable rollback tracking.
    /// For now, returns a migration log ID for future rollback support.
    /// </summary>
    public Task<int> LogMigrationExecutionAsync(
        string migrationName,
        string migrationSql,
        IReadOnlyList<TenantMigrationResult> results,
        CancellationToken cancellationToken = default);
}

public interface ITenantSchemaMigrationReporter
{
    Task ReportTenantResultAsync(TenantMigrationResult result, CancellationToken cancellationToken);
    Task ReportBatchCompletedAsync(TenantSchemaMigrationBatchReport report, CancellationToken cancellationToken);
    Task ReportRunCompletedAsync(TenantSchemaMigrationRunReport report, CancellationToken cancellationToken);
}
