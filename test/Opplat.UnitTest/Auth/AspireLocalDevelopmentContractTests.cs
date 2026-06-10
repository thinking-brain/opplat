namespace Opplat.UnitTest.Auth;

public class AspireLocalDevelopmentContractTests
{
    [Fact]
    public void AppHost_OrchestratesCoreApis_Keycloak_AndClientApps()
    {
        var appHost = TestRepository.ReadAllText("src", "Opplat.AppHost", "Program.cs");
        var appHostProject = TestRepository.ReadAllText("src", "Opplat.AppHost", "Opplat.AppHost.csproj");
        var appHostLaunchSettings = TestRepository.ReadAllText("src", "Opplat.AppHost", "Properties", "launchSettings.json");

        Assert.Contains("builder.AddPostgres(\"postgres\"", appHost);
        Assert.DoesNotContain("AddSqlServer", appHost);
        Assert.Contains("builder.AddContainer(\"postgres-bootstrap\", \"postgres\", \"17\")", appHost);
        Assert.Contains("builder.AddContainer(\"keycloak\"", appHost);
        Assert.Contains("builder.AddProject(", appHost);
        Assert.Contains("\"mainapp\",", appHost);
        Assert.Contains("\"sales-api\",", appHost);
        Assert.Contains("\"inventory-api\",", appHost);
        Assert.Contains("\"admin-api\",", appHost);
        Assert.Contains("builder.AddViteApp(", appHost);
        Assert.Contains("\"client-app\",", appHost);
        Assert.Contains("\"admin-app\",", appHost);
        Assert.Contains("PackageReference Include=\"Aspire.Hosting.JavaScript\"", appHostProject);
        Assert.Contains("Project Sdk=\"Aspire.AppHost.Sdk/13.0.0\"", appHostProject);
        Assert.Contains("ASPIRE_DASHBOARD_OTLP_ENDPOINT_URL", appHostLaunchSettings);
        Assert.Contains("ASPIRE_RESOURCE_SERVICE_ENDPOINT_URL", appHostLaunchSettings);
    }

    [Fact]
    public void AppHost_PreservesExpectedLocalPorts_AuthContracts_AndTenantCatalog()
    {
        var appHost = TestRepository.ReadAllText("src", "Opplat.AppHost", "Program.cs");

        Assert.Contains("port: 8080", appHost);
        Assert.Contains("port: 8082", appHost);
        Assert.Contains("port: 8083", appHost);
        Assert.Contains("port: 8084", appHost);
        Assert.Contains("port: 8180", appHost);
        Assert.Contains("port: 5432", appHost);
        Assert.Contains("endpoint.Port = 3200;", appHost);
        Assert.Contains("endpoint.Port = 3201;", appHost);
        Assert.Contains("Auth__Authority", appHost);
        Assert.Contains("Auth__MetadataAddress", appHost);
        Assert.Contains("VITE_API_URL", appHost);
        Assert.Contains("VITE_AUTH_API_URL", appHost);
        Assert.Contains("VITE_SALES_API_URL", appHost);
        Assert.Contains("VITE_INVENTORY_API_URL", appHost);
        Assert.Contains("VITE_DEV_PROXY_TARGET", appHost);
        Assert.Contains("ASPNETCORE_ENVIRONMENT\", \"Development\"", appHost);

        Assert.Contains("Tenants__0__Identifier\", \"mojocafe\"", appHost);
        Assert.Contains("Tenants__1__Identifier\", \"demo\"", appHost);
        Assert.Contains("Tenants__2__Identifier\", \"test\"", appHost);
        Assert.Contains("name: \"mainapp-http\"", appHost);
        Assert.Contains("name: \"sales-api-http\"", appHost);
        Assert.Contains("name: \"inventory-api-http\"", appHost);
        Assert.Contains("name: \"admin-api-http\"", appHost);
        Assert.Contains("BuildDatabaseConnectionStringAsync", appHost);
        Assert.Contains(".WaitFor(mainApp)", appHost);
        Assert.DoesNotContain("Host=postgres", appHost, StringComparison.Ordinal);
    }

    [Fact]
    public void AppHost_ReusesCommittedKeycloakRealmImport()
    {
        var appHost = TestRepository.ReadAllText("src", "Opplat.AppHost", "Program.cs");

        Assert.Contains("RepoPath(\"docker\", \"keycloak\", \"keycloak.conf\")", appHost);
        Assert.Contains("RepoPath(\"docker\", \"keycloak\", \"opplat-realm.json\")", appHost);
        Assert.Contains("--import-realm", appHost);
        Assert.Contains("--hostname=http://localhost:8180", appHost);
        Assert.Contains("KC_BOOTSTRAP_ADMIN_USERNAME", appHost);
        Assert.Contains("KC_BOOTSTRAP_ADMIN_PASSWORD", appHost);
    }

