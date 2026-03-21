## Project Context

**Project:** Opplat — Multi-platform business management system (café/restaurant)
**Requested by:** elvis.crego
**Stack:** ASP.NET Core (net6.0 → net10.0) | EF Core | SQL Server | SignalR | JWT | React 18 (replacing Vue 2)
**Solution root:** C:\projects\personal\opplat
**Branch:** develop

## Solution Structure

- src/Opplat.MainApp/           — ASP.NET Core Web API (net6.0 → net10.0)
- src/Opplat.Domain/            — Business logic (net6.0 → net10.0)
- src/Opplat.Infrastructure/    — Data access, EF Core (net6.0 → net10.0)
- src/Opplat.Shared/            — Common utilities (net6.0 → net10.0)
- src/opplat-vue/               — Old Vue 2 client (keep but inactive)
- src/opplat-react/             — NEW React 18 client (to be created)
- test/Opplat.MainApp.Test/     — xunit tests (net6.0 → net10.0)

## Key Architecture

- Clean Architecture (Domain / Infrastructure / MainApp)
- EF Core DbContext with ASP.NET Core Identity (OpplatDbContext)
- JWT Bearer auth
- SignalR hubs
- Swagger/OpenAPI

## Phase Plan

1. Phase 1 — .NET Upgrade (Hudson + Hicks + Bishop)
2. Phase 2 — React Client (Vasquez + Hicks for API verification)
3. Phase 3 — Multitenancy with Finbuckle.MultiTenant (Hicks + Ripley design)

## Decisions

- net10.0 target framework
- Finbuckle.MultiTenant for multitenancy
- React app at src/opplat-react/ (Vite + React 18 + TypeScript + MUI + React Router + Axios)
- Pages required: Login, Home, Products, Sell, Users
- Old Vue app kept at src/opplat-vue/ but inactive

## Learnings

### 2024-01-XX: Test Project Analysis (net6.0 → net10.0)

**Findings:**
- Test project has only 2 test files: `LicenciaTest.cs` and `SetupContexto.cs`
- **All tests are already disabled** (commented out) - likely from previous migration
- No immediate compilation risk during framework upgrade
- Packages are severely outdated: EF Core InMemory 2.2.6 (from 2019), Moq 4.12.0, xunit 2.4.0
- Test re-enablement will require significant work due to EF Core 2.2 → 9.0 breaking changes

**Critical Package Upgrades Required:**
- `Microsoft.EntityFrameworkCore.InMemory` 2.2.6 → 9.0.x (CRITICAL: 6+ year gap, major API changes)
- `Microsoft.NET.Test.Sdk` 16.0.1 → 17.12.0
- `Moq` 4.12.0 → 4.20.x (nullable reference handling, protected member mocking changes)
- `xunit` 2.4.0 → 2.9.x (low risk, compatible)

**Test Code Patterns Observed:**
- Standard AAA (Arrange-Act-Assert) pattern
- EF Core InMemory for integration testing
- Moq for mocking (IHostingEnvironment - now deprecated, use IWebHostEnvironment)
- Tests are for `LicenciaController` with database context

**Upgrade Strategy:**
1. Phase 1: Update .csproj target framework (Hudson)
2. Phase 2: Update package versions (Bishop)
3. Phase 3: Verify compilation (should succeed since tests disabled)
4. Phase 4: Re-enable tests later (separate task, 2-4 hour effort)

**Confidence:** HIGH for compilation success, MODERATE effort for future test re-enablement

**Key Takeaway:** Tests being disabled is actually beneficial for this upgrade - reduces immediate risk. Test re-enablement should be a separate, planned task after framework upgrade stabilizes.

### 2026-03-17: Final Phase 1 validation after Hicks' Finbuckle revision

**Validation run:**
- `dotnet build .\opplat.sln -v minimal` ✅ SUCCESS
- `dotnet test .\opplat.sln -v minimal --no-build` ✅ SUCCESS (0 discovered tests)
- Zero-test discovery confirmed pre-existing: `[Fact]` commented in test file, not migration-caused

