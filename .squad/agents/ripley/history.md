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

### 2026-03-17: Phase 1 .NET 10 Review & Governance

**Task:** Lead-level phase gate review of Hudson's .NET 10 package alignment.

**Review Process:**
- Cross-referenced all `.csproj` package changes against NuGet.org availability
- Analyzed code imports (Program.cs, OpplatDbContext.cs) for compatibility
- Compared against `.squad/decisions.md` approved architecture (v7.0.1)

**Critical Finding:**
Finbuckle.MultiTenant v10.0.4 **does not exist** on NuGet.org. Latest stable: v7.0.1 only. No versions 8, 9, or 10 have been published. Assumption that Finbuckle versions lock to .NET releases is architecturally incorrect.

**Code Analysis:**
1. Non-existent namespace imports in Program.cs: `.AspNetCore.Extensions`, `.Extensions`
2. Non-existent namespace import in OpplatDbContext.cs: `.EntityFrameworkCore.Extensions`
3. API signature mismatch: `.WithRouteStrategy("__tenant__", false)` includes boolean parameter absent in v7.0.1
4. Build succeeds only because packages not yet restored; restore will fail with NU1101

**Decision: REJECT**
- Violates team-approved v7.0.1 decision
- References packages that don't exist (will fail on restore)
- Incompatible API signatures
- Missed opportunity to verify package availability

**Architectural Lesson:**
Documented rejection includes guidance: **Never assume semantic versioning locks to .NET releases.** Each package has independent versioning.

**Governance Protocol Application:**
Initially reassigned revision back to Hudson (original author), then immediately recognized this violates **Reviewer Lockout Protocol** (author cannot revise own rejection). Self-corrected and reassigned to **Hicks (Backend Dev)**, who:
- Is Finbuckle.MultiTenant architect (designed Phase 3)
- Is backend expert
- Is NOT original author (no lockout violation)

**Decision records created:**
- `.squad/decisions/inbox/ripley-net10-review-REJECTED.md` (detailed rejection analysis)
- `.squad/decisions/inbox/ripley-finbuckle-reassignment.md` (lockout correction + Hicks assignment)

**Outcome:**
- Hudson temporarily locked from revision
- Hicks assigned revision ownership
- Team governance model strengthened through protocol enforcement
- Architectural lesson embedded for future package decisions

**Status:** ✅ COMPLETE — Review done, rejection issued, governance applied, ownership transferred

### 2026-03-17: Phase 1 Domain-Context-First Architecture Refactor

**Task:** Execute Phase 1 of modular refactor—extract Sales and Inventory into domain-context-first module structures (Domain/Application/Infrastructure per context) while preserving behavior, keeping solution building, and maintaining MainApp as presentation/composition root.

**Architecture Delivered:**
- Created `src/Modules/Sales/` and `src/Modules/Inventory/` directories
- Each context has **Domain** (entities, services, interfaces), **Infrastructure** (EF repos), **Application** (empty layer for future MediatR)
- 48 files migrated: 19 Sales files + 20 Inventory files + 9 supporting files
- All namespaces updated to `Opplat.Modules.{Context}.{Layer}`
- Controllers remain in `MainApp.Areas` (presentation layer)—no routing disruption
- OpplatDbContext unified in `MainApp.Data` (compromise for multitenancy simplicity)

**Key Decisions:**
- Presentation: MainApp.Areas (unchanged)
- Domain/Infrastructure: Extracted to modules
- Application: Empty layer prepared for Phase 2 MediatR migration
- DbContext: Unified (required for Finbuckle multitenancy)

**Build Result:** ✅ Succeeded (11 projects, 0 errors, 4 pre-existing warnings)

**Output:** `.squad/decisions/inbox/ripley-phase1-boundaries.md` — Complete architectural boundaries and mitigation strategies.

**Status:** ✅ COMPLETE — Phase 1 refactor done, solution building, ready for Phase 2

### 2026-03-20: Admin App + Auth0/Keycloak + Tenant Flow Design Review

**Task:** Architecture decision for admin app, OIDC auth (Auth0 prod / Keycloak local dev), tenant identity flow through login, and Docker Compose repair.

**Key Architectural Decisions:**
1. **Admin app is a new standalone React frontend** at `src/opplat-admin/` — separate audience (operators vs end-users), separate deployment lifecycle, same tech stack (Vite + React 18 + TS + MUI)
2. **OIDC replaces custom JWT issuance** — Auth0 for production, Keycloak for local dev. Backend validates tokens via OIDC discovery (`Authority` config). No more symmetric key signing in the backend.
3. **`react-oidc-context`** chosen over `@auth0/auth0-react` — provider-agnostic, works identically with Auth0 and Keycloak
4. **Tenant identity flows via token claims** — IdP injects `tenant_id` and `tenant_identifier` into access tokens. Finbuckle route strategy resolves tenant from URL. TenantValidationMiddleware cross-checks token claim vs resolved tenant. No Finbuckle changes needed.
5. **All backend surfaces stay in MainApp** — admin endpoints at `/admin/` prefix, role-gated. No new API project (overhead not justified for MVP).
6. **Keycloak realm import JSON** at `docker/keycloak/opplat-realm.json` — pre-seeded clients, mappers, roles, and test users per tenant.

**Critical Docker Compose Findings:**
- MainApp Dockerfile missing module .csproj COPY statements (restore will fail)
- Override file has broken volume mount and frontend build conflict
- CORS middleware ordered after authorization (will fail preflight)
- 7 services total: sqlserver, keycloak, api, sales-api, inventory-api, frontend, admin-frontend

**Work Split:** Hicks (backend auth + admin endpoints), Vasquez (admin app + client auth migration), Hudson (Docker + Keycloak), Bishop (integration tests)

**Output:** `.squad/decisions/inbox/ripley-admin-auth-tenant-design.md`

**Status:** ✅ COMPLETE — Design review done, decision approved, ready for implementation dispatch