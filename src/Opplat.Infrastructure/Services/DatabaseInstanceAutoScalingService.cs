using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Opplat.Application.Abstractions.Options;
using Opplat.Domain.Entities.Administration;
using Opplat.Infrastructure.Persistance.Data.Administration;

namespace Opplat.Infrastructure.Services;

/// <summary>
/// Module 3.2: Database Instance Auto-Scaling Service.
/// Monitors tenant schema counts and automatically provisions new database instances
/// when the threshold is reached on the current instance.
/// </summary>
public sealed class DatabaseInstanceAutoScalingService
{
    private readonly AdminTenantCatalogDbContext _db;
    private readonly DatabaseInstanceOptions _options;
    private readonly ILogger<DatabaseInstanceAutoScalingService> _logger;

    public DatabaseInstanceAutoScalingService(
        AdminTenantCatalogDbContext db,
        DatabaseInstanceOptions options,
        ILogger<DatabaseInstanceAutoScalingService> logger)
    {
        _db = db;
        _options = options;
        _logger = logger;
    }

    /// <summary>
    /// Checks if the target database instance has reached its capacity threshold.
    /// If so, provisions a new database instance automatically.
    /// Returns the database instance to use for the new tenant schema.
    /// </summary>
    public async Task<DatabaseInstance> EnsureAvailableInstanceAsync(
        DatabaseInstance currentInstance,
        int reservedSlots = 0,
        CancellationToken cancellationToken = default)
    {
        if (currentInstance == null)
            throw new ArgumentNullException(nameof(currentInstance));

        // Refresh the current schema count from the database.
        var instance = await _db.DatabaseInstances
            .FirstOrDefaultAsync(d => d.Id == currentInstance.Id, cancellationToken)
            ?? throw new InvalidOperationException($"Database instance {currentInstance.Id} not found.");

        var threshold = Math.Max(1, _options.MaxTenantsPerInstance);
        var effectiveSchemaCount = Math.Max(0, instance.CurrentTenantSchemaCount - Math.Max(0, reservedSlots));
        var underConfiguredThreshold = instance.CurrentTenantSchemaCount < _options.MaxTenantsPerInstance;

        _logger.LogInformation(
            "Checking capacity for database instance '{InstanceId}'. "
            + "Current schemas: {CurrentCount}, Reserved slots: {ReservedSlots}, Effective count: {EffectiveCount}, Threshold: {Threshold}, RawUnderThreshold: {UnderThreshold}",
            instance.Id, instance.CurrentTenantSchemaCount, reservedSlots, effectiveSchemaCount, threshold, underConfiguredThreshold);

        // If under threshold, use current instance.
        if (instance.Status == DatabaseInstanceStatus.Active && effectiveSchemaCount < threshold)
        {
            _logger.LogInformation(
                "Database instance '{InstanceId}' has capacity ({CurrentCount}/{Threshold}). Using for new tenant.",
                instance.Id, effectiveSchemaCount, threshold);
            return instance;
        }

        _logger.LogWarning(
            "Database instance '{InstanceId}' has reached capacity ({CurrentCount}/{Threshold}). "
            + "Provisioning new instance.",
            instance.Id, effectiveSchemaCount, threshold);

        // Threshold reached. Provision a new instance.
        var newInstance = await ProvisionNewDatabaseInstanceAsync(instance, cancellationToken);

        _logger.LogInformation(
            "New database instance '{NewInstanceId}' provisioned as overflow. "
            + "Original instance: '{OriginalInstanceId}'.",
            newInstance.Id, instance.Id);

        return newInstance;
    }

