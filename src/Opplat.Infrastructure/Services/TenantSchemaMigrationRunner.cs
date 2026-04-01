using System.Data;
using Npgsql;
using Microsoft.EntityFrameworkCore;
using Opplat.Infrastructure.Persistance.Data.Administration;
using Microsoft.Extensions.Logging;
using Opplat.Domain.Entities.Administration;
using Opplat.Application.Abstractions.Services;
using Opplat.Domain.Models.Administration;

namespace Opplat.Infrastructure.Services;

/// <summary>
/// Module 3.3: Schema Migration Runner.
/// Executes database schema changes across tenant schemas.
/// Supports per-tenant execution, bulk execution, phased/rolling deployments, and rollback.
/// </summary>
public sealed class TenantSchemaMigrationRunner : ITenantSchemaMigrationRunner
{
    private readonly AdminTenantCatalogDbContext _db;
    private readonly ILogger<TenantSchemaMigrationRunner> _logger;
    private readonly IEnumerable<ITenantSchemaMigrationReporter> _reporters;

    public TenantSchemaMigrationRunner(
        AdminTenantCatalogDbContext db,
        ILogger<TenantSchemaMigrationRunner> logger,
        IEnumerable<ITenantSchemaMigrationReporter> reporters)
    {
        _db = db;
        _logger = logger;
        _reporters = reporters;
    }

    public async Task<TenantSchemaMigrationRunReport> ExecuteMigrationForTenantAsync(
        string tenantIdentifier,
        TenantSchemaMigrationDefinition definition,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tenantIdentifier);
        ArgumentNullException.ThrowIfNull(definition);

        var tenant = await _db.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(
                model => model.Identifier == tenantIdentifier.Trim().ToLowerInvariant(),
                cancellationToken)
            ?? throw new KeyNotFoundException($"Tenant '{tenantIdentifier}' was not found.");

        var result = await ExecuteMigrationForTenantAsync(
            tenant,
            definition.MigrationName,
            definition.MigrationSql,
            definition.RollbackSql,
            cancellationToken);

