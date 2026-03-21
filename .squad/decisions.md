# Opplat Squad — Decisions

## Session 5 Decisions (2026-03-21)

### Keycloak OIDC Scope & Role Model Alignment (Complete)
**By:** Ripley (Architect), Hicks (Backend), Hudson (DevOps), Vasquez (Frontend), Bishop (Tester)
**Date:** 2026-03-21
**Status:** ✅ COMPLETE — All agents delivered, no blockers
**What:** Comprehensive Keycloak authentication bootstrap and role model alignment across all layers

#### 1. OIDC Scope Configuration (Ripley)
**Root Cause:** SPAs defaulted to `openid` only; missing `profile`, `email`, `roles`, `offline_access`. Keycloak realm was mostly correct; frontend scope requests were incomplete. Docker-compose never set `VITE_AUTH_SCOPE`.

**Resolution:**
- Frontend SPAs now request: `openid profile email roles` (+ `offline_access` if offline session needed)
- Keycloak realm already had default client scopes configured correctly
- Backend validates `audience + roles`, not scope claim directly
- No scope-based API authorization currently required (future option)

#### 2. Three-Tier Role Model & Boundaries (Ripley → Hicks, Vasquez, Bishop)
**Roles:**
- `SuperAdmin` → Platform-wide administrator, admin app only
- `TenantAdmin` → Per-tenant administrator, client app user management section
- `TenantUser` → Regular tenant user, client app operations

**Access Control:**
- `/admin/*` endpoints → SuperAdmin only
- `/users` client app routes → TenantAdmin only
- Client app root → Any authenticated user (not SuperAdmin)

**Tenant Scoping:** Via user attributes/groups, not roles (single-tenant-per-user model)

#### 3. Docker-Compose Infrastructure (Hudson)
**Changes:**
- Keycloak service with env-driven bootstrap (admin creds, realm import)
- Explicit bind mounts for `docker/keycloak/keycloak.conf` and realm JSON
- Realm-aware health checks for service dependency ordering
- Added API/sales/inventory healthchecks
- Fixed `.env`: `VITE_SALES_API_URL` port (8081 → 8083)
- Added Keycloak env vars: `KEYCLOAK_PORT`, `KEYCLOAK_REALM`, `KEYCLOAK_ADMIN_USERNAME`, `KEYCLOAK_ADMIN_PASSWORD`

**Validation:** ✅ docker-compose config PASS, health checks ready

#### 4. Backend Auth Alignment (Hicks)
**Findings:**
- Keycloak realm bootstrap already correct (pre-seeded test users, roles)
- Backend Program.cs already aligns with Keycloak issuer/audience
- No backend auth logic changes required

**Changes:**
- Updated `src/Opplat.MainApp/appsettings.Development.json` for local Keycloak host
- Updated frontend OIDC configs to request correct scopes
- Updated README with Keycloak bootstrap guidance, seeded user table, local OIDC setup

**Validation:** ✅ dotnet build PASS (44 pre-existing warnings, 0 errors)

#### 5. Frontend Auth Gating & Scope Alignment (Vasquez)
**Changes:**
- Admin app: All routes require SuperAdmin role
- Client app: `/users` route + nav item + dashboard quick-access require TenantAdmin
- Both frontends now request: `openid profile email roles`
- Claims parsing now includes `realm_access.roles` + `resource_access.*.roles`
- Audience query param skipped for Keycloak realm URLs (kept for custom providers)

**Validation:** ✅ Both apps lint+build successful

#### 6. Test Coverage & Validation (Bishop)
**Coverage Added:**
- AuthEndpointAuthorizationIntegrationTests
- FrontendAuthContractTests
- Updated CreateTenantUserCommand to reject non-tenant roles
- Enhanced OidcClaimsTransformation for safe role claim snapshotting

**Validation:** ✅ 36/36 tests PASS (100%)

### Seeded Development Users (From Keycloak realm)
| Username | Password | Role | Tenant | Site |
|----------|----------|------|--------|------|
| superadmin | SuperAdmin123! | SuperAdmin | N/A | Admin (3101) |
| admin@mojocafe | admin123 | TenantAdmin, TenantUser | mojocafe | Client (3100) |
| admin@demo | admin123 | TenantAdmin, TenantUser | demo | Client (3100) |
| user@mojocafe | user123 | TenantUser | mojocafe | Client (3100) |
| user@demo | user123 | TenantUser | demo | Client (3100) |

