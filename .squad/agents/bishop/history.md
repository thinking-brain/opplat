## Core Context

**CURRENT FOCUS:** Phase Gate 1 authorization wave for MainApp area-by-area migration to thin-host minimal APIs with comprehensive regression test coverage. **Session 18 COMPLETE**: Final regression gates locked thin-host composition, auth seams, and controller archival.

### Active Sessions Summary (2026-03-23)

**Session 20: .NET Maintenance Batch — Source Warning Fixes & CPM Validation** — ✅ COMPLETE
- Fixed 3 real backend warnings: ServiceResponse<T>.Value nullability (`src\Opplat.Shared\Services\Service.cs`), SalesService.Get(string) override (`src\Modules\Sales\Domain\Services\SalesService.cs`), BaseRepository exception logging (`src\Opplat.Infrastructure\Common\BaseRepository.cs`)
- Hudson's centralized package management validated (23 managed versions, 15 projects, 0 build warnings)
- Bishop's test suite validation complete (89/89 passing, architecture regression tests updated for flattened app layer)
- No suppressions used—all fixes are real source code corrections
- Solution rebuild blocked by CPM restore/configuration warnings (not Hicks' scope per boundary decision)

**Session 19: Application Layer Boundary Architecture Tests** — ✅ COMPLETE (89/89 passing)
- Created `ApplicationLayerBoundaryArchitectureTests.cs` to enforce approved application-layer boundary
- Validates shared `Opplat.Application` remains seam for cross-surface logic only
- Validates `Opplat.Modules.Sales.Application` and `Opplat.Modules.Inventory.Application` remain independent handlers
- Prevents silent drift: tests fail if someone moves Sales/Inventory handlers to shared layer
- Source-contract assertions lock boundary: no absorbing of module namespaces/refs, no unnecessary Admin API cross-module deps, explicit module assembly references required
- Final result: 89/89 PASSING (includes inherited tests from prior sessions + session-specific boundary validations)
- Flattening vulnerability closed; future violations fail automatically

**Session 18: MainApp Final Regression Gates** — ✅ COMPLETE (86/86 passing), pending Ripley Phase Gate 2 review
- Extended `ConvertedSurfaceArchitectureTests.cs` to validate final thin-host pattern (no `AddControllers`, all modules mapped)
- Extended `MultitenancyConfigurationTests.cs` to validate tenant-scoped admin endpoint routing
- Extended `AuthEndpointAuthorizationIntegrationTests.cs` to validate auth seam enforcement
- All tests execute without live database or external infrastructure
- Final result: 86/86 PASSING (includes all inherited + session-specific gates)
- Architecture contracts enforce composition: `Program.cs` thin-host, no MVC activation

**Session 17: MainApp Sales Regression Gates** — ✅ COMPLETE (81/81 passing), Phase Gate 2 approved by Ripley
- Extended `MicroserviceHostArchitectureTests.cs` to validate Sales thin-host pattern
- Architecture contract assertions: no `AddControllers()`, endpoints inject `IMediator`, controllers archived, handlers discoverable
- Sales-specific auth seam validation: sales-list protected, product-list unannotated
- Route surface validation: both `/sales/*` and `/{__tenant__}/sales/*` routed to MediatR
- Test delta: 79 inherited + 2 new Sales-specific = 81/81 PASSING

**Session 16: MainApp Inventory Regression Gates** — ✅ COMPLETE (79/79 passing), Phase Gate 2 approved by Ripley
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

- **Session 18 Completion (2026-03-23):** Final regression gates locked — mixed source/runtime contract approach distinguishes "archived reference" from "active MVC routing". Architecture tests enforce thin-host composition (no `AddControllers`/`MapControllers`), endpoint wiring, and auth seam enforcement.
- **Phase Gate 1 Reauthorization** (2026-03-23): Following defect corrections, Ripley re-approved MainApp area migration. Both Inventory & Sales authorized with passing regression gates (79/79, 81/81).
- **Regression Gate Innovation**: Encoded review criteria as architecture-contract tests. Prevents accidental route loss, duplicate endpoint activation, IService regression, or controller re-activation without explicit code changes.
- **Admin API Consolidation** (2026-03-22): Removed admin auth/BFF from MainApp. Dedicated `src/Opplat.AdminApi` owns tenant catalog + admin endpoints.

### Learnings & Patterns (2026-03-23)

- Architecture contract tests must pair source validation (Program.cs checks) with runtime validation (route/metadata assertions)
- Authorization seams documented per area (e.g., sales-list protected, product-list unannotated)
- Dual route families require auth consistency assertions across both surfaces
- Regression gates prevent silent drift—future violations fail automatically instead of requiring manual checklists
- **Mixed source/runtime validation:** Final MainApp minimal-API gates assert both host-level controller removal (`AddControllers`/`MapControllers`) and file-level archival markers so legacy controller code can remain as reference without reactivating live routing
- Test design: Composition checks + runtime assertions + auth seams = complete regression prevention without external infrastructure
- Shared-vs-module application boundaries are best locked with source-contract tests that check both project references and namespace ownership: `Opplat.Application` stays module-agnostic while Sales/Inventory handlers remain under `src\Modules\{Module}\Application\`
- When source-contract tests crawl project trees, exclude `bin/` and `obj/` so generated build artifacts do not create false failures
- User-directed boundary reversals are safest to encode with paired checks: assert handlers moved into `src\Opplat.Application\{Sales|Inventory}\**`, and assert legacy module application projects retain only DI/composition wrappers
- Source-contract crawlers should also ignore `obj-hicks/`; generated assembly attributes there can both cause false architecture failures and break `dotnet test` with duplicate assembly metadata

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

## Learnings
- Centralized .NET package management is active through Directory.Packages.props; keep PackageVersion items there and leave project files with versionless PackageReference entries.
- A repo-local NuGet.Config that clears inherited package feeds and keeps 
uget.org avoids CPM restore noise from user-specific feeds (for example NU1507 from a machine-level DevExpress source).
- Current application ownership is flattened into src\Opplat.Application\Sales\** and src\Opplat.Application\Inventory\**; legacy module application project files under src\Modules\{Sales|Inventory}\Application\ are gone.
- Main validation commands for this phase were dotnet build .\opplat.slnx -m:1 -v minimal, dotnet build .\opplat.slnx -t:Rebuild -m:1 -v minimal, and dotnet test .\test\Opplat.MainApp.Test\Opplat.MainApp.Test.csproj -m:1 -v minimal.
- Remaining rebuild warnings are broad nullable-analysis debt concentrated in legacy/shared files like src\Opplat.Shared\Services\Service.cs, src\Opplat.MainApp\Controllers\AccountController.cs, and older domain entities; they are not localized to the CPM maintenance changes.
- Repo CPM validation note: keep package versions in Directory.Packages.props and use versionless PackageReference items in project files.
- Repo NuGet source note: a repo-local NuGet.Config limited to nuget.org prevents NU1507 noise from inherited machine feeds during centralized package restores.
- Validation note: current shared application ownership lives in src\Opplat.Application\Sales\** and src\Opplat.Application\Inventory\**, so architecture tests should not expect src\Modules\{Sales|Inventory}\Application\ project files.
