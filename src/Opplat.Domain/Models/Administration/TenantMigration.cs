namespace Opplat.Domain.Models.Administration;

public sealed class TenantSchemaMigrationDefinition
{
    public required string MigrationName { get; init; }
    public required string MigrationSql { get; init; }
    public string? RollbackSql { get; init; }
}

public sealed class TenantSchemaMigrationRunReport
{
    public required string MigrationName { get; init; }
    public required IReadOnlyList<TenantMigrationResult> Results { get; init; }
    public int SuccessCount { get; init; }
    public int FailureCount { get; init; }
    public DateTime CompletedAt { get; init; }

    public static TenantSchemaMigrationRunReport Create(string migrationName, IReadOnlyList<TenantMigrationResult> results) =>
        new()
        {
            MigrationName = migrationName,
            Results = results,
            SuccessCount = results.Count(result => result.Status == TenantMigrationStatus.Success),
            FailureCount = results.Count(result => result.Status == TenantMigrationStatus.Failed),
            CompletedAt = DateTime.UtcNow
        };
}

public sealed class TenantSchemaMigrationBatchReport
{
    public required string MigrationName { get; init; }
    public int BatchNumber { get; init; }
    public required IReadOnlyList<TenantMigrationResult> Results { get; init; }
    public int SuccessCount { get; init; }
    public int FailureCount { get; init; }
    public DateTime CompletedAt { get; init; }

    public static TenantSchemaMigrationBatchReport Create(
        string migrationName,
        int batchNumber,
        IReadOnlyList<TenantMigrationResult> results) =>
        new()
        {
            MigrationName = migrationName,
            BatchNumber = batchNumber,
            Results = results,
            SuccessCount = results.Count(result => result.Status == TenantMigrationStatus.Success),
            FailureCount = results.Count(result => result.Status == TenantMigrationStatus.Failed),
            CompletedAt = DateTime.UtcNow
        };
}

/// <summary>
/// Result of executing a migration for a single tenant.
/// </summary>
public sealed class TenantMigrationResult
{
    public required string TenantId { get; set; }
    public required string TenantIdentifier { get; set; }
    public required string MigrationName { get; set; }
    public TenantMigrationStatus Status { get; set; } = TenantMigrationStatus.Pending;
    public string? ErrorMessage { get; set; }
    public bool RollbackAttempted { get; set; }
    public bool? RollbackSucceeded { get; set; }
    public string? RollbackErrorMessage { get; set; }
    public DateTime ExecutedAt { get; set; }
}

/// <summary>
/// Status of a tenant migration execution.
/// </summary>
public enum TenantMigrationStatus
{
    Pending,
    Success,
    Failed
}