### Files Modified
- docker-compose.yml (health checks, bind mounts, service deps)
- .env / .env.docker (Keycloak env vars)
- src/Opplat.MainApp/appsettings.Development.json
- src/opplat-admin/{auth/oidc.ts, auth/claims.ts, runtimeConfig.ts}
- src/opplat-react/{auth/oidc.ts, auth/claims.ts, runtimeConfig.ts}
- test/Opplat.MainApp.Test/ (auth regression coverage)
- README.md (startup, auth flow, role docs, seeded users)

### Outstanding Items (Deferred)
- Finbuckle file-backed tenant store for admin CRUD (separate session)
- Production Keycloak setup guidance (future phase)

### Acceptance Criteria Met
✅ Keycloak starts with valid realm import  
✅ Apps authenticate with correct scopes  
✅ SuperAdmin accesses admin site; tenant users cannot  
✅ Tenant users access client app; SuperAdmin cannot  
✅ README documents auth flow, roles, default credentials  
✅ Docker-compose health checks ready  
✅ All tests pass (36/36)  

**Status:** ✅ APPROVED & IMPLEMENTED

---

## Session 4 Decisions (2026-03-20)

### Phase 1 Sales and Inventory Module Extraction
**By:** Hicks (Backend Dev)
**Date:** 2026-03-18
**What:** Moved Sales and Inventory backend implementation out of legacy `src/Opplat.Domain/*` and `src/Opplat.Infrastructure/*` into modular structure:
- `src/Modules/Sales/{Domain,Infrastructure}`
- `src/Modules/Inventory/{Domain,Infrastructure}`
**Why:** Structural refactor aligns with Phase 1 architecture; keeps controllers in MainApp to preserve routing without invasive composition rewrite
**Decision:**
- MainApp references module Domain/Infrastructure projects directly
- Controllers depend on `Opplat.Modules.*` namespaces
- Legacy Sales/Inventory source trees removed after move
- Module Application projects left as placeholders (future MediatR handlers)
- Sales-to-Inventory coupling (`CostTab` → Inventory product) preserved for Phase 1
**Consequences:** Feature code now in module projects; MainApp owns routing, middleware, DbContext, auth, tenant pipeline
**Status:** ✅ Accepted
**Related Decision:** ripley-phase1-boundaries.md (architecture verification)

### Architecture Decision: Admin App, Auth0/Keycloak Auth, Tenant Identity Flow
**By:** Ripley (Lead/Architect)
**Date:** 2026-03-20
**Status:** ✅ APPROVED — Ready for implementation
**What:** Comprehensive redesign of auth stack and admin surface
**Key Decisions:**
1. **Admin App:** New standalone React app at `src/opplat-admin/` (Vite + React 18 + MUI, same stack as client)
   - Serves platform operators (tenant CRUD, user management, system config)
   - Independent from client app (`opplat-react`)
   - Docker service on port 3001
2. **Auth Architecture:** Replace custom JWT with OIDC/OAuth2 (Auth0 prod, Keycloak local dev)
   - Backend validates tokens from either IdP via OIDC discovery
   - `Auth__Authority` + `Auth__Audience` environment variables
   - Custom claims for tenant_id, tenant_identifier injected by IdP
   - Remove SymmetricSecurityKey, LoginCommand, custom token issuance
3. **Tenant Identity Flow:** Token claims + Finbuckle route strategy
   - User logs in → IdP issues token with tenant_id + tenant_identifier
   - Client app routes request as `/{tenant}/api/...`
   - TenantValidationMiddleware validates token claim matches route tenant
   - Prevents cross-tenant token replay
4. **Backend Surface:** All surfaces in MainApp (no separate API)
   - `/{__tenant__}/api/...` — client APIs (existing)
   - `/admin/tenants` — tenant CRUD (new, requires admin role)
   - `/admin/users` — cross-tenant user management (new, requires admin role)
   - `/admin/settings` — system config (new, requires admin role)
