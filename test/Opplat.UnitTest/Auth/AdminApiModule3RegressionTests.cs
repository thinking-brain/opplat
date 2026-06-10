using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Opplat.Application.Abstractions.Options;
using Opplat.Domain.Entities.Administration;
using Opplat.Infrastructure.Persistance.Data.Administration;
using Opplat.Infrastructure.Services;

namespace Opplat.UnitTest.Auth;

public class AdminApiModule3RegressionTests
{
    [Fact]
    public async Task DatabaseInstanceAutoScaling_UsesConfiguredThresholdAndKeepsCurrentInstanceBelowCapacity()
    {
        var options = CreateOptions();

        await using var setup = new AdminTenantCatalogDbContext(options);
        setup.DatabaseInstances.Add(new DatabaseInstance
        {
            Identifier = "db-primary-001",
            DatabaseName = "opplat_tenants_db1",
            CurrentTenantSchemaCount = 1,
            Status = DatabaseInstanceStatus.Active
        });
        await setup.SaveChangesAsync();

        var service = new DatabaseInstanceAutoScalingService(
            setup,
            new DatabaseInstanceOptions { MaxTenantsPerInstance = 2 },
            NullLogger<DatabaseInstanceAutoScalingService>.Instance);

        var currentInstance = await setup.DatabaseInstances.SingleAsync();
        var resolvedInstance = await service.EnsureAvailableInstanceAsync(currentInstance);

        Assert.Equal(currentInstance.Id, resolvedInstance.Id);
        Assert.Equal("db-primary-001", resolvedInstance.Identifier);
        Assert.Equal(1, resolvedInstance.CurrentTenantSchemaCount);
        Assert.Equal(1, await setup.DatabaseInstances.CountAsync());
    }

    [Fact]
    public async Task DatabaseInstanceAutoScaling_ProvisionsOverflowInstanceWhenThresholdReached()
    {
        var options = CreateOptions();

        await using var setup = new AdminTenantCatalogDbContext(options);
        setup.DatabaseInstances.Add(new DatabaseInstance
        {
            Identifier = "db-primary-001",
            DatabaseName = "opplat_tenants_db1",
            CurrentTenantSchemaCount = 2,
            Status = DatabaseInstanceStatus.Active
        });
        await setup.SaveChangesAsync();

        var service = new DatabaseInstanceAutoScalingService(
            setup,
            new DatabaseInstanceOptions { MaxTenantsPerInstance = 2 },
            NullLogger<DatabaseInstanceAutoScalingService>.Instance);

        var currentInstance = await setup.DatabaseInstances.SingleAsync();
        var resolvedInstance = await service.EnsureAvailableInstanceAsync(currentInstance);

        Assert.NotEqual(currentInstance.Id, resolvedInstance.Id);
        Assert.Equal("db-primary-002", resolvedInstance.Identifier);
        Assert.Equal("opplat_tenants_db2", resolvedInstance.DatabaseName);
        Assert.Equal(0, resolvedInstance.CurrentTenantSchemaCount);
        Assert.Equal(DatabaseInstanceStatus.Active, resolvedInstance.Status);
        Assert.Equal(2, await setup.DatabaseInstances.CountAsync());
    }

    [Fact]
    public async Task DatabaseInstanceAutoScaling_CountMutationsTrackProvisioningWithoutGoingNegative()
    {
        var options = CreateOptions();

        await using var setup = new AdminTenantCatalogDbContext(options);
        setup.DatabaseInstances.Add(new DatabaseInstance
        {
            Identifier = "db-primary-001",
            DatabaseName = "opplat_tenants_db1",
            CurrentTenantSchemaCount = 0,
            Status = DatabaseInstanceStatus.Active
        });
        await setup.SaveChangesAsync();

        var service = new DatabaseInstanceAutoScalingService(
            setup,
            new DatabaseInstanceOptions { MaxTenantsPerInstance = 5 },
            NullLogger<DatabaseInstanceAutoScalingService>.Instance);

        var instanceId = (await setup.DatabaseInstances.SingleAsync()).Id;

        await service.IncrementTenantCountAsync(instanceId);
        await service.IncrementTenantCountAsync(instanceId);
        await service.DecrementTenantCountAsync(instanceId);
        await service.DecrementTenantCountAsync(instanceId);
        await service.DecrementTenantCountAsync(instanceId);

        var persistedInstance = await setup.DatabaseInstances.SingleAsync();
        Assert.Equal(0, persistedInstance.CurrentTenantSchemaCount);
    }

    [Fact]
    public void TenantSchemaProvisioning_SourceContract_RemainsIdempotentAndSchemaScoped()
    {
        var source = TestRepository.ReadAllText("src", "Opplat.Api.Admin", "Services", "TenantSchemaProvisioningService.cs");

        Assert.Contains("SchemaExistsAsync(connection, tenant.DatabaseSchema, cancellationToken)", source);
        Assert.Contains("Skipping creation (idempotent)", source, StringComparison.Ordinal);
        Assert.Contains("CREATE SCHEMA \\\"{tenant.DatabaseSchema}\\\" AUTHORIZATION postgres;", source);
        Assert.Contains("DROP SCHEMA IF EXISTS \\\"{tenant.DatabaseSchema}\\\" CASCADE;", source);
        Assert.Contains("SELECT 1 FROM information_schema.schemata", source);
    }

    [Fact]
    public void TenantSchemaMigrationRunner_SourceContract_LocksPerTenantBulkAndRollbackSeams()
    {
        var source = TestRepository.ReadAllText("src", "Opplat.Api.Admin", "Services", "TenantSchemaMigrationRunner.cs");

        Assert.Contains("SET search_path TO \\\"{tenant.DatabaseSchema}\\\"; {migrationSql}", source);
        Assert.Contains(".Where(t => t.Status == TenantStatus.Active)", source);
        Assert.Contains("batchSize ??= 10", source);
        Assert.Contains("delayBetweenBatches ??= TimeSpan.FromSeconds(5)", source);
        Assert.Contains("await Task.WhenAll(batchTasks)", source);
        Assert.Contains("schema_migrations table for rollback tracking", source);
        Assert.Contains("TenantMigrationStatus.Success", source);
        Assert.Contains("TenantMigrationStatus.Failed", source);
    }

    [Fact]
    public void Module3OptionsAndServices_SourceContract_KeepThresholdConfigExternalized()
    {
        var optionsSource = TestRepository.ReadAllText("src", "Opplat.Api.Admin", "Configuration", "DatabaseInstanceOptions.cs");
        var autoScalingSource = TestRepository.ReadAllText("src", "Opplat.Api.Admin", "Services", "DatabaseInstanceAutoScalingService.cs");

        Assert.Contains("SectionName = \"DatabaseInstance\"", optionsSource);
        Assert.Contains("public int MaxTenantsPerInstance", optionsSource);
        Assert.Contains("_options.MaxTenantsPerInstance", autoScalingSource);
        Assert.Contains("instance.CurrentTenantSchemaCount < _options.MaxTenantsPerInstance", autoScalingSource);
        Assert.Contains("ProvisionNewDatabaseInstanceAsync", autoScalingSource);
    }

    private static DbContextOptions<AdminTenantCatalogDbContext> CreateOptions()
    {
        return new DbContextOptionsBuilder<AdminTenantCatalogDbContext>()
            .UseInMemoryDatabase($"admin-module3-{Guid.NewGuid():N}")
            .Options;
    }
}
