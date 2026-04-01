using Microsoft.EntityFrameworkCore;
using Opplat.Domain.Entities.Administration;
using Opplat.Infrastructure.Persistance.Data.Administration;

namespace Opplat.AdminApi.Data;

public static class DevDataSeeder
{
    public static async Task SeedAsync(IServiceProvider services, ILogger logger)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AdminTenantCatalogDbContext>();

        logger.LogInformation("Starting development data seeding...");

        await SeedSubscriptionPlansAsync(db, logger);
        await SeedDatabaseInstancesAsync(db, logger);

        logger.LogInformation("Development data seeding completed");
    }

    private static async Task SeedSubscriptionPlansAsync(AdminTenantCatalogDbContext db, ILogger logger)
    {
        if (await db.SubscriptionPlans.AnyAsync())
        {
            logger.LogInformation("Subscription plans already exist, skipping seed");
            return;
        }

        var plans = new[]
        {
            new SubscriptionPlan
            {
                Name = "Starter",
                Description = "Perfect for small businesses getting started",
                PricingMonthly = 29.99m,
                MaxActiveUsers = 5,
                MaxApiCallsPerMonth = 10000,
                MaxStorageGb = 10,
                IsActive = true,
                ResourceLimits = "{\"maxLocations\":1,\"maxMenuItems\":100}"
            },
            new SubscriptionPlan
            {
                Name = "Professional",
                Description = "For growing businesses with multiple locations",
                PricingMonthly = 99.99m,
                MaxActiveUsers = 20,
                MaxApiCallsPerMonth = 100000,
                MaxStorageGb = 50,
                IsActive = true,
                ResourceLimits = "{\"maxLocations\":5,\"maxMenuItems\":500}"
            },
            new SubscriptionPlan
            {
                Name = "Enterprise",
                Description = "For large organizations with complex needs",
                PricingMonthly = 299.99m,
                MaxActiveUsers = 100,
                MaxApiCallsPerMonth = 1000000,
                MaxStorageGb = 500,
                IsActive = true,
                ResourceLimits = "{\"maxLocations\":50,\"maxMenuItems\":5000}"
            }
        };

        await db.SubscriptionPlans.AddRangeAsync(plans);
        await db.SaveChangesAsync();

        logger.LogInformation("Seeded {Count} subscription plans", plans.Length);
    }

    private static async Task SeedDatabaseInstancesAsync(AdminTenantCatalogDbContext db, ILogger logger)
    {
        if (await db.DatabaseInstances.AnyAsync())
        {
            logger.LogInformation("Database instances already exist, skipping seed");
            return;
        }

        var instances = new[]
        {
            new DatabaseInstance
            {
                Identifier = "opplat_tenants_db1",
                ConnectionStringReference = "Host=localhost;Port=5432;Database=opplat_tenants_db1;Username=postgres;Password=Admin123*",
                CurrentTenantSchemaCount = 0,
                Status = DatabaseInstanceStatus.Active
            }
        };

        await db.DatabaseInstances.AddRangeAsync(instances);
        await db.SaveChangesAsync();

        logger.LogInformation("Seeded {Count} database instances", instances.Length);
    }
}
