## Core Context

**CURRENT FOCUS:** Phase Gate 1 authorization wave for MainApp area-by-area migration to thin-host minimal APIs with comprehensive regression test coverage. **Session 18 COMPLETE**: Final regression gates locked thin-host composition, auth seams, and controller archival. **Session 26 COMPLETE**: Aspire local development validation (source-contract tests, no full AppHost launch). **Session 27 COMPLETE**: AppHost startup validation & bootstrap contract documentation.

### Active Sessions Summary (2026-03-23)

**Session 28: PostgreSQL Migration — Source-Contract Validation & Multi-layer Coverage — ✅ COMPLETE (2026-03-23T18:45:44Z)**
- **Multi-layer Source-Contract Coverage:**
  - Runtime provider seams: Validated all `UseNpgsql` calls, confirmed no `UseSqlServer` references remain
  - AppHost/Docker orchestration: Verified `AddSqlServer` removed, all connection strings PostgreSQL format
  - Local development documentation: Updated README and `.env.docker` to PostgreSQL defaults
  - Multitenancy contracts: Assertions follow new `PostgresTenantConnectionStringResolver` seam
- **Test Suite Validation:**
  - ✅ Build status: 0 errors, 12 pre-existing warnings (unrelated to PostgreSQL changes)
  - ✅ Test coverage: Opplat.MainApp.Test **98/98 passing** (no regressions)
  - ✅ All service integration tests passing
  - Added `PostgresMigrationContractTests` to lock provider seams, AppHost configuration, docker-compose wiring, README defaults
- **Validation Results:**
  - ✅ Runtime provider seams: MainApp, Sales, Inventory all use Npgsql
  - ✅ AppHost configuration: PostgreSQL-only orchestration verified
  - ✅ Docker-compose wiring: All services connected to PostgreSQL
  - ✅ Multitenancy: Per-tenant database isolation validated
  - ✅ Build: Clean compilation, no new warnings
- **Architecture Decisions Locked:**
  - Database-per-tenant isolation (no schema-per-tenant at this stage)
  - Multitenancy via Finbuckle ConfigurationStore (unchanged)
  - Connection string format: PostgreSQL-native (Host, Port, Username)
  - Fresh migrations strategy: Archive SQL Server, generate new PostgreSQL migrations on startup
  - Four-layer source-contract coverage prevents silent drift
- **Risk Assessment:** Low risk — Aspire orchestration unchanged, docker-compose untouched, all tests passing, migration strategy safe and documented
- **Status:** Validation COMPLETE. PostgreSQL migration locked with comprehensive source-contract coverage. All 98 tests passing.
- **Orchestration Log:** `.squad/orchestration-log/2026-03-23T18-45-44Z-bishop.md`

---

**Session 27: Aspire AppHost Startup Validation — Bootstrap Contract Enforcement — ✅ COMPLETE (2026-03-23T18:21:13Z)**
- Collaborated with Hudson & Hicks on Aspire AppHost startup debugging
- Validated source-contract enforcement (AppHost bootstrap without requiring DCP/Dashboard runtime):
  - ✅ Path helpers: `FindRepoRoot()` + `RepoPath()` correctly computed
  - ✅ Endpoint configuration: `ConfigureProjectDefaults()` applied to all 4 service projects
  - ✅ All 5 resources have unique HTTP endpoint names (no conflicts)
  - ✅ AppHost launchSettings.json contains ASPIRE_* environment variables
- Locked 3-layer bootstrap contract for future maintenance:
  - Layer 1: Path computation (FindRepoRoot + RepoPath)
  - Layer 2: Endpoint configuration (ConfigureProjectDefaults + unique names)
  - Layer 3: AppHost launch settings (ASPIRE_DASHBOARD_OTLP_ENDPOINT_URL + ASPIRE_RESOURCE_SERVICE_ENDPOINT_URL)
- Validation results:
  - ✅ Build succeeds: `dotnet build src/Opplat.AppHost` → 0 errors, 12 pre-existing warnings
  - ✅ Regression suite: 89/89 tests passing (no service code regressions)
  - ✅ Configuration audit: All 3 bootstrap layers aligned and documented
  - ⚠️ Runtime launch: Blocked by missing DCP/Dashboard (system-level setup, not code-fixable)
- Architecture decision: Treat Aspire startup as multi-layer bootstrap contract. Source tests validate Layers 1–2; Layer 3 requires environment setup validation once DCP/Dashboard available.
- Decision merged: `.squad/decisions.md` Session 27 entry
- Orchestration log: `.squad/orchestration-log/2026-03-23T18-21-13Z-bishop.md`

