## Core Context

**Project:** Opplat — Multi-platform business management system (café/restaurant)  
**Stack:** ASP.NET Core (net10.0) | React 18 | OIDC (Auth0/Keycloak) | Vite  
**Root:** C:\projects\personal\opplat | **Branch:** develop

### 2026-03-23 Session 14: Admin API 500 Fix — Frontend Contract Alignment

**Role in Session 14 (Concluded):** Aligned admin frontend tenant contract to backend `databaseSchema` field and removed stale user CRUD calls from admin API client surface.

**What Was Done:**
1. **Contract Alignment:**
   - Updated `src\opplat-admin\src\types\index.ts` to model tenant payloads with `databaseSchema` 
   - Changed `TenantsPage` and `DashboardPage` to read/write `databaseSchema` (UI label remains "Schema")

2. **Removed Stale User Calls:**
   - Removed user CRUD operations from admin client
   - Deleted legacy auth/session/bootstrap dependencies
   - Removed admin-specific API client surface not in modern admin boundary

3. **Contract Testing:**
   - Added frontend contract tests pinning `databaseSchema` field
   - Ensures future admin API refactors fail fast in CI

**Validation:** ✅ `npm run lint` passes | ✅ `npm run build` succeeds | ✅ Contract tests pinned to databaseSchema

**Outcome:** Frontend contract aligned to backend. Removed deprecated user flows. Admin SPA now uses modern admin-api contract exclusively.

---

### Recent Sessions (2026-03-23)

**Session 15: Admin Tenant Boundary Refactor — Frontend Alignment** — Aligned admin SPA to enforce tenant catalog scope. Removed admin-side user management: deleted `UsersPage.tsx`, removed user CRUD API methods from `admin.api.ts`, removed user-related types from TypeScript interfaces, removed `/users` route from navigation. Updated `TenantsPage` to accept `databaseName` and `schema` instead of `connectionString`. Updated `DashboardPage` to show tenant metrics with `userCount` aggregation (no user details). Updated `AdminTenant` interface to reflect new contract (no `ConnectionString`, added `databaseName`, `schema`, `userCount`). Updated `SettingsPage` to remove tenant selection state. All builds and lint checks passing. Admin now handles only tenant catalog (create, list, update, deactivate) with subscription visibility via user counts.

### Prior Recent Sessions (2026-03-23)

**Session 15: Admin Boundary Alignment — Users Moved to Tenant Scope** — Restructured admin API and client to enforce hard boundary: users are now fully tenant-managed, not admin-managed. Backend: updated `AdminTenantDto` to drop `ConnectionString` and add `DatabaseName`, `Schema`, `UserCount`. Removed user DTOs (`AdminUserDto`, `AdminCreateUserRequest`, `AdminUpdateUserRequest`, `AdminSetUserRolesRequest`, `AdminSetUserActiveRequest`) from admin contracts. Frontend: removed `UsersPage.tsx` entirely, deleted all user CRUD endpoints from `adminApi`, updated `AdminTenant` interface to reflect new schema. Updated `TenantsPage` form to accept `databaseName` and `schema` fields. Updated `DashboardPage` to show only tenant-level metrics with `userCount` rollup. Removed `tenantSelection` state from `SettingsPage`. Admin now handles only tenant catalog (create, list, update, deactivate). All builds and compiles green.

**Session 14: Admin API Tenant/User Contract Delivery** — Implemented dedicated admin-api endpoints matching the current admin React pages: `/admin/tenants`, `/admin/users`, and tenant-scoped user mutations under `/admin/tenants/{tenantIdentifier}/users...`. Data endpoints are now callable without auth, persist tenant catalog to `src\Opplat.AdminApi\Data\tenant-catalog.json`, provision tenant identity storage from the supplied SQL Server connection string, and return frontend-aligned payloads/status codes. Verified via live smoke calls: tenant create/list, tenant user create/list, and invalid connection-string create returning HTTP 400.

**Session 13: Admin Auth Removal — Frontend Cleanup** — Completed final admin client auth removal cycle by deleting `AuthContext.tsx` session bootstrap and OIDC integration, removing login/logout/callback routes and auth redirect guards, removing CSRF token acquisition and `react-oidc-context` dependencies. Created `TemporaryAdminShell` component for authenticated users. Admin SPA now targets dedicated admin-api exclusively with no browser-side auth code. `npm build` and `npm lint` passing.

