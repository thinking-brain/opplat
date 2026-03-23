# Microservice startup contract validation

## When to use

Use this when a new or refactored service starts failing during `docker compose up`, especially if the change spans compose wiring, Dockerfiles, and ASP.NET startup.

## Pattern

1. **Pin the source contract**
   - Compose service exists with the expected Dockerfile path
   - Host/container port mapping is explicit
   - `ASPNETCORE_URLS` is set for the container port
   - Compose health check probes the intended endpoint
   - Dockerfile exposes the runtime port and starts the correct DLL
   - `Program.cs` maps controllers/endpoints needed by the health probe
   - For minimal-API migrations, `Program.cs` stays thin: shared host registration, module registration, shared middleware, endpoint-module mapping, no `AddControllers()`/`MapControllers()`
   - Endpoint modules inject `[FromServices] IMediator` and call handlers instead of legacy domain/application services
   - Any temporarily retained controllers are clearly archived (`*_Archived`, routing annotations commented, replacement endpoint file referenced)

2. **Pin it in tests**
   - Use a split contract/source test that reads compose, Dockerfile, and `Program.cs`
   - Assert the live health endpoint implementation shape if it is intentionally simple/stable
   - Add focused source-contract tests for endpoint modules and archived controllers so partial host conversions fail before runtime

3. **Do a live validation pass**
   - `dotnet build` the service project
   - `docker compose build <service>`
   - `docker compose up -d <service>`
   - Wait for healthy status, then hit the host-published health URL

## Opplat example

- Admin API compose service publishes `8084:8080`
- Container must answer `GET /health`
- Runtime entrypoint is `dotnet Opplat.AdminApi.dll`
- `Program.cs` must map the admin health endpoint directly from the root `src/Opplat.AdminApi` host so `/health` remains reachable without any legacy service project
- Sales/Inventory-style microservice conversions should also prove thin-host composition and MediatR-backed endpoint routing; if a host still injects `IProductService`-style services or leaves live controllers in place, the architecture test should fail the wave
