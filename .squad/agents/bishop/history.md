## Core Context

### 2026-03-23 Session 17: MainApp Sales Regression Gates — Phase Gate 1 Authorized

**Status:** ✅ COMPLETE (81/81 passing) — Pending Ripley Phase Gate 2 review

**Role:** Architecture + regression test suite validation for MainApp Sales endpoints

**What Happened:** Following Phase Gate 1 approval, validated Hicks's Sales conversion with comprehensive regression gates, extending the Inventory validation pattern.

1. **Architecture Regression Tests**
   - Extended `MicroserviceHostArchitectureTests.cs` to cover MainApp Sales patterns
   - Added contract assertions for thin-host pattern:
     - No `AddControllers()` registration
     - All endpoints inject `IMediator` (no legacy `IService` types)
     - Controllers archived with `_Archived` suffix, routes commented
     - MediatR handlers exist and are discoverable
     - `MapSalesEndpoints()` call present in routing

2. **Sales-Specific Authorization Seam Validation**
   - Pinned authorization boundary: sales-list endpoints protected
   - Product-list endpoints unannotated (per spec)
   - Verified auth contract consistent across dual route surfaces

3. **Route Surface Validation**
   - Legacy root `/sales/*` paths verified (non-tenant)
   - Tenant-aware `/{__tenant__}/sales/*` paths verified
   - Confirmed both surfaces routed to MediatR endpoints (not controllers)

4. **Decision Logged**
   - `.squad/decisions/inbox/bishop-mainapp-sales-tests.md` → merged to decisions.md
   - Why: Mixed-host regression contract protects Sales rollout without breaking Inventory
   - Impact: Architecture guardrails now prevent silent drift; future regressions fail automatically

**Test Result:** ✅ **81/81 PASSING** (79 inherited from Inventory + 2 new Sales-specific tests)

**Next Step:** Pending Ripley Phase Gate 2 review for merge authorization

---

### 2026-03-23 Session 16: MainApp Inventory Regression Gates — Phase Gate 1 Authorized

**Status:** ✅ COMPLETE (79/79 passing) — Pending Ripley Phase Gate 2 review

**Role:** Architecture + regression test suite validation for MainApp Inventory endpoints

**What Happened:** Following Phase Gate 1 approval, validated Hicks's Inventory conversion with comprehensive regression gates.

1. **Architecture Regression Tests**
   - Extended `MicroserviceHostArchitectureTests.cs` to cover MainApp Inventory patterns
   - Added contract assertions for thin-host pattern:
     - No `AddControllers()` registration
     - No `MapControllers()` invocation
     - All endpoints inject `IMediator` (no legacy `IService` types)
     - Controllers archived with `_Archived` suffix, routes commented
     - MediatR handlers exist and are discoverable

2. **Movement-Type Authorization Seam Validation**
   - Pinned existing movement-type auth boundary in endpoint layer
   - Verified tenant-aware route (`/{__tenant__}/inventory/*`) and root route (`/inventory/*`) both use same auth contract
   - Ensured authorization seam remains consistent across dual route surfaces

3. **Route Surface Validation**
   - Legacy root `/inventory/*` paths verified (non-tenant)
   - Tenant-aware `/{__tenant__}/inventory/*` paths verified
   - Confirmed both surfaces routed to MediatR endpoints (not controllers)

4. **Decision Logged**
   - `.squad/decisions/inbox/bishop-mainapp-inventory-tests.md` → merged to decisions.md
   - Why: Mixed-host regression contract protects Inventory rollout without breaking Sales
   - Impact: Architecture guardrails now prevent silent drift; future regressions fail automatically

**Test Result:** ✅ **79/79 PASSING** (77 inherited + 2 new movement-type auth seam validations)

**Next Step:** Pending Ripley Phase Gate 2 review for merge authorization

---

### 2026-03-23 Session 15: Application Layer Remediation — Wave 1 Regression Gates

**Role:** Encoded Ripley's Phase Gate 1 remediation criteria as architecture-contract tests in MicroserviceHostArchitectureTests.cs.

**Key Action:** Added regression gates that enforce thin-host pattern:
- No `AddControllers()` in microservice Program.cs
- Endpoints inject `IMediator` (not legacy IService)
- Controllers archived with `_Archived` suffix, routes commented
- MediatR handlers exist and are discoverable

**Outcome:** Sales passes gate; Inventory fails (pending Phase 2 controller archival). Regression gates now prevent architectural drift—future regressions fail automatically instead of requiring manual review checklists.

---

## Archived Context (Sessions 1–14)