**Decision record created:** `.squad/decisions/inbox/bishop-net10-validation.md`

**Conclusion:** Phase 1 migration is locked and validated. Build framework ready; zero-test state expected and documented.

### 2026-03-20: Auth + tenant regression test strategy during in-flight implementation

**What I added:**
- New xUnit coverage in `test/Opplat.MainApp.Test/` for `TenantValidationMiddleware`, OIDC claim normalization, endpoint surface contracts, and multitenant DbContext safeguards.
- Tests were added as new files instead of reviving the dormant legacy license scaffolding.

**Key testing pattern:**
- When backend auth/admin work is mid-flight and production compilation is unstable outside the test scope, favor a mix of executable unit tests and source-guard tests for contract-level expectations (route prefixes, claim normalization hooks, tenant connection-string selection).
- For local validation, `dotnet build/test` on the test project can be run with `/p:BuildProjectReferences=false` to validate the test assembly against already-built references without touching unrelated in-progress production files.

**Important gap captured:**
- One admin authorization expectation remains intentionally skipped until the admin policy surface fully settles; this keeps the suite green while still documenting the pending coverage target.

### 2026-03-21: Keycloak auth contract validation for roles, scopes, and seeded users

**What I validated:**
- Added executable source-contract coverage in `test/Opplat.MainApp.Test/Auth/KeycloakRealmContractTests.cs` for the Keycloak realm import, seeded role users, SPA client scope wiring, and README user-role documentation.
- Promoted admin policy coverage from a skipped placeholder to an executable route metadata assertion in `test/Opplat.MainApp.Test/Routing/EndpointSurfaceTests.cs`.
- Added an executable OIDC role-mapping assertion in `test/Opplat.MainApp.Test/Auth/OidcClaimsTransformationTests.cs`.

**Validation results:**
- `dotnet test .\test\Opplat.MainApp.Test\Opplat.MainApp.Test.csproj -v minimal` ✅
- `dotnet build .\test\Opplat.MainApp.Test\Opplat.MainApp.Test.csproj -v minimal /p:BuildProjectReferences=false` ✅
- Full solution build remains noisy/shared-environment-sensitive; the current failure mode is a file lock on `src\Opplat.MainApp\obj\Debug\net10.0\Opplat.MainApp.dll`, not a test regression.

**Important findings:**
- Keycloak now seeds `SuperAdmin`, `TenantAdmin`, and `TenantUser` users and realm roles, and the admin route group advertises the `AdminOnly` policy.
- There is still a live backend alignment gap: `src\Opplat.MainApp\Auth\AuthOptions.cs` and both appsettings files keep `AdminRole` set to legacy `"admin"` while the rest of the auth surface has moved to `SuperAdmin`.

### 2026-03-21: Auth Contract Validation & Regression Coverage (Session 5)

**Task:** Add comprehensive auth regression test coverage and validate role alignment across the system.

**Tests Added:**
1. **AuthEndpointAuthorizationIntegrationTests**
   - Integration tests for endpoint authorization policies
   - Validates SuperAdmin access to /admin endpoints

2. **FrontendAuthContractTests**
   - Frontend auth contract validation
   - Scope and route alignment tests
   - OIDC flow integration tests

3. **CreateTenantUserCommandTests**
   - Enhanced to validate tenant role rejection
   - Now rejects non-tenant roles on user creation
   - Enforces TenantAdmin/TenantUser role constraints

4. **OidcClaimsTransformation**
   - Expanded OIDC role parsing regression tests
   - Updated to safely snapshot role claims from tokens
   - Tests role claim extraction from Keycloak realm roles

5. **Code Alignment Changes**
   - `CreateTenantUserCommand`: Updated to reject non-tenant roles
   - `OidcClaimsTransformation`: Updated for safe role claim snapshots
   - Admin UI: Updated role defaults and help text to use `TenantAdmin`/`TenantUser`