### Older Sessions (2026-03-22)

**Session 12: Admin Client Admin-API Alignment Validation** — Verified admin client properly targets dedicated admin API for both data calls and auth/BFF flow. Admin client proxies `/admin/*` and `/auth/*` through Vite/Nginx to admin-api service. `VITE_ADMIN_API_URL` correctly set to `http://localhost:8084`. `VITE_BFF_BASE_URL` falls back as expected. Same-origin browser behavior preserved for cookie/CSRF flow. No regression in existing auth flow.

**Session 11: Admin Auth Runtime Brittleness Fix** — Fixed frontend runtime brittleness in admin `AuthContext` bootstrap by deduplicating in-flight session restore, tolerating transient CSRF bootstrap failure, and preventing React dev double-mount crashes. Session restore deduplication with shared promise ref. CSRF bootstrap failure tolerance—session succeeds independently, CSRF reacquired lazily on first mutation. Matches backend design where CSRF tokens re-fetched per-request anyway.

**Session 10: Temporary Admin Shell Mode Frontend Implementation** — Implemented minimal authenticated shell by adding `TemporaryAdminShell` component and collapsing feature routes while preserving auth flow. TemporaryAdminShell displays authenticated state (name, email, logout button) when `ShellModeEnabled` is true. Feature routes (`/tenants`, `/users`, `/settings`) redirect to `/`. Auth preserved (login, callback, session restore, CSRF, logout).

### Prior Sessions (2026-03-21 and earlier) — Summary

**Sessions 6–9 (2026-03-20–03-21):** SPA auth architecture and admin app setup:
- Admin app scaffold with React 18, Vite, OIDC integration (`src/opplat-admin/`)
- OIDC flow: PopStateEvent dispatch for callback, session recovery preference
- Storage cleanup on SPA startup to prune stale oidc-client-ts entries
- Keycloak two-client cost assessment (zero per-client cost)
- Admin redirect validation via callback observer and recovery
- Vite dev proxy configuration with `changeOrigin: false`

**Sessions 1–5:** Base app setup, auth scaffolding, initial OIDC routing

### Key Learnings

- **React 18 StrictMode:** Double-mount amplifies transient backend failures. Deduplicate in-flight requests with shared promise refs.
- **CSRF Bootstrap:** Don't make it a hard prerequisite for auth. Session restore succeeds independently; CSRF reacquired lazily on first mutation.
- **Storage Cleanup:** Clear stale oidc-client-ts entries on SPA startup before UserManager init—prevents noise in live debugging.
- **Session Recovery:** Prefer recovered sessions over new login in callback; improves UX during auth flow interruptions.
- **Vite Proxy:** Use `changeOrigin: false` to preserve browser origin for OIDC redirects and cookie scope.
- **Component Composition:** Shell mode toggles feature visibility without code changes—use configuration flag from backend session response.
- **Auth Removal Fallback:** When auth must be handled manually, keep the admin SPA routeable by removing client-side guards/redirects first, then preserve only neutral shell state like selected tenant and direct `/admin/*` API access.

### Architecture

- **Admin SPA:** React 18, Vite dev, separate app at `src/opplat-admin/`, port 3001
- **Auth Flow:** OIDC (Keycloak/Auth0), cookie-backed BFF via dedicated admin API
- **Shell Mode:** Temporary authenticated empty page (TemporaryAdminShell component); features hidden until auth stabilizes
- **API Target:** Dedicated admin API at `8084`; frontend proxies to it exclusively
- **State Management:** Auth context with session/CSRF separation, lazy CSRF reacquisition

## Learnings

- 2026-03-23 — The current dedicated admin API tenant contract uses `databaseSchema` in JSON (`DatabaseSchema` in C#), not `schema`. The admin frontend must send and read `databaseSchema` in `src\opplat-admin\src\types\index.ts`, `src\opplat-admin\src\pages\TenantsPage.tsx`, and `src\opplat-admin\src\pages\DashboardPage.tsx` to stay aligned with `src\Opplat.AdminApi\Endpoints\AdminContracts.cs`.
