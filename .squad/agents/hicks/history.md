## Project Context

**Project:** Opplat — Multi-platform business management system (café/restaurant)
**Requested by:** elvis.crego
**Stack:** ASP.NET Core (net6.0 → net10.0) | EF Core | SQL Server | SignalR | JWT | React 18 (replacing Vue 2)
**Solution root:** C:\projects\personal\opplat
**Branch:** develop

## Solution Structure

- src/Opplat.MainApp/           — ASP.NET Core Web API (net6.0 → net10.0)
- src/Opplat.Domain/            — Business logic (net6.0 → net10.0)
- src/Opplat.Infrastructure/    — Data access, EF Core (net6.0 → net10.0)
- src/Opplat.Shared/            — Common utilities (net6.0 → net10.0)
- src/opplat-vue/               — Old Vue 2 client (keep but inactive)
- src/opplat-react/             — NEW React 18 client (to be created)
- test/Opplat.MainApp.Test/     — xunit tests (net6.0 → net10.0)

## Key Architecture

- Clean Architecture (Domain / Infrastructure / MainApp)
- EF Core DbContext with ASP.NET Core Identity (OpplatDbContext)
- JWT Bearer auth
- SignalR hubs
- Swagger/OpenAPI

## Phase Plan

1. Phase 1 — .NET Upgrade (Hudson + Hicks + Bishop)
2. Phase 2 — React Client (Vasquez + Hicks for API verification)
3. Phase 3 — Multitenancy with Finbuckle.MultiTenant (Hicks + Ripley design)

## Decisions

- net10.0 target framework
- Finbuckle.MultiTenant for multitenancy
- React app at src/opplat-react/ (Vite + React 18 + TypeScript + MUI + React Router + Axios)
- Pages required: Login, Home, Products, Sell, Users
- Old Vue app kept at src/opplat-vue/ but inactive

## Learnings

### Phase 3: Finbuckle.MultiTenant Implementation (2026-02-27)

**Completed:** Full multi-tenant architecture using Finbuckle.MultiTenant 7.0.1

**Key Files Created:**
- `src/Opplat.MainApp/Models/AppTenantInfo.cs` — Tenant information model implementing ITenantInfo
- `src/Opplat.MainApp/Data/DesignTimeDbContextFactory.cs` — EF Core design-time factory for migrations
- `src/Opplat.MainApp/Services/TenantProvisioningService.cs` — Tenant database provisioning with migrations and seeding
- `src/Opplat.MainApp/Middleware/TenantValidationMiddleware.cs` — JWT tenant claim validation middleware

**Key Files Modified:**
- `src/Opplat.MainApp/Data/OpplatDbContext.cs` — Now implements IMultiTenantDbContext with per-tenant context
- `src/Opplat.MainApp/Program.cs` — Multi-tenant middleware, per-tenant DbContext, tenant-aware routing
- `src/Opplat.MainApp/appsettings.json` — Added Finbuckle configuration with 3 sample tenants (mojocafe, demo, test)
- `src/Opplat.MainApp/Controllers/AccountController.cs` — JWT now includes tenant_id and tenant_identifier claims

**Architecture Decisions:**
1. **Tenant Resolution:** Route strategy (/{tenant}/...) + header fallback (X-Tenant-Identifier)
2. **Data Isolation:** Full database-per-tenant (no shared tables)
3. **Tenant Store:** Configuration-based (in-memory, from appsettings.json)
4. **Identity:** Per-tenant ASP.NET Core Identity with separate admin users per tenant
5. **Backward Compatibility:** Kept old routes without tenant prefix for gradual migration

**Critical API Discovery:**
- `IMultiTenantContextAccessor<T>` is in `Finbuckle.MultiTenant.Abstractions` (not just `Finbuckle.MultiTenant`)
- Must use both namespaces: `using Finbuckle.MultiTenant;` AND `using Finbuckle.MultiTenant.Abstractions;`
- Constructor injection of `IMultiTenantContextAccessor<AppTenantInfo>` should be optional (nullable) for design-time scenarios

**Build Result:** ✅ SUCCESS (14 warnings, 0 errors)

**Next Steps for Team:**
- Vasquez (React): Update API base URL to include tenant identifier
- Bishop (Tests): Create test fixtures with mock tenant context
- Elvis: Decide on tenant onboarding strategy (manual vs. automated)