**Sessions 13–14 (Admin Boundary Refactor):** Enforced admin API boundary shift in test layer. Reset coverage to validate new tenant catalog scope. AdminApiContractTests now require `DatabaseName`, `DatabaseSchema`, `MaxUsers`, `CurrentUserCount`; reject `/admin/users` endpoints and full `ConnectionString` fields. Frontend contract tests aligned to admin-only tenant catalog (no users).

**Sessions 10–12 (Admin API Stabilization):** Added startup-contract assertions for admin API Docker compose wiring (port 8084, `/health` probe, Dockerfile entrypoint), temporary shell-mode feature gating (endpoints 503 when shell active, auth always accessible), sparse-claims regression coverage, and BFF session contract validation.

**Sessions 8–9 (Two-Client OIDC & Callback Recovery):** Validated two-client Keycloak architecture (separate clients for opplat-react and opplat-admin). Pinned realm contract, callback recovery flow, and protected route behavior. Repaired test suite after Ripley's defect discovery (removed assertions targeting deleted oidc.ts, rewrote FrontendAuthContractTests).

**Sessions 6–7 (Admin Auth Refactor):** Added regression coverage for admin auth simplification (tenant-agnostic bootstrap, origin-based fallback), admin BFF session contract, and Keycloak realm configuration.

**Sessions 1–5 (Foundation):** Built initial test suite foundations (auth, contract, CORS validation).

---

## Project Context

**Project:** Opplat — Multi-platform business management system (café/restaurant)  
**Stack:** ASP.NET Core 10.0 | EF Core | SQL Server | PostgreSQL (Admin) | SignalR | JWT | React 18 | Keycloak  
**Root:** C:\projects\personal\opplat | **Branch:** develop

**Key Architecture Patterns:**
- Regression tests as architectural guardrails — failures prevent silent drift
- Thin-host principle — business logic in class libraries, hosts are composition roots
- Two-client OIDC model — separate auth flows for admin (BFF) and client (browser-managed OIDC)
- Admin boundary isolation — admin owns tenant catalog only, users belong to tenant-managed databases


## Key Architecture

- Clean Architecture (Domain / Infrastructure / MainApp)
- EF Core DbContext with ASP.NET Core Identity
- JWT Bearer auth with Keycloak OIDC
- SignalR hubs
- Swagger/OpenAPI

## Learnings

