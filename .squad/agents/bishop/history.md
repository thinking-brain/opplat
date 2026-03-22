## Core Context

### 2026-03-22 Session 10: Admin Auth Simplification Test Coverage

**Role in Session 10:** Added focused regression coverage for simplified admin auth contract.

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

- Final admin login recovery is pinned at the frontend auth seam, not the realm-client seam alone: `AuthContext.tsx` must treat a restored non-expired OIDC user as authenticated, and `AuthCallbackPage.tsx` / `ProtectedRoute.tsx` must prefer that recovered session over transient shared auth errors.
- Keeping `opplat-client` and `opplat-admin` as separate public Keycloak clients remains the supported contract even with one shared backend audience; the regression suite now treats that separation plus callback recovery as the intended model.
- The repo already protects the admin SPA CORS/origin contract for `http://localhost:3201`: `KeycloakRealmContractTests` pins `opplat-admin` origins to `3101/3201/5174`, so a live token-endpoint CORS miss can still point to a stale running Keycloak realm rather than bad source.
- `react-oidc-context` setup needs its own regression seam beyond callback-page UI: source contracts should pin `AuthProvider` wiring of `onSigninCallback` and browser `localStorage` user persistence, because restored-session behavior depends on both pieces even when route/error handling tests are already green.
- During the admin-first BFF cutover, the safest testing pattern is split coverage: keep executable tests for current tenant-isolation seams (`X-Tenant-Identifier` + route alignment) and add skipped target-contract tests for `/bff/auth/session`, cookie auth, CSRF, and server-driven login/logout until Hicks/Vasquez land the implementation.
- For admin BFF endpoints that call `AuthenticateAsync("AdminCookie")` directly, TestServer coverage must register the same named scheme in addition to the default test scheme; otherwise current-user regressions hide behind fake auth setups and the endpoint path is never truly exercised.
- The most useful regression for `/admin/session/current-user` is a sparse-claim cookie principal: assert the route still returns `200 OK` with empty optional fields plus the expected login/logout/CSRF metadata, so missing profile claims cannot silently reintroduce a 500 during session bootstrap.
- To guard the admin redirect-origin bug, integration coverage must configure `AuthOptions.AdminBff.AllowedOrigins` in the test host and hit `/auth/bff/admin/login` with an `Origin` header; otherwise relative `returnUrl` assertions collapse to bare paths and miss the `3201 vs 3001` fallback behavior entirely.
- For the simplified admin rollout, auth/session regressions should treat tenant context as optional at sign-in bootstrap: assert sparse admin cookie claims still succeed with empty tenant fields, while keeping tenant-header validation pinned only on tenant-scoped admin API routes.
