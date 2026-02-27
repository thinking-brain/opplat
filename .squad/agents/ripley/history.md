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

### 2026-02-27: Finbuckle.MultiTenant Architecture Design

**Task:** Designed comprehensive multitenancy architecture for Opplat using Finbuckle.MultiTenant.

**Key Architectural Decisions:**
1. **Per-tenant database isolation** chosen over discriminator-based approach for maximum data security in restaurant management context
2. **Route-based tenant resolution** (`/{__tenant__}/...`) as primary strategy with `X-Tenant-Identifier` header as fallback
3. **In-memory configuration store** for MVP; can upgrade to EF Core store later
4. **JWT claims extended** with `tenant_id` and `tenant_identifier` for client-side tenant awareness
5. **Seed data removal** — hardcoded admin user MUST be moved to tenant provisioning service

**Technical Insights:**
- Finbuckle 7.0.1 requires `IMultiTenantDbContext` implementation with `TenantMismatchMode.Throw` for strict isolation
- `DbContextOptions` must be typed as `DbContextOptions<OpplatDbContext>` for proper DI with multitenancy
- Route strategy requires `__tenant__` placeholder in ALL route patterns (areas, attribute routes, conventional routes)
- `UseMultiTenant()` middleware MUST come before `UseAuthentication()` for tenant context to be available

**Breaking Changes Flagged:**
- All API URLs require tenant prefix
- Admin seed removal requires migration strategy
- Client apps must store and use tenant identifier

**Output:** `.squad/decisions/inbox/ripley-multitenancy-design.md` — ready for Hicks to implement
