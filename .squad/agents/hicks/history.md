## Core Context

### 2026-03-23 Session 15: Application Layer Remediation — Wave 1 (Locked Out)

**Status:** LOCKED OUT — Revisions owned by Hudson/Vasquez per Ripley's reviewer lockout protocol.

**What Happened:** Ripley's Phase Gate 1 review rejected Wave 1 due to critical defects:
1. SalesEndpoints.cs/InventoryEndpoints.cs injected legacy IService instead of IMediator (dead code)
2. All 8 Inventory controllers remained active instead of archived
3. Inventory Application project was a placeholder (only AssemblyMarker.cs)

**Revision Ownership:** Original defect author (Hicks) excluded from corrections per architectural review protocol. Hudson owns wiring + Inventory handlers. Vasquez owns controller archival.

**Wave 1 Status:** Pending Vasquez Phase 2 controller archival completion, then Ripley re-review.

---

## Archived Context (Sessions 10–14)

**Sessions 12–14 (Admin API Consolidation & Boundary Refactor):** Migrated admin auth/session/BFF from removed legacy service into dedicated `src/Opplat.AdminApi`. Removed admin-owned user management (users belong to tenant databases now). Updated AdminTenantInfo schema: removed `ConnectionString`, added `DatabaseName`, `DatabaseSchema`, `MaxUsers`, `CurrentUserCount`. Fixed docker compose startup (removed duplicate `/health` endpoint). Implemented temporary shell-mode gating for feature endpoints while preserving auth boundaries. Created AdminCatalogSchemaCompatibility layer to handle legacy-to-modern schema migration.

**Sessions 10–11 (Admin Auth Simplification):** Removed tenant context from admin session DTO. Pinned admin redirect fallback to 3201 via `Auth:AdminBff:DefaultOrigin` config. Added legacy client-id compatibility. Verified backend auth seams (session/csrf/login/logout all operationally sound).

**Sessions 1–9 (Foundation & Initial Implementation):** Built admin API from template. Implemented initial endpoint structures, auth middleware, and admin session contract.

---

## Project Context

**Project:** Opplat — Multi-platform business management system  
**Role:** Backend infrastructure, application-layer implementation  
**Stack:** ASP.NET Core 10.0 | EF Core | SQL Server (Main) | PostgreSQL (Admin) | SignalR | JWT | React 18 | Keycloak  
**Root:** C:\projects\personal\opplat | **Branch:** develop

**Key Patterns Established:**
- Admin API is thin tenant catalog (MediatR handlers + PostgreSQL)
- Multi-tenant MainApp hosts business logic (Sales/Inventory/Admin areas)
- Thin-host principle — business logic in Application/Domain layers, hosts are composition roots
- Schema compatibility layers handle gradual migrations (old contracts → new contracts)

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

- 2026-03-22: After splitting admin auth/BFF into the dedicated admin API, `Opplat.MainApp` should stay JwtBearer-only and stop owning `/admin/session*` or `/auth/bff/admin/*`; remove related compose/env wiring from the shared host at the same time.
- 2026-03-23: For first-wave controller retirement in isolated module services, the clean split is module `Application` handlers over repository interfaces plus host-side minimal endpoints that only bind HTTP concerns and translate command results to DTOs; implement handlers against shared `Opplat.Application.Abstractions` request contracts and keep the old controllers archived with route attributes removed until the rollout finishes.
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
- 2026-03-22: For dedicated admin CRUD in `src/Opplat.AdminApi`, the clean persistence seam is a single PostgreSQL-backed Identity `DbContext` that also owns tenant catalog rows; MediatR handlers can query/update that context directly while tests swap it to EF InMemory and reuse the same seeder.
- 2026-03-23: `src\Opplat.AdminApi\Data\AdminPortalDataSeeder.cs` now has to reconcile legacy `AdminTenants` schemas before seeding because `EnsureCreated()` does not migrate renamed columns or dropped constraints on an existing PostgreSQL database; the compatibility logic lives in `src\Opplat.AdminApi\Data\AdminCatalogSchemaCompatibility.cs`.
- 2026-03-23: The live admin client still posts/reads tenant schema as `schema`, so `src\Opplat.AdminApi\Endpoints\AdminContracts.cs` exposes a backward-compatible JSON alias while the canonical backend field remains `DatabaseSchema`.