    /// <summary>
    /// Increments the schema count for a database instance (called after successful schema creation).
    /// </summary>
    public async Task IncrementTenantCountAsync(Guid databaseInstanceId, CancellationToken cancellationToken = default)
    {
        var instance = await _db.DatabaseInstances
            .FirstOrDefaultAsync(d => d.Id == databaseInstanceId, cancellationToken);

        if (instance == null)
            throw new InvalidOperationException($"Database instance {databaseInstanceId} not found.");

        instance.CurrentTenantSchemaCount += 1;
        _db.DatabaseInstances.Update(instance);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Incremented tenant schema count for database instance '{InstanceId}' to {NewCount}.",
            instance.Id, instance.CurrentTenantSchemaCount);
    }

    /// <summary>
    /// Decrements the schema count for a database instance (called during tenant cleanup/offboarding).
    /// </summary>
    public async Task DecrementTenantCountAsync(Guid databaseInstanceId, CancellationToken cancellationToken = default)
    {
        var instance = await _db.DatabaseInstances
            .FirstOrDefaultAsync(d => d.Id == databaseInstanceId, cancellationToken);

        if (instance == null)
            throw new InvalidOperationException($"Database instance {databaseInstanceId} not found.");

        if (instance.CurrentTenantSchemaCount > 0)
            instance.CurrentTenantSchemaCount -= 1;

        _db.DatabaseInstances.Update(instance);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Decremented tenant schema count for database instance '{InstanceId}' to {NewCount}.",
            instance.Id, instance.CurrentTenantSchemaCount);
    }

    public async Task RecalculateTenantCountsAsync(CancellationToken cancellationToken = default)
    {
        var counts = await _db.Tenants
            .GroupBy(tenant => tenant.DatabaseInstanceId)
            .Select(group => new { DatabaseInstanceId = group.Key, Count = group.Count() })
            .ToDictionaryAsync(item => item.DatabaseInstanceId, item => item.Count, cancellationToken);

        var instances = await _db.DatabaseInstances.ToListAsync(cancellationToken);
        foreach (var instance in instances)
            instance.CurrentTenantSchemaCount = counts.TryGetValue(instance.Id, out var count) ? count : 0;

        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Provisions a new database instance as an overflow target.
    /// In production, this might provision infrastructure (e.g., RDS) and capture connection details.
    /// For now, reuses the same PostgreSQL infrastructure with a new database name.
    /// </summary>
    private async Task<DatabaseInstance> ProvisionNewDatabaseInstanceAsync(
        DatabaseInstance referenceInstance,
        CancellationToken cancellationToken = default)
    {
        // Generate unique identifier for the new instance.
        var allInstances = await _db.DatabaseInstances.ToListAsync(cancellationToken);
        var maxId = allInstances.Max(d => int.TryParse(d.Identifier.Split('-').Last(), out var id) ? id : 0);
        var newInstanceNumber = maxId + 1;
        var newIdentifier = $"db-primary-{newInstanceNumber:D3}";

        // For now, construct a new connection string pointing to a new database on the same PostgreSQL instance.
        // In production, this would provision new infrastructure and retrieve real connection details.
        var templateConnectionString = string.IsNullOrWhiteSpace(_options.DefaultConnectionString)
            ? referenceInstance.ConnectionStringReference
            : _options.DefaultConnectionString;
        var currentBuilder = new NpgsqlConnectionStringBuilder(templateConnectionString);
        var newDatabase = $"opplat_tenants_db{newInstanceNumber}";
        currentBuilder.Database = newDatabase;
        if (currentBuilder.SslMode == SslMode.Prefer)
            currentBuilder.SslMode = SslMode.Disable;
        var newConnectionString = currentBuilder.ConnectionString;

        var newInstance = new DatabaseInstance
        {
            Identifier = newIdentifier,
            ConnectionStringReference = newConnectionString,
            CurrentTenantSchemaCount = 0,
            Status = DatabaseInstanceStatus.Active
        };

        _db.DatabaseInstances.Add(newInstance);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "New database instance '{NewIdentifier}' (DB: {DatabaseName}) provisioned. "
            + "Connection reference stored in catalog.",
            newInstance.Identifier, newDatabase);

        return newInstance;
    }
}
