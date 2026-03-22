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

2. **Pin it in tests**
   - Use a split contract/source test that reads compose, Dockerfile, and `Program.cs`
   - Assert the live health endpoint implementation shape if it is intentionally simple/stable

3. **Do a live validation pass**
   - `dotnet build` the service project
   - `docker compose build <service>`
   - `docker compose up -d <service>`
   - Wait for healthy status, then hit the host-published health URL

## Opplat example

- Admin API compose service publishes `8084:8080`
- Container must answer `GET /health`
- Runtime entrypoint is `dotnet Opplat.Services.Admin.Api.dll`
- `Program.cs` must keep `AddControllers()` and `MapControllers()` so `HealthController` stays reachable
