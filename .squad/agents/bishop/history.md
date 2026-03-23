## Core Context

**CURRENT FOCUS:** Phase Gate 1 authorization wave for MainApp area-by-area migration to thin-host minimal APIs with comprehensive regression test coverage.

### Active Sessions Summary (2026-03-23)

**Session 17: MainApp Sales Regression Gates** — ✅ COMPLETE (81/81 passing), pending Ripley Phase Gate 2 review
- Extended `MicroserviceHostArchitectureTests.cs` to validate Sales thin-host pattern
- Architecture contract assertions: no `AddControllers()`, endpoints inject `IMediator`, controllers archived, handlers discoverable
- Sales-specific auth seam validation: sales-list protected, product-list unannotated
- Route surface validation: both `/sales/*` and `/{__tenant__}/sales/*` routed to MediatR
- Test delta: 79 inherited + 2 new Sales-specific = 81/81 PASSING

**Session 16: MainApp Inventory Regression Gates** — ✅ COMPLETE (79/79 passing), pending Ripley Phase Gate 2 review
- Extended architecture tests to cover Inventory thin-host pattern
- Movement-type authorization seam validation across dual route families
- All tests executable without live database or external infrastructure

### Pattern (Established, Tested, Validated)

**Thin-Host MainApp Area Migration Architecture:**
1. Create `Features/{Area}/Endpoints.cs` with MediatR-injected minimal APIs
2. Map dual routes: `/{__tenant__}/{area}/*` + `/{area}/*`
3. Archive legacy controllers (rename + comment route attributes)
4. Update `Program.cs`: remove conventional routes, add `app.Map{Area}Endpoints()`
5. Regression tests: validate thin-host composition, MediatR injection, controller archival, route surfaces, auth seams

**Test Strategy:** Source-only checks (Program.cs, controller markers) paired with executable route assertions and auth-seam validation. No live database or external infrastructure required.

### Key Outcomes

- **Phase Gate 1 Reauthorization** (2026-03-23): Following defect corrections, Ripley re-approved MainApp area migration. Both Inventory & Sales authorized with passing regression gates (79/79, 81/81).
- **Regression Gate Innovation**: Encoded review criteria as architecture-contract tests. Prevents accidental route loss, duplicate endpoint activation, IService regression, or controller re-activation without explicit code changes.
- **Admin API Consolidation** (2026-03-22): Removed admin auth/BFF from MainApp. Dedicated `src/Opplat.AdminApi` owns tenant catalog + admin endpoints.

### Learnings & Patterns (2026-03-23)

- Architecture contract tests must pair source validation (Program.cs checks) with runtime validation (route/metadata assertions)
- Authorization seams documented per area (e.g., sales-list protected, product-list unannotated)
- Dual route families require auth consistency assertions across both surfaces
- Regression gates prevent silent drift—future violations fail automatically instead of requiring manual checklists

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

**Sessions 1–9:** Admin API foundation, auth middleware, BFF implementation, claim normalization.  
**Sessions 10–12:** Admin auth simplification, startup contracts, shell-mode gating.  
**Sessions 13–14:** Admin boundary refactor (tenant catalog scope, removed user CRUD).  
**Session 15:** Application layer Wave 1 (rejected; ownership transferred to Hudson/Vasquez).  
**Sessions 16–17:** Phase Gate 1 reauthorization; Inventory & Sales regression gates complete & passing.

See `.squad/orchestration-log/` for detailed outcomes and `.squad/decisions.md` for architectural decisions.
