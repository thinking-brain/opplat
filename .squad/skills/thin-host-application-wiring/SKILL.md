---
name: "thin-host-application-wiring"
description: "Register MediatR and module DI through application libraries while keeping ASP.NET hosts minimal"
domain: "backend-architecture"
confidence: "high"
source: "repo-work"
---

## Context
Use this when a .NET solution is moving business orchestration out of web hosts, but the migration is staged and some domain contracts still need to be referenced by the host for HTTP request/response types.

## Pattern

### Add a root application registration project
Create a shared application project (for example `Opplat.Application`) that exposes a single registration method like `AddOpplatApplication(params Assembly[] assemblies)`. Let it own the `services.AddMediatR(...)` call so hosts do not each reinvent assembly-scanning setup.

### Let module Application projects own DI wiring
Keep repository/service registrations in the module Application project during the transition (`AddSalesApplication`, `AddInventoryApplication`). This moves composition knowledge out of `Program.cs` before every use case has been rewritten as handlers.

### Keep hosts limited to startup + endpoint mapping
Hosts should call:
- shared startup (`AddOpplatMicroserviceHost`, auth, tenancy, swagger)
- application registration (`AddOpplatApplication`)
- module registration (`Add{Module}Application`)
- endpoint mapping (`Map{Module}Endpoints`)

Avoid reintroducing repository/service wiring directly in the host.

### Update Docker restore graphs with every new project reference
If a Dockerfile copies `.csproj` files individually before restore, add every newly referenced Application project there immediately. Otherwise local builds can pass while container restores fail.

## Example

```csharp
builder.Services.AddOpplatMicroserviceHost<SalesDbContext>(builder.Configuration);
builder.Services.AddSalesApplication();
builder.Services.AddOpplatApplication(
    Assembly.GetExecutingAssembly(),
    typeof(Opplat.Modules.Sales.Application.AssemblyMarker).Assembly);

app.UseOpplatMicroserviceHost();
app.MapSalesEndpoints();
```

## Anti-Patterns
- **Keeping MediatR scanning only on `GetExecutingAssembly()`** - module handlers will never register.
- **Leaving service/repository wiring in `Program.cs`** - hosts stay fat and ownership remains unclear.
- **Updating project references without Dockerfiles** - CI/container restores break even when local solution builds succeed.
