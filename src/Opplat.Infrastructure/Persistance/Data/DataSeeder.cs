using Opplat.Domain.Entities.Administration;
using Opplat.Infrastructure.Persistance.Data.Administration;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Opplat.Infrastructure.Persistance.Data;

public static class DataSeeder
{
    public const string SubscriptionPlanId = "00000000-0000-0000-0000-000000000001";
    public const string DatabaseInstanceId = "00000000-0000-0000-0000-000000000001";
    public const string SeederUserId = "Seeder";

    public static async Task SeedAsync(
        AdminTenantCatalogDbContext db,
        CancellationToken cancellationToken = default)
    {
        await SeedSubscriptionPlans(db, cancellationToken);
        await SeedDatabaseInstancesAsync(db, cancellationToken);
        await SeedTenants(db, cancellationToken);
    }

    private static async Task SeedSubscriptionPlans(AdminTenantCatalogDbContext db, CancellationToken cancellationToken = default)
    {
        if (await db.SubscriptionPlans.AnyAsync(cancellationToken))
            return;

        var plans = new[]
        {
            new SubscriptionPlan
            {
                Id = Guid.Parse(SubscriptionPlanId),
                Name = "Starter",
                Description = "Perfect for small restaurants and cafes",
                MaxActiveUsers = 5,
                MaxApiCallsPerMonth = 10000,
                MaxStorageGb = 10,
                PricingMonthly = 9.99m,
                ResourceLimits = BuildResourceLimitsJson(5, 10000, 10),
                IsActive = true,
                CreatedBy = SeederUserId,
                CreatedAt = DateTime.UtcNow
            },
            new SubscriptionPlan
            {
                Name = "Professional",
                Description = "For growing businesses",
                MaxActiveUsers = 25,
                MaxApiCallsPerMonth = 100000,
                MaxStorageGb = 100,
                PricingMonthly = 19.99m,
                ResourceLimits = BuildResourceLimitsJson(25, 100000, 100),
                IsActive = true,
                CreatedBy = SeederUserId,
                CreatedAt = DateTime.UtcNow
            },
            new SubscriptionPlan
            {
                Name = "Enterprise",
                Description = "For large-scale operations",
                MaxActiveUsers = 500,
                MaxApiCallsPerMonth = 1000000,
                MaxStorageGb = 1000,
                PricingMonthly = 199.99m,
                ResourceLimits = BuildResourceLimitsJson(500, 1000000, 1000),
                IsActive = true,
                CreatedBy = SeederUserId,
                CreatedAt = DateTime.UtcNow
            }
        };

        db.SubscriptionPlans.AddRange(plans);
        await db.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedDatabaseInstancesAsync(
        AdminTenantCatalogDbContext db,
        CancellationToken cancellationToken = default)
    {
        if (await db.DatabaseInstances.AnyAsync(cancellationToken))
            return;

        var instances = new[]
        {
            new DatabaseInstance
            {
                Id = Guid.Parse(DatabaseInstanceId),
                Identifier = "db-primary-001",
                DatabaseName = "opplat_tenants_db1",
                CurrentTenantSchemaCount = 0,
                Status = DatabaseInstanceStatus.Active,
                CreatedBy = SeederUserId,
                CreatedAt = DateTime.UtcNow
            }
        };

        db.DatabaseInstances.AddRange(instances);
        await db.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedTenants(AdminTenantCatalogDbContext db, CancellationToken cancellationToken = default)
    {
        if (await db.Tenants.AnyAsync(cancellationToken))
            return;

        var tenants = new[]
        {
            new Tenant
            {
                Identifier = "acme-restaurant",
                Name = "Acme Restaurant",
                SubscriptionPlanId = Guid.Parse(SubscriptionPlanId),
                DatabaseInstanceId = Guid.Parse(DatabaseInstanceId),
                DatabaseSchema = "acme_restaurant",
                CreatedBy = SeederUserId,
                CreatedAt = DateTime.UtcNow,
                TenantUsers = [
                    new TenantUser
                    {
                        Id = Guid.NewGuid(),
                        Email = "admin@acme-restaurant.com",
                        EntraOid = Guid.NewGuid().ToString(),
                        IsPrimaryAdmin = true,
                        Role = TenantUserRole.PrimaryAdmin,
                        CreatedBy = SeederUserId,
                        CreatedAt = DateTime.UtcNow
                    }
                ]
            },
            new Tenant
            {
                Identifier = "mojo-pizza",
                Name = "Mojo Pizza",
                SubscriptionPlanId = Guid.Parse(SubscriptionPlanId),
                DatabaseInstanceId = Guid.Parse(DatabaseInstanceId),
                DatabaseSchema = "mojo_pizza",
                CreatedBy = SeederUserId,
                CreatedAt = DateTime.UtcNow,
                TenantUsers = [
                    new TenantUser
                    {
                        Id = Guid.NewGuid(),
                        Email = "admin@mojo-pizza.com",
                        IsPrimaryAdmin = true,
                        EntraOid = Guid.NewGuid().ToString(),
                        Role = TenantUserRole.PrimaryAdmin,
                        CreatedBy = SeederUserId,
                        CreatedAt = DateTime.UtcNow
                    }
                ]
            },
            new Tenant
            {
                Identifier = "own-cafe",
                Name = "Own Cafe",
                SubscriptionPlanId = Guid.Parse(SubscriptionPlanId),
                DatabaseInstanceId = Guid.Parse(DatabaseInstanceId),
                DatabaseSchema = "own_cafe",
                CreatedBy = SeederUserId,
                CreatedAt = DateTime.UtcNow,
                TenantUsers = [
                    new TenantUser
                    {
                        Id = Guid.NewGuid(),
                        Email = "admin@own-cafe.com",
                        IsPrimaryAdmin = true,
                        EntraOid = Guid.NewGuid().ToString(),
                        Role = TenantUserRole.PrimaryAdmin,
                        CreatedBy = SeederUserId,
                        CreatedAt = DateTime.UtcNow
                    }
                ]
            },
        };

        db.Tenants.AddRange(tenants);
        await db.SaveChangesAsync(cancellationToken);
    }

    private static string BuildResourceLimitsJson(int maxActiveUsers, decimal maxApiCallsPerMonth, decimal maxStorageGb) =>
        JsonSerializer.Serialize(new Dictionary<string, decimal>
        {
            ["max_active_users"] = maxActiveUsers,
            ["api_calls_per_month"] = maxApiCallsPerMonth,
            ["storage_quota_gb"] = maxStorageGb
        });
}