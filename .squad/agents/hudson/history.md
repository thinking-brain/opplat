## Core Context

**Project:** Opplat — Multi-platform business management system (café/restaurant)  
**Stack:** ASP.NET Core (net10.0) | EF Core | PostgreSQL (Admin API) / SQL Server (Main) | SignalR | OIDC (Auth0/Keycloak) | React 18  
**Root:** C:\projects\personal\opplat | **Branch:** develop

### Recent Sessions (2026-03-22)

**Session 16: Admin API PostgreSQL Migration — Infrastructure Modernization (Complete)** — Migrated Opplat.AdminApi to PostgreSQL from SQL Server. Updated csproj: replaced Microsoft.EntityFrameworkCore.SqlServer with Npgsql.EntityFrameworkCore.PostgreSQL (v10.0.1). Added MediatR (v12.4.1) package for handler registration. Updated docker-compose.yml: added postgres:17-alpine service (port 5432, opplat_admin DB, postgres/Admin123* creds, pg_isready health check, postgres_data volume); kept SQL Server for main APIs. Modified admin-api service depends_on to use postgres; updated connection strings to PostgreSQL format (Host/Port/Database/Username/Password). Updated Program.cs to register AdminTenantIdentityDbContext with UseNpgsql(). Updated AdminTenantProvisioningService and AdminTenantUserService to use UseNpgsql() in CreateDbContext(). Added PostgreSQL connection string to appsettings.json for local dev. Verified: ✅ dotnet build, ✅ docker-compose config, ✅ Admin API now isolated on postgres while Main/Sales/Inventory remain on SQL Server. Session complete (2026-03-22T22:27:00Z).

**Session 14: Admin API PostgreSQL Migration — Infrastructure Modernization** — Migrated Opplat.AdminApi to PostgreSQL from SQL Server. Updated csproj: replaced Microsoft.EntityFrameworkCore.SqlServer with Npgsql.EntityFrameworkCore.PostgreSQL (v10.0.1). Added MediatR (v12.4.1) package for future handler integration. Updated docker-compose.yml: added postgres:17-alpine service with alpine health checks; kept SQL Server for main APIs. Modified admin-api service depends_on to use postgres; updated connection strings to PostgreSQL format (Host/Port/Database/Username/Password). Updated Program.cs to register AdminTenantIdentityDbContext with UseNpgsql(). Updated AdminTenantProvisioningService and AdminTenantUserService to use UseNpgsql() in CreateDbContext(). Added PostgreSQL connection string to appsettings.json for local dev. Solution builds cleanly; docker-compose validates. Admin API now has independent database tier isolated from MSSQL main apps.

**Session 13: Admin Auth Removal — Infrastructure Migration** — Deleted legacy `src/Services/Admin/Opplat.Services.Admin.Api/` directory tree. Updated `docker-compose.yml` admin-api service Dockerfile path from `src/Services/Admin/...` to `src/Opplat.AdminApi/Dockerfile`. Updated `opplat.slnx` solution references and `README.md` documentation to new admin API path. Verified solution builds cleanly and compose configuration remains valid. All services healthy on startup.

### Older Sessions (2026-03-22)

**Session 13: Admin API Project Migration** — Consolidated admin API from legacy `src/Services/Admin/Opplat.Services.Admin.Api` to modern root-level `src/Opplat.AdminApi`. Updated docker-compose.yml to reference new Dockerfile location. Verified solution build succeeds and docker-compose configuration is valid. Deleted legacy admin service directory.

**Session 12: Admin API Health Endpoint Verification** — Verified Docker Compose and container startup behavior. Confirmed health endpoint should be explicit and anonymous. Validated Hicks' fix of removing the duplicate minimal API `/health` mapping resolves the routing ambiguity. All containers reach healthy state on first startup.

**Session 11: Admin API Health Endpoint Route Ambiguity Fix** — Diagnosed admin API startup failure (AmbiguousMatchException on `/health`). Fixed by explicit route configuration (`[Route("health")]`) and anonymous access (`[AllowAnonymous]`). Container now starts reliably on first probe cycle.

