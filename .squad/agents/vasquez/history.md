## Core Context

### 2026-03-21 Session 8: CORS Investigation Coordination & Operational Procedures

**Role in Session 8:** Coordinated admin SPA auth diagnosis across all agents. Verified:
1. Admin SPA `runtimeConfig.ts` correctly resolves `client_id=opplat-admin` and `authority=http://localhost:8180/realms/opplat`
2. `oidc.ts` performs standard PKCE code exchange against realm authority (no custom token paths)
3. Verified callback recovery fix already in place: `AuthCallbackPage.tsx` redirects authenticated users away from error screen
4. Confirmed protected route guards use `error && !isAuthenticated` to avoid showing errors for restored sessions

**Finding:** CORS failure is NOT a callback or client wiring issue. It's a stale Keycloak container state. The repo contract is already correct.

**Implementation Review:** Both admin and client SPAs now have:
- Session recovery preference in callback pages (redirect authenticated users to `/` immediately)
- Protected route error guards that respect restored sessions (`error && !isAuthenticated`)
- No further code changes needed for CORS issue; operator must reset Keycloak container state
Fixed remaining SPA-side post-login failure by updating both admin and client `ProtectedRoute.tsx` to only show auth-error when `error && !isAuthenticated`. Root cause: `react-oidc-context` keeps error populated independently from session restoration. Valid users could complete login, have session restored, and still hit error wall on protected route. Updated both SPAs to prefer recovered sessions over transient shared auth errors. Coordinated with Ripley (architecture validation), Hicks (backend validation), and Bishop (regression coverage). Both SPAs now properly handle recovered sessions without stranding users behind auth error.

### Live Admin Callback Recovery Seam (2026-03-21 Session 6b)
Focused on callback page recovery for live admin flow. The issue: admin users completing Keycloak signin and session restoration still landed on `/auth/callback` error screen due to transient `react-oidc-context` error state persisting independently of the restored session. Decision: callback page should prefer resolved session state (`isAuthenticated && !loading`) over error state and redirect to `/` immediately. Implemented in `AuthCallbackPage.tsx`. Coordinated with Hicks (bootstrap resilience) and Bishop (regression coverage).

### Foundation Work Summary (Phases 1–3d; 2026-02-27 to 2026-03-21)
Phase 1: .NET 10.0 upgrade. Phase 2: Complete React 18 SPA (`src/opplat-react/`) with Vite, MUI v5, React Router, Axios; feature parity to Vue app (5 pages: Login, Home, Products, Sell, Users). Phase 3a–b: Recovered admin portal (`src/opplat-admin/`) with OIDC, MVP pages (Login, Dashboard, Tenants, Users, Settings). Phase 3c: Unified both SPAs to `react-oidc-context` / `oidc-client-ts` flow; normalized Auth0/Keycloak claims. Phase 3d: Canonicalized roles to SuperAdmin/TenantAdmin/TenantUser; gated admin behind SuperAdmin; aligned OIDC scope to `openid profile email offline_access` (NOT roles).

## Project Context

**Project:** Opplat — Multi-platform business management system (café/restaurant)  
**Requested by:** elvis.crego  
**Stack:** ASP.NET Core 10.0 | EF Core | SQL Server | SignalR | Keycloak OIDC | React 18  
**Solution root:** C:\projects\personal\opplat  
**Branch:** develop  

## Solution Structure

- `src/Opplat.MainApp/` — ASP.NET Core Web API (net10.0)
- `src/Opplat.Domain/` — Business logic (net10.0)
- `src/Opplat.Infrastructure/` — Data access, EF Core (net10.0)
- `src/Opplat.Shared/` — Common utilities (net10.0)
- `src/opplat-react/` — Client React 18 SPA (Vite, MUI, React Router, Axios)
- `src/opplat-admin/` — Admin React 18 SPA (same stack, SuperAdmin-only)
- `src/opplat-vue/` — Old Vue 2 app (kept but inactive)
- `test/Opplat.MainApp.Test/` — xunit tests (net10.0)

## Architecture & Key Decisions

- **Auth:** Keycloak OIDC via `react-oidc-context` and `oidc-client-ts` (both SPAs)
- **Roles:** SuperAdmin (admin app), TenantAdmin (user mgmt in client), TenantUser (operations)
- **Multi-tenancy:** Finbuckle.MultiTenant with 3 seeded tenants (mojocafe, demo, test)
- **Routing:** React Router v6 with role-based `ProtectedRoute` gating
- **API Client:** Axios with JWT Bearer interceptor + tenant header (`X-Tenant-Identifier`)
- **State:** React Context for auth; localStorage for token persistence
- **Frontend Scope:** `openid profile email offline_access` (roles injected via Keycloak default mappers, NOT requested)
- **UI:** Material-UI v5 for consistent, responsive design

## Recent Session Work

### 2026-03-21 Session 6: Admin Callback Fix (Vasquez + Hicks + Bishop)
**Callback Recovery:** Updated `AuthCallbackPage.tsx` to redirect authenticated users away from error screen; avoid `react-oidc-context` error state lingering after session restoration.  
**Bootstrap Resilience:** `GetAdminUsersQueryHandler` now skips/logs unreachable tenant DBs instead of failing whole request.  
**Regression Coverage:** Added callback contract tests in `FrontendAuthContractTests.cs`.  
**Status:** ✅ All tests pass, builds successful.

### 2026-03-21 Session 5: OIDC Scope & Role Alignment
- Aligned both frontends to request `openid profile email offline_access` (removed roles from scope)
- Implemented role-based route/component gating (SuperAdmin, TenantAdmin, TenantUser)
- Normalized claim parsing for both Keycloak and Auth0 token shapes
- ✅ Both apps lint + build successful

### Earlier Sessions (2026-02-27 to 2026-03-20)
See condensed Phase summaries above; extensive detailed work documented in team decisions.md and session logs.

## Learnings

- A restored `react-oidc-context` session can still carry a transient shared `error`, so admin/client `ProtectedRoute` components must only block on auth errors while the user remains unauthenticated.
- The two Keycloak SPA clients are intentional: `opplat-client` serves the tenant-facing app origins and `opplat-admin` serves the admin app origins; the live post-login error was not caused by duplicating clients.
- `react-oidc-context` can briefly expose a restored non-expired `user` before its `isAuthenticated` flag settles, so Opplat auth contexts should derive authenticated state from either signal to avoid post-callback redirect loops back to `/login`.
- The admin SPA's local dev path is correctly wired for `http://localhost:3201`: `runtimeConfig.ts` resolves the Keycloak authority to `http://localhost:8180/realms/opplat`, compose injects `VITE_AUTH_CLIENT_ID=opplat-admin`, and the realm export already allows `http://localhost:3201` in both `redirectUris` and `webOrigins`, so a live token-endpoint CORS miss points back to Keycloak runtime client state rather than the SPA code.