        var report = TenantSchemaMigrationRunReport.Create(definition.MigrationName, [result]);
        await ReportRunCompletedAsync(report, cancellationToken);
        return report;
    }

    /// <summary>
    /// Executes a migration against a single tenant's schema.
    /// Logs success/failure per tenant.
    /// </summary>
    public async Task<TenantMigrationResult> ExecuteMigrationForTenantAsync(
        Tenant tenant,
        string migrationName,
        string migrationSql,
        string? rollbackSql = null,
        CancellationToken cancellationToken = default)
    {
        if (tenant == null)
            throw new ArgumentNullException(nameof(tenant));

        if (string.IsNullOrWhiteSpace(migrationName))
            throw new ArgumentException("Migration name cannot be empty.", nameof(migrationName));

        if (string.IsNullOrWhiteSpace(migrationSql))
            throw new ArgumentException("Migration SQL cannot be empty.", nameof(migrationSql));

        var result = new TenantMigrationResult
        {
            TenantId = tenant.Id.ToString(),
            TenantIdentifier = tenant.Identifier,
            MigrationName = migrationName,
            ExecutedAt = DateTime.UtcNow
        };

        try
        {
            var databaseInstance = await _db.DatabaseInstances
                .FirstOrDefaultAsync(d => d.Id == tenant.DatabaseInstanceId, cancellationToken);

            if (databaseInstance == null)
            {
                result.Status = TenantMigrationStatus.Failed;
                result.ErrorMessage = $"Database instance {tenant.DatabaseInstanceId} not found.";
                _logger.LogError(
                    "Migration '{MigrationName}' failed for tenant '{TenantId}': {Error}",
                    migrationName, tenant.Id, result.ErrorMessage);
                return result;
            }

            var connectionString = databaseInstance.ConnectionStringReference;
            var normalized = NormalizeConnectionString(connectionString);

            _logger.LogInformation(
                "Executing migration '{MigrationName}' for tenant '{TenantId}' (schema: '{Schema}').",
                migrationName, tenant.Id, tenant.DatabaseSchema);

            await using var connection = new NpgsqlConnection(normalized);
            await connection.OpenAsync(cancellationToken);

            try
            {
                await using var command = connection.CreateCommand();
                command.CommandText = $"SET search_path TO \"{tenant.DatabaseSchema}\"; {migrationSql}";
                await command.ExecuteNonQueryAsync(cancellationToken);

                result.Status = TenantMigrationStatus.Success;
                _logger.LogInformation(
                    "Migration '{MigrationName}' successfully executed for tenant '{TenantId}'.",
                    migrationName, tenant.Id);
            }
            finally
            {
                await connection.CloseAsync();
            }
        }
        catch (Exception ex)
        {
            result.Status = TenantMigrationStatus.Failed;
            result.ErrorMessage = ex.Message;

            if (!string.IsNullOrWhiteSpace(rollbackSql))
                await TryRollbackAsync(tenant, rollbackSql, result, cancellationToken);

            _logger.LogError(
                ex,
                "Migration '{MigrationName}' failed for tenant '{TenantId}': {Error}",
                migrationName, tenant.Id, ex.Message);
        }

        await ReportTenantResultAsync(result, cancellationToken);
        return result;
    }

    public async Task<TenantSchemaMigrationRunReport> RunMigrationBulkAsync(
        TenantSchemaMigrationDefinition definition,
        int? batchSize = null,
        TimeSpan? delayBetweenBatches = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var results = await ExecuteMigrationBulkAsync(
            definition.MigrationName,
            definition.MigrationSql,
            definition.RollbackSql,
            batchSize,
            delayBetweenBatches,
            cancellationToken);

        var report = TenantSchemaMigrationRunReport.Create(definition.MigrationName, results);
        await ReportRunCompletedAsync(report, cancellationToken);
        return report;
    }

    /// <summary>
    /// Executes a migration in bulk across all active tenants.
    /// Uses phased execution to minimize downtime: can pause between batches.
    /// Returns results for all tenants, with detailed success/failure tracking.
    /// </summary>
    public async Task<IReadOnlyList<TenantMigrationResult>> ExecuteMigrationBulkAsync(
        string migrationName,
        string migrationSql,
        string? rollbackSql = null,
        int? batchSize = null,
        TimeSpan? delayBetweenBatches = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(migrationName))
            throw new ArgumentException("Migration name cannot be empty.", nameof(migrationName));

        if (string.IsNullOrWhiteSpace(migrationSql))
            throw new ArgumentException("Migration SQL cannot be empty.", nameof(migrationSql));

        batchSize ??= 10; // Default: migrate up to 10 tenants per batch.
        delayBetweenBatches ??= TimeSpan.FromSeconds(5); // Default: 5 seconds between batches.

        _logger.LogInformation(
            "Starting bulk migration '{MigrationName}' across all active tenants. "
            + "Batch size: {BatchSize}, delay: {DelayMs}ms.",
            migrationName, batchSize, delayBetweenBatches.Value.TotalMilliseconds);

        var activeTenants = await _db.Tenants
            .Where(t => t.Status == TenantStatus.Active)
            .ToListAsync(cancellationToken);

        var results = new List<TenantMigrationResult>();

        // Execute in batches for phased deployment.
        for (var i = 0; i < activeTenants.Count; i += batchSize.Value)
        {
            var batch = activeTenants.Skip(i).Take(batchSize.Value).ToList();
            var batchNumber = i / batchSize.Value + 1;

            _logger.LogInformation(
                "Executing migration batch {BatchNumber} ({TenantCount} tenants).",
                batchNumber, batch.Count);

            var batchTasks = batch.Select(tenant =>
                ExecuteMigrationForTenantAsync(tenant, migrationName, migrationSql, rollbackSql, cancellationToken));

            var batchResults = await Task.WhenAll(batchTasks);
            results.AddRange(batchResults);

            var batchReport = TenantSchemaMigrationBatchReport.Create(migrationName, batchNumber, batchResults);
            await ReportBatchCompletedAsync(batchReport, cancellationToken);

            // Pause before next batch (unless this is the final batch).
            if (i + batchSize.Value < activeTenants.Count)
            {
                _logger.LogInformation(
                    "Batch {BatchNumber} completed. Pausing {DelayMs}ms before next batch.",
                    batchNumber, delayBetweenBatches.Value.TotalMilliseconds);
                await Task.Delay(delayBetweenBatches.Value, cancellationToken);
            }
        }

        var successCount = results.Count(r => r.Status == TenantMigrationStatus.Success);
        var failureCount = results.Count(r => r.Status == TenantMigrationStatus.Failed);

        _logger.LogInformation(
            "Bulk migration '{MigrationName}' completed. Success: {SuccessCount}, Failures: {FailureCount}.",
            migrationName, successCount, failureCount);

        return results.AsReadOnly();
    }

    /// <summary>
    /// Logs a migration execution record to enable rollback tracking.
    /// For now, returns a migration log ID for future rollback support.
    /// </summary>
    public async Task<int> LogMigrationExecutionAsync(
        string migrationName,
        string migrationSql,
        IReadOnlyList<TenantMigrationResult> results,
        CancellationToken cancellationToken = default)
    {
        if (results == null)
            throw new ArgumentNullException(nameof(results));

        var successCount = results.Count(r => r.Status == TenantMigrationStatus.Success);
        var failureCount = results.Count(r => r.Status == TenantMigrationStatus.Failed);

        _logger.LogInformation(
            "Logging migration execution: '{MigrationName}' "
            + "(Success: {SuccessCount}, Failures: {FailureCount}).",
            migrationName, successCount, failureCount);

        // Placeholder: In production, store this in a schema_migrations table for rollback tracking.
        var migrationLogId = new Random().Next(1, int.MaxValue);
        return migrationLogId;
    }

    private static string NormalizeConnectionString(string connectionString)
    {
        var builder = new NpgsqlConnectionStringBuilder(connectionString);
        if (builder.SslMode == SslMode.Prefer)
            builder.SslMode = SslMode.Disable;
        return builder.ConnectionString;
    }

    private async Task TryRollbackAsync(
        Tenant tenant,
        string rollbackSql,
        TenantMigrationResult result,
        CancellationToken cancellationToken)
    {
        try
        {
            var databaseInstance = await _db.DatabaseInstances
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == tenant.DatabaseInstanceId, cancellationToken);

            if (databaseInstance == null)
                return;

            await using var rollbackConnection = new NpgsqlConnection(NormalizeConnectionString(databaseInstance.ConnectionStringReference));
            await rollbackConnection.OpenAsync(cancellationToken);
            await using var rollbackCommand = rollbackConnection.CreateCommand();
            rollbackCommand.CommandText = $"SET search_path TO \"{tenant.DatabaseSchema}\"; {rollbackSql}";
            await rollbackCommand.ExecuteNonQueryAsync(cancellationToken);

            result.RollbackAttempted = true;
            result.RollbackSucceeded = true;
        }
        catch (Exception rollbackException)
        {
            result.RollbackAttempted = true;
            result.RollbackSucceeded = false;
            result.RollbackErrorMessage = rollbackException.Message;
            _logger.LogError(
                rollbackException,
                "Rollback for migration '{MigrationName}' failed for tenant '{TenantId}'.",
                result.MigrationName,
                tenant.Id);
        }
    }

    private async Task ReportTenantResultAsync(TenantMigrationResult result, CancellationToken cancellationToken)
    {
        foreach (var reporter in _reporters)
            await reporter.ReportTenantResultAsync(result, cancellationToken);
    }

    private async Task ReportBatchCompletedAsync(TenantSchemaMigrationBatchReport report, CancellationToken cancellationToken)
    {
        foreach (var reporter in _reporters)
            await reporter.ReportBatchCompletedAsync(report, cancellationToken);
    }

    private async Task ReportRunCompletedAsync(TenantSchemaMigrationRunReport report, CancellationToken cancellationToken)
    {
        foreach (var reporter in _reporters)
            await reporter.ReportRunCompletedAsync(report, cancellationToken);
    }
}
