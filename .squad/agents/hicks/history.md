## Core Context

**CURRENT FOCUS:** Phase Gate 1 authorization wave for MainApp area-by-area migration to thin-host minimal APIs (MediatR-driven). Active execution: Sessions 16–17 (Inventory & Sales waves).

### Active Sessions Summary (2026-03-23)

**Session 17: MainApp Sales Wave** — ✅ COMPLETE, pending Ripley Phase Gate 2 review
- Converted `Areas\Sales` → `Features\Sales\SalesEndpoints.cs` (MediatR minimal APIs)
- Dual routes: `/sales/*`, `/{__tenant__}/sales/*`
- Controllers archived with `_Archived` suffix
- Program.cs updated (removed live routes, added `app.MapSalesEndpoints()`)
- Test validation: 81/81 passing (Bishop)

**Session 16: MainApp Inventory Wave** — ✅ COMPLETE, pending Ripley Phase Gate 2 review
- Converted `Areas\Inventory` → `Features\Inventory\InventoryEndpoints.cs` (MediatR minimal APIs)
- Dual routes: `/inventory/*`, `/{__tenant__}/inventory/*`
- 8 controllers archived
- Program.cs updated (removed live routes, added `app.MapInventoryEndpoints()`)
- Test validation: 79/79 passing (Bishop)

### Pattern (Established & Validated)

**Thin-Host MainApp Area Migration:**
1. Create `Features/{Area}/Endpoints.cs` with MediatR-injected minimal APIs
2. Map dual routes: `/{__tenant__}/{area}/*` + `/{area}/*`
3. Archive legacy controllers (rename + comment attributes)
4. Update `Program.cs`: remove conventional routes, add `app.Map{Area}Endpoints()`
5. Regression tests validate thin-host, MediatR injection, archival, routes, auth seams

### Key Outcomes

- **Phase Gate 1 Reauthorization** (2026-03-23): Ripley re-approved MainApp area migration after defect corrections (IService injection, active controllers). Both Inventory & Sales now authorized, passing regression gates.
- **Admin API Consolidation** (2026-03-22): Removed admin auth/BFF from MainApp. Dedicated `src/Opplat.AdminApi` owns tenant catalog + endpoints. User handles admin auth externally.
- **Regression Gate Innovation**: Encoded review criteria as architecture-contract tests. Prevents route loss, duplicate activation, IService regression without explicit code changes.

### Learnings & Patterns (2026-03-23)

- MainApp area migrations convert one area at a time; dual minimal API groups preserve both tenant-aware and root routes
- Authorization seams documented per area (e.g., sales-list protected, product-list unannotated)
- Controllers archived with `_Archived` suffix + commented route attributes prevent accidental re-activation during mixed-host rollout
- Regression tests must validate both composition (`Program.cs` checks) and runtime (route/auth assertions) without external infrastructure

---

## Project Context

**Project:** Opplat — Multi-platform business management system (café/restaurant)  
**Stack:** ASP.NET Core 10.0 | EF Core | SQL Server (Main) | PostgreSQL (Admin) | SignalR | JWT | React 18 | Keycloak  
**Root:** C:\projects\personal\opplat | **Branch:** develop

**Architecture Principles:**
- Clean Architecture: Domain / Infrastructure / MainApp
- Thin hosts (composition roots only)
- MediatR for business logic dispatch
- Minimal APIs for HTTP binding
- Multi-tenant via Finbuckle.MultiTenant 7.0.1

**Key Services:**
- `src/Opplat.MainApp/` — Multi-tenant business logic host (Sales, Inventory, etc.)
- `src/Opplat.AdminApi/` — Tenant catalog (PostgreSQL, MediatR, minimal APIs)
- `src/opplat-react/` — Client SPA (Vite, MUI, React Router)
- `src/opplat-admin/` — Admin SPA

---

## Session Archive (1–15)

**Sessions 1–9:** Admin API foundation (template, auth middleware, BFF implementation, claim normalization).  
**Sessions 10–12:** Admin auth simplification, startup contracts, shell-mode gating.  
**Sessions 13–14:** Admin boundary refactor (tenant catalog scope, removed user CRUD).  
**Session 15:** Application layer Wave 1 (rejected due to defects; ownership transferred to Hudson/Vasquez).  
**Sessions 16–17:** Phase Gate 1 reauthorization; Inventory & Sales waves complete & passing gates.

See `.squad/orchestration-log/` for detailed session outcomes and `.squad/decisions.md` for architectural decisions.
