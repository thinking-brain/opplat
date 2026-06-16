using Aspire.Hosting.ApplicationModel;
using Npgsql;

const string PostgresPasswordValue = "Admin123*";
const string KeycloakRealm = "opplat";
const string KeycloakAuthority = $"http://localhost:8180/realms/{KeycloakRealm}";
const string KeycloakMetadataAddress = $"{KeycloakAuthority}/.well-known/openid-configuration";
const string AdminDatabaseName = "opplat_admin";

var repoRoot = FindRepoRoot();
var builder = DistributedApplication.CreateBuilder(args);

var postgresUser = builder.AddParameter("postgres-user", "postgres");
var postgresPassword = builder.AddParameter("postgres-password", PostgresPasswordValue, secret: true);

var postgres = builder.AddPostgres("postgres", postgresUser, postgresPassword, port: 5432)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithContainerName("opplat-postgres")
    .WithDataVolume("opplat-postgres-data")
    .WithContainerRuntimeArgs("--label", "com.docker.compose.project=opplat");

var keycloak = builder.AddContainer("keycloak", "quay.io/keycloak/keycloak", "26.0")
    .WithBindMount(RepoPath("docker", "keycloak", "keycloak.conf"), "/opt/keycloak/conf/keycloak.conf", isReadOnly: true)
    .WithBindMount(RepoPath("docker", "keycloak", "opplat-realm.json"), "/opt/keycloak/data/import/opplat-realm.json", isReadOnly: true)
    .WithHttpEndpoint(port: 8180, targetPort: 8180, name: "keycloak-http")
    .WithArgs("start-dev", "--import-realm", "--hostname=http://localhost:8180", "--hostname-backchannel-dynamic=true")
    .WithEnvironment("KC_BOOTSTRAP_ADMIN_USERNAME", "admin")
    .WithEnvironment("KC_BOOTSTRAP_ADMIN_PASSWORD", "admin")
    .WithContainerName("opplat-keycloak")
    .WithVolume("opplat-keycloak-data", "/opt/keycloak/data")
    .WithContainerRuntimeArgs("--label", "com.docker.compose.project=opplat");

var mainApi = builder.AddProject(
        "main-api",
        RepoPath("src", "Apis", "Opplat.Api.Main", "Opplat.Api.Main.csproj"),
        ConfigureProjectDefaults)
    .WithReference(postgres)
    .WaitFor(postgres)
    .WithHttpEndpoint(port: 8080, name: "main-api-http")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment(context => ConfigureTenantAwareApiEnvironmentAsync(
        context,
        postgres.Resource,
        AdminDatabaseName))
    .WithEnvironment("Auth__Authority", KeycloakAuthority)
    .WithEnvironment("Auth__MetadataAddress", KeycloakMetadataAddress)
    .WithEnvironment("Auth__Audience", "opplat-api")
    .WithEnvironment("Auth__ClientIdClient", "opplat-client");

var adminApi = builder.AddProject(
        "admin-api",
        RepoPath("src", "Apis", "Opplat.Api.Admin", "Opplat.Api.Admin.csproj"),
        ConfigureProjectDefaults)
    .WithReference(postgres)
    .WaitFor(postgres)
    .WithHttpEndpoint(port: 8084, name: "admin-api-http")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment(context => ConfigureAdminApiEnvironmentAsync(context, postgres.Resource, AdminDatabaseName))
    .WithEnvironment("Auth__Authority", KeycloakAuthority)
    .WithEnvironment("Auth__MetadataAddress", KeycloakMetadataAddress)
    .WithEnvironment("Auth__Audience", "opplat-api")
    .WithEnvironment("Auth__AdminBff__ClientId", "opplat-admin");

var mainApiHttp = mainApi.GetEndpoint("main-api-http");
var adminApiHttp = adminApi.GetEndpoint("admin-api-http");

var clientApp = builder.AddViteApp(
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
    .WaitFor(mainApi)
    .WaitFor(adminApi)
    .WaitFor(keycloak)
    .WithEnvironment("BROWSER", "none")
    .WithEnvironment("OPPLAT_RUNNING_IN_ASPIRE", "true")
    .WithEnvironment("VITE_APP_NAME", "Opplat Client")
    .WithEnvironment("VITE_API_URL", mainApiHttp)
    .WithEnvironment("VITE_DEV_PROXY_TARGET", adminApiHttp)
    .WithEnvironment("VITE_ADMIN_API_URL", string.Empty)
    .WithEnvironment("VITE_AUTH_AUTHORITY", KeycloakAuthority)
    .WithEnvironment("VITE_AUTH_CLIENT_ID", "opplat-client")
    .WithEnvironment("VITE_AUTH_AUDIENCE", "opplat-api")
    .WithEnvironment("VITE_AUTH_SCOPE", "openid profile email offline_access");

var adminApp = builder.AddViteApp(
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
    .WithEnvironment("VITE_DEV_PROXY_TARGET", adminApiHttp)
    .WithEnvironment("VITE_ADMIN_API_URL", string.Empty);

var clientAppHttp = clientApp.GetEndpoint("http");
var adminAppHttp = adminApp.GetEndpoint("http");

adminApi
    .WithEnvironment("Auth__AdminBff__DefaultOrigin", adminAppHttp)
    .WithEnvironment("Auth__AdminBff__AllowedOrigins__0", clientAppHttp)
    .WithEnvironment("Auth__AdminBff__AllowedOrigins__1", adminAppHttp);

await builder.Build().RunAsync();

static async Task ConfigureTenantAwareApiEnvironmentAsync(
    EnvironmentCallbackContext context,
    IResourceWithConnectionString postgres,
    string mainDatabaseName)
{
    context.EnvironmentVariables["ConnectionStrings__DefaultConnection"] =
        await BuildDatabaseConnectionStringAsync(postgres, mainDatabaseName, null, context.CancellationToken);
    context.EnvironmentVariables["ConnectionStrings__MainConnection"] =
        await BuildDatabaseConnectionStringAsync(postgres, mainDatabaseName, null, context.CancellationToken);
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