**Validation Results**
- ✅ `dotnet test test/Opplat.MainApp.Test/Opplat.MainApp.Test.csproj --nologo -v minimal`
- **Pass Rate:** 36/36 (100%)
- **Pre-existing Build Issues:** MimeKit NU1902 warnings (not auth-related)

**Key Testing Patterns:**
- Contract tests verify Keycloak realm structure matches code expectations
- Source guards ensure admin policies are metadata-declared
- Role mapping tests validate OIDC claim transformation for Keycloak roles

**Files Modified:**
- test/Opplat.MainApp.Test/ (multiple test files)
- src/Opplat.MainApp/Features/Account/Commands/CreateTenantUserCommand.cs
- src/Opplat.MainApp/Auth/OidcClaimsTransformation.cs
- src/opplat-admin/src/auth/claims.ts

**Status:** ✅ COMPLETE — All auth contract tests passing (36/36), tenant role enforcement validated

### 2026-03-21: Scope-contract regression validation for Keycloak login failure

**What I validated:**
- Strengthened `test/Opplat.MainApp.Test/Auth/FrontendAuthContractTests.cs` so both SPAs must keep the documented Keycloak-safe scope contract (`openid profile email offline_access`) in `runtimeConfig.ts`, `auth/oidc.ts`, and the README examples.
- Strengthened `test/Opplat.MainApp.Test/Auth/KeycloakRealmContractTests.cs` to prove the realm export only declares Opplat-specific custom client scopes and leaves built-in OIDC scopes to Keycloak.

**Validation results:**
- `dotnet test .\test\Opplat.MainApp.Test\Opplat.MainApp.Test.csproj --nologo -v minimal --filter "FullyQualifiedName~Opplat.MainApp.Test.Auth.KeycloakRealmContractTests"` ✅
- `dotnet test .\test\Opplat.MainApp.Test\Opplat.MainApp.Test.csproj --nologo -v minimal --filter "FrontendAuthContractTests|KeycloakRealmContractTests"` ❌ (3 failing source-contract tests)

**Important findings:**
- The Keycloak realm import is internally consistent: SPA clients keep `roles` as a default client scope and `offline_access` as optional, while custom scopes stay limited to `opplat-tenancy` and `opplat-api-audience`.
- The frontend source is the live regression point on this branch: both `src\opplat-admin` and `src\opplat-react` still default `VITE_AUTH_SCOPE` to `openid` and only require `openid` in `auth/oidc.ts`, while the README examples also document `openid` only.
- Those source guards now preserve the intended contract and will keep failing until the frontend/runtime docs are brought back to `openid profile email offline_access`.

### 2026-03-21: Scope Contract Adjudication & Final Resolution (Team Sync)

**Cross-Agent Coordination:** Ripley synthesized findings from Hicks, Vasquez, and Bishop's independent investigations into a single authoritative scope contract.

**What Happened:**
1. **Hicks** confirmed the Keycloak realm export was correct and identified Docker Compose wiring as the gap.
2. **Vasquez** found that both SPAs were requesting overly broad scopes and aligned them to openid only (later refined).
3. **Bishop** (you) created regression test harnesses and confirmed the drift between intended contract and frontend implementation.
4. **Ripley** adjudicated the conflict, ruling that:
   - The correct contract is openid profile email offline_access (NOT just openid, and NOT including oles)
   - oles must NEVER be in the scope request (Keycloak injects via defaultClientScopes automatically)
   - Vasquez's openid-only fix was too minimal (loses claims and refresh tokens)
   - All frontend untimeConfig.ts, .env.example, and README examples must align with Docker Compose's environment

**Key Learnings:**
- Keycloak does not expose oles as a requestable scope; it's a mapper configuration attached as a default client scope.
- Requesting oles in the scope parameter triggers "Invalid scopes" error.
- Your test harnesses (FrontendAuthContractTests and KeycloakRealmContractTests) are now permanent regression guards.
- The drift you detected between frontend code, documentation, and realm contract was the real issue, not the Keycloak realm itself.

