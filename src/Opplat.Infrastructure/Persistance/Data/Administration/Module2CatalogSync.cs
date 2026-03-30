using Microsoft.EntityFrameworkCore;
using Npgsql;
using Opplat.Domain.Entities.Administration;

namespace Opplat.Infrastructure.Persistance.Data.Administration;

internal static class Module2CatalogSync
{
    public static async Task SynchronizeLegacyTenantAsync(
        AdminTenantCatalogDbContext db,
        AdminTenantInfo legacyTenant,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(db);
        ArgumentNullException.ThrowIfNull(legacyTenant);

        var plan = await db.SubscriptionPlans
            .OrderBy(plan => plan.Id)
            .FirstOrDefaultAsync(plan => plan.IsActive, cancellationToken)
            ?? throw new InvalidOperationException("At least one active subscription plan must exist before tenants can be synchronized.");

        var databaseInstance = await ResolveDatabaseInstanceAsync(db, legacyTenant.DatabaseName, cancellationToken);

        var tenant = await db.Tenants
            .Include(model => model.TenantUsers)
            .FirstOrDefaultAsync(model => model.Id == legacyTenant.Id, cancellationToken)
            ?? await db.Tenants
                .Include(model => model.TenantUsers)
                .FirstOrDefaultAsync(model => model.Identifier == legacyTenant.Identifier, cancellationToken);

        var now = DateTime.UtcNow;
        if (tenant is null)
        {
            tenant = new Tenant
            {
                Id = legacyTenant.Id,
                Identifier = legacyTenant.Identifier,
                Name = legacyTenant.Name,
                Status = legacyTenant.IsActive ? TenantStatus.Active : TenantStatus.Inactive,
                SubscriptionPlanId = plan.Id,
                DatabaseInstanceId = databaseInstance.Id,
                DatabaseSchema = NormalizeDatabaseSchema(legacyTenant.DatabaseSchema, legacyTenant.Identifier),
                CreatedAt = now,
                InactivatedAt = legacyTenant.IsActive ? null : now,
                DatabaseName = string.Empty,                
            };

            db.Tenants.Add(tenant);
        }
        else
        {
            tenant.Identifier = legacyTenant.Identifier;
            tenant.Name = legacyTenant.Name;
            tenant.DatabaseInstanceId = databaseInstance.Id;
            tenant.DatabaseSchema = NormalizeDatabaseSchema(legacyTenant.DatabaseSchema, legacyTenant.Identifier);
            tenant.Status = legacyTenant.IsActive ? TenantStatus.Active : TenantStatus.Inactive;
            tenant.InactivatedAt = legacyTenant.IsActive ? null : tenant.InactivatedAt ?? now;

            if (tenant.SubscriptionPlanId == default)
                tenant.SubscriptionPlanId = plan.Id;
        }

        await db.SaveChangesAsync(cancellationToken);
        await EnsurePrimaryAdminAsync(db, tenant, legacyTenant.IsActive, cancellationToken);
        await RecalculateDatabaseInstanceCountsAsync(db, cancellationToken);
    }

