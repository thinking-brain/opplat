using Aspire.Hosting.ApplicationModel;
using Npgsql;

const string PostgresPasswordValue = "Admin123*";
const string KeycloakRealm = "opplat";
const string KeycloakAuthority = $"http://localhost:8180/realms/{KeycloakRealm}";
const string KeycloakMetadataAddress = $"{KeycloakAuthority}/.well-known/openid-configuration";
const string MainDatabaseName = "opplat_main";
const string MojocafeDatabaseName = "opplat_mojocafe";
const string DemoDatabaseName = "opplat_demo";
const string TestDatabaseName = "opplat_test";
const string AdminDatabaseName = "opplat_admin";
const string MojocafeSchema = "tenant_mojocafe";
const string DemoSchema = "tenant_demo";
const string TestSchema = "tenant_test";

var repoRoot = FindRepoRoot();
var builder = DistributedApplication.CreateBuilder(args);

var postgresUser = builder.AddParameter("postgres-user", "postgres");
var postgresPassword = builder.AddParameter("postgres-password", PostgresPasswordValue, secret: true);

var postgres = builder.AddPostgres("postgres", postgresUser, postgresPassword, port: 5432)
    .WithLifetime(ContainerLifetime.Persistent);

var postgresBootstrap = builder.AddContainer("postgres-bootstrap", "postgres", "17")
    .WithReference(postgres)
    .WaitFor(postgres)
    .WithEnvironment("PGPASSWORD", PostgresPasswordValue)
    .WithEntrypoint("sh")
    .WithArgs("-c", BuildPostgresBootstrapCommand());

var keycloak = builder.AddContainer("keycloak", "quay.io/keycloak/keycloak", "26.0")
    .WithBindMount(RepoPath("docker", "keycloak", "keycloak.conf"), "/opt/keycloak/conf/keycloak.conf", isReadOnly: true)
    .WithBindMount(RepoPath("docker", "keycloak", "opplat-realm.json"), "/opt/keycloak/data/import/opplat-realm.json", isReadOnly: true)
    .WithHttpEndpoint(port: 8180, targetPort: 8180, name: "keycloak-http")
    .WithArgs("start-dev", "--import-realm", "--hostname=http://localhost:8180", "--hostname-backchannel-dynamic=true")
    .WithEnvironment("KC_BOOTSTRAP_ADMIN_USERNAME", "admin")
    .WithEnvironment("KC_BOOTSTRAP_ADMIN_PASSWORD", "admin");

var mainApp = builder.AddProject(
        "mainapp",
        RepoPath("src", "Opplat.MainApp", "Opplat.MainApp.csproj"),
        ConfigureProjectDefaults)
    .WithReference(postgres)
    .WaitFor(postgresBootstrap)
    .WithHttpEndpoint(port: 8080, name: "mainapp-http")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment(context => ConfigureTenantAwareApiEnvironmentAsync(
        context,
        postgres.Resource,
        MainDatabaseName,
        (MojocafeDatabaseName, MojocafeSchema),
        (DemoDatabaseName, DemoSchema),
        (TestDatabaseName, TestSchema)))
    .WithEnvironment("Auth__Authority", KeycloakAuthority)
    .WithEnvironment("Auth__MetadataAddress", KeycloakMetadataAddress)
    .WithEnvironment("Auth__Audience", "opplat-api")
    .WithEnvironment("Auth__ClientIdClient", "opplat-client")
    .WithEnvironment("Finbuckle__MultiTenant__Stores__ConfigurationStore__Tenants__0__Id", "mojocafe")
    .WithEnvironment("Finbuckle__MultiTenant__Stores__ConfigurationStore__Tenants__0__Identifier", "mojocafe")
    .WithEnvironment("Finbuckle__MultiTenant__Stores__ConfigurationStore__Tenants__0__Name", "MojoCafe")
    .WithEnvironment("Finbuckle__MultiTenant__Stores__ConfigurationStore__Tenants__0__DatabaseName", MojocafeDatabaseName)
    .WithEnvironment("Finbuckle__MultiTenant__Stores__ConfigurationStore__Tenants__0__DatabaseSchema", MojocafeSchema)
    .WithEnvironment("Finbuckle__MultiTenant__Stores__ConfigurationStore__Tenants__1__Id", "demo")
    .WithEnvironment("Finbuckle__MultiTenant__Stores__ConfigurationStore__Tenants__1__Identifier", "demo")
    .WithEnvironment("Finbuckle__MultiTenant__Stores__ConfigurationStore__Tenants__1__Name", "Demo")
    .WithEnvironment("Finbuckle__MultiTenant__Stores__ConfigurationStore__Tenants__1__DatabaseName", DemoDatabaseName)
    .WithEnvironment("Finbuckle__MultiTenant__Stores__ConfigurationStore__Tenants__1__DatabaseSchema", DemoSchema)
    .WithEnvironment("Finbuckle__MultiTenant__Stores__ConfigurationStore__Tenants__2__Id", "test")
    .WithEnvironment("Finbuckle__MultiTenant__Stores__ConfigurationStore__Tenants__2__Identifier", "test")
    .WithEnvironment("Finbuckle__MultiTenant__Stores__ConfigurationStore__Tenants__2__Name", "Test")
    .WithEnvironment("Finbuckle__MultiTenant__Stores__ConfigurationStore__Tenants__2__DatabaseName", TestDatabaseName)
    .WithEnvironment("Finbuckle__MultiTenant__Stores__ConfigurationStore__Tenants__2__DatabaseSchema", TestSchema);

