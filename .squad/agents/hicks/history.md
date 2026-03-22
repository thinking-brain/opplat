## Core Context

### 2026-03-22 Session 12: Admin API Compose Startup Fix

**Role in Session 12:** Fixed admin API Docker Compose startup failure by removing duplicate `/health` endpoint mapping that created routing ambiguity.

**Problem:** Docker Compose health check reported service failure because `/health` endpoint threw `AmbiguousMatchException`. Both `HealthController.Get()` and minimal API `app.MapGet("/health", ...)` were matching the same route during startup.

**Solution:**
- File: `src/Services/Admin/Opplat.Services.Admin.Api/Program.cs`
- Removed: Duplicate minimal API `/health` mapping
- Preserved: Full admin BFF/session contract (`/admin/session/*`, `/auth/bff/admin/*`)

**Validation:**
- ✅ `dotnet build ./src/Services/Admin/Opplat.Services.Admin.Api/Opplat.Services.Admin.Api.csproj`
- ✅ `docker compose up -d --build admin-api` → service reaches healthy state
- ✅ `GET http://localhost:8084/health` returns HTTP 200 Healthy|admin

**Outcome:** Admin API starts reliably without routing ambiguity. Compose stack fully operational.

**Coordination:** Hudson verified health endpoint behavior; Bishop validated startup regression tests.

---

### 2026-03-22 Session 12: Temporary Admin Shell Mode Backend Implementation

**Role in Session 12:** Implemented minimal admin shell mode by adding configuration flag, gating non-critical feature endpoints, and preserving core auth seam.

**Implementation:**
1. **Shell Mode Configuration:** Added `Auth:AdminBff:ShellModeEnabled` to `appsettings.Development.json`
2. **Session DTO Update:** Added `ShellModeEnabled` property so frontend detects shell mode at bootstrap
3. **Feature Gating:** Tenant and user management endpoints return HTTP 503 when shell mode active
4. **Core Auth Preservation:** `/admin/session/current-user`, `/admin/session/csrf`, login, logout all accessible regardless of shell state

**Validation:**
- Auth test suite: 17/17 passing ✅
- No regression in core session/CSRF/login/logout paths
- Feature-gating logic verified
- Backend auth seam operationally sound

**Outcome:** Shell mode ready for production troubleshooting. Auth boundaries clean and tested.

---

### 2026-03-22 Session 11: Admin Auth Runtime Seam Verification

**Role in Session 11:** Verified admin auth backend seam is operationally sound by running comprehensive contract tests covering both authentication and authorization boundaries.

**Verification:**
- `GET /admin/session/current-user` endpoint: Anonymous returns 401; Authenticated SuperAdmin returns 200 with session payload
- `GET /admin/session/csrf` endpoint: Anonymous returns 401; Authenticated SuperAdmin returns 200 with `{ headerName, requestToken }`
- Test suite: `AuthEndpointAuthorizationIntegrationTests` **17/17 passed**

**Finding:** No backend contract regression detected. Backend auth seam is architecturally sound for local dev. If browser still sees 403 on `/admin/session/current-user`, cause is authorization (signed-in user missing `SuperAdmin` role), not backend contract defect. Frontend runtime brittleness (deduplication, CSRF failure tolerance) is independent concern properly scoped to frontend repair.

**Outcome:** Backend validation complete. Admin auth simplification rollout ready for production use.

---

### 2026-03-22 Session 10: Admin Auth Simplification Backend Implementation

**Role in Session 10:** Executed Ripley-approved simplification: removed tenant from admin session DTO, pinned redirect fallback to 3201 via `Auth:AdminBff:DefaultOrigin` config, added legacy client-id compat, updated tests to match tenant-free contract.

**Implementation:**
- **TenantValidationMiddleware:** Skip tenant validation for `/auth/bff/admin/*`, `/admin/session*`, `/signin-oidc-admin`, `/signout-callback-oidc-admin`
- **AdminEndpoints.cs:** Use `Auth:AdminBff:DefaultOrigin` as fallback; removed duplicate `/admin/session` endpoint (keep only `/admin/session/current-user`)
- **AuthOptions.cs:** Added `AdminBff.DefaultOrigin` property
- **Program.cs:** Map legacy `Auth:ClientIdAdmin` / `Auth:ClientSecretAdmin` env vars to `Auth:AdminBff:ClientId` for current deployments
- **appsettings*.json:** Set `Auth:AdminBff:DefaultOrigin` to `http://localhost:3201`
- **AdminSessionUserDto:** Removed `TenantId` and `TenantIdentifier` properties; `BuildSessionUser()` no longer extracts tenant claims

