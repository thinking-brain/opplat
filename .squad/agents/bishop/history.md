## Core Context

### 2026-03-23 Session 14: Admin Tenant Boundary Refactor — Test Validation & Regression Coverage

**Role in Session 14:** Enforced admin API boundary shift in regression test layer. Reset coverage to validate new tenant catalog scope.

**What Was Done:**
1. **Admin Contract Tests:** Reset `AdminApiContractTests` to require new schema: `DatabaseName`, `DatabaseSchema`, `MaxUsers`, `CurrentUserCount`. Tests now fail if any user-owned fields remain in admin contracts.
2. **Boundary Enforcement:** Regression coverage rejects any remaining `/admin/users` endpoints, admin-owned user DTOs, or full `ConnectionString` fields in admin API shape.
3. **Frontend Contract Tests:** Fixed `FrontendAuthContractTests` to align with new admin SPA shape (catalog only, no users). Tests no longer expect user management routes or bearer token handling in admin SPA.
4. **Test Results:** ✅ All 65/65 tests passing | ✅ Frontend build: 0 errors | ✅ Frontend lint: passing

**Outcome:** Regression coverage enforces new architectural boundary. Any future code that tries to reintroduce admin-owned user management or exposed connection strings will fail validation immediately.

**Validation:**
- ✅ `dotnet build src\Opplat.AdminApi\Opplat.AdminApi.csproj --no-restore`
- ✅ `dotnet test test\Opplat.MainApp.Test\Opplat.MainApp.Test.csproj --no-restore` (65/65 pass)
- ✅ `npm --prefix src\opplat-admin run lint`
- ✅ `npm --prefix src\opplat-admin run build`
- ✅ `docker compose config --quiet`

**Coordination:** Ripley approved boundary definition; Hicks implemented backend; Vasquez aligned frontend; Bishop locked coverage.

---

### 2026-03-22 Session 13: Admin Auth Removal — Test Cleanup and Validation

**Role in Session 13:** Completed final admin auth removal cycle by retiring legacy admin auth regression tests per user directive and validating `src/Opplat.AdminApi` integration.

**What Was Done:**
1. **Retired Legacy Tests:** Removed regression tests pinning old shared admin auth/BFF surface in MainApp. Removed admin SPA auth bootstrap assertions (user owns auth now).
2. **Kept Essential Coverage:** Preserved client SPA OIDC seams; preserved admin frontend dedicated-api routing contract; preserved MainApp tenant isolation seams.
3. **Validated Admin API Split:** Verified `src/Opplat.AdminApi` project structure, Docker Compose wiring, health endpoint routing, admin frontend proxy contract.
4. **Final Results:** ✅ 65/65 auth tests passing | ✅ No false-red regressions | ✅ Compose startup validation passing

**Outcome:** Test suite clean. Legacy assertions removed. Admin-api contract locked. Ready for production.

**Coordination:** Hicks consolidated admin auth; Vasquez removed auth from admin client; Hudson deleted legacy service directory.

---

### 2026-03-22 Session 12: Admin API Startup Regression Coverage

**Role in Session 12:** Added focused startup-contract assertions to pin admin API compose port binding, `/health` probe path, controller mapping, and Dockerfile runtime entrypoint.

**Coverage Added:**
- File: `test/Opplat.MainApp.Test/Auth/AdminApiSplitContractTests.cs`
- Assertions: Admin API compose port `8084`, `/health` probe returns 200, `HealthController` explicit route, Dockerfile entrypoint correct
- Container health check: Verified `docker compose up -d admin-api` yields healthy container
- Validation stance: Startup is green only when source contracts pass **and** `http://localhost:8084/health` responds

**Test Results:**
- ✅ Focused AdminApi startup regression tests pass
- ✅ No regressions in existing test suite

**Outcome:** Startup regression coverage locked. Compose startup is deterministic and verifiable.

**Coordination:** Hicks removed duplicate routing; Hudson verified health endpoint behavior.

---

### 2026-03-22 Session 12: Temporary Admin Shell Mode Test Coverage

**Role in Session 12:** Locked temporary admin shell mode contract by implementing focused regression tests covering feature gating and auth boundary preservation.

**Implementation:**
1. **Shell Mode Contract Tests:** Feature endpoints return 503 when shell mode active; core auth routes always accessible
2. **Auth Preservation Tests:** `/admin/session/current-user`, `/admin/session/csrf`, login, logout all work regardless of shell state
3. **BFF Integration Tests:** Cookie auth, CSRF flow, redirect origin all passing
4. **Regression Guards:** Shell mode cannot silently break auth; feature gates cannot silently re-enable without code change

**Test Suite Status:**
- Full auth test suite: **65/65 passing** ✅
- Feature-gate tests: All green
- Auth core tests: All green
- No regression from prior work

**Outcome:** Shell mode contract locked in. Regression points are executable guardrails. Ready for deployment.

---

### 2026-03-22 Session 11: Admin Auth Simplification Test Coverage

**Role in Session 11:** Added focused regression coverage for simplified admin auth contract.

**Test Additions:**
1. **Admin bootstrap tenant-agnostic:** `/admin/session/current-user` returns 200 with session DTO for SuperAdmin principal even when tenant claims are absent
2. **Origin regression pinned:** Integration test exercises `/auth/bff/admin/login` with `Origin: http://localhost:3201` and relative `returnUrl`; future fallback changes cannot silently bounce admins to 3001
3. **Tenant isolation verified:** Tenant-scoped admin API routes remain isolated via `X-Tenant-Identifier` header and route/header alignment tests