5. **Keycloak Local Dev Setup:** Realm import JSON with pre-seeded clients, users, mappers
   - Realm: `opplat`
   - Clients: `opplat-client` (3000), `opplat-admin` (3001)
   - Test users: admin@{mojocafe,demo,test}, user@mojocafe
   - Protocol mappers inject tenant claims
   - Port: 8180 (avoid conflict with API port 8080)
6. **Frontend Auth Library:** `react-oidc-context` (wraps `oidc-client-ts`)
   - Works identically with Auth0 and Keycloak
   - Provider-agnostic; no Auth0-specific SDK
7. **Work Split:**
   - **Hicks:** Implement Program.cs OIDC validation, admin endpoints, CORS fix
   - **Vasquez:** Create admin app, migrate client app auth to OIDC
   - **Hudson:** Create Keycloak realm JSON, fix Docker Compose, admin Dockerfile
   - **Bishop:** Integration tests (auth flow, tenant isolation, docker smoke tests)
**Rationale:** Keycloak enables local dev without Auth0 account; OIDC future-proofs for prod; admin app separates concerns; tenant claims prevent cross-tenant access
**Dependencies:** Blocks Hicks (backend auth), Vasquez (admin + client auth), Bishop (validation)
**Risks Mitigated:**
- Keycloak realm JSON complexity → Hudson provides working template
- Auth0 vs Keycloak claims → Backend middleware normalizes claims
- Dynamic tenant CRUD → Phase 1 uses static config; Phase 3 migrates to EF Core store
- Docker service growth → Health checks and documented dependencies
**Status:** ✅ APPROVED
**Next:** Hicks + Vasquez + Hudson execute in parallel

### Docker Compose & Keycloak Infrastructure Implementation
**By:** Hudson (DevOps)
**Date:** 2026-03-20
**What:** Complete Docker Compose repair and Keycloak local dev infrastructure per Ripley's approved design
**Deliverables:**
1. **Keycloak realm JSON** (`docker/keycloak/opplat-realm.json`)
   - Realm `opplat` with login theme
   - Clients: `opplat-client` (localhost:3000), `opplat-admin` (localhost:3001)
   - Client scopes with protocol mappers for `tenant_id`, `tenant_identifier` claims
   - Roles: `admin`, `operator`, `user` (realm-level)
   - Test users: admin@{mojocafe,demo,test}, user@mojocafe (passwords: admin123/user123)
2. **docker-compose.yml rewrite**
   - Added Keycloak service (8180, realm auto-import, health check)
   - Updated all API services: removed `Authorization__Password`, added `Auth__Authority`, `Auth__Audience`, `Auth__ClientId*`
   - Added `depends_on: keycloak: service_healthy` for startup sequencing
   - Added admin-frontend service (3001, OIDC env vars)
   - Updated frontend service with OIDC variables
3. **docker-compose.override.yml fixes**
   - Removed broken `api` volume mount (`./src/Opplat.MainApp:/app/src/...` dead path)
   - Fixed `frontend` build conflict (removed conflicting image directive)
   - Added `admin-frontend` development override (node:20-alpine, Vite hot-reload on 5173)
   - Both frontends mapped to localhost:{3000,3001}
4. **MainApp Dockerfile repair**
   - Added missing module .csproj COPY statements (all 7 module projects)
   - Prevents restore failures (NU1101) when MainApp references modules
5. **Admin frontend Dockerfile** (`src/opplat-admin/Dockerfile`)
   - Multi-stage build: node:20-alpine → nginx:alpine
   - SPA routing with try_files fallback
   - Cache control headers for assets
   - Placeholder ready for Vasquez population
6. **.env.docker update**
   - Removed `JWT_SECRET` (no longer used with OIDC)
   - Added `AUTH__AUTHORITY`, `AUTH__AUDIENCE`, `AUTH__CLIENT_ID_*`
   - Added `VITE_AUTH_*` for client app (Keycloak/Auth0 agnostic)
   - Added `VITE_ADMIN_AUTH_*` for admin app