- For dedicated microservice startup regressions, the most reliable guard is a split contract: pin compose port/env wiring (`ASPNETCORE_URLS=http://+:8080`), the Dockerfile runtime entrypoint/exposed port, and `Program.cs` controller mapping together, then confirm the live `/health` probe answers from the compose-published host port.
- Final admin login recovery is pinned at the frontend auth seam, not the realm-client seam alone: `AuthContext.tsx` must treat a restored non-expired OIDC user as authenticated, and `AuthCallbackPage.tsx` / `ProtectedRoute.tsx` must prefer that recovered session over transient shared auth errors.
- Keeping `opplat-client` and `opplat-admin` as separate public Keycloak clients remains the supported contract even with one shared backend audience; the regression suite now treats that separation plus callback recovery as the intended model.
- The repo already protects the admin SPA CORS/origin contract for `http://localhost:3201`: `KeycloakRealmContractTests` pins `opplat-admin` origins to `3101/3201/5174`, so a live token-endpoint CORS miss can still point to a stale running Keycloak realm rather than bad source.
- `react-oidc-context` setup needs its own regression seam beyond callback-page UI: source contracts should pin `AuthProvider` wiring of `onSigninCallback` and browser `localStorage` user persistence, because restored-session behavior depends on both pieces even when route/error handling tests are already green.
- During the admin-first BFF cutover, the safest testing pattern is split coverage: keep executable tests for current tenant-isolation seams (`X-Tenant-Identifier` + route alignment) and add skipped target-contract tests for `/bff/auth/session`, cookie auth, CSRF, and server-driven login/logout until Hicks/Vasquez land the implementation.
- For admin BFF endpoints that call `AuthenticateAsync("AdminCookie")` directly, TestServer coverage must register the same named scheme in addition to the default test scheme; otherwise current-user regressions hide behind fake auth setups and the endpoint path is never truly exercised.
- The most useful regression for `/admin/session/current-user` is a sparse-claim cookie principal: assert the route still returns `200 OK` with empty optional fields plus the expected login/logout/CSRF metadata, so missing profile claims cannot silently reintroduce a 500 during session bootstrap.
- To guard the admin redirect-origin bug, integration coverage must configure `AuthOptions.AdminBff.AllowedOrigins` in the test host and hit `/auth/bff/admin/login` with an `Origin` header; otherwise relative `returnUrl` assertions collapse to bare paths and miss the `3201 vs 3001` fallback behavior entirely.
- For the simplified admin rollout, auth/session regressions should treat tenant context as optional at sign-in bootstrap: assert sparse admin cookie claims still succeed with empty tenant fields, while keeping tenant-header validation pinned only on tenant-scoped admin API routes.
- For minimal-API microservice migrations, the most effective architecture gate is a thin-host source contract: assert `Program.cs` only composes shared host setup plus endpoint modules, endpoint files inject `[FromServices] IMediator` instead of legacy domain services, and any retained controllers are explicitly archived with `_Archived` markers so partial conversions fail fast.
- Source-contract tests in this repo should resolve the root via a shared helper that accepts both `opplat.sln` and `opplat.slnx`; otherwise solution-file churn creates false-red auth failures unrelated to the behavior under test.
- For the admin API MediatR/PostgreSQL migration, the safest regression pattern is dual coverage: execute the real endpoint handlers against an in-memory `AdminTenantIdentityDbContext` seeded with PostgreSQL-shaped tenant data, and separately pin source contracts for `AddMediatR`, `IMediator` endpoint injection, `UseNpgsql`, and tenant-claim normalization.
- When the admin API boundary drops user CRUD, add page-level source-contract tests for `src/opplat-admin/src/pages/DashboardPage.tsx` and `TenantsPage.tsx` in addition to API wrapper tests; route/type assertions alone will miss stale calls to removed `/admin/users` flows or old tenant fields such as `connectionString`.
- When hosts switch from direct `AddMediatR(...)` calls to the shared `AddOpplatApplication(...)` wrapper, keep the regression seam split across both files: assert the host calls the wrapper and assert the wrapper still registers MediatR assemblies. That catches DI drift without overfitting tests to one registration style.
- For MainApp area-by-area migrations, the safest inventory regression gate is a mixed-host contract: keep `MapControllers()` only for still-live MVC areas, remove the converted area's conventional routes, map the same minimal endpoint module under both `/inventory` and `/{__tenant__}/inventory`, and archive the retired controllers so the old surface cannot silently reactivate.
- For MainApp Sales rollouts, pair converted-surface source contracts with executable route-metadata tests: pin `app.MapSalesEndpoints();`, require both `/sales/*` and `/{__tenant__}/sales/*`, assert the sales list endpoint keeps authorization while product reads stay unannotated, and verify archived Sales controllers still reference `Features/Sales/SalesEndpoints.cs` so mixed-host drift fails fast.

---

### 2026-03-22: Legacy Admin Project Removal Validation

**Role:** Bishop (QA)

**Changes Made:**
- Updated `test/Opplat.MainApp.Test/Auth/AdminApiSplitContractTests.cs` to pin removal of the legacy admin service host from solution/docs/compose references.
- Added focused regression checks for new `src/Opplat.AdminApi` endpoint wiring and compose health-probe alignment.
- Re-validated admin frontend runtime/build wiring against the dedicated admin API contract.

**Validation Results:**
- ✅ `dotnet build src\\Opplat.AdminApi\\Opplat.AdminApi.csproj`
- ✅ `npm run build` in `src\\opplat-admin`
- ✅ `docker compose config` resolves `admin-api` to `src/Opplat.AdminApi/Dockerfile`
- ❌ `dotnet test test\\Opplat.MainApp.Test\\Opplat.MainApp.Test.csproj --filter FullyQualifiedName~AdminApiSplitContractTests`

**Failures Confirmed:**
1. `src/Opplat.AdminApi/Program.cs` still ships the template `weatherforecast` app and does not map `GeneralEndpoints` / `AdminEndpoints`.
2. `docker-compose.yml` still health-checks `GET /health`, while the new admin API source contract exposes `/healthcheck`.

**Unrelated Notes:**
- Restore emits existing `MimeKit` NU1902 vulnerability warnings during .NET validation; these did not block the targeted findings.

---

### 2026-03-22: Final Admin Migration Test Reconciliation

**Role:** Bishop (QA)

**Changes Made:**
- Updated `test/Opplat.MainApp.Test/Auth/AdminApiSplitContractTests.cs` so compose assertions follow the final `src/Opplat.AdminApi` source contract instead of the stale `/healthcheck`-only expectation.
- Updated `test/Opplat.MainApp.Test/Architecture/MultitenancyConfigurationTests.cs` to use the shared repository resolver, preventing false failures when the repo is rooted by `opplat.slnx` instead of `opplat.sln`.
- Kept admin frontend auth-removal coverage intact: the regression suite still asserts `opplat-admin` no longer depends on browser OIDC/auth bootstrap files.