    private static async Task EnsurePrimaryAdminAsync(
        AdminTenantCatalogDbContext db,
        Tenant tenant,
        bool isTenantActive,
        CancellationToken cancellationToken)
    {
        var primaryAdmin = await db.TenantUsers
            .FirstOrDefaultAsync(
                user => user.TenantId == tenant.Id && user.IsPrimaryAdmin,
                cancellationToken);

        var normalizedIdentifier = NormalizeIdentifier(tenant.Identifier);
        if (primaryAdmin is null)
        {
            db.TenantUsers.Add(new TenantUser
            {
                EntraOid = $"local-{normalizedIdentifier}-primary-admin",
                TenantId = tenant.Id,
                Email = $"admin@{normalizedIdentifier}.local",
                Role = TenantUserRole.PrimaryAdmin,
                IsPrimaryAdmin = true,
                IsActive = isTenantActive
            });
        }
        else
        {
            primaryAdmin.Role = TenantUserRole.PrimaryAdmin;
            primaryAdmin.IsPrimaryAdmin = true;
            primaryAdmin.IsActive = isTenantActive;
            primaryAdmin.DeactivatedAt = isTenantActive ? null : primaryAdmin.DeactivatedAt ?? DateTime.UtcNow;
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    private static async Task<DatabaseInstance> ResolveDatabaseInstanceAsync(
        AdminTenantCatalogDbContext db,
        string databaseName,
        CancellationToken cancellationToken)
    {
        var normalizedDatabaseName = NormalizeDatabaseName(databaseName);
        var instances = await db.DatabaseInstances
            .OrderBy(instance => instance.Id)
            .ToListAsync(cancellationToken);

        foreach (var instance in instances)
        {
            if (string.Equals(ReadDatabaseName(instance.ConnectionStringReference), normalizedDatabaseName, StringComparison.OrdinalIgnoreCase))
                return instance;
        }

        var activeInstance = instances.FirstOrDefault(instance => instance.Status == DatabaseInstanceStatus.Active);
        if (activeInstance is not null)
            return activeInstance;

        var templateConnectionString = instances
            .Select(instance => instance.ConnectionStringReference)
            .FirstOrDefault(connectionString => !string.IsNullOrWhiteSpace(connectionString));

        var connectionStringBuilder = string.IsNullOrWhiteSpace(templateConnectionString)
            ? new NpgsqlConnectionStringBuilder
            {
                Host = "localhost",
                Port = 5432,
                Username = "postgres",
                Password = "Admin123*"
            }
            : new NpgsqlConnectionStringBuilder(templateConnectionString);

        connectionStringBuilder.Database = normalizedDatabaseName;
        if (connectionStringBuilder.SslMode == SslMode.Prefer)
            connectionStringBuilder.SslMode = SslMode.Disable;

        var newInstance = new DatabaseInstance
        {
            Identifier = $"db-{NormalizeIdentifier(normalizedDatabaseName)}",
            ConnectionStringReference = connectionStringBuilder.ConnectionString,
            CurrentTenantSchemaCount = 0,
            Status = DatabaseInstanceStatus.Active
        };

        db.DatabaseInstances.Add(newInstance);
        await db.SaveChangesAsync(cancellationToken);
        return newInstance;
    }

    private static async Task RecalculateDatabaseInstanceCountsAsync(
        AdminTenantCatalogDbContext db,
        CancellationToken cancellationToken)
    {
        var counts = await db.Tenants
            .GroupBy(tenant => tenant.DatabaseInstanceId)
            .Select(group => new { DatabaseInstanceId = group.Key, Count = group.Count() })
            .ToDictionaryAsync(item => item.DatabaseInstanceId, item => item.Count, cancellationToken);

        var instances = await db.DatabaseInstances.ToListAsync(cancellationToken);
        foreach (var instance in instances)
            instance.CurrentTenantSchemaCount = counts.TryGetValue(instance.Id, out var count) ? count : 0;

        await db.SaveChangesAsync(cancellationToken);
    }

    private static string NormalizeDatabaseName(string? databaseName)
    {
        var normalized = databaseName?.Trim();
        return string.IsNullOrWhiteSpace(normalized) ? "opplat_tenants" : normalized;
    }

    private static string NormalizeDatabaseSchema(string? databaseSchema, string? tenantIdentifier)
    {
        var normalized = databaseSchema?.Trim();
        if (!string.IsNullOrWhiteSpace(normalized))
            return normalized;

        return $"tenant_{NormalizeIdentifier(tenantIdentifier)}";
    }

    private static string ReadDatabaseName(string connectionString)
    {
        try
        {
            return new NpgsqlConnectionStringBuilder(connectionString).Database ?? string.Empty;
        }
        catch (ArgumentException)
        {
            return string.Empty;
        }
    }

    private static string NormalizeIdentifier(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "default";

        return new string(value
            .Trim()
            .ToLowerInvariant()
            .Select(ch => char.IsLetterOrDigit(ch) ? ch : '-')
            .ToArray())
            .Trim('-');
    }
}