**Validation:** ✅ docker-compose config, Keycloak JSON, Dockerfile syntax, service dependencies all valid
**Design Patterns:**
- Keycloak health check: 60s start period + 10 retries (realm import init time)
- OIDC authority: container DNS (`keycloak:8180`) for inter-service, localhost for browser
- Override pattern: production multi-stage builds replaced by dev containers in override
- Realm import: auto-import via `--import-realm` + volume mount to `/opt/keycloak/data/import/`
- Protocol mappers: OIDC attribute mappers for tenant claims
**Status:** ✅ COMPLETE — Docker infrastructure ready for backend + frontend implementation
**Dependencies:** Awaiting Vasquez (admin app), Hicks (backend OIDC), Bishop (integration tests)

### Phase 1 Refactor: Domain-Context-First Architecture Boundaries
**By:** Ripley (Lead/Architect)
**Date:** 2026-03-17
**Status:** ✅ COMPLETE & VERIFIED
**What:** Establish modular, domain-context-first architecture for Sales and Inventory
**Key Decisions:**
- **Directory Structure:** Modules/{Sales,Inventory}/{Domain,Infrastructure,Application} + MainApp.Areas for controllers
- **Three-Layer Module Structure:** Domain (entities, services, interfaces), Infrastructure (EF repos), Application (handlers—empty for now)
- **Single Shared DbContext:** OpplatDbContext in MainApp.Data; unified for multitenancy simplicity
- **Service Registration:** All module services registered in MainApp.Program.cs via aliases
- **Presentation in MainApp:** Controllers remain in MainApp.Areas (route discovery, DI simplicity)
- **Accounting Preserved As-Is:** ~20 files in Opplat.Domain (incomplete feature; extraction would create compliance risk)
- **Controller Namespaces:** Logical organization (e.g., `Opplat.Modules.Sales.Application` for discoverability)
**Project References:**
```
MainApp → Sales.Domain, Sales.Infrastructure, Sales.Application
        → Inventory.Domain, Inventory.Infrastructure, Inventory.Application
        → Opplat.Domain (Accounting), Opplat.Infrastructure (base)
Infrastructure → Domain, Shared
Domain → Shared
```
**Boundary Rules (Non-Negotiable):**
- Modules do NOT reference each other
- Modules only reference MainApp.Data (DbContext) + Program.cs (DI)
- MainApp references all module projects; modules reference none
- Opplat.Shared cross-cutting only (logging, utilities, DTOs, validators)
**Verification:** ✅ Build successful (0 errors, 4 pre-existing warnings)
- 9 projects compiled correctly
- Controllers route correctly (Area discovery verified)
- Services resolve (DI aliases work)
- No circular dependencies
- No namespace collisions
**Behavioral Preservation:**
- API endpoints unchanged (routes, contracts, response shapes)
- Database schema unchanged (no migrations added)
- Clients see zero changes
**Impact on Future Phases:**
- Phase 2 (React): No blocking changes; API contracts preserved
- Phase 3 (Multitenancy): DbContext decomposition may be needed if per-tenant schema divergence occurs
- Phase 4 (CQRS/MediatR): Application layer ready for handler implementation
**Compromises (Phase 1):**
- DbContext remains unified (multitenancy simplicity trade-off; reassess Phase 3)
- Controllers in MainApp, not modules (ASP.NET Area routing tightly bound)
- No inter-module DTOs (entities shared; low conflict risk)
**Status:** ✅ APPROVED & ENFORCED
**Sign-Off:** Ripley (Lead); Ready for Phase 2

## Session 3 Decisions (2026-03-17)

### Phase 1: .NET 10 Migration — Package Alignment Review (Rejected)
**By:** Hudson (DevOps) — Submitted | Ripley (Lead) — Reviewed & Rejected
**Date:** 2026-03-17
**What:** Attempted to align all packages to .NET 10; upgraded Finbuckle.MultiTenant to v10.0.4
**Why:** Assumed Finbuckle version-locks to .NET releases (v10.0.4 for .NET 10)
**Critical Issue:** Finbuckle.MultiTenant v10.0.4 **does not exist** on NuGet.org (latest: v7.0.1)
**Code Problems:**
  - Non-existent namespace imports: `.AspNetCore.Extensions`, `.EntityFrameworkCore.Extensions`
  - API signature mismatch: `.WithRouteStrategy("__tenant__", false)` (boolean param not in v7.0.1)
  - Build succeeds only because packages not yet restored
