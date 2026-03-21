## Core Context

### Admin Callback & Claim Contract Fix (2026-03-21)
Normalized Keycloak role claims across backend auth to handle both nested (realm_access.roles, resource_access.{client}.roles) and flat dotted claim shapes. Updated AuthClaimTypes.cs and OidcClaimsTransformation.cs to accept both payload materializations without changing SuperAdmin role contract. Aligned with Vasquez's callback router fix and Bishop's regression testing; all validations pass.

### Finbuckle.MultiTenant Architecture (2026-02-27)
Full multi-tenant implementation with per-database isolation, route-based tenant resolution (/{tenant}/...), and header fallback (X-Tenant-Identifier). Database-per-tenant isolation chosen for maximum security. JWT tokens include tenant_id and tenant_identifier claims. Configuration-based tenant store in appsettings.json with 3 sample tenants (mojocafe, demo, test).

### Sales/Inventory Module Extraction (2026-03-18)
Moved existing Sales and Inventory business logic from Opplat.Domain/Opplat.Infrastructure into dedicated module projects (src/Modules/Sales, src/Modules/Inventory) while keeping Opplat.MainApp as HTTP composition root. Controllers remain in MainApp consuming module namespaces. Architecture supports future module independence.

### Finbuckle Package Alignment Revision (2026-03-17)
Reverted Hudson's attempted v10.0.4 upgrade (non-existent version) back to approved v7.0.1. Root namespace only; no .Extensions subnamespaces exist in v7.0.1. Route strategy single parameter. Finbuckle 7.0.1 is stable and compatible with .NET 10/EF Core 10.
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

### Admin auth backend implementation (2026-03-20)

- `Opplat.MainApp` now validates bearer tokens through OIDC authority/audience discovery instead of the old symmetric signing key flow.
- Finbuckle tenant resolution still uses route plus `X-Tenant-Identifier`, but the tenant catalog now persists through `Data/tenant-catalog.json` so admin tenant CRUD can update the active store without touching project files.
- Account endpoints no longer mint local JWTs; password-oriented flows now return clear IdP-owned messages while metadata/user-role management remains in ASP.NET Identity.
- Admin backend slices live under `Features/Admin/` and combine tenant catalog CRUD, cross-tenant user listing, and tenant-scoped user management endpoints.

### Phase 3: Finbuckle.MultiTenant Implementation (2026-02-27)

**Completed:** Full multi-tenant architecture using Finbuckle.MultiTenant 7.0.1

**Key Files Created:**
- `src/Opplat.MainApp/Models/AppTenantInfo.cs` — Tenant information model implementing ITenantInfo
- `src/Opplat.MainApp/Data/DesignTimeDbContextFactory.cs` — EF Core design-time factory for migrations
- `src/Opplat.MainApp/Services/TenantProvisioningService.cs` — Tenant database provisioning with migrations and seeding
- `src/Opplat.MainApp/Middleware/TenantValidationMiddleware.cs` — JWT tenant claim validation middleware

**Key Files Modified:**
- `src/Opplat.MainApp/Data/OpplatDbContext.cs` — Now implements IMultiTenantDbContext with per-tenant context
- `src/Opplat.MainApp/Program.cs` — Multi-tenant middleware, per-tenant DbContext, tenant-aware routing
- `src/Opplat.MainApp/appsettings.json` — Added Finbuckle configuration with 3 sample tenants (mojocafe, demo, test)
- `src/Opplat.MainApp/Controllers/AccountController.cs` — JWT now includes tenant_id and tenant_identifier claims

**Architecture Decisions:**
1. **Tenant Resolution:** Route strategy (/{tenant}/...) + header fallback (X-Tenant-Identifier)
2. **Data Isolation:** Full database-per-tenant (no shared tables)
3. **Tenant Store:** Configuration-based (in-memory, from appsettings.json)
4. **Identity:** Per-tenant ASP.NET Core Identity with separate admin users per tenant
5. **Backward Compatibility:** Kept old routes without tenant prefix for gradual migration

**Critical API Discovery:**
- `IMultiTenantContextAccessor<T>` is in `Finbuckle.MultiTenant.Abstractions` (not just `Finbuckle.MultiTenant`)
- Must use both namespaces: `using Finbuckle.MultiTenant;` AND `using Finbuckle.MultiTenant.Abstractions;`
- Constructor injection of `IMultiTenantContextAccessor<AppTenantInfo>` should be optional (nullable) for design-time scenarios

**Build Result:** ✅ SUCCESS (14 warnings, 0 errors)

**Next Steps for Team:**
- Vasquez (React): Update API base URL to include tenant identifier
- Bishop (Tests): Create test fixtures with mock tenant context
- Elvis: Decide on tenant onboarding strategy (manual vs. automated)

### Finbuckle Revision under Reviewer Lockout Protocol (2026-03-17)

**Context:** Hudson's Finbuckle upgrade attempt to v10.0.4 was rejected because that version doesn't exist. Ripley applied Reviewer Lockout Protocol (author cannot revise own rejection) and reassigned to Hicks.

