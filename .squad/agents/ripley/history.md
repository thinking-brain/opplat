## Core Context

**Project:** Opplat — Multi-platform business management system (café/restaurant)  
**Role:** Architect, senior reviewer, design validation  
**Root:** C:\projects\personal\opplat | **Branch:** develop

### Recent Sessions (2026-03-22)

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
