# Skill: Aspire-local host compatibility for existing ASP.NET Core services

**Purpose:** Make existing ASP.NET Core hosts participate cleanly in .NET Aspire local orchestration without turning hosts into orchestration-aware blobs or coupling code changes to package-wiring work.

---

## Use this when
- The repo is adding Aspire AppHost / ServiceDefaults in stages.
- Runtime behavior must be prepared before package references are finalized.
- Existing services already run independently and should remain thin composition roots.

---

## Pattern

### 1. Add one thin host seam for local orchestration concerns
- Register health checks with a `"self"` liveness probe tagged `"live"`.
- In Development only, trust forwarded headers so proxy-hosted requests preserve scheme/host.
- Wrap `UseHttpsRedirection()` behind a helper that first verifies an HTTPS binding exists.

### 2. Standardize health endpoints
- Map `/health` for readiness.
- Map `/alive` for liveness.
- Keep legacy aliases (for example `/healthcheck`) only where existing callers rely on them.
- Return a small JSON payload with `status` and `service` so existing compose checks and humans can both read it.

### 3. Keep shared behavior in the nearest host-shared layer
- Microservices: put the helper in `src\Opplat.Microservices.Shared\Extensions\`.
- Standalone hosts without that reference: add a local helper file rather than bloating `Program.cs`.
- Leave package/AppHost wiring to the teammate who owns csproj changes.

---

## Example checklist
- [ ] `builder.Services.AddHealthChecks().AddCheck("self", ..., ["live"])`
- [ ] Development-only `ForwardedHeadersOptions` with cleared local proxy lists
- [ ] `app.UseForwardedHeaders()` before HTTPS/auth middleware
- [ ] Conditional HTTPS redirection helper
- [ ] `app.MapHealthChecks("/health", ...)`
- [ ] `app.MapHealthChecks("/alive", live-only ...)`

---

## Why it works
- Aspire expects predictable health surfaces for local orchestration.
- Local reverse proxies need forwarded-header handling so generated URLs, auth callbacks, and Swagger links stay correct.
- HTTP-only local endpoints are common during orchestration; conditional HTTPS avoids redirect loops and broken startup.

---

## Opplat-specific application
- `src\Opplat.Microservices.Shared\Extensions\AspireDevelopmentExtensions.cs`
- `src\Opplat.MainApp\Hosting\AspireDevelopmentExtensions.cs`
- `src\Opplat.AdminApi\Hosting\AspireDevelopmentExtensions.cs`

These helpers keep the host composition roots short while making MainApp, AdminApi, Sales, and Inventory behave consistently under Aspire-driven local development.