**Assignment Rationale:** Hicks is Finbuckle.MultiTenant architect (designed Phase 3 architecture), backend expert, and not the original author of rejected work.

**Task:** Revise Finbuckle package alignment back to approved v7.0.1.

**Work Performed:**
- Reverted Finbuckle.MultiTenant packages to v7.0.1 in both MainApp and Infrastructure
- Removed non-existent namespace imports (`.AspNetCore.Extensions`, `.EntityFrameworkCore.Extensions`)
- Fixed API signature: `.WithRouteStrategy("__tenant__", false)` → `.WithRouteStrategy("__tenant__")`
- Preserved all other .NET 10 / EF Core 10 alignment (Npgsql at 10.0.0, ASP.NET Core at 10.0.5)

**Validation:**
- `dotnet build .\opplat.sln` ✅ SUCCESS
- `dotnet test .\opplat.sln --no-build` ✅ SUCCESS (0 discovered tests expected)
- No prerelease packages required

**Key Technical Lesson:**
Finbuckle 7.0.1 exposes root-namespace methods only (`UseMultiTenant`, `ConfigureMultiTenant`, `EnforceMultiTenant`). No `.Extensions` subnamespaces exist in v7.0.1. Route strategy accepts single parameter (the `__tenant__` placeholder name); boolean variants are not part of v7.0.1 API surface.

**Decision record created:** `.squad/decisions/inbox/hicks-finbuckle-revision.md`

**Status:** ✅ COMPLETE — Revision approved; Phase 1 locked and validated

### Phase 1: Sales / Inventory Module Extraction (2026-03-18)

**Task:** Move existing Sales and Inventory backend logic out of the legacy `Opplat.Domain` / `Opplat.Infrastructure` folders into the module projects under `src/Modules/Sales` and `src/Modules/Inventory` while keeping `Opplat.MainApp` as the HTTP composition root.

**Key Files Updated:**
- `src/Modules/Sales/Domain/**` and `src/Modules/Sales/Infrastructure/**` — now own Sales entities, services, repositories, and EF-backed repository implementations
- `src/Modules/Inventory/Domain/**` and `src/Modules/Inventory/Infrastructure/**` — now own Inventory entities, DTOs, services, repositories, and EF-backed repository implementations
- `src/Opplat.MainApp/Program.cs` — DI registrations now point at `Opplat.Modules.*` namespaces
- `src/Opplat.MainApp/Data/OpplatDbContext.cs` — DbSet aliases now point at module entity namespaces
- `src/Opplat.MainApp/Areas/Sales/Controllers/**` and `src/Opplat.MainApp/Areas/Inventory/Controllers/**` — controllers remain in MainApp but consume module namespaces
- `src/Opplat.MainApp/Opplat.MainApp.csproj` — references module Domain/Infrastructure projects

**Architecture Notes:**
1. `Opplat.MainApp` remains the presentation and composition root; Sales/Inventory controllers were intentionally kept there.
2. The legacy `src/Opplat.Domain/{Sales,Inventory}` and `src/Opplat.Infrastructure/{Sales,Inventory}` trees were removed after moving the code into the module projects.
3. Existing `src/Modules/*/Application` projects were kept as placeholders only; duplicate controllers were removed to avoid splitting the presentation root in Phase 1.
4. Historical EF migration designer/snapshot files were namespace-adjusted for compile integrity, but no new migration was created.
5. `CostTab` still depends on Inventory product entities across modules; that coupling was preserved deliberately to avoid behavioral churn during the structure-only refactor.

**Validation:**
- `dotnet build .\opplat.sln` ✅ SUCCESS
- `dotnet test .\opplat.sln --no-build` ✅ SUCCESS (0 discovered tests, pre-existing)

**User Preference Reinforced:**
- Prefer minimal API and behavior changes for structural refactors; keep incomplete Accounting/account features in place unless integrity requires a touch.

### Keycloak dev bootstrap and tenant role alignment (2026-03-21)

- `docker/keycloak/keycloak.conf` now owns the local Keycloak HTTP/hostname boot settings, while `docker/keycloak/opplat-realm.json` seeds the realm, clients, roles, and test users.
- The Keycloak import leaves built-in OIDC scopes (`profile`, `email`, `offline_access`) attached to the SPA clients and moves Opplat-specific tenant/audience mappers into dedicated scopes (`opplat-tenancy`, `opplat-api-audience`) to avoid scope-validation failures.
- Backend admin authorization now expects the `SuperAdmin` role, and tenant-side user-management endpoints in `Features/Account/AccountEndpoints.cs` are restricted to `TenantAdmin`.
- Tenant user creation and role changes now stay inside the `TenantAdmin` / `TenantUser` model: new tenant users default to `TenantUser`, and tenant role updates reject anything outside those tenant roles.

### 2026-03-21: Keycloak OIDC Scope Alignment & Backend Validation (Session 5)

**Task:** Implement frontend OIDC scope request updates and validate backend auth alignment with Keycloak bootstrap.