var adminApi = builder.AddProject(
        "admin-api",
        RepoPath("src", "Opplat.AdminApi", "Opplat.AdminApi.csproj"),
        ConfigureProjectDefaults)
    .WithReference(postgres)
    .WaitFor(postgresBootstrap)
    .WithHttpEndpoint(port: 8084, name: "admin-api-http")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment(context => ConfigureAdminApiEnvironmentAsync(context, postgres.Resource, AdminDatabaseName))
    .WithEnvironment("Auth__Authority", KeycloakAuthority)
    .WithEnvironment("Auth__MetadataAddress", KeycloakMetadataAddress)
    .WithEnvironment("Auth__Audience", "opplat-api")
    .WithEnvironment("Auth__AdminBff__ClientId", "opplat-admin")
    .WithEnvironment("Auth__AdminBff__DefaultOrigin", "http://localhost:3201")
    .WithEnvironment("Auth__AdminBff__AllowedOrigins__0", "http://localhost:3101")
    .WithEnvironment("Auth__AdminBff__AllowedOrigins__1", "http://localhost:3201")
    .WithEnvironment("Auth__AdminBff__AllowedOrigins__2", "http://localhost:5174");

builder.AddViteApp(
        "client-app",
        RepoPath("src", "opplat-react"),
        "dev:aspire")
    .WithEndpoint("http", endpoint =>
    {
        endpoint.Port = 3200;
        endpoint.TargetPort = 3200;
        endpoint.IsProxied = false;
    })
    .WithEnvironment("PORT", "3200")
    .WaitFor(mainApp)
    .WaitFor(keycloak)
    .WithEnvironment("BROWSER", "none")
    .WithEnvironment("OPPLAT_RUNNING_IN_ASPIRE", "true")
    .WithEnvironment("VITE_APP_NAME", "Opplat Client")
    .WithEnvironment("VITE_API_URL", "http://localhost:8080")
    .WithEnvironment("VITE_AUTH_API_URL", "http://localhost:8080")
    .WithEnvironment("VITE_SALES_API_URL", "http://localhost:8083")
    .WithEnvironment("VITE_INVENTORY_API_URL", "http://localhost:8082")
    .WithEnvironment("VITE_AUTH_AUTHORITY", KeycloakAuthority)
    .WithEnvironment("VITE_AUTH_CLIENT_ID", "opplat-client")
    .WithEnvironment("VITE_AUTH_AUDIENCE", "opplat-api")
    .WithEnvironment("VITE_AUTH_SCOPE", "openid profile email offline_access")
    .WithEnvironment("VITE_AUTH_USE_AUDIENCE_QUERY_PARAM", "false");

builder.AddViteApp(
        "admin-app",
        RepoPath("src", "opplat-admin"),
        "dev:aspire")
    .WithEndpoint("http", endpoint =>
    {
        endpoint.Port = 3201;
        endpoint.TargetPort = 3201;
        endpoint.IsProxied = false;
    })
    .WithEnvironment("PORT", "3201")
    .WaitFor(adminApi)
    .WithEnvironment("BROWSER", "none")
    .WithEnvironment("OPPLAT_RUNNING_IN_ASPIRE", "true")
    .WithEnvironment("VITE_APP_NAME", "Opplat Admin")
    .WithEnvironment("VITE_DEV_PROXY_TARGET", "http://localhost:8084")
    .WithEnvironment("VITE_ADMIN_API_URL", string.Empty);

await builder.Build().RunAsync();