**Test Suite Status:**
- Admin auth-focused tests: ✅ green
- No regression in tenant isolation or admin management endpoints
- All assertions aligned to tenant-free session contract

**Outcome:** Auth simplification locked in. Regression points are executable guardrails.

---

### 2026-03-22 Session 9: Admin BFF Test Suite Repair (Lockout Protocol)

**Role in Session 9:** Locked out by Ripley's defect discovery on test suite. Repaired test assertions to match current implementation.

**Repairs Implemented:**
1. **Rewrote `FrontendAuthContractTests.cs`:** Now targets mixed-auth tree: `opplat-react` remains browser-managed OIDC; `opplat-admin` now uses server-backed BFF session contract. Removed 12 failing assertions targeting removed `oidc.ts` file.
2. **Un-skipped `AdminBffSessionContractTests.cs`:** Rewrote to validate actual BFF implementation shape: `/admin/session/current-user`, `/admin/session/csrf`, `/auth/bff/admin/login`, `/auth/bff/admin/logout`, antiforgery contract, cookie/OIDC dual mode.
3. **Updated `KeycloakRealmContractTests.cs`:** Changed validation from OIDC client config to BFF-driven config (`VITE_BFF_BASE_URL`, logout redirect).

**Validation Results:**
- `dotnet test test\Opplat.MainApp.Test\Opplat.MainApp.Test.csproj --filter "FullyQualifiedName~Auth"` ✅
- `dotnet test test\Opplat.MainApp.Test\Opplat.MainApp.Test.csproj` ✅
- 50 tests pass, 0 skip, 0 fail.

**Consequence:** Auth suite now reflects current integrated tree instead of targeting deleted/pre-migration code paths. All regression coverage active and green.

**Status:** Ready for Ripley re-review.

---

### 2026-03-22 Session 10: Admin Current-User 500 Sparse-Claims Regression Coverage

**Role in Session 10:** Added focused regression coverage for `/admin/session/current-user` endpoint after Ripley diagnosed Docker dev proxy networking issue (not auth design defect).

**Test Coverage Added:**
1. **Sparse-claims principal test** — Validates endpoint returns `200 OK` with session DTO even when optional profile/tenant fields are missing. Guards against null-reference 500 reintroduction during live bootstrap.
2. **Healthy full-claim principal test** — Validates complete BFF session DTO with all expected fields, login/logout/CSRF metadata.

**Implementation Details:**
- Registered both default test scheme and exact `"AdminCookie"` named scheme in TestHost
- Exercises actual `AuthenticateAsync("AdminCookie")` path that live admin SPA hits from `http://localhost:3201`
- Pinned contract ensures endpoint shape is defended against future claim-shape regressions

**Validation Results:**
- Full auth test suite: **56/56 passing** ✅
- Sparse-claim test: ✅ Green
- Healthy-claim test: ✅ Green
- No regressions from prior session

**Consequence:** Backend regression suite now guards endpoint contract. Any future change that reintroduces null-path errors on sparse claims will surface as test failure instead of leaving admin SPA to discover at runtime.

**Status:** Testing complete. Docker proxy fix implementation (Hudson) can proceed with confidence that endpoint contract is defended.

---


**Role in Session 8:** Validated regression test suite coverage for two-client model and callback recovery. Confirmed:
1. Realm contract tests already pin exactly two SPA clients with correct origin families
2. Callback contract tests already verify restored sessions override transient error state
3. Protected route tests already validate auth-error only shows when truly unauthenticated
4. Repo contract validates `http://localhost:3201` for `opplat-admin` client

**Finding:** CORS failure on token endpoint is compatible with repo being correct. A live Keycloak CORS miss can indicate stale container state even when source is correct. Regression suite now serves as early detection: if callback or protected route tests fail, the callback recovery seam (not the realm) is the problem.

**Cross-Team Coordination:** Bishop confirmed repo contract with Hudson (infrastructure), Vasquez (frontend), and Ripley (architecture validation).
Added comprehensive regression test coverage validating the two-client Keycloak architecture and callback recovery flow:
1. Realm contract tests pin exactly two SPA clients with expected origin families
2. Frontend callback contract tests require both SPAs to prefer restored authenticated session over transient shared auth errors
3. Protected route contract tests validate recovered sessions unblock UI before showing auth errors

All builds passing, all tests passing. Two-client Keycloak contract now guarded in test suite.

### Callback Regression Contract Coverage (2026-03-21 Session 6b)
Added focused test coverage for callback return-target handling in \FrontendAuthContractTests.cs\ pinning safe recovery seam.

## Archived Context (Prior Sessions)

See \.squad/orchestration-log/\ for detailed session outcomes. Key testing milestones: admin callback regression coverage (2026-03-21 Session 6b), two-client contract validation (2026-03-21 Session 7).

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
- Source-contract tests in this repo should resolve the root via a shared helper that accepts both `opplat.sln` and `opplat.slnx`; otherwise solution-file churn creates false-red auth failures unrelated to the behavior under test.
- For the admin API MediatR/PostgreSQL migration, the safest regression pattern is dual coverage: execute the real endpoint handlers against an in-memory `AdminTenantIdentityDbContext` seeded with PostgreSQL-shaped tenant data, and separately pin source contracts for `AddMediatR`, `IMediator` endpoint injection, `UseNpgsql`, and tenant-claim normalization.

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