### Finbuckle Revision under Reviewer Lockout Protocol (2026-03-17)

**Context:** Hudson's Finbuckle upgrade attempt to v10.0.4 was rejected because that version doesn't exist. Ripley applied Reviewer Lockout Protocol (author cannot revise own rejection) and reassigned to Hicks.

**Assignment Rationale:** Hicks is Finbuckle.MultiTenant architect (designed Phase 3 architecture), backend expert, and not the original author of rejected work.

**Task:** Revise Finbuckle package alignment back to approved v7.0.1.

**Work Performed:**
- Reverted Finbuckle.MultiTenant packages to v7.0.1 in both MainApp and Infrastructure
- Removed non-existent namespace imports (`.AspNetCore.Extensions`, `.EntityFrameworkCore.Extensions`)
- Fixed API signature: `.WithRouteStrategy("__tenant__", false)` → `.WithRouteStrategy("__tenant__")`
- Preserved all other .NET 10 / EF Core 10 alignment (Npgsql at 10.0.0, ASP.NET Core at 10.0.5)

**Validation:**
- `dotnet build .\opplat.sln` ✅ SUCCESS
- `dotnet test .\opplat.sln --no-build` ✅ SUCCESS (0 discovered tests expected)
- No prerelease packages required

**Key Technical Lesson:**
Finbuckle 7.0.1 exposes root-namespace methods only (`UseMultiTenant`, `ConfigureMultiTenant`, `EnforceMultiTenant`). No `.Extensions` subnamespaces exist in v7.0.1. Route strategy accepts single parameter (the `__tenant__` placeholder name); boolean variants are not part of v7.0.1 API surface.

**Decision record created:** `.squad/decisions/inbox/hicks-finbuckle-revision.md`

**Status:** ✅ COMPLETE — Revision approved; Phase 1 locked and validated

### Phase 1: Sales / Inventory Module Extraction (2026-03-18)

**Task:** Move existing Sales and Inventory backend logic out of the legacy `Opplat.Domain` / `Opplat.Infrastructure` folders into the module projects under `src/Modules/Sales` and `src/Modules/Inventory` while keeping `Opplat.MainApp` as the HTTP composition root.

**Key Files Updated:**
- `src/Modules/Sales/Domain/**` and `src/Modules/Sales/Infrastructure/**` — now own Sales entities, services, repositories, and EF-backed repository implementations
- `src/Modules/Inventory/Domain/**` and `src/Modules/Inventory/Infrastructure/**` — now own Inventory entities, DTOs, services, repositories, and EF-backed repository implementations
- `src/Opplat.MainApp/Program.cs` — DI registrations now point at `Opplat.Modules.*` namespaces
- `src/Opplat.MainApp/Data/OpplatDbContext.cs` — DbSet aliases now point at module entity namespaces
- `src/Opplat.MainApp/Areas/Sales/Controllers/**` and `src/Opplat.MainApp/Areas/Inventory/Controllers/**` — controllers remain in MainApp but consume module namespaces
- `src/Opplat.MainApp/Opplat.MainApp.csproj` — references module Domain/Infrastructure projects

**Architecture Notes:**
1. `Opplat.MainApp` remains the presentation and composition root; Sales/Inventory controllers were intentionally kept there.
2. The legacy `src/Opplat.Domain/{Sales,Inventory}` and `src/Opplat.Infrastructure/{Sales,Inventory}` trees were removed after moving the code into the module projects.
3. Existing `src/Modules/*/Application` projects were kept as placeholders only; duplicate controllers were removed to avoid splitting the presentation root in Phase 1.
4. Historical EF migration designer/snapshot files were namespace-adjusted for compile integrity, but no new migration was created.
5. `CostTab` still depends on Inventory product entities across modules; that coupling was preserved deliberately to avoid behavioral churn during the structure-only refactor.

**Validation:**
- `dotnet build .\opplat.sln` ✅ SUCCESS
- `dotnet test .\opplat.sln --no-build` ✅ SUCCESS (0 discovered tests, pre-existing)

**User Preference Reinforced:**
- Prefer minimal API and behavior changes for structural refactors; keep incomplete Accounting/account features in place unless integrity requires a touch.