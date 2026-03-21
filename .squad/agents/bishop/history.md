## Core Context

### Two-Client Realm & Callback Contract Regression Coverage (2026-03-21 Session 7)
Added comprehensive regression test coverage validating the two-client Keycloak architecture and callback recovery flow:
1. Realm contract tests pin exactly two SPA clients with expected origin families
2. Frontend callback contract tests require both SPAs to prefer restored authenticated session over transient shared auth errors
3. Protected route contract tests validate recovered sessions unblock UI before showing auth errors

All builds passing, all tests passing. Two-client Keycloak contract now guarded in test suite.

### Callback Regression Contract Coverage (2026-03-21 Session 6b)
Added focused test coverage for callback return-target handling in \FrontendAuthContractTests.cs\ pinning safe recovery seam.

## Archived Context (Prior Sessions)

See \.squad/orchestration-log/\ for detailed session outcomes. Key testing milestones: admin callback regression coverage (2026-03-21 Session 6b), two-client contract validation (2026-03-21 Session 7).

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
