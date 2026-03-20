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

### 2025-01-XX: Phase 1 — .NET 10 Upgrade Completed
- **Framework:** All projects upgraded from net6.0 → net10.0
- **Packages upgraded:**
  - mailkit: 3.1.0 → 4.10.0
  - Microsoft.AspNetCore.Authentication.JwtBearer: 6.0.11 → 9.0.5
  - Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore: 6.0.4 → 9.0.5
  - Microsoft.AspNetCore.Identity.EntityFrameworkCore: 6.0.4 → 9.0.5
  - Microsoft.AspNetCore.Identity.UI: 6.0.4 → 9.0.5
  - Microsoft.EntityFrameworkCore.Relational: 6.0.4 → 9.0.5
  - Microsoft.EntityFrameworkCore.SqlServer: 6.0.4 → 9.0.5
  - Microsoft.EntityFrameworkCore.Tools: 6.0.4 → 9.0.5
  - Microsoft.EntityFrameworkCore.InMemory: 2.2.6 → 9.0.5
  - Microsoft.Extensions.Logging.Abstractions: 7.0.0 → 9.0.5
  - Npgsql.EntityFrameworkCore.PostgreSQL: 6.0.4 → 9.0.4
  - Swashbuckle.AspNetCore: 6.2.3 → 7.3.1
  - Microsoft.NET.Test.Sdk: 16.0.1 → 17.14.0
  - Moq: 4.12.0 → 4.20.72
  - xunit: 2.4.0 → 2.9.3
  - xunit.runner.visualstudio: 2.4.0 → 3.0.0 (2.9.3 not available)
  - Added: coverlet.collector 6.0.4 to test project
- **Issues resolved:**
  - LicenceChecker package not found on NuGet → commented out package reference and disabled functionality in code (Controllers\LicenciaController.cs, Utils\LicenciaService.cs)
  - xunit.runner.visualstudio 2.9.3 not available → used 3.0.0 instead
- **Build status:** ✅ Success with 4 warnings (nullable reference types - pre-existing)
- **Next steps:** Code review and testing recommended before Phase 2

### 2025-01-XX: Phase 3 Prep — Finbuckle.MultiTenant Packages Added
- **Package added to Opplat.MainApp:**
  - Finbuckle.MultiTenant.AspNetCore 7.0.1
  - Finbuckle.MultiTenant.EntityFrameworkCore 7.0.1
- **Package added to Opplat.Infrastructure:**
  - Finbuckle.MultiTenant.EntityFrameworkCore 7.0.1
- **Restore status:** ✅ Success - all packages resolved
- **Purpose:** Enable multitenancy support for Phase 3 implementation
- **Next steps:** Hicks and Ripley will design and implement multitenancy architecture

### 2025-01-XX: Docker Infrastructure Setup
- **Created comprehensive Docker setup for local development and deployment**
- **Files created:**
  - `src/Opplat.MainApp/Dockerfile` — Multi-stage Dockerfile for .NET 10 API
  - `src/opplat-react/Dockerfile` — Multi-stage Dockerfile for React frontend with nginx
  - `docker-compose.yml` — Production-ready compose file with SQL Server, API, and frontend
  - `docker-compose.override.yml` — Development overrides with hot reload for frontend
  - `.env.docker` — Example environment variables template
  - `README.md` — Comprehensive project documentation
- **Key decisions:**
  - Build context for API is repo root (.) to access solution file and all projects
  - SQL Server 2022 with health checks for proper startup sequencing
  - Multi-tenant environment variables configured for 3 tenants (mojocafe, demo, test)
  - Frontend runs on nginx in production, Vite dev server in development mode
  - nginx configured with SPA routing (try_files fallback to index.html)
  - All services on dedicated bridge network for isolation
- **Services configured:**
  - `sqlserver`: SQL Server 2022, port 1433, with persistent volume
  - `api`: .NET 10 API, port 8080, depends on SQL Server health
  - `frontend`: React app, port 3000 (nginx) / 5173 (dev mode)
