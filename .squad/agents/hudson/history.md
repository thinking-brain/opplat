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

### 2025-01-XX: Phase 1 — .NET 10 Upgrade Completed
- **Framework:** All projects upgraded from net6.0 → net10.0
- **Packages upgraded:**
  - mailkit: 3.1.0 → 4.10.0
  - Microsoft.AspNetCore.Authentication.JwtBearer: 6.0.11 → 9.0.5
  - Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore: 6.0.4 → 9.0.5
  - Microsoft.AspNetCore.Identity.EntityFrameworkCore: 6.0.4 → 9.0.5
  - Microsoft.AspNetCore.Identity.UI: 6.0.4 → 9.0.5
  - Microsoft.EntityFrameworkCore.Relational: 6.0.4 → 9.0.5
  - Microsoft.EntityFrameworkCore.SqlServer: 6.0.4 → 9.0.5
  - Microsoft.EntityFrameworkCore.Tools: 6.0.4 → 9.0.5
  - Microsoft.EntityFrameworkCore.InMemory: 2.2.6 → 9.0.5
  - Microsoft.Extensions.Logging.Abstractions: 7.0.0 → 9.0.5
  - Npgsql.EntityFrameworkCore.PostgreSQL: 6.0.4 → 9.0.4
  - Swashbuckle.AspNetCore: 6.2.3 → 7.3.1
  - Microsoft.NET.Test.Sdk: 16.0.1 → 17.14.0
  - Moq: 4.12.0 → 4.20.72
  - xunit: 2.4.0 → 2.9.3
  - xunit.runner.visualstudio: 2.4.0 → 3.0.0 (2.9.3 not available)
  - Added: coverlet.collector 6.0.4 to test project
- **Issues resolved:**
  - LicenceChecker package not found on NuGet → commented out package reference and disabled functionality in code (Controllers\LicenciaController.cs, Utils\LicenciaService.cs)
  - xunit.runner.visualstudio 2.9.3 not available → used 3.0.0 instead
- **Build status:** ✅ Success with 4 warnings (nullable reference types - pre-existing)
- **Next steps:** Code review and testing recommended before Phase 2

### 2025-01-XX: Phase 3 Prep — Finbuckle.MultiTenant Packages Added
- **Package added to Opplat.MainApp:**
  - Finbuckle.MultiTenant.AspNetCore 7.0.1
  - Finbuckle.MultiTenant.EntityFrameworkCore 7.0.1
- **Package added to Opplat.Infrastructure:**
  - Finbuckle.MultiTenant.EntityFrameworkCore 7.0.1
- **Restore status:** ✅ Success - all packages resolved
- **Purpose:** Enable multitenancy support for Phase 3 implementation
- **Next steps:** Hicks and Ripley will design and implement multitenancy architecture