### 2026-03-22 Session 14: Admin Tenant/User Minimal API Delivery

**Role in Session 14:** Implemented the admin client's current tenant and user management surface directly in src/Opplat.AdminApi with minimal APIs that work without auth gating.

**Implementation:**
- Added functional /admin/tenants, /admin/users, and /admin/tenants/{tenantIdentifier}/users* endpoints in Opplat.AdminApi.
- Backed the new admin surface with an in-memory AdminPortalStore seeded with realistic tenant/user placeholder data so the existing admin pages can load, create, update, deactivate, and toggle state immediately.
- Kept auth middleware/seams intact for later hardening, but made the current AdminOnly policy permissive so admin pages are usable before auth is finalized.

**Validation:**
- ✅ dotnet build .\src\Opplat.AdminApi\Opplat.AdminApi.csproj -p:UseAppHost=false
- ✅ dotnet test .\test\Opplat.MainApp.Test\Opplat.MainApp.Test.csproj --filter AdminApiMinimalEndpointContractTests -p:UseAppHost=false
- ✅ Manual smoke test of tenant/user CRUD and tenant-header mismatch on http://127.0.0.1:5099

**Outcome:** Admin API now serves the existing admin client tenant/user pages with stable minimal-API contracts and no auth dependency, while leaving room to swap the placeholder store for real persistence later.

---

### 2026-03-22 Session 16: Admin API MediatR + PostgreSQL Persistence Refactor (Complete)

**Role in Session 16:** Replaced the placeholder admin store/service implementation in `src/Opplat.AdminApi` with MediatR handlers backed directly by EF Core + Identity on PostgreSQL, while keeping the current admin client endpoint contract stable. Coordinated with Ripley (architecture), Hudson (infrastructure), Bishop (testing). Session completed 2026-03-22T22:27:00Z.

**Implementation:**
- Added `Features/Admin` request/handler slices for tenant list/create/update/deactivate and tenant/user CRUD, with minimal endpoints delegating all business logic through MediatR.
- Expanded `AdminTenantIdentityDbContext` so the same PostgreSQL-backed context owns both `AdminTenants` and tenant-scoped `AspNetUsers` data, including tenant foreign keys and tenant-scoped username indexing.
- Introduced `AdminPortalDataSeeder` so the runtime host can `EnsureCreated()` + seed baseline tenants/users, and contract tests can reuse the same seed with EF InMemory instead of external infrastructure.
- Removed the old `AdminPortalStore` / catalog / provisioning / user service classes so the admin API no longer has a parallel store/service path for this surface.
- All handlers injected with AdminTenantIdentityDbContext directly; no intermediate services.

**Validation:**
- ✅ `dotnet build .\src\Opplat.AdminApi\Opplat.AdminApi.csproj --no-restore`
- ✅ `dotnet test .\test\Opplat.MainApp.Test\Opplat.MainApp.Test.csproj --no-restore` (65/65 tests passing)
- ✅ `npm --prefix .\src\opplat-admin run build`
- ✅ `docker compose config --quiet`

**Outcome:** The admin API now persists tenant/user admin data through PostgreSQL-oriented EF Core code, all admin CRUD behavior lives in MediatR handlers, and the existing admin React pages keep the same `/admin/*` contract. Auth pipeline and endpoint contracts frozen. Session complete.

### 2026-03-22 Session 15: Admin API MediatR + PostgreSQL Persistence Refactor

**Role in Session 15:** Replaced the placeholder admin store/service implementation in `src/Opplat.AdminApi` with MediatR handlers backed directly by EF Core + Identity on PostgreSQL, while keeping the current admin client endpoint contract stable.

