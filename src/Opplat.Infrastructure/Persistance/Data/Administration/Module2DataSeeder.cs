using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Opplat.Application.Abstractions.Options;
using Opplat.Domain.Entities.Administration;

namespace Opplat.Infrastructure.Persistance.Data.Administration;

public static class Module2DataSeeder
{
    public static async Task SeedModule2TablesAsync(
        AdminTenantCatalogDbContext db,
        DatabaseInstanceOptions? databaseInstanceOptions = null,
        CancellationToken cancellationToken = default)
    {
        await SeedSubscriptionPlansAsync(db, cancellationToken);
        await SeedDatabaseInstancesAsync(db, databaseInstanceOptions, cancellationToken);
    }

    private static async Task SeedSubscriptionPlansAsync(AdminTenantCatalogDbContext db, CancellationToken cancellationToken = default)
    {
        if (await db.SubscriptionPlans.AnyAsync(cancellationToken))
            return;

        var plans = new[]
        {
            new SubscriptionPlan
            {
                Name = "Starter",
                Description = "Perfect for small restaurants and cafes",
                MaxActiveUsers = 5,
                MaxApiCallsPerMonth = 10000,
                MaxStorageGb = 10,
                PricingMonthly = 29.99m,
                ResourceLimits = BuildResourceLimitsJson(5, 10000, 10),
                IsActive = true
            },
            new SubscriptionPlan
            {
                Name = "Professional",
                Description = "For growing businesses",
                MaxActiveUsers = 25,
                MaxApiCallsPerMonth = 100000,
                MaxStorageGb = 100,
                PricingMonthly = 99.99m,
                ResourceLimits = BuildResourceLimitsJson(25, 100000, 100),
                IsActive = true
            },
            new SubscriptionPlan
            {
                Name = "Enterprise",
                Description = "For large-scale operations",
                MaxActiveUsers = 500,
                MaxApiCallsPerMonth = 1000000,
                MaxStorageGb = 1000,
                PricingMonthly = 499.99m,
                ResourceLimits = BuildResourceLimitsJson(500, 1000000, 1000),
                IsActive = true
            }
        };

        db.SubscriptionPlans.AddRange(plans);
        await db.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedDatabaseInstancesAsync(
        AdminTenantCatalogDbContext db,
        DatabaseInstanceOptions? databaseInstanceOptions,
        CancellationToken cancellationToken = default)
    {
        if (await db.DatabaseInstances.AnyAsync(cancellationToken))
            return;

        var defaultConnectionString = NormalizeConnectionString(
            string.IsNullOrWhiteSpace(databaseInstanceOptions?.DefaultConnectionString)
                ? "Host=localhost;Port=5432;Database=opplat_tenants_db1;Username=postgres;Password=Admin123*"
                : databaseInstanceOptions!.DefaultConnectionString);

        var instances = new[]
        {
            new DatabaseInstance
            {
                Identifier = "db-primary-001",
                ConnectionStringReference = defaultConnectionString,
                CurrentTenantSchemaCount = 0,
                Status = DatabaseInstanceStatus.Active
            }
        };

        db.DatabaseInstances.AddRange(instances);
        await db.SaveChangesAsync(cancellationToken);
    }

    private static string BuildResourceLimitsJson(int maxActiveUsers, decimal maxApiCallsPerMonth, decimal maxStorageGb) =>
        JsonSerializer.Serialize(new Dictionary<string, decimal>
        {
            ["max_active_users"] = maxActiveUsers,
            ["api_calls_per_month"] = maxApiCallsPerMonth,
            ["storage_quota_gb"] = maxStorageGb
        });

    private static string NormalizeConnectionString(string connectionString)
    {
        var builder = new NpgsqlConnectionStringBuilder(connectionString);
        if (builder.SslMode == SslMode.Prefer)
            builder.SslMode = SslMode.Disable;

        return builder.ConnectionString;
    }
}