**Session 26: Aspire Local Development Validation — Source-Contract & Build/Test Gates — COMPLETE (2026-03-23)**
- Validated Aspire AppHost builds without errors (0 errors)
- Validated all 4 backend services integrate with Aspire orchestration (MainApp, AdminApi, Sales API, Inventory API)
- Regression coverage: All 89 tests passing (no regressions from Aspire setup)
- Source-contract validation: Confirmed health endpoints functional, service bindings correct (8080, 8084, 8083, 8082)
- Configuration audit: No drift detected between AppHost, docker-compose, Keycloak realm import, README guidance
- Aspire scope validation: Single AppHost for backends only ✅, SQL Server (3 DBs) + PostgreSQL (1 DB) + Keycloak ✅, frontend Vite apps separate ✅
- Risk assessment: Low risk—Aspire pure orchestration, docker-compose untouched, no service code changes, dual-mode verified
- Validation approach: Source-contract tests only (no full AppHost launch in shared environment per Bishop constraint)
- Result: Aspire ready for team integration; all agents completed assigned tasks
- Decision merged: `.squad/decisions.md` Session 26 Bishop subsection
- Orchestration log: `.squad/orchestration-log/20260323T175012Z-bishop.md`

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
- Aspire local-dev validation is best kept source-based here: contract tests should lock AppHost resource wiring, Keycloak realm mount reuse, documented Docker dependency, and the fact that Vite SPAs still run outside Aspire.
- Aspire AppHost on this repo only starts cleanly when three bootstrap pieces stay aligned: repo-root-based project paths/bind mounts, project defaults that exclude launch-profile and Kestrel-derived endpoints before fixed `WithHttpEndpoint(...)` calls, and AppHost launch settings that define `ASPIRE_DASHBOARD_OTLP_ENDPOINT_URL` plus `ASPIRE_RESOURCE_SERVICE_ENDPOINT_URL`.
- PostgreSQL migrations on this repo are safest to validate with source contracts across four layers at once: runtime provider calls (`UseNpgsql`), local config defaults (`appsettings*.json` / `.env.docker`), orchestration (`docker-compose.yml` / `src\Opplat.AppHost\Program.cs`), and README local-dev guidance. If any one layer keeps SQL Server assumptions, local validation drifts even when the services compile.
- AppHost-backed .NET processes on this repo must receive PostgreSQL connection overrides from Aspire resource expressions (`*.Resource.ConnectionStringExpression`); hardcoded `Host=postgres` only fits container-to-container traffic, and `GetConnectionString()` is not a valid `IResourceBuilder<PostgresDatabaseResource>` API in Aspire 13.
- MainApp Swagger should treat XML comments as optional during AppHost runs; if the XML doc file is absent, unconditional `IncludeXmlComments(...)` turns even `/health` into a 500 and masks the real runtime validation signal.
- Aspire can host the repo's Vite SPAs directly, but the stable contract is source-based: `Aspire.Hosting.JavaScript` in the AppHost, `AddViteApp(...)` resources with fixed `WithHttpEndpoint(..., env: "PORT")`, and frontend `dev:aspire` scripts/config that honor `PORT`, `strictPort`, and no-auto-open behavior.
- Module 1 test planning: Graph-specific retry/account-management tests are currently planning-only because the repo has no `Microsoft.Graph`, `Azure.Identity`, `Polly`, or Graph service seam yet; first executable gates should target the auth normalization and endpoint contracts that already exist.
- The live Module 1 oid-claim validation seams are split across both hosts: `src\Opplat.MainApp\Auth\OidcClaimsTransformation.cs`, `src\Opplat.MainApp\Auth\OidcClaimsNormalizer.cs`, `src\Opplat.MainApp\Program.cs`, `src\Opplat.MainApp\Features\Admin\AdminEndpoints.cs`, plus `src\Opplat.AdminApi\Program.cs`, `src\Opplat.AdminApi\Auth\OidcClaimsNormalizer.cs`, and `src\Opplat.AdminApi\Endpoints\AdminEndpoints.cs`.
- Best current test patterns for identity work are: source-contract assertions through `test\Opplat.MainApp.Test\Auth\TestRepository.cs`, route/metadata assertions through `test\Opplat.MainApp.Test\Routing\EndpointSurfaceTests.cs`, and TestHost auth behavior via `test\Opplat.MainApp.Test\Auth\AuthEndpointAuthorizationIntegrationTests.cs`.
- User preference for this wave: work module-by-module and, for Module 1 right now, inspect/plan without modifying production code or test sources beyond squad coordination artifacts.

- Module 1 executable Graph tests are viable here because GraphUserService uses a raw HttpClient seam with an IGraphAccessTokenProvider; a custom HttpMessageHandler can lock request JSON, bearer headers, and 429/503 retry behavior without live Entra dependencies.
- Admin session contract tests should register a bearer test handler under the real Bearer scheme, because BuildSessionAsync explicitly calls AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme) before shaping xpiresAtUtc and ccessToken.
