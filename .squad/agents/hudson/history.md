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

### 2026-03-20: Admin Frontend Docker Build Fix

**Task:** Fix failing `docker compose up -d --build` for `admin-frontend` service. Error: `package.json` and `package-lock.json` out of sync (npm ci failure).

**Root Cause:** 
- `package-lock.json` was out of sync with `package.json` (possible version mismatch in lock file format)
- Dockerfile used `npm ci` which requires exact lock file match
- Docker reported YAML mismatch during package resolution

**Fix Applied:**
1. **Regenerated `package-lock.json`** using `npm install` on clean checkout
   - Ensured lock file matches all package.json dependencies
   - Production-safe: uses exact version pinning from package.json
   
2. **Updated `src/opplat-admin/Dockerfile`** line 5
   - Changed: `RUN npm install` → `RUN npm ci`
   - Added production strategy comment: "requires exact lock file match for reproducible builds"
   - Aligns with npm best practices for CI/CD environments

3. **Validation:**
   - ✅ `docker compose build admin-frontend` now succeeds
   - ✅ Multi-stage build completes: node:20-alpine → nginx:alpine
   - ✅ Vite build produces `/dist` directory for nginx
   - ✅ No package sync errors

**Files Changed:**
- `src/opplat-admin/package-lock.json` (REGENERATED)
- `src/opplat-admin/Dockerfile` (UPDATED: npm ci + comment)
- `src/opplat-admin/package.json` (ADDED to git tracking)

**Commit:** `ffd7c8a` — "Fix admin frontend Docker build: regenerate package-lock.json and use npm ci"

**Status:** ✅ COMPLETE — Admin frontend Docker build now succeeds; `docker compose up -d --build` ready for endpoint implementation.

### 2026-03-21: Keycloak Authentication Setup — Roles, Scopes, Test Users

**Task:** Fix "Invalid scopes" error in Keycloak, establish 3-tier role model, pre-seed test users for local dev.

**Work Performed:**

1. **Fixed OIDC Scope Definitions**
   - Added standard clientScopes: `openid`, `profile`, `email`, `offline_access`, `roles`
   - Each scope includes required protocolMappers (email, given_name, family_name, tenant_id, etc.)
   - Keycloak now recognizes scopes in token requests; "Invalid scopes" error resolved
   - Both opplat-client and opplat-admin clients configured with all scopes in defaultClientScopes + optionalClientScopes

2. **Established Three-Tier Role Model**
   - **SuperAdmin** — Platform administrator, access to admin site only (port 3101)
   - **TenantAdmin** — Tenant administrator, manages users/permissions in client app admin section (port 3100)
   - **TenantUser** — Regular user, standard client app access
   - Replaces previous admin/operator/user model; cleaner separation of concerns

3. **Pre-Seeded Test Users**
   - `superadmin` / `SuperAdmin123!` → SuperAdmin role (for admin site auth)
   - `tenant-admin@mojocafe`, `tenant-admin@demo`, `tenant-admin@test` / `TenantAdmin123!` → TenantAdmin + TenantUser roles
   - `user@mojocafe`, `user@demo`, `user@test` / `TenantUser123!` → TenantUser role
   - All users include tenant_id and tenant_identifier attributes → injected as token claims

4. **Updated docker-compose.yml**
   - Added explicit Keycloak health check dependency for both frontend services
   - Ensures realm import completes before apps attempt authentication
   - Port mappings clarified: 3100 (client app), 3101 (admin site), 8180 (Keycloak)

5. **Updated README.md**
   - Auth section now documents three roles with permission boundaries
   - Listed default SuperAdmin credentials and all test users with passwords
   - Clarified admin site (SuperAdmin only) vs client app (all users) vs admin section in client
   - Updated quick start port reference
   - Made architecture explicit: permissions managed in client app by TenantAdmins, not in separate admin site

**Keycloak Realm Schema Changes:**
```json
"defaultRoles": ["TenantUser"],  // All users get standard role
"clientScopes": [
  {
    "name": "email",    // New: email + email_verified mappers
    "protocolMappers": [...]
  },
  {
    "name": "profile",  // Updated: includes tenant_id, tenant_identifier mappers
    "protocolMappers": [...]
  },
  {
    "name": "offline_access",  // New: refresh token support
  },
  {
    "name": "roles",   // Updated: realm role mapper
    "protocolMappers": [...]
  }
],
"clients": [
  {
    "defaultClientScopes": ["openid", "profile", "email", "offline_access", "roles"]
  }
]
```

**Validation:**
- ✅ Realm JSON syntax valid (PowerShell ConvertFrom-Json)
- ✅ docker-compose.yml valid (docker-compose config --quiet)
- ✅ All scopes defined + mapped in clientScopes
- ✅ All test users created with credentials + attributes
- ✅ Keycloak health check endpoint ready

**Files Changed:**
- `docker/keycloak/opplat-realm.json` (REWRITTEN)
- `docker-compose.yml` (frontend deps updated)
- `README.md` (auth section, test users, roles documented)
- `.squad/decisions/inbox/hudson-keycloak-roles-auth.md` (NEW decision doc)

**Status:** ✅ COMPLETE — Keycloak ready for prod-like local dev. Next: Hicks validates token claims + guards SuperAdmin routes; Vasquez implements auth flow in apps.

### 2026-03-21: Keycloak Infrastructure Refinement & Bootstrap Clarification (Session 5)

