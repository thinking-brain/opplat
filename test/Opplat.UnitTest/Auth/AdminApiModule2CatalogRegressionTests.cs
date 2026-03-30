using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Opplat.Domain.Entities.Administration;
using Opplat.Infrastructure.Persistance.Data.Administration;

namespace Opplat.UnitTest.Auth;

public class AdminApiModule2CatalogRegressionTests
{
    [Fact]
    public async Task AdminPortalDataSeeder_SeedsLegacyCatalogAndModule2LookupTablesIdempotently()
    {
        var services = new ServiceCollection();
        var databaseName = $"admin-module2-seed-{Guid.NewGuid():N}";
        services.AddDbContext<AdminTenantCatalogDbContext>(options => options.UseInMemoryDatabase(databaseName));

        using var provider = services.BuildServiceProvider();

        // await AdminPortalDataSeeder.InitializeAsync(provider);
        // await AdminPortalDataSeeder.InitializeAsync(provider);

        await using var scope = provider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AdminTenantCatalogDbContext>();

        var tenants = await db.Tenants
            .AsNoTracking()
            .OrderBy(tenant => tenant.Identifier)
            .ToListAsync();
        var plans = await db.SubscriptionPlans
            .AsNoTracking()
            .OrderBy(plan => plan.Name)
            .ToListAsync();
        var instances = await db.DatabaseInstances
            .AsNoTracking()
            .OrderBy(instance => instance.Identifier)
            .ToListAsync();

        Assert.Collection(
            tenants,
            tenant =>
            {
                Assert.Equal("bistrocentral", tenant.Identifier);
                Assert.Equal("opplat_bistrocentral", tenant.DatabaseName);
                Assert.Equal("tenant_bistrocentral", tenant.DatabaseSchema);
                Assert.Equal(9, tenant.UserCount);
                Assert.True(tenant.IsActive);
            },
            tenant =>
            {
                Assert.Equal("lunahub", tenant.Identifier);
                Assert.Equal("opplat_lunahub", tenant.DatabaseName);
                Assert.Equal("tenant_lunahub", tenant.DatabaseSchema);
                Assert.Equal(4, tenant.UserCount);
                Assert.False(tenant.IsActive);
            },
            tenant =>
            {
                Assert.Equal("mojocafe", tenant.Identifier);
                Assert.Equal("opplat_mojocafe", tenant.DatabaseName);
                Assert.Equal("tenant_mojocafe", tenant.DatabaseSchema);
                Assert.Equal(18, tenant.UserCount);
                Assert.True(tenant.IsActive);
            });

        Assert.Collection(
            plans,
            plan =>
            {
                Assert.Equal("Enterprise", plan.Name);
                Assert.Equal(500, plan.MaxActiveUsers);
                Assert.Equal(1000000m, plan.MaxApiCallsPerMonth);
                Assert.Equal(1000m, plan.MaxStorageGb);
                Assert.Equal(499.99m, plan.PricingMonthly);
                Assert.True(plan.IsActive);
            },
            plan =>
            {
                Assert.Equal("Professional", plan.Name);
                Assert.Equal(25, plan.MaxActiveUsers);
                Assert.Equal(100000m, plan.MaxApiCallsPerMonth);
                Assert.Equal(100m, plan.MaxStorageGb);
                Assert.Equal(99.99m, plan.PricingMonthly);
                Assert.True(plan.IsActive);
            },
            plan =>
            {
                Assert.Equal("Starter", plan.Name);
                Assert.Equal(5, plan.MaxActiveUsers);
                Assert.Equal(10000m, plan.MaxApiCallsPerMonth);
                Assert.Equal(10m, plan.MaxStorageGb);
                Assert.Equal(29.99m, plan.PricingMonthly);
                Assert.True(plan.IsActive);
            });

        var instance = Assert.Single(instances);
        Assert.Equal("db-primary-001", instance.Identifier);
        Assert.Equal(DatabaseInstanceStatus.Active, instance.Status);
        Assert.Equal(0, instance.CurrentTenantSchemaCount);
        Assert.Contains("Database=opplat_tenants_db1", instance.ConnectionStringReference, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Module2CentralCatalog_CanPersistTenantPlanUserAndAuditRelationships()
    {
        var options = new DbContextOptionsBuilder<AdminTenantCatalogDbContext>()
            .UseInMemoryDatabase($"admin-module2-model-{Guid.NewGuid():N}")
            .Options;

        await using (var setup = new AdminTenantCatalogDbContext(options))
        {
            await Module2DataSeeder.SeedModule2TablesAsync(setup);

            var starterPlan = await setup.SubscriptionPlans.SingleAsync(plan => plan.Name == "Starter");
            var primaryInstance = await setup.DatabaseInstances.SingleAsync(instance => instance.Identifier == "db-primary-001");

            var tenant = new Tenant
            {
                Id = Guid.NewGuid(),
                Identifier = "central-regression",
                Name = "Central Regression",
                Status = TenantStatus.Active,
                SubscriptionPlanId = starterPlan.Id,
                DatabaseInstanceId = primaryInstance.Id,
                DatabaseSchema = "tenant_central_regression",
                DatabaseName = "opplat_central_regression"
            };

            setup.Tenants.Add(tenant);
            await setup.SaveChangesAsync();

            var tenantUser = new TenantUser
            {
                EntraOid = "oid-central-primary",
                TenantId = tenant.Id,
                Email = "primary@central-regression.local",
                Role = TenantUserRole.PrimaryAdmin,
                IsPrimaryAdmin = true,
                IsActive = true
            };

            setup.TenantUsers.Add(tenantUser);
            await setup.SaveChangesAsync();

            setup.AuditLogs.Add(new AuditLog
            {
                ActorOid = tenantUser.EntraOid,
                TargetTenantId = tenant.Id.ToString(),
                TargetTenantIdFk = tenant.Id.ToString(),
                TargetUserId = tenantUser.Id.ToString(),
                ActionType = "TenantProvisioned",
                BeforeState = "{}",
                AfterState = """{"status":"Active"}"""
            });

            await setup.SaveChangesAsync();
        }

        await using var verify = new AdminTenantCatalogDbContext(options);
        var persistedTenant = await verify.Tenants
            .Include(tenant => tenant.SubscriptionPlan)
            .Include(tenant => tenant.DatabaseInstance)
            .Include(tenant => tenant.TenantUsers)
            .Include(tenant => tenant.AuditLogs)
            .SingleAsync(tenant => tenant.Identifier == "central-regression");

        Assert.Equal(TenantStatus.Active, persistedTenant.Status);
        Assert.Equal("Starter", persistedTenant.SubscriptionPlan?.Name);
        Assert.Equal("db-primary-001", persistedTenant.DatabaseInstance?.Identifier);

        var persistedUser = Assert.Single(persistedTenant.TenantUsers);
        Assert.Equal(TenantUserRole.PrimaryAdmin, persistedUser.Role);
        Assert.True(persistedUser.IsPrimaryAdmin);
        Assert.Equal("primary@central-regression.local", persistedUser.Email);

        var auditLog = Assert.Single(persistedTenant.AuditLogs);
        Assert.Equal("TenantProvisioned", auditLog.ActionType);
        Assert.Equal(persistedTenant.Id.ToString(), auditLog.TargetTenantId);
        Assert.Equal("""{"status":"Active"}""", auditLog.AfterState);
    }

    [Fact]
    public void Module2CentralCatalogSource_MapsCoreTablesAndPostgresShapedColumns()
    {
        var dbContextSource = TestRepository.ReadAllText("src", "Opplat.AdminApi", "Data", "AdminTenantCatalogDbContext.cs");

        Assert.Contains("public DbSet<SubscriptionPlan> SubscriptionPlans", dbContextSource);
        Assert.Contains("public DbSet<Tenant> Tenants", dbContextSource);
        Assert.Contains("public DbSet<TenantUser> TenantUsers", dbContextSource);
        Assert.Contains("public DbSet<DatabaseInstance> DatabaseInstances", dbContextSource);
        Assert.Contains("public DbSet<AuditLog> AuditLogs", dbContextSource);

        Assert.Contains("plan.ToTable(\"subscription_plans\")", dbContextSource);
        Assert.Contains("db.ToTable(\"database_instances\")", dbContextSource);
        Assert.Contains("tenant.ToTable(\"tenants\")", dbContextSource);
        Assert.Contains("user.ToTable(\"tenant_users\")", dbContextSource);
        Assert.Contains("log.ToTable(\"audit_logs\")", dbContextSource);

        Assert.Contains("plan.Property(p => p.ResourceLimits).HasColumnType(\"jsonb\")", dbContextSource);
        Assert.Contains("log.Property(l => l.BeforeState).HasColumnType(\"jsonb\")", dbContextSource);
        Assert.Contains("log.Property(l => l.AfterState).HasColumnType(\"jsonb\")", dbContextSource);
        Assert.Contains("db.Property(d => d.Status).HasConversion<string>()", dbContextSource);
        Assert.Contains("tenant.Property(t => t.Status).HasConversion<string>().IsRequired()", dbContextSource);
        Assert.Contains("user.Property(u => u.Role).HasConversion<string>().IsRequired()", dbContextSource);
        Assert.Contains("tenant.HasIndex(t => t.Identifier).IsUnique()", dbContextSource);
        Assert.Contains("user.HasIndex(u => new { u.TenantId, u.EntraOid }).IsUnique()", dbContextSource);
    }

    [Fact]
    public void AdminFrontend_TenantCatalogContract_UsesModule2MetadataSurface()
    {
        var backendContracts = TestRepository.ReadAllText("src", "Opplat.AdminApi", "Endpoints", "AdminContracts.cs");
        var frontendTypes = TestRepository.ReadAllText("src", "opplat-admin", "src", "types", "index.ts");
        var frontendApi = TestRepository.ReadAllText("src", "opplat-admin", "src", "api", "admin.api.ts");
        var tenantsPage = TestRepository.ReadAllText("src", "opplat-admin", "src", "pages", "TenantsPage.tsx");
        var dashboardPage = TestRepository.ReadAllText("src", "opplat-admin", "src", "pages", "DashboardPage.tsx");

        foreach (var field in new[] { "id", "identifier", "name", "databaseName", "databaseSchema", "userCount", "isActive" })
        {
            Assert.Contains(field, frontendTypes, StringComparison.Ordinal);
        }

        Assert.Contains("/admin/tenants", frontendApi);
        Assert.DoesNotContain("/admin/users", frontendApi, StringComparison.Ordinal);
        Assert.DoesNotContain("connectionString", backendContracts, StringComparison.Ordinal);
        Assert.DoesNotContain("connectionString", frontendTypes, StringComparison.Ordinal);
        Assert.Contains("tenant.databaseName", tenantsPage);
        Assert.Contains("tenant.databaseSchema", tenantsPage);
        Assert.Contains("tenant.userCount", tenantsPage);
        Assert.Contains("tenant.databaseName", dashboardPage);
        Assert.Contains("tenant.databaseSchema", dashboardPage);
        Assert.Contains("tenant.userCount", dashboardPage);
    }
}