**Validation Results:**
- ✅ `dotnet build src\\Opplat.AdminApi\\Opplat.AdminApi.csproj --no-restore`
- ✅ `npm run build` in `src\\opplat-admin`
- ✅ `docker compose config`
- ✅ `dotnet test test\\Opplat.MainApp.Test\\Opplat.MainApp.Test.csproj --no-restore`

**Exact Final Status:**
- Full backend test suite: **54 total, 54 passed, 0 failed, 0 skipped**
- Existing warnings only: `MimeKit` `NU1902` on `src\\Opplat.MainApp\\Opplat.MainApp.csproj` and `test\\Opplat.MainApp.Test\\Opplat.MainApp.Test.csproj`
- Admin migration regression coverage is green against the final `src\\Opplat.AdminApi` layout.

---

### 2026-03-22 Session 14: Minimal Admin API Endpoint Regression Coverage

**Role:** Bishop (QA)

**Changes Made:**
- Added `test\\Opplat.MainApp.Test\\Auth\\AdminApiMinimalEndpointContractTests.cs`.
- Pinned the tenant/user route surface that the current admin SPA pages actually call (`/admin/tenants`, `/admin/users`, tenant-scoped user mutations).
- Pinned the client-facing payload contract by asserting the current admin SPA helper/types/pages all agree on tenant and user fields, without expanding auth scope.

**Validation Results:**
- ❌ `dotnet test test\\Opplat.MainApp.Test\\Opplat.MainApp.Test.csproj --filter FullyQualifiedName~AdminApiMinimalEndpointContractTests --no-restore`
- ❌ `dotnet test test\\Opplat.MainApp.Test\\Opplat.MainApp.Test.csproj --no-restore`

**Outcome:**
- The current admin client contract now has executable guardrails for the minimal admin API tenant/user surface.
- Auth remains intentionally out of scope for these new regression checks.
- Validation is currently blocked by unrelated `src\\Opplat.AdminApi` compile conflicts already present in the landed work: duplicated endpoint DTO/request definitions in `Endpoints\\AdminContracts.cs` and duplicate `NotFound` members in `Services\\AdminTenantCatalogStore.cs` / `Services\\AdminTenantUserService.cs`.

---

### 2026-03-22 Session 16: Admin API MediatR + PostgreSQL Migration Testing (Complete)

**Role:** Bishop (QA) — Validation of Admin API MediatR + PostgreSQL migration. Session completed 2026-03-22T22:27:00Z.

**Test Strategy:**
- External behavior focus: endpoint routes, payload shapes, tenant isolation
- Source contracts: MediatR registration, DbContext injection, Npgsql provider
- In-memory AdminTenantIdentityDbContext seeded with PostgreSQL-style connection strings

**Coverage Added:**
1. Endpoint route + write-flow contracts tested against in-memory DbContext
2. Tenant isolation via scoped reads and filtered /admin/users?tenantIdentifier=
3. Admin API claim normalization: tenant_id / tenant_identifier assertions
4. Source-level assertions:
   - MediatR package reference in csproj
   - builder.Services.AddMediatR(...) in Program.cs
   - IMediator injection in AdminEndpoints
   - UseNpgsql() wiring (no UseSqlServer)

**Validation Results:**
- ✅ `dotnet test test\\Opplat.MainApp.Test\\Opplat.MainApp.Test.csproj --no-restore` (65/65 tests pass)
- ✅ `dotnet build src\\Opplat.AdminApi\\Opplat.AdminApi.csproj --no-restore`
- ✅ `npm --prefix src\\opplat-admin run build`
- ✅ `docker compose config --quiet`

**Outcome:** Regression coverage locked for MediatR handlers, PostgreSQL persistence, and admin API contracts. Endpoint contracts stable for frontend. Migration validated end-to-end.

### 2026-03-22 Session 15: Minimal Admin API Test Compile Repair

**Role:** Bishop (QA)

**Changes Made:**
- Repaired `test\\Opplat.MainApp.Test\\Auth\\AdminApiMinimalEndpointContractTests.cs` to match the actual `src\\Opplat.AdminApi` layout.
- Removed the stale `Opplat.AdminApi.Admin` import, switched the test host registration from nonexistent `AdminCatalogStore` to `AdminPortalStore`, and added the `AdminOnly` authorization policy required by `AdminEndpoints`.

**Validation Results:**
- ✅ `dotnet test test\\Opplat.MainApp.Test\\Opplat.MainApp.Test.csproj --filter FullyQualifiedName~AdminApiMinimalEndpointContractTests --no-restore`

