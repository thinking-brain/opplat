## Core Context

**Project:** Opplat — Multi-platform business management system (café/restaurant)  
**Stack:** ASP.NET Core (net10.0) | EF Core | SQL Server | SignalR | OIDC (Auth0/Keycloak) | React 18  
**Root:** C:\projects\personal\opplat | **Branch:** develop

### Recent Sessions (2026-03-22)

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

- **Docker Dev Proxy:** Use env vars for bridge network DNS. `VITE_DEV_PROXY_TARGET=http://<service>:port`. Always `changeOrigin: false` for cookies/OIDC.
- **Keycloak Scopes:** Explicitly declare in `clientScopes[]` with protocol mappers; register in client's `defaultClientScopes` + `optionalClientScopes`.
- **OIDC Multi-Client:** Zero infrastructure cost (per-instance not per-client).
- **Admin BFF CSRF:** Separate session restore from CSRF bootstrap; reacquire lazily on first mutation.
- **Keycloak Realm Import:** Takes 30-60s on first boot; services should depend on healthcheck.
- **Live State Triage:** Check (1) running container env vars, (2) live Keycloak admin API, (3) token endpoint probe. If all pass, issue is operational not code.

### Architecture

- **Admin Portal (3001):** SuperAdmin-only, separate React SPA (`src/opplat-admin/`)
- **Client App (3000):** All users, tenant-scoped, standard SPA (`src/opplat-react/`)
- **Microservices:** Main API (8080), Sales (8083), Inventory (8082), Admin API (8084)
- **Keycloak (8180):** Local OIDC; Auth0 for prod
- **Database:** SQL Server, multi-tenant via X-Tenant-Identifier header and EF filters
