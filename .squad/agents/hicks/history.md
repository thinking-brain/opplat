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