**Work Performed:**
1. **Updated Frontend OIDC Configurations**
   - Both `src/opplat-admin/src/auth/oidc.ts` and `src/opplat-react/src/auth/oidc.ts`
   - Now always request: `openid profile email offline_access`
   - Ensures frontend scopes align with Keycloak realm configuration

2. **Updated Backend Configuration**
   - Modified `src/Opplat.MainApp/appsettings.Development.json`
   - Added explicit local Keycloak host for dev environments
   - Added `TenantAdminRole` and `TenantUserRole` settings

3. **Updated Documentation**
   - README.md now includes:
     - Keycloak bootstrap guidance
     - Seeded user table with credentials
     - Local OIDC environment setup instructions
     - Realm-role mapping explanation

4. **Validation Results**
   - ✅ docker-compose config PASS
   - ✅ JSON syntax validation PASS (appsettings.Development.json, opplat-realm.json)
   - ✅ dotnet build PASS (44 pre-existing warnings, 0 errors)

**Key Findings:**
- Backend auth logic already correctly aligned with Keycloak issuer/audience
- Keycloak realm bootstrap already seeds correct users (SuperAdmin, TenantAdmin, TenantUser)
- No backend auth implementation changes required; only frontend scope + config updates needed

**Files Modified:**
- src/Opplat.MainApp/appsettings.Development.json
- src/opplat-admin/src/auth/oidc.ts
- src/opplat-react/src/auth/oidc.ts
- README.md

**Status:** ✅ COMPLETE — Frontend scopes aligned, backend config ready, all validation passing

### 2026-03-21: Compose-pinned OIDC scope contract for local Keycloak

- The remaining `Invalid scopes: openid profile email roles offline_access` login failure came from the SPA bootstrap contract, not the Keycloak realm export: the hot-reload Compose services never set `VITE_AUTH_SCOPE`, so the apps fell back to an older runtime default that still injected `roles`.
- Local Keycloak already bootstraps the right built-in scopes plus the Opplat custom scopes; the repo fix was to pin `VITE_AUTH_SCOPE=openid profile email offline_access` in both base and override Compose frontend services so every local path requests the same scope set.
- Safest validation for this kind of contract change is `docker-compose config` plus JSON parsing of the realm export, because it proves the merged env and Keycloak import agree without mutating shared local containers.

### 2026-03-21: Scope Contract Adjudication & Final Resolution (Team Sync)

**Cross-Agent Coordination:** Ripley synthesized findings from Hicks, Vasquez, and Bishop's independent investigations into a single authoritative scope contract.

**What Happened:**
1. **Hicks** confirmed the Keycloak realm export was correct and identified Docker Compose wiring as the gap.
2. **Vasquez** found that both SPAs were requesting overly broad scopes and aligned them to openid only.
3. **Bishop** created regression test harnesses and confirmed the drift between intended contract and frontend implementation.
4. **Ripley** adjudicated the conflict, ruling that:
   - The correct contract is openid profile email offline_access (NOT just openid, and NOT including oles)
   - oles must NEVER be in the scope request (Keycloak injects via defaultClientScopes automatically)
   - Vasquez's openid-only fix was too minimal (loses claims and refresh tokens)
   - All frontend untimeConfig.ts, .env.example, and README examples must align with Docker Compose's environment

**Key Learnings:**
- Keycloak does not expose oles as a requestable scope; it's a mapper configuration attached as a default client scope.
- Requesting oles in the scope parameter triggers "Invalid scopes" error.
- The openid profile email roles offline_access login failure came from stale .env.local or browser state, not current code.
- Scope configuration is multi-layer: changes must coordinate across Docker Compose, runtimeConfig, .env files, Dockerfile, and documentation.

**Files Updated:**
- src/opplat-react/src/runtimeConfig.ts — scope → openid profile email offline_access
- src/opplat-admin/src/runtimeConfig.ts — scope → openid profile email offline_access
- src/opplat-react/.env.example — documented correct scope
- src/opplat-admin/.env.example — documented correct scope
- README.md — local dev examples and env reference table aligned
- .squad/decisions.md — new section on agent findings merged from inbox

**Test Harnesses:** Both FrontendAuthContractTests and KeycloakRealmContractTests are now permanent regression guards for scope contracts.

**Status:** ✅ COMPLETE — Scope contract finalized across all layers, minimal corrections applied, team consensus recorded.

### 2026-03-21: Admin post-login role-claim normalization

- The admin portal and API both still require the `SuperAdmin` role after OIDC login; the seeded Keycloak `superadmin` user already matches that contract.
- The fragile seam was claim shape, not role naming: Keycloak role mappers can surface roles either as nested `realm_access` / `resource_access` objects or as flat dotted claim keys such as `realm_access.roles`.
- I normalized both backend and SPA claim readers to accept either shape, which removes a silent source of post-login authorization failures without changing the role model itself.
- Validation passed with `dotnet test .\test\Opplat.MainApp.Test\Opplat.MainApp.Test.csproj --filter Auth`, `dotnet build .\opplat.sln`, `npm --prefix .\src\opplat-admin run build`, and `npm --prefix .\src\opplat-react run build`.