- **Environment handling:**
  - Connection strings point to containerized SQL Server
  - JWT secret configurable via environment variable
  - Finbuckle multi-tenant configuration with 3 tenant databases
  - Development vs production mode toggles
- **Developer experience:**
  - Simple `docker-compose up -d` to start entire stack
  - Hot reload in override mode for frontend development
  - README includes quick start, local dev instructions, troubleshooting
  - Clear documentation of multi-tenancy routing and usage
- **Learnings:**
  - .NET 10 SDK/runtime images use mcr.microsoft.com/dotnet/sdk:10.0 and aspnet:10.0
  - Multi-stage builds reduce final image size significantly
  - Health checks prevent API startup failures when SQL Server isn't ready
  - Docker Compose v3.8 supports depends_on with condition: service_healthy
  - Environment variable substitution with $${VAR} escaping needed in compose files
  - Volume mount /app/node_modules prevents host overwriting container dependencies

### 2026-03-17: .NET 10 Package Alignment Submission — REJECTED

**Task:** Align all project packages to .NET 10-compatible versions.

**Work Performed:**
- Upgraded all projects to net10.0 target framework
- Updated 20+ packages to .NET 10 compatibility levels (ASP.NET Core 10.0.5, EF Core 10.0.5, test framework 17.14.0, etc.)
- Added Finbuckle.MultiTenant.AspNetCore and EntityFrameworkCore v7.0.1 to both MainApp and Infrastructure

**Critical Error:**
Attempted to upgrade Finbuckle.MultiTenant from v7.0.1 to v10.0.4, assuming package versions lock to .NET releases. **This version does not exist on NuGet.org.** Latest stable Finbuckle: v7.0.1 only. No v8, v9, or v10 releases published.

**Code Issues from Non-Existent Package:**
- Imports non-existent namespaces: `.AspNetCore.Extensions`, `.EntityFrameworkCore.Extensions`
- API signature changed: `.WithRouteStrategy("__tenant__", false)` assumes v10.0.4 boolean parameter
- Build succeeds only because packages not yet restored; restore will fail with NU1101 error

**Reviewer Finding:**
Ripley (Lead) rejected submission with detailed analysis. Violation of team-approved decision (`.squad/decisions.md`: v7.0.1 explicitly approved for Phase 1).

**Architectural Lesson Documented:**
Never assume semantic versioning locks to .NET major versions. Each package has independent versioning. Always verify NuGet.org availability before upgrades.

**Reviewer Lockout Protocol Applied:**
Per governance, Hudson (original author) locked from revising own rejection. **Ownership reassigned to Hicks (Backend Dev)** for correction.

**Decision records created:**
- `.squad/decisions/inbox/hudson-net10-packages.md` (submission)
- `.squad/decisions/inbox/ripley-net10-review-REJECTED.md` (rejection analysis)
- `.squad/decisions/inbox/ripley-finbuckle-reassignment.md` (lockout + reassignment)

**Status:** ❌ REJECTED — Awaiting Hicks revision; Hudson temporarily locked

### 2026-03-20: Docker Compose + Keycloak Infrastructure Implementation

**Task:** Implement Docker Compose repair and Keycloak local dev infrastructure per Ripley's approved admin-auth-tenant design.

**Work Performed:**

1. **Created Keycloak Realm Export (`docker/keycloak/opplat-realm.json`)**
   - Realm: `opplat` with all themes configured
   - Clients: `opplat-client` (port 3000) and `opplat-admin` (port 3001)
   - Client scopes with custom protocol mappers for `tenant_id` and `tenant_identifier` claims
   - Roles: `admin`, `operator`, `user` (realm-level)
   - Test users (4): admin@{mojocafe,demo,test}, user@mojocafe
   - All users pre-populated with tenant attributes for immediate testing
   - Credentials: `admin123` and `user123`

