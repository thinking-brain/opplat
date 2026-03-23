## Core Context

**Project:** Opplat — Multi-platform business management system (café/restaurant)  
**Role:** Architect, senior reviewer, design validation, reviewer lockout enforcement  
**Root:** C:\projects\personal\opplat | **Branch:** develop

## Core Context

**Project:** Opplat — Multi-platform business management system (café/restaurant)  
**Role:** Architect, senior reviewer, design validation, reviewer lockout enforcement  
**Root:** C:\projects\personal\opplat | **Branch:** develop

### Recent Sessions (2026-03-23)

**Session 20: Application Layer Remediation — Phase Gate 1 Review & Lockout Protocol**

**Status:** REJECTED & LOCKED. First rejection with reviewer lockout protocol enforcement.

**Review Outcome:**
- **SalesEndpoints.cs:** ⚠️ PARTIAL — Endpoints correctly archived, BUT endpoints still inject legacy IService (dead code)
- **InventoryEndpoints.cs:** ❌ REJECTED — Same wiring defect + controllers not archived
- **Inventory Module:** ❌ REJECTED — No handlers implemented (only AssemblyMarker.cs)
- **Regression Tests:** ⏳ PENDING — Bishop to encode acceptance criteria as gates

**Defects Found:**
1. Endpoints inject `IProductService`, `IToppingService` directly — MediatR handlers exist but are never called
2. Inventory controllers remain active (ProductsController, InventoriesController, etc.)
3. Inventory Application project is a placeholder

**Reviewer Lockout Protocol Enforced:**
Per Ripley's authority, original defect author (Hicks) excluded from revision ownership:
- **SalesEndpoints.cs** → Hudson (wiring fix)
- **InventoryEndpoints.cs** → Hudson (wiring) + Vasquez (archival)
- **Inventory Handlers** → Hudson (creation)
- **Tests** → Bishop (regression gates)

**Correction Wave Status:** Awaiting Hudson/Vasquez Phase 2 completion (Inventory controller archival), then scheduled for re-review.

---

**Session 19: Application Layer Refactor — Design Review & Approval**— Ran full Design Review ceremony for Elvis's request to consolidate business logic into class libraries with MediatR. APPROVED with detailed architecture decision. Key findings: (1) Module Application projects exist but are empty placeholders; (2) Legacy IService<T,K> pattern in Domain/Services needs migration to MediatR handlers; (3) 13 controllers in MainApp/Areas, 5 in Sales microservice, 8+ in Inventory microservice need conversion to minimal API. Decision: Multiple Application libraries per bounded context (not single shared), new Opplat.Application.Abstractions for cross-cutting behaviors, handlers inject DbContext directly. Conversion order: Sales microservice → Inventory microservice → MainApp Areas (lowest to highest risk). Phase gates with test requirements at each stage. Agent assignments: Hudson (project config), Hicks (handlers + endpoints), Bishop (tests), Vasquez (none needed).

---

### Prior Sessions (2026-03-22 and earlier)

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
6. **Admin Boundary Shift — User & Connection Isolation** — Admin API owns tenant catalog only, not users. Users belong to tenant DBs. Admin tracks MaxUsers/CurrentUserCount for subscription enforcement. ConnectionString replaced with DatabaseName+SchemaName (credentials resolved at runtime).
7. **Application Layer Refactor** — Multiple Application libraries per bounded context + shared Opplat.Application.Abstractions. MediatR handlers replace IService<T,K> pattern. Thin API hosts keep only startup/endpoints. Controller-to-MinimalAPI conversion order: microservices first (low risk), MainApp Areas last (higher risk due to multi-tenant middleware).

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

### Session 19 Learnings

1. **Module Application projects were planned but never populated** — Sales and Inventory have empty Application projects with only AssemblyMarker.cs. Infrastructure already exists and works. Just need to add MediatR handlers.

2. **Legacy IService<T,K> pattern in Opplat.Shared** — BaseService wraps repositories with CRUD boilerplate. MediatR handlers should replace this — handlers call repositories directly, no extra service layer.

3. **Controller-to-MinimalAPI risk gradient** — Microservices are safer to convert first (isolated, simple startup). MainApp Areas have multi-tenant middleware, shared composition root — higher risk, convert last.

4. **MediatR assembly scanning strategy** — Don't rely solely on `GetExecutingAssembly()`. Explicitly list all Application assemblies containing handlers to avoid missing registrations.

### Session 20 Learnings (Phase Gate 1 Review)

1. **Surface-level conversion != architectural change** — Minimal API endpoints file can exist while still using legacy IService pattern. Always verify IMediator is actually injected and handlers invoked, not just that the file exists.

2. **Controller archival must be complete** — Sales archived correctly (5 controllers), Inventory left 8 active. Partial conversion creates risk of dual HTTP surfaces.

3. **Handler existence != handler usage** — Sales module has full MediatR handlers implemented but SalesEndpoints.cs never calls them. Dead code until endpoints are rewired.

4. **Microservice hosts need dedicated regression tests** — MainApp and AdminApi have architecture tests, but microservices lack coverage for the same patterns. Test parity across all hosts is required.

### Session 21 Learnings (Phase Gate 1 Re-Review — APPROVED)

1. **Remediation verified complete** — All defects from Session 20 corrected: Inventory's 8 controllers archived with `_Archived` suffix and route attributes commented, Sales and Inventory endpoints both inject `[FromServices] IMediator` and call handlers directly, no legacy IService injection in endpoint files.

2. **Thin-host contract validated** — Both Program.cs files are sub-25 LOC, no AddControllers/MapControllers, proper assembly scanning for MediatR handlers via `AddOpplatApplication()`, shared middleware through `UseOpplatMicroserviceHost()`.

3. **Architecture test coverage now adequate** — `MicroserviceThinHostArchitectureTests.cs` has three tests: SalesApiHost thin-host validation, InventoryApiHost thin-host validation, and Inventory controller archival verification. Tests assert no legacy service injection, MediatR usage, and proper archival format.

4. **Host registrations follow module pattern** — `AddSalesModuleServices()` delegates to `AddSalesApplication()`, same for Inventory. No direct `AddScoped` in host extensions — keeps module ownership clear.

### Session 22 Learnings (MainApp Inventory Wave — APPROVED)

1. **MainApp Inventory conversion complete** — All 8 Inventory controllers archived with `_Archived` suffix, route attributes commented, replacement file referenced. InventoryEndpoints.cs maps all feature slices via minimal API with explicit `[FromServices] IMediator` injection.

2. **Dual route shapes preserved** — `/inventory` (non-tenant) and `/{__tenant__}/inventory` (tenant-prefixed) both map to the same handler functions via `MapInventoryGroup()` reuse. Multi-tenant middleware continues to work unchanged.

3. **MediatR assembly scanning correct** — MainApp Program.cs explicitly lists `Opplat.Modules.Inventory.Application.AssemblyMarker` in `AddOpplatApplication()` call, ensuring handler discovery without relying on convention.

4. **Test coverage expanded for MainApp surfaces** — `ConvertedSurfaceArchitectureTests.cs` validates: (a) Program.cs maps converted features via minimal API extensions, (b) endpoint modules stay mediator-backed and controller-free, (c) archived controllers stay unmapped after conversion.