    [Fact]
    public void AppHost_ResolvesProjectPathsFromRepositoryRoot()
    {
        var appHost = TestRepository.ReadAllText("src", "Opplat.AppHost", "Program.cs");

        Assert.Contains("var repoRoot = FindRepoRoot();", appHost);
        Assert.Contains("File.Exists(Path.Combine(current.FullName, \"opplat.slnx\"))", appHost);
        Assert.Contains("ConfigureProjectDefaults", appHost);
        Assert.Contains("options.ExcludeLaunchProfile = true;", appHost);
        Assert.Contains("options.ExcludeKestrelEndpoints = true;", appHost);
        Assert.Contains("RepoPath(\"src\", \"opplat-react\")", appHost);
        Assert.Contains("RepoPath(\"src\", \"opplat-admin\")", appHost);
        Assert.Contains("RepoPath(\"src\", \"Opplat.Api.Main\", \"Opplat.Api.Main.csproj\")", appHost);
        Assert.Contains("RepoPath(\"src\", \"Opplat.Api.Admin\", \"Opplat.Api.Admin.csproj\")", appHost);
        Assert.Contains("RepoPath(\"src\", \"Services\", \"Sales\", \"Opplat.Services.Sales.Api\", \"Opplat.Services.Sales.Api.csproj\")", appHost);
        Assert.Contains("RepoPath(\"src\", \"Services\", \"Inventory\", \"Opplat.Services.Inventory.Api\", \"Opplat.Services.Inventory.Api.csproj\")", appHost);
    }

    [Fact]
    public void MainApp_SwaggerXmlComments_AreOptionalForAppHostRuntime()
    {
        var mainProgram = TestRepository.ReadAllText("src", "Opplat.Api.Main", "Program.cs");

        Assert.Contains("if (File.Exists(xmlPath))", mainProgram);
        Assert.Contains("c.IncludeXmlComments(xmlPath);", mainProgram);
        Assert.Contains("await ProvisionDevelopmentTenantsAsync(app);", mainProgram);
    }

    [Fact]
    public void Readme_DocumentsAspireWorkflow_AndItsCurrentLimitations()
    {
        var readme = TestRepository.ReadAllText("README.md");

        Assert.Contains("## Quick Start with .NET Aspire", readme);
        Assert.Contains(@"dotnet run --project .\src\Opplat.AppHost\Opplat.AppHost.csproj", readme);
        Assert.Contains("Aspire still relies on Docker Desktop", readme);
        Assert.Contains("AppHost is for local orchestration only", readme);
        Assert.Contains("now also orchestrates the two Vite apps", readme);
        Assert.Contains("The AppHost keeps the Vite apps in development mode", readme);
        Assert.Contains("PostgreSQL: `localhost:5432`", readme);
        Assert.DoesNotContain("SQL Server", readme, StringComparison.Ordinal);
    }

    [Fact]
    public void ClientVitePort_AlignsWithAspireClientIntegrationContract()
    {
        var clientPackage = TestRepository.ReadAllText("src", "opplat-react", "package.json");
        var adminPackage = TestRepository.ReadAllText("src", "opplat-admin", "package.json");
        var clientViteConfig = TestRepository.ReadAllText("src", "opplat-react", "vite.config.ts");
        var adminViteConfig = TestRepository.ReadAllText("src", "opplat-admin", "vite.config.ts");
        var appHost = TestRepository.ReadAllText("src", "Opplat.AppHost", "Program.cs");
        var readme = TestRepository.ReadAllText("README.md");

        Assert.Contains("\"dev:aspire\": \"vite --host 127.0.0.1 --strictPort\"", clientPackage);
        Assert.Contains("\"dev:aspire\": \"node ./node_modules/vite/bin/vite.js --host 127.0.0.1 --strictPort\"", adminPackage);
        Assert.Contains("port: Number(env.PORT || env.VITE_PORT || '3200')", clientViteConfig);
        Assert.Contains("port: Number(env.PORT || env.VITE_PORT || '3201')", adminViteConfig);
        Assert.Contains("strictPort: runningInAspire", clientViteConfig);
        Assert.Contains("strictPort: runningInAspire", adminViteConfig);
        Assert.Contains("\"dev:aspire\")", appHost);
        Assert.Contains(".WithEndpoint(\"http\", endpoint =>", appHost);
        Assert.Contains("endpoint.Port = 3200;", appHost);
        Assert.Contains("endpoint.TargetPort = 3200;", appHost);
        Assert.Contains("endpoint.IsProxied = false;", appHost);
        Assert.Contains(".WithEnvironment(\"PORT\", \"3200\")", appHost);
        Assert.Contains("endpoint.Port = 3201;", appHost);
        Assert.Contains("endpoint.TargetPort = 3201;", appHost);
        Assert.Contains(".WithEnvironment(\"PORT\", \"3201\")", appHost);
        Assert.Contains("OPPLAT_RUNNING_IN_ASPIRE", appHost);
        Assert.Contains("VITE_AUTH_AUTHORITY", appHost);
        Assert.Contains("VITE_ADMIN_API_URL", appHost);
        Assert.Contains("http://localhost:3200", readme);
        Assert.Contains("http://localhost:3201", readme);
        Assert.DoesNotContain("http://localhost:5173", readme);
    }
}
