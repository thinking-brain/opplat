## Core Context

**Project:** Opplat — Multi-platform business management system (café/restaurant)  
**Role:** Architect, senior reviewer, design validation, reviewer lockout enforcement  
**Root:** C:\projects\personal\opplat | **Branch:** develop

**Key Design Decisions:**
- **Admin-Auth-Tenant Design** — Separate admin app (port 3001) with dedicated OIDC flow, tenant-scoped backend routes, role-to-surface mapping
- **Temporary Admin Shell Mode** — Authenticated but feature-gated UI; allows auth stabilization without exposing broken business logic
- **Admin API MediatR + PostgreSQL** — Replace mock stores with MediatR handlers, dedicated postgres for admin-api; auth pipeline frozen
- **Application Layer Refactor** — Multiple Application libraries per bounded context + MediatR handlers replace IService pattern. Thin API hosts keep only startup/endpoints
- **Controller-to-MinimalAPI Conversion** — Microservices first (Sales, Inventory), MainApp Areas last. All archived with `_Archived` suffix + commented route attributes

**Patterns Established:**
- Role-to-Surface Mapping: SuperAdmin → admin portal; TenantAdmin/TenantUser → client app with admin features per tenant
- Dual route shapes preserved in MainApp (non-tenant + tenant-prefixed routes) via MapSalesGroup/MapInventoryGroup reuse
- Regression gates pin thin-host composition (no AddControllers/MapControllers), MediatR ownership, auth seams at HTTP boundary

**Status:** Phase Gate approvals completed for Sales & Inventory conversions (Sessions 20–21). Session 22 final wave pending Phase Gate 2 closeout review.

---

### Session Archive (Sessions 1–21, 2026-03-20–03-23)

**Sessions 19–21 Summary:**
- **Session 19 (Design Review):** Approved MediatR refactor + controller-to-minimal-API conversion strategy (Sales microservice → Inventory microservice → MainApp Areas, lowest to highest risk)
- **Session 20 (Phase Gate 1 Review):** Rejected due to defects (legacy IService injection, controllers not archived); lockout protocol enforced; Hicks excluded from remediation
- **Session 21 (Phase Gate 1 Re-Review):** Approved after defects corrected (Inventory/Sales both fully converted with archival, MediatR wiring, regression gates)

**Admin API Sessions (6–18 archive):**
Sessions covering admin API design, MediatR+PostgreSQL migration, boundary refactor, shell mode, auth simplification, and auth removal. All architectural patterns established and approved. See `.squad/orchestration-log/` for detailed outcomes.

---

---

**Session 16: Admin API MediatR + PostgreSQL Migration — APPROVED**
Reviewed and approved MediatR handlers + PostgreSQL migration. Auth pipeline & endpoint contracts frozen.

**Session 13: Admin API MediatR + PostgreSQL Architecture Review — APPROVED**
Design approval for separate postgres container, dedicated AdminDbContext, handler pattern.

**Session 12: Admin API Startup Fix Verification — ✅ CONFIRMED**
Spot-checked implementation. All tests (65/65) passing, docker-compose valid, builds green.

### Prior Sessions (Sessions 1–11, 2026-03-20 and earlier)

**Sessions 6–11:** Admin-auth-tenant design approval, BFF migration, OIDC two-client assessment, shell mode validation
**Sessions 1–5:** Initial architecture setup, module structure refactoring, design patterns

See `.squad/orchestration-log/` for detailed session outcomes and `.squad/decisions.md` for architectural decisions.