**Implementation:**
- Added `Features/Admin` request/handler slices for tenant list/create/update/deactivate and tenant/user CRUD, with minimal endpoints delegating all business logic through MediatR.
- Expanded `AdminTenantIdentityDbContext` so the same PostgreSQL-backed context owns both `AdminTenants` and tenant-scoped `AspNetUsers` data, including tenant foreign keys and tenant-scoped username indexing.
- Introduced `AdminPortalDataSeeder` so the runtime host can `EnsureCreated()` + seed baseline tenants/users, and contract tests can reuse the same seed with EF InMemory instead of external infrastructure.
- Removed the old `AdminPortalStore` / catalog / provisioning / user service classes so the admin API no longer has a parallel store/service path for this surface.

**Validation:**
- ✅ `dotnet build .\src\Opplat.AdminApi\Opplat.AdminApi.csproj -p:UseAppHost=false`
- ✅ `dotnet test .\test\Opplat.MainApp.Test\Opplat.MainApp.Test.csproj --filter AdminApiMinimalEndpointContractTests -p:UseAppHost=false`

**Outcome:** The admin API now persists tenant/user admin data through PostgreSQL-oriented EF Core code, all admin CRUD behavior lives in MediatR handlers, and the existing admin React pages keep the same `/admin/*` contract.

### 2026-03-22 Session 17: Admin Tenant Boundary Refactor

**Role in Session 17:** Refactored `src/Opplat.AdminApi` so the admin service owns only tenant catalog metadata and subscription-relevant user counts, not tenant user records.

**Implementation:**
- Replaced admin tenant connection-string contracts/storage with `DatabaseName` + `DatabaseSchema` metadata and exposed `UserCount` on tenant DTOs.
- Removed tenant user persistence/query/command paths from the admin API (`AdminTenantUser`, user handlers, `/admin/users`, `/admin/tenants/{tenantIdentifier}/users*`).
- Simplified persistence from `AdminTenantIdentityDbContext` to `AdminTenantCatalogDbContext`, keeping PostgreSQL + MediatR but reducing the catalog to tenant metadata only.
- Updated admin API contract tests to pin the new metadata-first surface and explicitly reject admin-owned user CRUD.

**Validation:**
- ✅ `dotnet build .\src\Opplat.AdminApi\Opplat.AdminApi.csproj -p:UseAppHost=false`
- ✅ `dotnet test .\test\Opplat.MainApp.Test\Opplat.MainApp.Test.csproj --filter "AdminApiMinimalEndpointContractTests|AdminApiMigrationContractTests" -p:UseAppHost=false`

**Outcome:** Admin API now treats tenant users as tenant-owned data, while still exposing the count needed for subscription governance. Sensitive tenant DB connectivity details are no longer stored or returned by this service.


### 2026-03-23 Session 15: App Layer Wave 1 — Sales Refactor Implementation
**Role:** Backend implementation for first application-layer refactor wave
**Outcome:** ✅ Sales Application handlers complete; SalesEndpoints.cs owns HTTP contract; legacy controllers archived

**What Was Done:**
1. Populated Modules\Sales\Application\ with MediatR request/handler slices:
   - Products/ (CreateProductCommand, UpdateProductCommand, DeleteProductCommand, GetProductsQuery)
   - Toppings/ (CRUD handlers)
   - ProductTags/ (CRUD handlers)
   - CostTabs/ (CRUD handlers)
   - Sales/ (CRUD handlers)
2. Created Services.Sales.Api\Endpoints\SalesEndpoints.cs minimal API group mapping all routes
3. Updated Program.cs: AddMediatR wiring + MapSalesEndpoints() instead of MapControllers()
4. Archived legacy controllers (renamed, route attributes removed)
5. Validation: ✅ Sales API build green

**Coordination:**
- Hudson provided shared wiring (Opplat.Application.Abstractions + Opplat.Application)
- Bishop locked regression coverage for thin-host pattern
- Phase Gate 1 approved for Sales refactor

---

