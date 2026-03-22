## Core Context

**Project:** Opplat — Multi-platform business management system (café/restaurant)  
**Stack:** ASP.NET Core (net10.0) | React 18 | OIDC (Auth0/Keycloak) | Vite  
**Root:** C:\projects\personal\opplat | **Branch:** develop

### Recent Sessions (2026-03-22)

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
