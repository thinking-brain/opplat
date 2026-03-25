# Skill: Aspire Local Development Orchestration

## Purpose

Integrate .NET Aspire for local development orchestration of multi-service .NET repositories.

## When to Apply

- .NET 9+ solutions with multiple API hosts
- Projects using external dependencies (databases, identity providers)
- Teams wanting unified local dev experience with dashboards, logs, traces

## Pattern

### Project Structure

```
src/
├── {Solution}.AppHost/           # Aspire orchestrator
│   └── {Solution}.AppHost.csproj
├── {Solution}.ServiceDefaults/   # Shared defaults
│   └── {Solution}.ServiceDefaults.csproj
└── [existing API projects...]
```

### AppHost Wiring

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Container resources
var sqlserver = builder.AddSqlServer("sqlserver")
    .AddDatabase("maindb");

var postgres = builder.AddPostgres("postgres")
    .AddDatabase("admindb");

var keycloak = builder.AddContainer("keycloak", "quay.io/keycloak/keycloak", "26.0")
    .WithBindMount("./docker/keycloak/realm.json", "/opt/keycloak/data/import/realm.json")
    .WithArgs("start-dev", "--import-realm")
    .WithHttpEndpoint(port: 8180, targetPort: 8180);

// .NET API projects
builder.AddProject<Projects.MainApp>("mainapp")
    .WithReference(sqlserver)
    .WithReference(keycloak)
    .WithEnvironment("Auth__Authority", "http://localhost:8180/realms/opplat");

builder.Build().Run();
```

### ServiceDefaults Setup

```csharp
public static class Extensions
{
    public static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder)
    {
        builder.ConfigureOpenTelemetry();
        builder.AddDefaultHealthChecks();
        builder.Services.AddServiceDiscovery();
        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            http.AddStandardResilienceHandler();
            http.AddServiceDiscovery();
        });
        return builder;
    }
}
```

### API Integration

Each API's `Program.cs`:

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();  // Early in pipeline
// ... rest of setup
```

## Constraints

1. **Dual-mode required** — Both Aspire and Docker Compose must work
2. **Frontends stay native** — SPAs should run as Vite/npm processes for HMR, but Aspire can orchestrate them via `AddViteApp(...)`
3. **Fixed infra ports** — Databases and auth use fixed ports for tool compatibility
4. **Dynamic app ports** — Aspire assigns ports to .NET projects

## Opplat runtime guardrails

- If the AppHost is launched from the repo root with `dotnet run --project src\{Solution}.AppHost`, resolve `AddProject(...)` file paths and bind mounts from a repo-root helper instead of relying on ad-hoc relative strings.
- When you intentionally pin ASP.NET Core project ports with `WithHttpEndpoint(...)`, set project defaults to exclude launch-profile and Kestrel-derived endpoints first. Otherwise Aspire can import an implicit `http` endpoint from `launchSettings.json` and fail with a duplicate-endpoint exception before startup.
- For this repo's AppHost, use the versioned SDK form (`Aspire.AppHost.Sdk/13.x`) so the CLI can resolve the AppHost SDK without relying on a preinstalled workload resolver.
- AppHost `Properties\launchSettings.json` must include `ASPIRE_DASHBOARD_OTLP_ENDPOINT_URL` and `ASPIRE_RESOURCE_SERVICE_ENDPOINT_URL` for each profile; without them `dotnet run` can build successfully and still crash before orchestration starts.
- When a host-launched project still needs explicit `ConnectionStrings__*` overrides, feed them from Aspire resources with `database.Resource.ConnectionStringExpression`. Hardcoding `Host=postgres` only works for container-to-container traffic, and `GetConnectionString()` is not available on `IResourceBuilder<PostgresDatabaseResource>` in Aspire 13.
- Keep AppHost runtime probes resilient to missing XML-doc outputs: Swagger XML comments should be conditional (`if (File.Exists(xmlPath))`) so `/health` and other non-doc endpoints do not fail during local orchestration.
- For Vite SPAs, add `Aspire.Hosting.JavaScript`, register the app with `AddViteApp(...)`, pin the frontend port with `WithHttpEndpoint(port: ..., env: "PORT")`, and use a `dev:aspire` script/config that reads `PORT`, binds to `127.0.0.1`, keeps `strictPort`, and disables `open` when AppHost launches the app.
- For browser-facing SPA env vars under Aspire, inject loopback URLs (`http://localhost:...`) or endpoint references resolved for the host network. Do not use internal container/service DNS names in `VITE_*` settings because the Vite process and the browser both run on the host machine in this repo.

## Anti-Patterns

- **Don't** containerize frontends in Aspire (HMR is faster native)
- **Don't** remove Docker Compose (needed for CI, prod-like testing)
- **Don't** create separate AppHosts per microservice unless truly independent deployments

## Verification

```bash
# Aspire mode
dotnet run --project src/{Solution}.AppHost
# Dashboard at https://localhost:15xxx (port in console output)

# Docker Compose mode (unchanged)
docker-compose up
```

## Package References

```xml
<!-- Directory.Packages.props -->
<PackageVersion Include="Aspire.Hosting" Version="9.x.x" />
<PackageVersion Include="Aspire.Hosting.SqlServer" Version="9.x.x" />
<PackageVersion Include="Aspire.Hosting.PostgreSQL" Version="9.x.x" />
<PackageVersion Include="Microsoft.Extensions.ServiceDiscovery" Version="9.x.x" />
<PackageVersion Include="Microsoft.Extensions.Http.Resilience" Version="9.x.x" />
```
