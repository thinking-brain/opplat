## Core Context

**Project:** Opplat — Multi-platform business management system (café/restaurant)  
**Role:** Architect, senior reviewer, design validation  
**Root:** C:\projects\personal\opplat | **Branch:** develop

### Recent Sessions (2026-03-22)

**Session 16: Admin API MediatR + PostgreSQL Migration — Architecture Review & Approval** — Reviewed and approved user request to replace stores/services with MediatR handlers and switch to PostgreSQL. Approved with guidance (2026-03-22T22:27:00Z). Key findings: (1) AdminPortalStore is mock in-memory data, not persisted; (2) MediatR pattern already established in MainApp; (3) Npgsql already referenced in csproj. Decision: separate postgres container for admin-api only, dedicated AdminDbContext for tenant/user CRUD, handler pattern mirroring MainApp/Features. Auth pipeline and endpoint contracts FROZEN — implementation touches business logic only. Ripley-approved boundaries: MUST PRESERVE auth pipeline (cookie/OIDC/JWT), endpoint contracts (/admin/* routes), session contracts (DTO shapes locked). MUST AVOID modifying MainApp DB config, sharing DbContext, adding postgres dependency to other services. Hudson/Hicks/Bishop coordinated execution. All tests passing (65/65), docker-compose valid, builds green. Session complete.

**Session 13: Admin API MediatR + PostgreSQL Architecture Review** — Reviewed user request to replace stores/services with MediatR handlers and switch to PostgreSQL. Approved with guidance. Key findings: (1) AdminPortalStore is mock in-memory data, not persisted; (2) MediatR pattern already established in MainApp; (3) Npgsql already referenced in csproj. Decision: separate postgres container for admin-api only, dedicated AdminDbContext (not Identity-based), handler pattern mirroring MainApp/Features. Auth pipeline and endpoint contracts FROZEN — implementation touches business logic only.

**Session 12: Admin API Startup Fix Verification** — Spot-checked implementation across all agents. Confirmed admin API startup fix is correct and complete. Verified docker compose startup behavior and test coverage. Spot-checked Program.cs (removed duplicate /health mapping, admin BFF/session contract preserved), HealthController.cs (explicit Route and AllowAnonymous), Docker Compose service startup (reaches healthy state), tests (focused AdminApi startup regression tests pass), health endpoint (GET http://localhost:8084/health returns 200 Healthy|admin).

### Prior Sessions (2026-03-21 and earlier) — Summary

**Sessions 6–11 (2026-03-20–03-21):** Architecture validation and design review:
- Admin-auth-tenant design approval (Decision 4.2)
- Admin BFF migration outcome validation
- Admin app structure and module placement review
- Keycloak two-client assessment (zero cost)
- Admin shell mode approval (temporary authenticated UI until auth stabilizes)
- Temporary admin shell mode validation

**Sessions 1–5:** Initial architecture setup, module structure refactoring, design patterns

### Design Decisions Authored

1. **Admin-Auth-Tenant Design** — Separate admin app with dedicated OIDC flow, tenant-scoped backend routes, role-to-surface mapping (SuperAdmin → admin, TenantAdmin → client admin section, TenantUser → client standard)
2. **Temporary Admin Shell Mode** — Authenticated but feature-gated UI; allows auth stabilization without exposing broken business logic surfaces
3. **Two-Client OIDC Model** — Separate clients for admin and client apps; zero infrastructure cost in Keycloak (per-instance, not per-client)
4. **Admin Module Location** — Standalone admin app (`src/opplat-admin/`), not built into MainApp; cleaner separation, future flexibility
5. **Admin API MediatR + PostgreSQL** — Replace mock stores with MediatR handlers, dedicated postgres instance for admin-api; auth pipeline frozen, handlers get DbContext directly

### Key Architectural Patterns

- **Role-to-Surface Mapping:** SuperAdmin → admin portal (port 3001), TenantAdmin/TenantUser → client app with admin features per tenant
- **Shell Mode Transition:** Start with minimal shell, expand features as auth/session hardness increases; no code changes, just config flag toggle
- **Dedicated Admin Backend:** Separate microservice (port 8084) for admin API; admin SPA targets it exclusively
- **Multi-Client OIDC:** Operationally valuable (separate redirect URIs, session isolation), zero cost
- **Tenant Isolation:** X-Tenant-Identifier header validation + EF query filters + Keycloak token claims

### Coordination Focus

- Validate design decisions across team implementations
- Spot-check builds, tests, and container startup
- Approve major refactors before team proceeds
- Ensure architectural patterns are followed consistently

## Learnings

### Session 13 Learnings

1. **AdminPortalStore was never real persistence** — In-memory mock with hardcoded seed data. Moving to real DB via MediatR is a clean break, not a migration.

2. **Npgsql already anticipated** — Package reference existed in csproj before this request. Postgres support was pre-planned.

3. **Multi-database compose pattern** — Safe to add postgres alongside sqlserver; admin-api targets postgres, all other services stay on sqlserver. No cross-service impact if connection strings isolated (`AdminConnection` vs `DefaultConnection`).

4. **Auth isolation pattern works** — Auth pipeline (cookie/JWT/OIDC) can remain frozen while business logic layer is completely replaced. Clean separation validated.

