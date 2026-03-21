## Core Context

### Keycloak Two-Client Architecture Validation (2026-03-21 Session 7)
Adjudicated architectural decision to keep two separate Keycloak clients (\opplat-client\ for tenant SPA, \opplat-admin\ for admin SPA). Confirmed decision is correct for:
1. Distinct redirect URI scoping (different ports per SPA)
2. Session isolation (prevents oidc-client-ts storage key collisions)
3. Same backend audience (\opplat-api\) validation model
4. Future per-client role/mapper flexibility

Coordinated with Hicks (backend validation), Vasquez (frontend callback fix), and Bishop (regression coverage). Root cause was frontend callback/protected-route auth-error handling, not two-client model.

### Multitenancy & Keycloak Architecture (2026-02-27 → 2026-03-21)
Designed comprehensive multitenancy using Finbuckle.MultiTenant 7.0.1 with per-database isolation. Dual-strategy tenant resolution: route-based and header fallback. 3-tier role model (SuperAdmin/TenantAdmin/TenantUser). Keycloak realm JSON seeds realm, clients, roles, and test users.

## Archived Context (Prior Sessions)

See \.squad/orchestration-log/\ for detailed session outcomes across all phases.

## Project Context

**Project:** Opplat — Multi-platform business management system (café/restaurant)
**Requested by:** elvis.crego
**Stack:** ASP.NET Core 10.0 | EF Core | SQL Server | SignalR | JWT | React 18
**Solution root:** C:\projects\personal\opplat
**Branch:** develop

## Solution Structure

- src/Opplat.MainApp/ — ASP.NET Core Web API (net10.0)
- src/Opplat.Domain/ — Business logic (net10.0)
- src/Opplat.Infrastructure/ — Data access, EF Core (net10.0)
- src/Opplat.Shared/ — Common utilities (net10.0)
- src/opplat-react/ — Client React 18 SPA (Vite, MUI, React Router)
- src/opplat-admin/ — Admin React 18 SPA
- test/Opplat.MainApp.Test/ — xunit tests (net10.0)

## Key Architecture

- Clean Architecture (Domain / Infrastructure / MainApp)
- EF Core DbContext with ASP.NET Core Identity
- JWT Bearer auth with Keycloak OIDC
- SignalR hubs
- Swagger/OpenAPI