**Exact Final Status:**
- Focused admin endpoint validation: **5 total, 5 passed, 0 failed, 0 skipped**
- Duration: **2.4s**
- Build/test completed successfully; remaining output only reported the existing warning count from the wider solution build path.

---

### 2026-03-22 Session 17: Admin Boundary Regression Reset

**Role:** Bishop (QA)

**Changes Made:**
- Replaced `AdminApiMinimalEndpointContractTests` with boundary-focused coverage that pins the admin API to tenant catalog routes only.
- Removed test-side assumptions that admin owns tenant users or exposes tenant connection strings.
- Strengthened `AdminApiMigrationContractTests` to require `DatabaseName`, `Schema`, and `UserCount` contracts while rejecting lingering user-management handlers and connection-string storage.
- Deleted the obsolete test seeder that still modeled admin-owned tenant users.

**Validation Results:**
- PASS `npm --prefix src\\opplat-admin run build`
- FAIL `dotnet build src\\Opplat.AdminApi\\Opplat.AdminApi.csproj --no-restore`
- FAIL `dotnet test test\\Opplat.MainApp.Test\\Opplat.MainApp.Test.csproj --no-restore`

**Blocking Findings:**
1. `src\\Opplat.AdminApi\\Endpoints\\AdminEndpoints.cs` still references removed user-management queries/requests (`GetAdminUsersQuery`, `GetTenantUsersQuery`, `AdminCreateUserRequest`, `AdminUpdateUserRequest`, `AdminSetUserRolesRequest`, `AdminSetUserActiveRequest`).
2. `src\\Opplat.AdminApi` still contains stale tenant persistence references that have not been reconciled with the revised admin boundary (`AdminTenantIdentityDbContext` / catalog storage alignment).
3. The admin frontend now builds cleanly with the tenant-catalog-only contract, but backend validation remains red until Hicks/Hudson finish removing the legacy admin-owned user surface.

**Outcome:** Regression coverage now reflects the intended admin boundary. Backend validation is correctly failing on leftover user-management/server-storage seams instead of silently preserving the old contract.

### 2026-03-22 Session 17: Frontend Admin Contract Test Reconciliation

**Role in Session 17:** Updated stale admin frontend contract assertions after the tenant-catalog boundary landed.

**Changes Made:**
- Revised `test\\Opplat.MainApp.Test\\Auth\\FrontendAuthContractTests.cs` to stop expecting admin user routes/pages in `opplat-admin`.
- Pinned the current admin SPA boundary: catalog-only tenant management, no admin user CRUD, no browser bearer tokens, and tenant metadata limited to database name + schema + user count.
- Confirmed the admin settings/tenant pages still communicate that user management belongs inside each tenant application.

**Validation Results:**
- ✅ `dotnet test test\\Opplat.MainApp.Test\\Opplat.MainApp.Test.csproj --filter "FullyQualifiedName~FrontendAuthContractTests" --nologo`
- ✅ `npm run lint` in `src\\opplat-admin`
- ✅ `npm run build` in `src\\opplat-admin`

**Outcome:** Frontend auth contract tests now match the landed admin boundary and are green again. Existing `MimeKit` NU1902 warnings remain unchanged.

### 2026-03-23 Session 15: App Layer Wave 1 — Regression Coverage & Validation
**Role:** Testing + regression coverage for first application-layer refactor wave
**Outcome:** ✅ Regression coverage complete; Opplat.MainApp.Test 74/74 green; Phase Gate 1 gates locked

**What Was Done:**
1. Updated architecture regression suite to treat AddOpplatApplication(...) as approved host-level MediatR seam
2. Added source-contract guardrail on ServiceCollectionExtensions.cs (AddMediatR contract pinned)
3. Added converted-host surface assertions:
   - Opplat.MainApp account/license/menu features on minimal endpoints
   - Archived controllers remain un-mapped
   - Route logic stays in endpoint modules, not Program.cs
4. Expanded multitenancy/auth regression checks:
   - X-Tenant-Identifier header resolution
   - Finbuckle enforcement in OpplatDbContext
   - Middleware tenant-identifier rejection
   - Claim normalization (tenant_id / tenant_identifier)
5. Refreshed admin migration contract test
6. Validation: ✅ Opplat.MainApp.Test: 74/74 passing

**Phase Gate 1 Approval:**
- [x] Abstractions + Application created
- [x] Sales Application handlers complete (≥5 slices)
- [x] Sales.Api → minimal endpoints + MediatR
- [x] Build + tests green
- [x] Regression gates locked

**Coordination:**
- Validated Hicks Sales refactor (handlers, endpoints, archives)
- Validated Hudson infrastructure (DI seams, project references)
- Ready to enforce same coverage on Inventory + MainApp phases

---

