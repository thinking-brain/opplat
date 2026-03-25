## Core Context

**CURRENT FOCUS:** Phase Gate 1 authorization wave for MainApp area-by-area migration to thin-host minimal APIs (MediatR-driven). **Session 18 COMPLETE**: Final minimal API wave — removed all MVC registration, wired all endpoint modules, locked thin-host composition. **Session 26 COMPLETE**: Aspire-safe runtime seams (/health, /alive, forwarded headers, conditional HTTPS). **Session 27 COMPLETE**: AppHost startup debugging & runtime configuration fixes.

### Active Sessions Summary (2026-03-23)

**Session 28: PostgreSQL Migration — Runtime Provider & Seam Changes — ✅ COMPLETE (2026-03-23T18:45:44Z)**
- **Phase 2 (Runtime Wiring):**
  - Updated all DbContext registrations: `UseSqlServer()` → `UseNpgsql()` across all projects
  - Updated `src/Opplat.MainApp/Program.cs` DbContext registration
  - Updated `src/Opplat.Microservices.Shared/Extensions/ServiceCollectionExtensions.cs` DbContext registration
  - Updated `src/Opplat.MainApp/Data/DesignTimeDbContextFactory.cs` to use PostgreSQL connection string format
- **Phase 3 (Tenant Provisioning & Admin):**
  - Updated `src/Opplat.MainApp/Services/TenantProvisioningService.cs` to use PostgreSQL
  - Updated `src/Opplat.MainApp/Features/Admin/Queries/GetAdminUsersQuery.cs` to use PostgreSQL
- **Thin-Host PostgreSQL Seam Design:**
  - Tenant database access resolved at request time through Npgsql
  - Tenants configurable via full `ConnectionString` (legacy support) OR catalog metadata (`DatabaseName` + optional `DatabaseSchema`)
  - Allows smooth transition during wider architecture migration
- **Migration Management:**
  - Deleted SQL Server-specific migrations: `20221121050808_Initial.*`, `20221123213125_RemoveDuende.*`, `20221123231550_AddCostTabs.*`, `OpplatDbContextModelSnapshot.cs`
  - Rationale: EF Core will auto-generate fresh PostgreSQL migrations on first app startup
- **AppHost & Local Development:**
  - AppHost now provisions PostgreSQL databases for main + tenant slices
  - Passes tenant metadata alongside connection strings
  - All environment variables updated to PostgreSQL format
- **Admin Catalog Compatibility:**
  - Catalog resolver can derive database names from both PostgreSQL and legacy SQL Server connection strings
- **Status:** Runtime changes COMPLETE. All DbContext, tenant provisioning, and admin queries use PostgreSQL.
- **Orchestration Log:** `.squad/orchestration-log/2026-03-23T18-45-44Z-hicks.md`

---

**Session 27: Aspire AppHost Startup Debugging — Runtime Configuration Fixes — ✅ COMPLETE (2026-03-23T18:21:13Z)**
- Collaborated with Hudson & Bishop on diagnosing Aspire AppHost startup failures
- Fixed path resolution defects: Worked with Hudson on `FindRepoRoot()` + `RepoPath()` helpers in Program.cs for absolute path computation from repo root
- Fixed endpoint naming conflicts: Created `ConfigureProjectDefaults()` helper (applied to all service projects) to exclude launch-profile + Kestrel-derived endpoints before explicit HTTP endpoint naming
- Validated all fixes working correctly:
  - ✅ Clean build (0 errors, 12 pre-existing warnings)
  - ✅ Path resolution working from any working directory
  - ✅ All 5 resources have unique endpoint names (mainapp-http, sales-api-http, inventory-api-http, admin-api-http, keycloak-http)
  - ✅ 89/89 regression tests passing (no service code regressions)
- Identified system-level blocker: Missing DCP/Dashboard (not code-fixable; requires external installation)
- Architecture decision: Keep orchestration fixes in AppHost; maintain thin service hosts with no Aspire-specific dependencies
- Decision merged: `.squad/decisions.md` Session 27 entry
- Orchestration log: `.squad/orchestration-log/2026-03-23T18-21-13Z-hicks.md`