**Status:** ❌ REJECTED
**Lesson:** Never assume package semantic versioning locks to .NET major versions. Always verify NuGet.org availability.

### Phase 1: .NET 10 Migration — Finbuckle Package Revision
**By:** Hicks (Backend Dev) — Assigned by Ripley per Reviewer Lockout Protocol
**Date:** 2026-03-17
**What:** Reverted Finbuckle.MultiTenant from non-existent v10.0.4 back to approved v7.0.1
**Why:** Team decision approved v7.0.1; v10.0.4 doesn't exist; fix API signatures for v7.0.1 compatibility
**Changes:**
  - Finbuckle.MultiTenant.AspNetCore: 7.0.1 (MainApp)
  - Finbuckle.MultiTenant.EntityFrameworkCore: 7.0.1 (MainApp + Infrastructure)
  - Removed invalid namespace imports (`.Extensions` subnamespaces)
  - Fixed API: `.WithRouteStrategy("__tenant__", false)` → `.WithRouteStrategy("__tenant__")`
  - Preserved all EF Core 10.0.5 and other .NET 10 alignment
**Validation:** Build ✅ | Test ✅ (0 discovered tests, pre-existing)
**Status:** Accepted ✅
**Technical Insight:** Finbuckle 7.0.1 root namespace only—no `.Extensions` subnamespaces. Route strategy single parameter.

### Phase 1: .NET 10 Migration — Test Framework Validation
**By:** Bishop (Tester)
**Date:** 2026-03-17
**What:** Independently validated build and test status after package alignment correction
**Findings:**
  - `dotnet build` ✅ SUCCESS
  - `dotnet test` ✅ SUCCESS (0 discovered tests)
  - Zero-test discovery is pre-existing: `LicenciaTest.cs` [Fact] commented, `SetupContexto.cs` is stub
  - Not a migration regression; no test-code changes warranted
**Decision:** Do not modify test code beyond package/framework alignment during migration
**Status:** Accepted ✅
**Follow-on:** Future multitenancy tests should be added as new files alongside dormant legacy scaffolding

### Reviewer Lockout Protocol Application
**By:** Ripley (Lead/Architect)
**Date:** 2026-03-17
**What:** Corrected initial reassignment error; applied Reviewer Lockout Protocol to Finbuckle revision
**Issue:** Initially reassigned revision back to Hudson (original author), violating lockout protocol
**Rule:** Original author of rejected work cannot revise their own rejection
**Correction:** Reassigned to Hicks (Backend Dev, Finbuckle expert, not original author)
**Rationale for Hicks:** Backend concern, Finbuckle.MultiTenant architect, no authorship conflict
**Status:** ✅ Protocol Enforced
**Governance:** Team governance model strengthened; protocol prevents author-bias in revisions

## Session 2 Decisions (2026-02-27)

### Docker Infrastructure Setup
**By:** Hudson (DevOps)
**Date:** 2026-02-27
**What:** Complete Docker Compose setup with multi-stage builds, SQL Server, multi-tenant configuration, and hot-reload frontend
**Key Decisions:**
- Build context at repository root for API Dockerfile (access to opplat.sln)
- SQL Server 2022 with health checks for readiness
- Multi-tenant database setup (opplat-main, opplat-mojocafe, opplat-demo, opplat-test)
- Frontend: nginx + static build (prod), Vite dev server (dev)
- Custom bridge network `opplat-network` for service discovery
- Environment variables via `.env.docker` template
**Impact:** One-command setup: `cp .env.docker .env && docker-compose up -d`
**Status:** Accepted ✅