2. **Fixed docker-compose.yml**
   - Added Keycloak service (port 8180) with realm import and health check
   - Updated all API services (api, sales-api, inventory-api) with:
     - `Auth__Authority` (OIDC discovery URL)
     - `Auth__Audience` and `Auth__ClientId*` (role-based config)
     - `depends_on: keycloak: service_healthy` (startup sequencing)
     - Removed old `Authorization__Password` JWT config
   - Added admin-frontend service (port 3001) with OIDC env vars
   - Updated frontend service with OIDC auth variables
   - All services on opplat-network bridge

3. **Fixed docker-compose.override.yml**
   - Removed broken `api` volume mount (source: /app/src/Opplat.MainApp, dead bind)
   - Fixed `frontend` build conflict (removed image that conflicted with build directive)
   - Added development command + volume overrides for hot-reload on 5173 (Vite)
   - Added `admin-frontend` development override with matching pattern
   - Both frontends mapped to localhost:{3000,3001} for browser access

4. **Fixed Mainapp Dockerfile**
   - Added all module .csproj COPY statements:
     - Opplat.Microservices.Shared
     - Modules/{Sales,Inventory}/{Domain,Application,Infrastructure}
   - Prevents restore failures (NU1101) when MainApp references these projects

5. **Created admin-frontend Dockerfile (placeholder)**
   - Multi-stage build: node:20-alpine → nginx:alpine
   - SPA routing with try_files fallback
   - Cache control headers for assets
   - Ready for Vasquez to populate with actual admin app source

6. **Updated .env.docker with OIDC variables**
   - Removed obsolete `JWT_SECRET`
   - Added complete Auth (backend) and Vite (frontend) configuration
   - Supports both Keycloak dev and Auth0 prod via env var override
   - Includes admin-frontend-specific vars

**Validation:**
- ✅ `docker-compose config --quiet` — syntax valid
- ✅ Keycloak realm JSON — valid structure, all required fields
- ✅ Dockerfile syntax — multi-stage builds parse correctly
- ✅ Service dependency graph — correct startup order

**Design Patterns Established:**
- **Keycloak Health Check:** 60s start period + 10 retries for realm import init time
- **OIDC Authority:** Container DNS for inter-service (keycloak:8180), localhost for browser
- **Override Pattern:** Production multi-stage builds replaced by dev containers (node + npm run dev) in override
- **Realm Import:** Auto-import via --import-realm command + volume mount to /opt/keycloak/data/import/
- **Protocol Mappers:** OIDC attribute mappers for tenant claims (standard Keycloak pattern)

**Dependencies:**
- ✅ Unblocked: Module projects exist (Ripley refactor)
- ⏳ Awaiting Vasquez: Actual admin app source (package.json, etc.)
- ⏳ Awaiting Hicks: Backend OIDC auth config (Program.cs changes)

**Files Changed:**
- `docker/keycloak/opplat-realm.json` (NEW)
- `docker-compose.yml` (REWRITTEN)
- `docker-compose.override.yml` (FIXED)
- `.env.docker` (UPDATED)
- `src/Opplat.MainApp/Dockerfile` (MODULE COPY ADDED)
- `src/opplat-admin/Dockerfile` (NEW — placeholder)

**Status:** ✅ COMPLETE — Docker infrastructure ready for backend + frontend implementation

### 2026-03-20: Team Updates

**Ripley (Architect)** approved admin-auth-tenant design (Decision 4.2). Blocks Hicks, Vasquez, Bishop.

**Hicks (Backend)** assigned OIDC auth implementation + admin endpoints. Will refactor Program.cs, remove LoginCommand, create /admin/* routes. Parallel with Vasquez.

**Vasquez (Frontend)** assigned admin app scaffold + client app auth migration. Will create src/opplat-admin/, migrate opplat-react to react-oidc-context. Parallel with Hicks.

**Bishop (Tester)** assigned integration tests (post-Hicks/Vasquez). Will validate Keycloak flow, tenant isolation, docker smoke tests.

**Scribe** completed session logs and decision merging. Decisions now centralized in .squad/decisions.md (Session 4).

**Status:** ✅ Hudson delivery COMPLETE; Awaiting Hicks + Vasquez for Phase 2 validation.

