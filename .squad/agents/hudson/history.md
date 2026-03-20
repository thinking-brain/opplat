## Core Context

**Project:** Opplat — Multi-platform business management system (café/restaurant)  
**Stack:** ASP.NET Core (net6.0 → net10.0) | EF Core | SQL Server | SignalR | OIDC (Auth0/Keycloak) | React 18  
**Root:** C:\projects\personal\opplat | **Branch:** develop

**Key Decisions (Summarized):**
- net10.0 target framework (Phase 1: ✅ complete)
- Finbuckle.MultiTenant v7.0.1 locked (v10.0.4 doesn't exist; lesson: verify NuGet availability)
- Module structure: Sales/Inventory in Domain/Infra layers; controllers in MainApp.Areas
- Single shared DbContext for multitenancy simplicity (Phase 1 trade-off)
- OIDC auth (Auth0 prod, Keycloak local dev) replacing symmetric JWT
- Admin app at src/opplat-admin/ (separate React 18 app)
- Docker: 7 services (sqlserver, keycloak, api, sales-api, inventory-api, frontend, admin-frontend)

**Last Significant Updates:**
- 2025-01-XX: Phase 1 .NET 10 Upgrade + Docker Infrastructure (base)
- 2026-03-17: Package alignment rejected; Hicks corrected Finbuckle to v7.0.1
- 2026-03-20: Keycloak + Docker Compose rewrite; OIDC auth approved; team work assignments

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