### React App Feature Parity with Vue
**By:** Vasquez (Frontend)
**Date:** 2026-02-27
**What:** Full feature parity across ProductsPage, SellPage, UsersPage to match Vue 2 reference app
**Enhancements:**
- **ProductsPage:** Search, image display, image upload, activate/deactivate toggle, MUI Dialog instead of browser confirm
- **SellPage:** Sale metadata fields (Dependiente, Posición, Comanda, Observaciones), loading states
- **UsersPage:** Profile picture column, delete functionality with confirm dialog
- **Types:** Added `active`, `imageUrl` to ProductForSale; `profilePicture` to User
- **APIs:** New endpoints for toggle, upload, delete
**Impact:** Seamless user transition from Vue to React; no retraining needed
**Status:** Accepted ✅

### MediatR + Minimal API Refactor for Account / License / Menus
**By:** Hicks (Backend)
**Date:** 2026-02-27
**What:** Refactored Account, License, Menus endpoints to use MediatR (v12.4.1) and Minimal API (vertical slice architecture)
**Structure:**
- Features/ folder with Command/Query pattern under Features/Account/, Features/License/, Features/Menus/
- Extension methods (MapAccountEndpoints, MapLicenseEndpoints, MapMenusEndpoints) in Program.cs
- Dual routing: /{__tenant__}/auth/account and /auth/account for backward compatibility
- Old controllers archived (not deleted); Sales/Inventory areas remain MVC for now
**Fixes:**
- Added missing DI registrations: LicenciaService, MenuLoader
- Added app.MapControllers() for area controllers
- Added AddEndpointsApiExplorer before Swagger
**Impact:** Cleaner separation of concerns; easier testing and refactoring
**Status:** Accepted ✅
**Follow-on:** Convert Sales/Inventory to MediatR + Minimal API (lower priority)

## Session 1 Decisions (2026-02-27)

### Phase 1: .NET 10.0 Upgrade — LicenceChecker Package Handling
**By:** Hudson (DevOps)
**Date:** 2026-02-27
**What:** Commented out LicenceChecker package reference and disabled related functionality
**Why:** Package "LicenceChecker" version 1.0.0 does not exist on NuGet.org or configured feeds; appears to be custom/local package not available in public repositories
**Impact:** License validation functionality temporarily disabled; flagged for future resolution
**Status:** Accepted
**Action Required:** Locate original LicenceChecker package source or DLL and restore after upgrade

### Phase 1: .NET 10.0 Upgrade — xunit.runner.visualstudio Version
**By:** Hudson (DevOps)
**Date:** 2026-02-27
**What:** Used xunit.runner.visualstudio version 3.0.0 instead of planned 2.9.3
**Why:** Version 2.9.3 not available on NuGet; 3.0.0 is latest stable and compatible with net10.0
**Impact:** Minor version bump, no breaking changes expected for test runner
**Status:** Accepted

### Phase 1: .NET 10.0 Upgrade — Framework Target Version
**By:** Hudson (DevOps)
**Date:** 2026-02-27
**What:** All projects upgraded to net10.0
**Projects Affected:**
  - src/Opplat.MainApp (Web API)
  - src/Opplat.Infrastructure (Data access)
  - src/Opplat.Domain (Business logic)
  - src/Opplat.Shared (Utilities)
  - test/Opplat.MainApp.Test (Unit tests)
**Why:** User requirement for modernization; .NET 10.0 is latest stable LTS release
**Status:** Accepted
**Build Result:** ✅ SUCCESS (0 errors, 4 pre-existing warnings)

### Phase 1: Add Finbuckle.MultiTenant Packages
**By:** Hudson (DevOps)
**Date:** 2026-02-27
**What:** Added Finbuckle.MultiTenant packages version 7.0.1 to the solution
**Packages Added:**
  - Finbuckle.MultiTenant.AspNetCore 7.0.1 → Opplat.MainApp
  - Finbuckle.MultiTenant.EntityFrameworkCore 7.0.1 → Opplat.MainApp and Opplat.Infrastructure
**Why:** Latest stable version compatible with .NET 10 and EF Core 9.x; required for Phase 3 implementation
**Status:** Accepted
**Next:** Hicks to implement in Phase 3

### Phase 2: Build Tool — Vite over Create React App
**By:** Vasquez (Frontend)
**Date:** 2026-02-27
**What:** Use Vite instead of Create React App (CRA) for React 18 app
**Why:** Significantly faster dev server startup, native ESM, better production builds, smaller config overhead; CRA no longer actively maintained
**Impact:** Faster development cycles, better DX
**Status:** Accepted

