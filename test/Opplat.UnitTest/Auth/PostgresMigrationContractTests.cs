namespace Opplat.UnitTest.Auth;

public class PostgresMigrationContractTests
{
    [Fact]
    public void MainRuntimeSources_UsePostgresProviders_ForTenantAndDesignTimeFlows()
    {
        var mainProgram = TestRepository.ReadAllText("src", "Opplat.Api.Main", "Program.cs");
        var designTimeFactory = TestRepository.ReadAllText("src", "Opplat.Api.Main", "Data", "DesignTimeDbContextFactory.cs");
        var tenantProvisioning = TestRepository.ReadAllText("src", "Opplat.Api.Main", "Services", "TenantProvisioningService.cs");
        var adminUserQuery = TestRepository.ReadAllText("src", "Opplat.Api.Main", "Features", "Admin", "Queries", "GetAdminUsersQuery.cs");

        Assert.Contains("options.UseNpgsql(connectionString);", mainProgram);
        Assert.Contains("optionsBuilder.UseNpgsql(", designTimeFactory);
        Assert.Contains("optionsBuilder.UseNpgsql(", tenantProvisioning);
        Assert.Contains(".UseNpgsql(PostgresTenantConnectionStringResolver.Resolve(", adminUserQuery);

        Assert.DoesNotContain("UseSqlServer", mainProgram, StringComparison.Ordinal);
        Assert.DoesNotContain("UseSqlServer", designTimeFactory, StringComparison.Ordinal);
        Assert.DoesNotContain("UseSqlServer", tenantProvisioning, StringComparison.Ordinal);
        Assert.DoesNotContain("UseSqlServer", adminUserQuery, StringComparison.Ordinal);
    }

    [Fact]
    public void ComposeAndAppHost_ProvisionPostgresForAllBackendServices()
    {
        var compose = TestRepository.ReadAllText("docker-compose.yml");
        var appHost = TestRepository.ReadAllText("src", "Opplat.AppHost", "Program.cs");

        Assert.Contains("postgres:", compose);
        Assert.DoesNotContain("sqlserver:", compose, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("ConnectionStrings__DefaultConnection=Host=postgres", compose);
        Assert.Contains("ConnectionStrings__MainConnection=Host=postgres", compose);
        Assert.Contains("Finbuckle__MultiTenant__Stores__ConfigurationStore__Tenants__0__ConnectionString=Host=postgres", compose);
        Assert.Contains("Finbuckle__MultiTenant__Stores__ConfigurationStore__Tenants__1__ConnectionString=Host=postgres", compose);
        Assert.Contains("Finbuckle__MultiTenant__Stores__ConfigurationStore__Tenants__2__ConnectionString=Host=postgres", compose);

        Assert.Contains("builder.AddPostgres(\"postgres\"", appHost);
        Assert.DoesNotContain("AddSqlServer", appHost, StringComparison.Ordinal);
        Assert.Contains("builder.AddContainer(\"postgres-bootstrap\", \"postgres\", \"17\")", appHost);
        Assert.Contains(".WaitFor(postgresBootstrap)", appHost);
        Assert.Contains("BuildDatabaseConnectionStringAsync", appHost);
        Assert.Contains("BuildPostgresBootstrapCommand", appHost);
        Assert.DoesNotContain(".AddDatabase(", appHost, StringComparison.Ordinal);
        Assert.DoesNotContain("Host=postgres", appHost, StringComparison.Ordinal);
    }

    [Fact]
    public void LocalDevelopmentDefaults_AndDocs_StopReferencingSqlServer()
    {
        var readme = TestRepository.ReadAllText("README.md");
        var envDocker = TestRepository.ReadAllText(".env.docker");
        var mainSettings = TestRepository.ReadAllText("src", "Opplat.Api.Main", "appsettings.json");
        var salesSettings = TestRepository.ReadAllText("src", "Services", "Sales", "Opplat.Services.Sales.Api", "appsettings.json");
        var inventorySettings = TestRepository.ReadAllText("src", "Services", "Inventory", "Opplat.Services.Inventory.Api", "appsettings.json");

        Assert.Contains("PostgreSQL", readme);
        Assert.DoesNotContain("SQL Server", readme, StringComparison.Ordinal);
        Assert.Contains("POSTGRES_PASSWORD", readme);
        Assert.Contains("POSTGRES_PORT", readme);
        Assert.Contains("PostgreSQL Connection Issues", readme);

        Assert.Contains("POSTGRES_DB=opplat_admin", envDocker);
        Assert.Contains("POSTGRES_USER=postgres", envDocker);
        Assert.Contains("POSTGRES_PASSWORD=Admin123*", envDocker);
        Assert.Contains("POSTGRES_PORT=5432", envDocker);
        Assert.DoesNotContain("SA_PASSWORD", envDocker, StringComparison.Ordinal);
        Assert.DoesNotContain("SQLSERVER_PORT", envDocker, StringComparison.Ordinal);

        Assert.DoesNotContain("Server=", mainSettings, StringComparison.Ordinal);
        Assert.DoesNotContain("Server=", salesSettings, StringComparison.Ordinal);
        Assert.DoesNotContain("Server=", inventorySettings, StringComparison.Ordinal);
        Assert.Contains("Host=localhost;Port=5432;", mainSettings);
        Assert.Contains("Host=localhost;Port=5432;", salesSettings);
        Assert.Contains("Host=localhost;Port=5432;", inventorySettings);
    }
}