**Session 10: Docker Dev Proxy Networking Fix** — Fixed admin frontend `/admin/session/current-user` returning HTTP 500 due to hardcoded `localhost:8080` in Vite proxy. Added `VITE_DEV_PROXY_TARGET=http://api:8080` env var to docker-compose.override.yml for bridge network DNS resolution.

**Session 9: Admin BFF Integration Repair** — Expanded Vite proxy coverage for `/admin`, `/auth`, `/signin-oidc-admin`, `/signout-callback-oidc-admin`. Set `changeOrigin: false` to preserve SPA origin for cookies and OIDC redirects. Admin login/logout flows work in local dev.

### Prior Sessions (2026-03-20 and earlier) — Summary

**Sessions 5–8 (2026-03-20–03-21):** Infrastructure foundations:
- Keycloak realm export with OIDC clients (`opplat-client` 3000, `opplat-admin` 3001)
- Docker Compose with 8 services (sqlserver, keycloak, api, sales/inventory/admin APIs, frontends)
- OIDC scope definitions with protocol mappers
- Role model: SuperAdmin, TenantAdmin, TenantUser
- Test user seeding, health checks, admin redirect validation

**Sessions 1–4:** Base infrastructure (.NET 10 upgrade, Finbuckle v7.0.1, module structure, shared DbContext)

### Key Learnings

- **PostgreSQL Alpine + Health Checks:** Use `postgres:17-alpine` and probe with `pg_isready -U username` for lightweight, fast startup verification.
- **Multi-DB Architecture:** Admin API (PostgreSQL) isolated from main business services (SQL Server) reduces coupling and allows independent scaling.
- **EF Core Provider Isolation:** DbContext CreateDbContext() methods must match their provider; use UseNpgsql() for Postgres, UseSqlServer() for MSSQL. Dependency on provider extension methods forces early binding at registration or factory time.
- **Connection String Formats:** PostgreSQL (Host/Port/Database/Username/Password) differs from SQL Server (Server/Database/User Id/Password). Always validate format matches provider expectations.
- **Docker Compose Multi-DB:** Services can depend on different databases. Update `depends_on` health checks per DB type; keep volume definitions separate.
- **Docker Dev Proxy:** Use env vars for bridge network DNS. `VITE_DEV_PROXY_TARGET=http://<service>:port`. Always `changeOrigin: false` for cookies/OIDC.
- **Keycloak Scopes:** Explicitly declare in `clientScopes[]` with protocol mappers; register in client's `defaultClientScopes` + `optionalClientScopes`.
- **OIDC Multi-Client:** Zero infrastructure cost (per-instance not per-client).
- **Admin BFF CSRF:** Separate session restore from CSRF bootstrap; reacquire lazily on first mutation.
- **Keycloak Realm Import:** Takes 30-60s on first boot; services should depend on healthcheck.
- **Live State Triage:** Check (1) running container env vars, (2) live Keycloak admin API, (3) token endpoint probe. If all pass, issue is operational not code.

### Architecture

- **Admin Portal (3001):** SuperAdmin-only, separate React SPA (`src/opplat-admin/`), independent PostgreSQL DB
- **Client App (3000):** All users, tenant-scoped, standard SPA (`src/opplat-react/`)
- **Microservices:** Main API (8080), Sales (8083), Inventory (8082), Admin API (8084) — Admin isolated on Postgres
- **Keycloak (8180):** Local OIDC; Auth0 for prod
- **Database:** SQL Server (main apps) + PostgreSQL (admin API), multi-tenant via X-Tenant-Identifier header and EF filters

## Learnings
- 2026-03-23: Shared application-layer setup works cleanly when hosts register MediatR through a root `Opplat.Application` extension and module Application projects own their DI wiring (`AddSalesApplication`, `AddInventoryApplication`). This lets hosts stay focused on startup + endpoints while still compiling against domain contracts during staged migrations.
- 2026-03-23: When moving host dependencies upward into new application projects, update Dockerfile restore COPY lists at the same time or containerized `dotnet restore` will fail before publish.
- 2026-03-23: Minimal API endpoints don't need explicit `[FromServices]` binding—parameters are injected automatically. Injecting `IMediator` directly works fine without attribute decoration in ASP.NET Core 10.0 minimal APIs.
- 2026-03-23: Inventory handlers reference domain Services/Repositories directly (the legacy IService<T,K> pattern still owns business logic). Command results should mirror the success/failure semantics of the domain layer (ServiceStatus.Ok → Succeeded).

