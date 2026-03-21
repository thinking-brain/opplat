## Core Context

### 2026-03-21 Session 8: Regression Test Coverage Validation & CORS Diagnosis

**Role in Session 8:** Validated regression test suite coverage for two-client model and callback recovery. Confirmed:
1. Realm contract tests already pin exactly two SPA clients with correct origin families
2. Callback contract tests already verify restored sessions override transient error state
3. Protected route tests already validate auth-error only shows when truly unauthenticated
4. Repo contract validates `http://localhost:3201` for `opplat-admin` client

**Finding:** CORS failure on token endpoint is compatible with repo being correct. A live Keycloak CORS miss can indicate stale container state even when source is correct. Regression suite now serves as early detection: if callback or protected route tests fail, the callback recovery seam (not the realm) is the problem.

**Cross-Team Coordination:** Bishop confirmed repo contract with Hudson (infrastructure), Vasquez (frontend), and Ripley (architecture validation).
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

## Learnings

- Final admin login recovery is pinned at the frontend auth seam, not the realm-client seam alone: `AuthContext.tsx` must treat a restored non-expired OIDC user as authenticated, and `AuthCallbackPage.tsx` / `ProtectedRoute.tsx` must prefer that recovered session over transient shared auth errors.
- Keeping `opplat-client` and `opplat-admin` as separate public Keycloak clients remains the supported contract even with one shared backend audience; the regression suite now treats that separation plus callback recovery as the intended model.
- The repo already protects the admin SPA CORS/origin contract for `http://localhost:3201`: `KeycloakRealmContractTests` pins `opplat-admin` origins to `3101/3201/5174`, so a live token-endpoint CORS miss can still point to a stale running Keycloak realm rather than bad source.
- `react-oidc-context` setup needs its own regression seam beyond callback-page UI: source contracts should pin `AuthProvider` wiring of `onSigninCallback` and browser `localStorage` user persistence, because restored-session behavior depends on both pieces even when route/error handling tests are already green.
