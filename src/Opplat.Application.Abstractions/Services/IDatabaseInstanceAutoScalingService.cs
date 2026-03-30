

using Opplat.Domain.Entities.Administration;

namespace Opplat.Application.Abstractions.Services;

public interface IDatabaseInstanceAutoScalingService
{
    /// <summary>
    /// Checks if the target database instance has reached its capacity threshold.
    /// If so, provisions a new database instance automatically.
    /// Returns the database instance to use for the new tenant schema.
    /// </summary>
    public Task<DatabaseInstance> EnsureAvailableInstanceAsync(
        DatabaseInstance currentInstance,
        int reservedSlots = 0,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Increments the schema count for a database instance (called after successful schema creation).
    /// </summary>
    public Task IncrementTenantCountAsync(int databaseInstanceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Decrements the schema count for a database instance (called during tenant cleanup/offboarding).
    /// </summary>
    public Task DecrementTenantCountAsync(int databaseInstanceId, CancellationToken cancellationToken = default);

    public Task RecalculateTenantCountsAsync(CancellationToken cancellationToken = default);
}