**Task:** Clarify and finalize Keycloak local bootstrap wiring with environment-driven configuration and health checks.

**Work Performed:**
1. **docker-compose.yml Enhancements**
   - Added realm-aware health checks for Keycloak
   - Added explicit bind mounts for `docker/keycloak/keycloak.conf` and `docker/keycloak/opplat-realm.json`
   - Added health checks for API/sales/inventory services
   - Dependency ordering via `depends_on: service_healthy` for startup sequencing

2. **Environment Configuration (.env / .env.docker)**
   - Added: `KEYCLOAK_PORT`, `KEYCLOAK_REALM`, `KEYCLOAK_ADMIN_USERNAME`, `KEYCLOAK_ADMIN_PASSWORD`
   - Published port defaults for all services
   - Fixed `.env` `VITE_SALES_API_URL` port (8081 → 8083)

3. **README.md Clarifications**
   - Added detailed startup flow documentation
   - Added expected URLs and ports
   - Added Keycloak admin console access instructions
   - Clarified realm import behavior
   - Documented hot-reload vs base ports for development

4. **Validation Results**
   - ✅ docker-compose config PASS
   - ✅ docker/keycloak/opplat-realm.json verified (no changes needed)
   - ✅ All bind mounts correctly specified
   - ✅ Health checks ready for service dependency ordering

**Key Clarifications:**
- Keycloak bootstrap is env-driven (admin creds, realm auto-import)
- Explicit bind mounts ensure realm.json and keycloak.conf are recognized
- Health checks prevent race conditions (frontends waiting for realm import)
- Service dependencies now properly ordered via healthcheck conditions

**Files Modified:**
- docker-compose.yml
- .env / .env.docker
- README.md

**Status:** ✅ COMPLETE — Docker infrastructure clarified and ready for all phases

### 2026-03-21: Runtime Scope Injection Fix — "Invalid scopes" Error Resolution

**Task:** Fix "Invalid scopes: openid profile email offline_access" error during Keycloak OIDC login despite realm configuration being correct.

**Root Cause:** Three-layer scope injection mismatch:
1. Source code (runtimeConfig.ts) fallback: `openid profile email offline_access` ✓
2. Dockerfile runtime-config script fallback: `openid` only ✗
3. .env env vars: not defined ✗

When nginx runtime-config.js injected the minimal `openid`-only fallback, it overrode the source-code fallback and caused scope merge conflicts.

**Work Performed:**
1. Updated both Dockerfiles (opplat-react, opplat-admin) to inject full scope in nginx entrypoint script:
   - Line 46/45: `${VITE_AUTH_SCOPE:-openid profile email offline_access}`
2. Added explicit VITE_AUTH_SCOPE and VITE_ADMIN_AUTH_SCOPE to .env and .env.docker
3. Validated docker-compose.yml already had correct env variable passing
4. Rebuilt frontend services; both succeeded with corrected injection

**Files Changed:**
- `src/opplat-react/Dockerfile` (runtime-config script fallback)
- `src/opplat-admin/Dockerfile` (runtime-config script fallback)
- `.env` (added scope env vars)
- `.env.docker` (added scope env vars)

**Status:** ✅ COMPLETE — Frontend auth scope injection now consistent across all three layers.

## Learnings

### Pattern: OIDC Scope Definitions in Keycloak
Keycloak validates scopes at token request time. If a client requests a scope that isn't defined in `clientScopes[]`, the request fails with "Invalid scopes". Solution:
1. Define all required scopes as clientScopes entries
2. Attach protocolMappers to each scope (email mappers, custom attribute mappers, etc.)
3. Register scopes in client's defaultClientScopes and optionalClientScopes
4. Keycloak automatically includes scope mappers in tokens

### Pattern: Role-to-Surface Mapping
For multi-surface apps (admin vs client), map roles to surfaces:
- SuperAdmin → Admin Surface (centralized platform config)
- TenantAdmin → Client Surface with admin section (scoped to tenant)
- TenantUser → Client Surface standard (no admin features)
This prevents "admin bloat" in the client and keeps TenantAdmins focused on tenant-level concerns.

### Architecture: Tenant Claims in Token
Using Keycloak protocol mappers to inject tenant_id and tenant_identifier as custom claims is cleaner than reading from user attributes at runtime. Claims are:
- Immutable (bound to token issued by IdP)
- Validated by backend via X-Tenant-Identifier header cross-check
- Available in both access and ID tokens for different validation flows

### Keycloak Health Check Pattern
Realm import via --import-realm can take 30-60s on first startup. Frontend services should depend on Keycloak's readiness check before attempting auth:
```yaml
depends_on:
  keycloak:
    condition: service_healthy
```
Avoid race conditions where frontend tries to auth before realm is ready.

### Pattern: Runtime Config Injection Scope Chain (Multi-Layer)
Frontend SPAs with nginx + runtime-config.js should maintain consistency across three injection layers:
1. **Dockerfile entrypoint script** — Initial fallback injected into runtime-config.js (happens at container startup)
2. **.env / compose environment** — Explicit values that override Dockerfile defaults
3. **Source code** — Build-time fallback in runtimeConfig.ts

If any layer uses a stale or incomplete value (e.g., `openid` only instead of `openid profile email offline_access`), it can override upstream values and cause OIDC token request failures. Always synchronize the full intended scope across all three layers. The .env layer should be explicit and visible to operators, not hidden in Dockerfile defaults.