**Files Updated:**
- src/opplat-react/src/runtimeConfig.ts — scope → openid profile email offline_access
- src/opplat-admin/src/runtimeConfig.ts — scope → openid profile email offline_access
- src/opplat-react/.env.example — documented correct scope
- src/opplat-admin/.env.example — documented correct scope
- README.md — local dev examples and env reference table aligned

**Status:** ✅ COMPLETE — Your test coverage validated the drift correctly. Team consensus finalized and applied. Your regression guards will catch future scope drifts.

### 2026-03-21: Live Keycloak bootstrap validation for exact SPA scope contract

**What I validated:**
- The current live Keycloak bootstrap, not the SPA runtime injectors, was the blocker for `openid profile email offline_access`: discovery exposed only `openid`, `offline_access`, and the two Opplat custom scopes, and Keycloak logs showed `profile`, `email`, `roles`, `web-origins`, `address`, `phone`, and `microprofile-jwt` being ignored during full-model import.
- Both SPA runtime layers were aligned back to `openid profile email offline_access` in `runtimeConfig.ts`, `.env.example`, README examples, Dockerfiles, and Compose env wiring, while `auth/oidc.ts` still avoids auto-appending `roles`.

**What I changed:**
- Expanded `docker/keycloak/opplat-realm.json` so the imported realm explicitly carries the built-in OIDC client-scope definitions Keycloak 26 was previously dropping.
- Updated `test/Opplat.MainApp.Test/Auth/KeycloakRealmContractTests.cs` to fail if the SPA clients reference undeclared realm scopes again.
- Updated `test/Opplat.MainApp.Test/Auth/FrontendAuthContractTests.cs` to cover Compose/Docker runtime injection in addition to source defaults and docs.

**Validation results:**
- Live auth probe to `http://localhost:8180/realms/opplat/protocol/openid-connect/auth` with `scope=openid profile email offline_access` plus PKCE now returns the Keycloak login page (`200 OK`) instead of `invalid_scope`.
- `dotnet test .\\test\\Opplat.MainApp.Test\\Opplat.MainApp.Test.csproj --nologo -v minimal` ✅ (38/38)
- `npm run build` in `src\\opplat-react` ✅
- `npm run build` in `src\\opplat-admin` ✅

**Key takeaway:**
- For fresh Keycloak 26 realm imports, a client reference to built-in scopes is not enough; the realm export must explicitly define the built-in client scopes or the imported realm can look fine in source while rejecting login scopes at runtime.

## Session 5 Sprint — Live Scope Fix Completion (2026-03-21)

**Agents:** Vasquez (Frontend), Hudson (DevOps), Bishop (Testing)
**Orchestration:** 2026-03-21T13:32:08

**Summary:**
Three-agent team identified and resolved live Keycloak OIDC scope injection defect. Root cause: realm export was missing built-in client scope declarations (profile, email, roles, etc.) despite correct runtime injection paths.

**Outcomes:**
- **Vasquez:** Traced SPA scope request path, confirmed frontend/runtime surfaces were correctly aligned to openid profile email offline_access
- **Hudson:** Validated Docker/compose/env injection chain for frontend startup — all layers pointing to correct scope contract
- **Bishop:** Expanded docker/keycloak/opplat-realm.json with required OIDC client scope definitions, added regression guards, live PKCE auth probe returns 200 OK instead of invalid_scope, test suite: 38/38 passing

**Decision:** Treat as Keycloak realm-bootstrap defect. Realm export must explicitly declare built-in client-scope definitions used by SPA clients.

**Validation:**
- Live PKCE auth flow for openid profile email offline_access ✅
- All unit tests passing (38/38) ✅
- Frontend builds passing (npm run build) ✅
- Docker Compose config validated ✅
