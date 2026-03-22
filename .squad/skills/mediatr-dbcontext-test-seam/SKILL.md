# MediatR DbContext test seam

## When to use

Use this when a dedicated ASP.NET Core service exposes thin minimal APIs, the user wants business logic moved into MediatR handlers, and the runtime data store should be real EF Core while tests stay self-contained.

## Pattern

1. **Keep endpoints thin**
   - Inject `IMediator`
   - Translate route/header validation at the endpoint edge
   - Delegate CRUD logic to request handlers

2. **Let handlers own the EF work**
   - Inject the relevant `DbContext` directly into handlers
   - Keep normalization, duplicate checks, and shape mapping in shared feature helpers
   - Throw `ArgumentException`, `InvalidOperationException`, and `KeyNotFoundException` so endpoints can map them consistently to HTTP results

3. **Create one reusable seed path**
   - Add a seeder that calls `Database.EnsureCreated()` and inserts baseline rows idempotently
   - Call it from the real host at startup
   - Reuse the same seeder in tests

4. **Swap providers only in tests**
   - Runtime host uses the real provider (for Opplat admin, Npgsql/PostgreSQL)
   - Contract tests register the same `DbContext` with `UseInMemoryDatabase(<stable-name-per-app>)`
   - Register MediatR from the service assembly so tests execute the real handlers

## Opplat example

- `src\Opplat.AdminApi\Endpoints\AdminEndpoints.cs` maps the admin CRUD surface and sends MediatR requests
- `src\Opplat.AdminApi\Features\Admin\**\*.cs` contains the tenant/user handlers and shared mapping helpers
- `src\Opplat.AdminApi\Data\AdminPortalDataSeeder.cs` seeds baseline tenants/users for both runtime and tests
- `test\Opplat.MainApp.Test\Auth\AdminApiMinimalEndpointContractTests.cs` swaps the admin `DbContext` to EF InMemory and still exercises the real admin handlers/endpoints
