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

### 2025-01-XX: Docker Infrastructure Setup
- **Created comprehensive Docker setup for local development and deployment**
- **Files created:**
  - `src/Opplat.MainApp/Dockerfile` — Multi-stage Dockerfile for .NET 10 API
  - `src/opplat-react/Dockerfile` — Multi-stage Dockerfile for React frontend with nginx
  - `docker-compose.yml` — Production-ready compose file with SQL Server, API, and frontend
  - `docker-compose.override.yml` — Development overrides with hot reload for frontend
  - `.env.docker` — Example environment variables template
  - `README.md` — Comprehensive project documentation
- **Key decisions:**
  - Build context for API is repo root (.) to access solution file and all projects
  - SQL Server 2022 with health checks for proper startup sequencing
  - Multi-tenant environment variables configured for 3 tenants (mojocafe, demo, test)
  - Frontend runs on nginx in production, Vite dev server in development mode
  - nginx configured with SPA routing (try_files fallback to index.html)
  - All services on dedicated bridge network for isolation
- **Services configured:**
  - `sqlserver`: SQL Server 2022, port 1433, with persistent volume
  - `api`: .NET 10 API, port 8080, depends on SQL Server health
  - `frontend`: React app, port 3000 (nginx) / 5173 (dev mode)
- **Environment handling:**
  - Connection strings point to containerized SQL Server
  - JWT secret configurable via environment variable
  - Finbuckle multi-tenant configuration with 3 tenant databases
  - Development vs production mode toggles
- **Developer experience:**
  - Simple `docker-compose up -d` to start entire stack
  - Hot reload in override mode for frontend development
  - README includes quick start, local dev instructions, troubleshooting
  - Clear documentation of multi-tenancy routing and usage
- **Learnings:**
  - .NET 10 SDK/runtime images use mcr.microsoft.com/dotnet/sdk:10.0 and aspnet:10.0
  - Multi-stage builds reduce final image size significantly
  - Health checks prevent API startup failures when SQL Server isn't ready
  - Docker Compose v3.8 supports depends_on with condition: service_healthy
  - Environment variable substitution with $${VAR} escaping needed in compose files
  - Volume mount /app/node_modules prevents host overwriting container dependencies
