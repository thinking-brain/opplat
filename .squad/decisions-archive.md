# Opplat Squad — Decisions Archive

Archived sessions older than 30 days. Active decisions appear in decisions.md.

---

## Session 2 Decisions (2026-02-27) — ARCHIVED

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

## Session 1 Decisions (2026-02-27) — ARCHIVED

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

---

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