static async Task ConfigureTenantAwareApiEnvironmentAsync(
    EnvironmentCallbackContext context,
    IResourceWithConnectionString postgres,
    string mainDatabaseName,
    (string DatabaseName, string? Schema) mojocafe,
    (string DatabaseName, string? Schema) demo,
    (string DatabaseName, string? Schema) test)
{
    context.EnvironmentVariables["ConnectionStrings__DefaultConnection"] =
        await BuildDatabaseConnectionStringAsync(postgres, mainDatabaseName, null, context.CancellationToken);
    context.EnvironmentVariables["ConnectionStrings__MainConnection"] =
        await BuildDatabaseConnectionStringAsync(postgres, mainDatabaseName, null, context.CancellationToken);
    context.EnvironmentVariables["Finbuckle__MultiTenant__Stores__ConfigurationStore__Tenants__0__ConnectionString"] =
        await BuildDatabaseConnectionStringAsync(postgres, mojocafe.DatabaseName, mojocafe.Schema, context.CancellationToken);
    context.EnvironmentVariables["Finbuckle__MultiTenant__Stores__ConfigurationStore__Tenants__1__ConnectionString"] =
        await BuildDatabaseConnectionStringAsync(postgres, demo.DatabaseName, demo.Schema, context.CancellationToken);
    context.EnvironmentVariables["Finbuckle__MultiTenant__Stores__ConfigurationStore__Tenants__2__ConnectionString"] =
        await BuildDatabaseConnectionStringAsync(postgres, test.DatabaseName, test.Schema, context.CancellationToken);
}

static async Task ConfigureAdminApiEnvironmentAsync(
    EnvironmentCallbackContext context,
    IResourceWithConnectionString postgres,
    string databaseName)
{
    context.EnvironmentVariables["ConnectionStrings__DefaultConnection"] =
        await BuildDatabaseConnectionStringAsync(postgres, databaseName, null, context.CancellationToken);
}

static async Task<string> BuildDatabaseConnectionStringAsync(
    IResourceWithConnectionString resource,
    string databaseName,
    string? searchPath,
    CancellationToken cancellationToken)
{
    var connectionString = await GetRequiredConnectionStringAsync(resource, cancellationToken);
    var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString)
    {
        Database = databaseName
    };

    if (connectionStringBuilder.SslMode == SslMode.Prefer)
        connectionStringBuilder.SslMode = SslMode.Disable;

    if (!string.IsNullOrWhiteSpace(searchPath))
        connectionStringBuilder.SearchPath = searchPath.Trim();

    return connectionStringBuilder.ConnectionString;
}

static async Task<string> GetRequiredConnectionStringAsync(
    IResourceWithConnectionString resource,
    CancellationToken cancellationToken)
{
    var connectionString = await resource.GetConnectionStringAsync(cancellationToken);
    if (string.IsNullOrWhiteSpace(connectionString))
        throw new InvalidOperationException($"Connection string for resource '{resource.Name}' is unavailable.");

    return connectionString;
}

static string BuildPostgresBootstrapCommand() =>
    "until pg_isready -h postgres -p 5432 -U postgres; do sleep 1; done; " +
    "for db in opplat_main opplat_mojocafe opplat_demo opplat_test opplat_admin; do " +
    "psql -h postgres -U postgres -d postgres -tAc \"SELECT 1 FROM pg_database WHERE datname='${db}'\" | grep -q 1 || " +
    "psql -h postgres -U postgres -d postgres -v ON_ERROR_STOP=1 -c \"CREATE DATABASE \\\"${db}\\\"\"; " +
    "done; " +
    "for spec in opplat_mojocafe:tenant_mojocafe opplat_demo:tenant_demo opplat_test:tenant_test; do " +
    "db=${spec%%:*}; schema=${spec#*:}; " +
    "psql -h postgres -U postgres -d \"$db\" -v ON_ERROR_STOP=1 -c \"CREATE SCHEMA IF NOT EXISTS \\\"${schema}\\\"\"; " +
    "done";

string RepoPath(params string[] segments) => Path.Combine([repoRoot, .. segments]);

static void ConfigureProjectDefaults(ProjectResourceOptions options)
{
    options.ExcludeLaunchProfile = true;
    options.ExcludeKestrelEndpoints = true;
}

static string FindRepoRoot()
{
    var current = new DirectoryInfo(AppContext.BaseDirectory);

    while (current is not null)
    {
        if (File.Exists(Path.Combine(current.FullName, "opplat.slnx")))
            return current.FullName;

        current = current.Parent;
    }

    throw new InvalidOperationException("Could not locate the repository root for Opplat.AppHost.");
}