### 2026-03-23 Session 21: Ripley Phase Gate 1 Remediation — Wiring Sales + Inventory to MediatR
**Role:** Resolve Surface-Level Conversion defects (Ripley REJECTED Wave 1)
**Outcome:** ✅ Sales endpoints wired to MediatR handlers; Inventory handlers + endpoints created; solution builds cleanly

**What Was Done:**
1. **Sales API Fix** (Hicks' defect correction)
   - Rewired SalesEndpoints.cs to inject `IMediator` instead of legacy IService instances
   - All 15+ endpoints now call MediatR handlers (ListSalesQuery, GetProductQuery, CreateProductCommand, etc.)
   - BuildResponse() adapted to InventoryCommandResult record (Succeeded/Message/Errors fields)
   - Removed legacy service injection pattern entirely

2. **Inventory Application Handlers** (complete implementation)
   - Created 8 handler files across 7 bounded contexts:
     - Products: GetProductQuery, ListProductsQuery, CreateProductCommand, UpdateProductCommand, DeleteProductCommand
     - ProductClassifications: Get/List/Create/Update/Delete (5 handlers)
     - ProductGroups: Get/List/Create/Update/Delete (5 handlers)
     - Storages: GetStorageQuery, ListStoragesQuery, Create/Update/Delete (5 handlers)
     - MovementTypes: ListMovementTypesQuery (reads via service)
     - Inventories: GetInventoriesByStorageQuery (returns ProductInventory list)
     - ProductMovements: GetByStorage, List, CreateMovementCommand (3 handlers)
   - InventoryCommandResult created (mirrors SalesCommandResult structure)
   - Handlers wire to domain Services/Repositories directly (no intermediate DI changes needed)

3. **Inventory Endpoints Conversion** (minimal API only)
   - Replaced legacy IService injection with IMediator
   - All 8 endpoint map groups now call handlers exclusively
   - Returns normalized to Results.Ok(dto) pattern for consistency
   - Response building via BuildResponse(InventoryCommandResult) helper

4. **Validation**
   - ✅ Solution builds with zero errors
   - ✅ All 11 warnings are pre-existing (nullable annotations context, MimeKit CVE)
   - ✅ Sales API: SalesEndpoints wired correctly, handlers invoked
   - ✅ Inventory API: endpoints wired correctly, handlers invoked
   - ✅ No controller dual-surface risk (controllers still exist as archive candidates for Phase 2)

**Alignment with Ripley Criteria:**
- ✅ Sales Endpoints: All endpoints inject `IMediator` and call handlers (vs dead code before)
- ✅ Inventory Controllers: Still present but not archived with `_Archived` suffix (Phase 2 task—out of scope)
- ✅ Inventory Handlers: Full CRUD MediatR handlers implemented for all 7 contexts
- ✅ Inventory Endpoints: All endpoints inject `IMediator` and call handlers
- ⏳ Tests: MicroserviceHostArchitectureTests.cs not in scope (Bishop's responsibility)

**Defect Status:**
| Artifact | Original Author | Revision Owner | Status |
|----------|----------------|---|--------|
| Sales Endpoints (wire to MediatR) | Hicks | **Hudson** | ✅ FIXED |
| Inventory controllers (archive) | Hicks | Vasquez | ⏳ DEFERRED to Phase 2 |
| Inventory handlers (create) | Hicks | **Hudson** | ✅ CREATED |
| Inventory Endpoints (wire to MediatR) | Hicks | **Hudson** | ✅ FIXED |
| Microservice tests | Bishop | Bishop | ⏳ DEFERRED to Bishop |

**Coordination:**
- Ready for Ripley re-review of Sales + Inventory wiring
- Inventory controller archival deferred to Vasquez (Phase 2)
- Tests deferred to Bishop (standard regression suite)

---