**Session 26: Aspire Local Development — Runtime Seams & Host Adaptation — COMPLETE (2026-03-23)**
- Standardized /health and /alive endpoints across MainApp, AdminApi, Sales API, Inventory API
- Added forwarded header support in Development for local Aspire reverse-proxy flows
- Conditional HTTPS redirection: Only redirect when HTTPS binding configured (prevents broken redirects under Aspire HTTP-only orchestration)
- Shared patterns in `Opplat.Microservices.Shared`: Sales, Inventory APIs use standard Aspire helpers
- Local helpers: MainApp, AdminApi kept local until package/reference wiring finalized
- No package dependencies added by Hicks (Hudson's responsibility)
- Runtime contract locked: /health, /alive, forwarded headers, conditional HTTPS; Program.cs calls `builder.AddServiceDefaults()`
- Validation: ✅ All touched backend tests green (89/89 passing)
- Result: All 4 hosts ready for Aspire orchestration with predictable health endpoints, proxy-aware behavior
- Decision merged: `.squad/decisions.md` Session 26 Hicks subsection
- Orchestration log: `.squad/orchestration-log/20260323T175012Z-hicks.md`

**Session 20: .NET Maintenance Batch — Source Warning Fixes & CPM Validation** — ✅ COMPLETE
- Hicks fixed 3 real backend warnings (ServiceResponse<T>.Value nullability, SalesService.Get(string) override, BaseRepository exception logging)
- Hudson's CPM rollout completed (Directory.Packages.props, Directory.Build.props, nuget.config)
- Bishop validated: build succeeds, 89/89 tests passing, architecture regression tests updated
- Decisions merged into `.squad/decisions.md` (Session 15 entry created)
- Package governance finalized: 23 managed versions, all 15 projects inherit via central props

**Session 19: Shared Admin/Client Contracts Extraction** — ✅ COMPLETE
- Extracted canonical admin session DTOs into `src\Opplat.Application.Abstractions\Admin\AdminSessionContracts.cs` (single source of truth)
- Extracted auth constants into `src\Opplat.Application.Abstractions\Auth\` (centralized, consistent across hosts)
- Repointed MainApp and AdminApi admin endpoints to reference shared contracts instead of local copies
- Repointed host auth constant wrappers to shared constants
- Preserved host-specific auth implementations (MainApp and AdminApi remain separate, only shared contracts extracted)
- Build validation: ✅ Success (no new errors), ✅ No test regressions
- Result: Eliminated duplication while preserving host autonomy; module projects can now reference shared contracts via Abstractions layer

**Session 18: MainApp Final Minimal API Wave** — ✅ COMPLETE, pending Ripley Phase Gate 2 review
- Removed `AddControllers()` / `MapControllers()` from `Program.cs`
- Explicitly wired all surviving endpoint modules: `AdminEndpoints`, `SalesEndpoints`, `InventoryEndpoints`
- Preserved tenant middleware and auth policy ordering
- Test validation: 86/86 passing (no regression)
- Build: ✅ Success (0 errors, 7 warnings)

**Session 17: MainApp Sales Wave** — ✅ COMPLETE, Phase Gate 2 approved by Ripley
- Converted `Areas\Sales` → `Features\Sales\SalesEndpoints.cs` (MediatR minimal APIs)
- Dual routes: `/sales/*`, `/{__tenant__}/sales/*`
- Controllers archived with `_Archived` suffix
- Program.cs updated (removed live routes, added `app.MapSalesEndpoints()`)
- Test validation: 81/81 passing (Bishop)

**Session 16: MainApp Inventory Wave** — ✅ COMPLETE, Phase Gate 2 approved by Ripley
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

- **Session 18 Completion (2026-03-23):** Thin-host composition finalized — `AddControllers`/`MapControllers` removed entirely, all endpoint modules wired explicitly in `Program.cs`. No MVC controller reactivation possible.
- **Phase Gate 1 Reauthorization** (2026-03-23): Ripley re-approved MainApp area migration after defect corrections (IService injection, active controllers). Both Inventory & Sales now authorized, passing regression gates.
- **Admin API Consolidation** (2026-03-22): Removed admin auth/BFF from MainApp. Dedicated `src/Opplat.AdminApi` owns tenant catalog + endpoints. User handles admin auth externally.
- **Regression Gate Innovation**: Encoded review criteria as architecture-contract tests. Prevents route loss, duplicate activation, IService regression without explicit code changes.

### Learnings & Patterns (2026-03-23)

- MainApp area migrations convert one area at a time; dual minimal API groups preserve both tenant-aware and root routes
- Authorization seams documented per area (e.g., sales-list protected, product-list unannotated)
- Controllers archived with `_Archived` suffix + commented route attributes prevent accidental re-activation during mixed-host rollout
- Regression tests must validate both composition (`Program.cs` checks) and runtime (route/auth assertions) without external infrastructure
- **Final MainApp cleanup strategy:** Remove `AddControllers()`/`MapControllers()` once every retained endpoint module is explicitly mapped in `Program.cs`. This eliminates all controller reactivation paths while keeping archived source as reference artifacts.
- Thin-host composition locked via source contracts (Program.cs) paired with runtime contracts (endpoint/auth seams); mixed approach distinguishes "reference artifact" from "active route".

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

## Learnings

- Flattening `src\Modules\{Sales|Inventory}\Application\` into `src\Opplat.Application\{Module}\` would simplify MediatR assembly scanning, but it breaks the current thin-host modular boundary by forcing Sales and Inventory hosts to ship each other's handlers through one shared application assembly.
- In this repo, `src\Opplat.Application\DependencyInjection\ServiceCollectionExtensions.cs` is a registration seam, while module `Application\DependencyInjection\ServiceCollectionExtensions.cs` files still own module service/repository wiring; moving handlers without moving that ownership would leave application concerns split across two places.
- MainApp and both microservice hosts currently stay coherent by explicitly composing module application assemblies (`Program.cs` + endpoint `using` statements), so the safer discovery fix is documentation/facade cleanup rather than flattening the module application projects.

- When MainApp and AdminApi share identical admin-session/auth contract types, extract the canonical DTOs/constants into `src\Opplat.Application.Abstractions\` and let each host keep only the divergent runtime pieces (for example OIDC normalization or tenant persistence) so shared contracts converge without flattening module handlers.
- When the user explicitly overrides the modular-boundary preference and asks for flattened business logic, move the request/handler slices plus command-result types into `src\Opplat.Application\{Sales|Inventory}\`, add the needed module domain/infrastructure references there, and leave `src\Modules\{Sales|Inventory}\Application\` as thin DI/composition wrappers only.
- After a flattening move, hosts should scan only `Opplat.Application` for MediatR handlers (`AddOpplatApplication(Assembly.GetExecutingAssembly())`) while continuing to call `AddSalesApplication` / `AddInventoryApplication` for module-specific repository/service registration so startup stays thin without losing module wiring.
- The fixable .NET source warnings in this branch were legacy code warnings, not startup/minimal-API regressions: `src\Opplat.Shared\Services\Service.cs` needed nullable-annotation cleanup, `src\Modules\Sales\Domain\Services\SalesService.cs` needed an explicit `override`, and `src\Opplat.Infrastructure\Common\BaseRepository.cs` should log caught exceptions instead of discarding them.
- While Hudson's centralized package management rollout is in flight, solution-level validation is blocked by restore/config warnings (`NU1008`/`NU1507`) rather than backend source compilation; Hicks should limit warning-remediation work to code paths and report the package-source/PackageReference blockers instead of editing project files.
- For Aspire local-dev readiness without touching project/package wiring, keep each ASP.NET host thin and add one host-level seam that standardizes `/health` + `/alive`, trusts forwarded headers only in Development, and skips HTTPS redirection unless an HTTPS binding is actually configured.
- In Opplat's AppHost, `builder.AddProject(path)` and container bind mounts must be resolved from a stable repo-root helper instead of assuming the caller's working directory; otherwise `dotnet run --project src\Opplat.AppHost` can point at `C:\projects\personal\Opplat.*` and fail before orchestration starts.
- When an Aspire AppHost forces explicit fixed HTTP ports for existing ASP.NET Core services, exclude launch-profile and Kestrel-derived endpoints first; otherwise `WithHttpEndpoint(...)` collides with the implicit `http` endpoint from launch settings and the host dies before any service starts.
- For the PostgreSQL migration, keep ASP.NET hosts thin by moving tenant database selection into a small resolver that accepts either a full tenant connection string or catalog metadata (`DatabaseName` + optional `DatabaseSchema`) and then hands EF a finalized Npgsql connection string.
- Safe runtime migration work can switch `UseSqlServer` call sites, design-time factories, tenant provisioning, and AppHost/dev connection defaults to Npgsql before schema-per-tenant and production catalog decisions are finalized, as long as existing tenant `ConnectionString` values still remain valid.
- Module 1's live OIDC/BFF seam already sits in `src\Opplat.AdminApi\Program.cs` and `src\Opplat.AdminApi\Endpoints\AdminEndpoints.cs`; MainApp currently validates bearer tokens in `src\Opplat.MainApp\Program.cs` and its `Features\Admin\AdminEndpoints.cs` still contains stale `AdminOidc`/`AdminCookie` routes that are not wired by the host.
- The best reuse seam for Entra claims is `src\Opplat.Application.Abstractions\Auth\`; if Module 1 needs stable `oid` support, add the constant there and let both `Opplat.AdminApi\Auth\OidcClaimsNormalizer.cs` and `Opplat.MainApp\Auth\OidcClaimsNormalizer.cs` consume it.
- Current JWT/OIDC validation logic is duplicated but structurally aligned across `src\Opplat.AdminApi\Auth\{AuthOptions,OidcClaimsTransformation,OidcClaimsNormalizer}.cs` and `src\Opplat.MainApp\Auth\{AuthOptions,OidcClaimsTransformation,OidcClaimsNormalizer}.cs`; extend those seams for Entra issuer/claim handling instead of adding a third normalization path.
- For Module 1 Entra rollout, keep `src\Opplat.AdminApi` provider-neutral at the ASP.NET Core layer but add an explicit `Auth:Provider` + `Auth:Entra:*` config seam so production can target Entra while `appsettings.Development.json` still overrides back to local Keycloak without forking the host.
- Reuse the existing admin session endpoints for token handoff: extend `AdminSessionDto`/`AdminSessionUserDto` to surface the normalized `oid` and current access token rather than adding a separate auth controller.
- A testable Graph seam in this repo fits best as `IGraphUserService` in `Opplat.Application.Abstractions` with an `HttpClient` + token-provider implementation in `Opplat.Infrastructure`; explicit per-call retry handling for 429/503 keeps Graph lifecycle operations independent of the web host.