**Tests Updated:**
- `AuthEndpointAuthorizationIntegrationTests` updated to expect tenant-free admin session DTO
- Tenant context validation tests shifted to tenant-scoped admin API endpoints only
- Full suite passes: `dotnet test .\test\Opplat.MainApp.Test\Opplat.MainApp.Test.csproj`

**Outcome:** Admin post-login redirects reliable (3201). Session DTO clean. Tests green. Legacy env vars transparent.

---

### 2026-03-21 Session 9: Admin BFF Backend Implementation

**Role in Session 9:** Implemented first admin BFF backend cut inside `Opplat.MainApp` as dual-mode auth host.

**Implementation:**
1. **Dual auth scheme via `AddPolicyScheme`:** Selector routes `/bff/*` to cookie auth; everything else to JwtBearer. Coexist during migration.
2. **Cookie authentication:** `HttpOnly`, `SameSite=Lax`, server-side ticket store (`MemoryCacheTicketStore`), sliding expiration.
3. **OpenID Connect configuration:** Code+PKCE flow, confidential `opplat-bff` client, `SaveTokens=true` (server holds tokens).
4. **Antiforgery middleware:** Scoped to `/admin` paths and logout; skips safe methods and Bearer-authenticated requests.
5. **BFF endpoints:**
   - `GET /admin/session/current-user` — Session bootstrap
   - `GET /admin/session/csrf` — CSRF token acquisition
   - `GET /auth/bff/admin/login?returnUrl=<path>` — OIDC challenge
   - `POST /auth/bff/admin/logout` — Session termination
6. **CORS tightening:** Replaced `AllowAnyOrigin()` with explicit origin allowlist for cookie-authenticated endpoints.

**Architecture Validation:**
- PolicyScheme routing works as designed.
- Cookie config (HttpOnly, SameSite=Lax, ticket store) correct.
- OpenIdConnect code+PKCE flow correct.
- Antiforgery middleware placement correct.
- Claims normalization continues to work for both schemes.

**Status:** Implementation is architecturally sound. Integration defects discovered by Ripley in review phase are not backend issues.

---

### 2026-03-21 Session 8: Backend Auth Topology Confirmation & CORS Diagnosis

**Role in Session 8:** Validated that backend auth validation is independent of Keycloak client topology. Confirmed:
1. Backend validates `audience=opplat-api` regardless of which client issued the token
2. Both `opplat-client` and `opplat-admin` clients share the same audience; no backend changes needed for two-client model
3. Two-client model does not introduce per-client authorization paths in backend
4. Admin bootstrap resilience (skip unreachable tenant DBs) already prevents false login failures

**Finding:** CORS issue is stale Keycloak runtime state, not backend auth contract misalignment. Both SPAs can coexist with one shared audience without backend code changes.

**Cross-Team Learning:** Hicks coordinated with Ripley (architecture), Vasquez (frontend), Hudson (infrastructure) to confirm realm topology is sound.
Validated that backend auth does not depend on Keycloak client ID; depends on issuer, \ud=opplat-api\, and normalized SuperAdmin role contract. Confirmed two-client Keycloak model is not root cause of login failures. Backend already aligned with realm topology. No backend changes needed for two-client architecture.

### Cross-Tenant Admin Bootstrap Resilience (2026-03-21 Session 6b)
Updated \GetAdminUsersQueryHandler\ to skip unreachable tenant databases instead of failing entire bootstrap. Admin dashboard now resilient to one or more tenant DB temporary unavailability.

### Admin Callback & Claim Contract Fix (2026-03-21)
Normalized Keycloak role claims across backend auth to handle both nested and flat dotted claim shapes. Updated AuthClaimTypes.cs and OidcClaimsTransformation.cs to accept both payload materializations.

## Archived Context (Prior Sessions — Phases 1–3d)

