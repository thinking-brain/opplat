## Core Context

### Backend/Realm Topology Validation (2026-03-21 Session 7)
Validated that backend auth does not depend on Keycloak client ID; depends on issuer, \ud=opplat-api\, and normalized SuperAdmin role contract. Confirmed two-client Keycloak model is not root cause of login failures. Backend already aligned with realm topology. No backend changes needed for two-client architecture.

### Cross-Tenant Admin Bootstrap Resilience (2026-03-21 Session 6b)
Updated \GetAdminUsersQueryHandler\ to skip unreachable tenant databases instead of failing entire bootstrap. Admin dashboard now resilient to one or more tenant DB temporary unavailability.

### Admin Callback & Claim Contract Fix (2026-03-21)
Normalized Keycloak role claims across backend auth to handle both nested and flat dotted claim shapes. Updated AuthClaimTypes.cs and OidcClaimsTransformation.cs to accept both payload materializations.

## Archived Context (Prior Sessions — Phases 1–3d)

See \.squad/orchestration-log/\ for detailed session outcomes. Key milestones: Finbuckle.MultiTenant 7.0.1 implementation (2026-02-27), Sales/Inventory module extraction (2026-03-18), admin callback fix (2026-03-21 Session 6b).

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