### Phase 2: UI Framework — Material-UI v5
**By:** Vasquez (Frontend)
**Date:** 2026-02-27
**What:** Use MUI v5 as the component library
**Why:** Comprehensive component set, professional design, excellent TypeScript support, responsive by default, strong documentation, theme customization
**Impact:** Faster UI development, consistent design, less custom CSS
**Status:** Accepted

### Phase 2: State Management — React Context API
**By:** Vasquez (Frontend)
**Date:** 2026-02-27
**What:** Use React Context API for auth state instead of Redux/Zustand
**Why:** Application has simple state needs (mostly auth), Context is built-in, easier onboarding, server state handled by API calls
**Impact:** Simpler architecture, less boilerplate
**Status:** Accepted

### Phase 2: Routing — React Router v6
**By:** Vasquez (Frontend)
**Date:** 2026-02-27
**What:** Use React Router v6
**Why:** Industry standard, excellent TypeScript support, nested routes, declarative API, good documentation
**Impact:** Standard, maintainable routing setup
**Status:** Accepted

### Phase 2: HTTP Client — Axios
**By:** Vasquez (Frontend)
**Date:** 2026-02-27
**What:** Use Axios instead of fetch
**Why:** Cleaner API, built-in request/response interceptors (critical for JWT), automatic JSON parsing, better error handling, team already familiar
**Impact:** Simpler API integration, easier JWT handling
**Status:** Accepted

### Phase 2: Token Storage — localStorage
**By:** Vasquez (Frontend)
**Date:** 2026-02-27
**What:** Use localStorage instead of sessionStorage
**Why:** Users expect persistent login across browser sessions, better UX, standard SPA practice, can implement auto-logout with token expiration
**Impact:** Better user experience
**Status:** Accepted

### Phase 2: TypeScript Configuration
**By:** Vasquez (Frontend)
**Date:** 2026-02-27
**What:** Enable strict mode and unused variable checks
**Why:** Catch errors at compile time, better code quality, self-documenting code, easier refactoring
**Impact:** Higher code quality, fewer runtime errors
**Status:** Accepted

### Phase 2: Project Architecture — Feature-Based Organization
**By:** Vasquez (Frontend)
**Date:** 2026-02-27
**What:** Feature-based organization with clear separation (/api, /auth, /pages, /components, /types)
**Why:** Easy to navigate, clear responsibility boundaries, scalable, industry best practice
**Impact:** Maintainable, organized codebase
**Status:** Accepted

### Phase 2: React Pages Required
**By:** Vasquez (Frontend)
**Date:** 2026-02-27
**What:** React app includes: Login, Home, Products, Sell, Users pages
**Why:** Parity with existing Vue app functionality; 100% API compatibility maintained
**Status:** Accepted
**Result:** ✅ All 5 pages implemented with protected routes

### Phase 2: Development Server Port
**By:** Vasquez (Frontend)
**Date:** 2026-02-27
**What:** Run React dev server on port 3000
**Why:** Standard React dev port, no conflict with backend (port 5000)
**Impact:** No conflicts, familiar to React developers
**Status:** Accepted

### Phase 3: Finbuckle Architecture — Database Isolation Strategy
**By:** Ripley (Architect)
**Date:** 2026-02-27
**What:** Implement full database-per-tenant isolation (one database per tenant)
**Why:** Maximum data security; tenants completely isolated at data layer
**Status:** Accepted
**Impact:** Supports up to thousands of tenants with full data separation

### Phase 3: Finbuckle Architecture — Tenant Resolution Strategy
**By:** Ripley (Architect)
**Date:** 2026-02-27
**What:** Dual-strategy resolution: Route-based primary (/{__tenant__}/api/...), Header-based fallback (X-Tenant-Identifier)
**Why:** Route strategy provides user-friendly URLs; header fallback supports API clients that cannot modify routes; strategy priority prevents ambiguity
**Status:** Accepted