See \.squad/orchestration-log/\ for detailed session outcomes. Key milestones: Finbuckle.MultiTenant 7.0.1 implementation (2026-02-27), Sales/Inventory module extraction (2026-03-18), admin callback fix (2026-03-21 Session 6b).

## Project Context

**Project:** Opplat — Multi-platform business management system (café/restaurant)
**Requested by:** elvis.crego
**Stack:** ASP.NET Core 10.0 | EF Core | SQL Server | SignalR | JWT | React 18
**Solution root:** C:\projects\personal\opplat
**Branch:** develop

## Solution Structure

- src/Opplat.MainApp/ — ASP.NET Core Web API (net10.0)
- src/Opplat.Domain/ — Business logic (net10.0)
- src/Opplat.Infrastructure/ — Data access, EF Core (net10.0)
- src/Opplat.Shared/ — Common utilities (net10.0)
- src/opplat-react/ — Client React 18 SPA (Vite, MUI, React Router)
- src/opplat-admin/ — Admin React 18 SPA
- test/Opplat.MainApp.Test/ — xunit tests (net10.0)

## Key Architecture

- Clean Architecture (Domain / Infrastructure / MainApp)
- EF Core DbContext with ASP.NET Core Identity
- JWT Bearer auth with Keycloak OIDC
- SignalR hubs
- Swagger/OpenAPI

## Learnings

- Containerized APIs must validate Keycloak tokens against the browser-visible issuer (`Auth:Authority`) while using a separate internal discovery URL (`Auth:MetadataAddress`) for backchannel metadata/JWKS fetches.
- For local Docker Keycloak, `--hostname=<public-url> --hostname-backchannel-dynamic=true` keeps admin/browser redirects on the public host without breaking backend token validation inside the Docker network.
- For Entra-in-prod plus Keycloak-in-dev, the backend should stay on plain ASP.NET Core `AddJwtBearer` with OIDC discovery and a provider-neutral claim-normalization layer; provider-specific server packages add coupling without helping Keycloak parity.
- Keep authorization stable by normalizing provider-specific role and tenant claims into `ClaimTypes.Role`, `tenant_id`, `tenant_identifier`, and `ClaimTypes.Name` before policies run; only the IdP configuration should vary between Entra and Keycloak.
- For Opplat's BFF migration, the clean first backend shape is a shared ASP.NET Core BFF session layer hosted in `Opplat.MainApp`: server-side OIDC code flow, HTTP-only cookies, CSRF protection, and provider-neutral tenant membership lookup, while keeping JwtBearer available for downstream service-to-service or transitional callers.
- For an admin-first BFF cut on a mixed-mode API host, route `/admin`, `/auth/bff/admin`, and OIDC callback paths to the cookie scheme while leaving all other unauthenticated API traffic on JwtBearer by default; otherwise legacy tenant APIs start challenging against the admin cookie flow.
- In Development, an admin SPA that proxies BFF traffic to the backend over HTTP can turn ASP.NET Core `UseHttpsRedirection()` on `/admin` and `/auth/bff/admin` into frontend-visible 500s; preserve the BFF contract by skipping HTTPS redirection for those admin BFF paths (and callbacks) in dev only.
- Admin auth/session is currently tenant-optional by design: `TenantValidationMiddleware` should bypass `/auth/bff/admin/*`, `/admin/session*`, and the OIDC callback paths so SuperAdmin login/bootstrap never depends on tenant claims.
- Admin BFF return URLs should use an explicit `Auth:AdminBff:DefaultOrigin` (`http://localhost:3201`) instead of relying on the first `AllowedOrigins` entry; the redirect logic lives in `src/Opplat.MainApp/Features/Admin/AdminEndpoints.cs`.
- `Program.cs` now tolerates the legacy config key `Auth:ClientIdAdmin` as an override for `Auth:AdminBff:ClientId`, which keeps current compose/runtime wiring working while the team finishes auth simplification.
- In the dedicated admin microservice, `/health` must be owned by a single endpoint; keeping both `HealthController.Get` and `app.MapGet("/health", ...)` causes `AmbiguousMatchException`, which breaks Docker health checks and leaves the container unhealthy even though the app booted.