### Phase 3: Finbuckle Architecture — Tenant Store
**By:** Ripley (Architect)
**Date:** 2026-02-27
**What:** Use configuration-based (in-memory) tenant store with tenants in appsettings.json
**Why:** Simple to implement and test; sufficient for initial 3 tenants; can migrate to EF Core store later for dynamic management
**Status:** Accepted
**Future Enhancement:** Upgrade to EF Core store for tenant CRUD operations

### Phase 3: Finbuckle Architecture — JWT Tenant Claims
**By:** Ripley (Architect)
**Date:** 2026-02-27
**What:** JWT tokens include tenant_id and tenant_identifier claims
**Why:** Enables client-side tenant awareness; allows TenantValidationMiddleware to verify token matches request tenant; prevents cross-tenant token replay attacks; supports per-tenant JWT signing keys
**Status:** Accepted

### Phase 3: Finbuckle Architecture — Identity Isolation
**By:** Ripley (Architect)
**Date:** 2026-02-27
**What:** Remove hardcoded admin user/role seed from OnModelCreating; implement per-tenant provisioning
**Why:** Global seed conflicts with per-tenant isolation; each tenant should have its own admin user with tenant-specific credentials
**Status:** Accepted
**Workaround:** TenantProvisioningService handles per-tenant seeding

### Phase 3: Implementation — DbContext Changes
**By:** Hicks (Backend)
**Date:** 2026-02-27
**What:** OpplatDbContext implements IMultiTenantDbContext; injects IMultiTenantContextAccessor<AppTenantInfo>; calls builder.ConfigureMultiTenant()
**Why:** Required for Finbuckle EF Core integration; provides tenant context to DbContext
**Status:** Accepted
**Impact:** Automatic tenant filtering at database level

### Phase 3: Implementation — Program.cs Configuration
**By:** Hicks (Backend)
**Date:** 2026-02-27
**What:** Register multi-tenant services with route and header strategies; configure DbContext with per-tenant connection string resolution
**Why:** Enables tenant resolution and per-request connection string routing
**Status:** Accepted

### Phase 3: Implementation — Middleware Order
**By:** Hicks (Backend)
**Date:** 2026-02-27
**What:** Place UseMultiTenant() BEFORE UseRouting() in middleware pipeline
**Why:** Tenant context must be available before route resolution; authentication needs tenant context for per-tenant Identity
**Status:** Accepted

### Phase 3: Implementation — Backward Compatibility Routes
**By:** Hicks (Backend)
**Date:** 2026-02-27
**What:** Maintain non-tenant-prefixed routes alongside tenant-aware routes
**Why:** Allows gradual migration of client applications; existing Vue client continues to work; new React client can adopt tenant-aware URLs
**Status:** Accepted
**Impact:** Zero downtime migration path

## Active Decisions (Earlier Sessions)

### 2025-01-01: Project Modernization Scope
**By:** elvis.crego
**What:** Upgrade Opplat from net6.0 to net10.0, replace Vue 2 client with React 18 (Vite + TypeScript + MUI), add Finbuckle.MultiTenant multitenancy
**Why:** User request — modernize stack, improve maintainability
**Status:** Accepted ✅ COMPLETED

### 2025-01-01: Execution Order
**By:** elvis.crego
**What:** Execute in order: (1) .NET upgrade first, (2) React client second, (3) Multitenancy third
**Why:** Each phase is a stable foundation for the next; upgrade before new implementations
**Status:** Accepted ✅ COMPLETED

### 2025-01-01: Multitenancy Package
**By:** elvis.crego
**What:** Use Finbuckle.MultiTenant for multitenancy (not custom implementation)
**Why:** User explicit requirement
**Status:** Accepted ✅ COMPLETED

### 2025-01-01: React Client Location
**By:** elvis.crego
**What:** New React app lives at src/opplat-react/. Old src/opplat-vue/ is kept but inactive.
**Why:** Preserve existing code, new client is the active one
**Status:** Accepted ✅ COMPLETED

### 2025-01-01: React Pages Required
**By:** elvis.crego
**What:** React app must include: Login, Home, Products, Sell, Users pages
**Why:** Parity with existing Vue app functionality
**Status:** Accepted ✅ COMPLETED
