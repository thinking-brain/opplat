## Core Context

### 2026-03-22 Session 9: Admin BFF Migration Final Review & Rejection (Lockout Protocol)

**Role in Session 9:** Reviewed completed admin BFF migration implementation across backend (Hicks), frontend (Vasquez), tests (Bishop), and infrastructure (Hudson). **Rejected** due to five critical integration defects.

**Defects Found:**
1. **Route mismatch** — Frontend calls `/bff/auth/session|login|logout`; backend serves `/admin/session/*` + `/auth/bff/admin/login|logout`. All BFF calls 404.
2. **Query param mismatch** — Frontend sends `returnTo` query param; backend expects `returnUrl`.
3. **CSRF header mismatch** — Frontend hardcodes `X-XSRF-TOKEN` header; backend configures `X-Opplat-CSRF`.
4. **CSRF token never acquired** — Frontend never calls `/admin/session/csrf` endpoint; token remains null.
5. **Test suite broken** — 16 of 58 tests fail (7 crashes on removed `oidc.ts`, 9 assertion failures on stale pre-migration assertions).

**Lockout Protocol Activated:** Vasquez, Bishop, Hudson locked out until defects fixed. Hicks in review-only mode.

**Architecture Assessment:** PolicyScheme routing, cookie config (HttpOnly/SameSite/ticket store), OpenIdConnect code+PKCE flow, antiforgery middleware, CORS tightening, claims normalization, frontend AuthContext pattern, ProtectedRoute error priority, and admin API layer (`withCredentials`, tenant header, 401 redirect) are all architecturally sound. Defects are pure integration-layer mismatches, not design flaws.

**Re-Review Trigger:** Once Vasquez + Bishop fixes submitted and Hudson's Vite proxy updated, Ripley re-reviews against four acceptance criteria:
1. All 58 tests pass (0 skip, 0 fail).
2. Frontend BFF calls match backend routes exactly.
3. CSRF token fetched from `/admin/session/csrf` with correct dynamic header name.
4. Vite proxy covers all BFF and OIDC callback paths.

---

### 2026-03-22 Session 10: Admin Auth Simplification — Tenant-Free Auth Boundary

**Role in Session 10:** Architected and approved simplified admin auth contract. Removed tenant from auth boundary, pinned redirect fallback to 3201, preserved tenant-aware admin management endpoints, locked in guardrails and regression tests.

**Approval Given To:**
- **Hicks (Backend):** Add `Auth:AdminBff:DefaultOrigin` config, remove `TenantId`/`TenantIdentifier` from `AdminSessionUserDto`, preserve legacy `Auth:ClientIdAdmin` compat, skip tenant validation on admin auth paths, remove duplicate session endpoint
- **Vasquez (Frontend):** Remove tenant fields from auth context and types, align dev origin to 3201
- **Bishop (QA):** Add tenant-agnostic bootstrap test, pin origin regression with integration test, verify tenant isolation on scoped endpoints

**Simplification Rationale:**
- Tenant is an operational parameter for admin management, not an identity concern
- Auth boundary now clean: admin identity + SuperAdmin role only
- Tenant context injected via `X-Tenant-Identifier` header and route params for management operations
- Solves redirect-to-3001 regression; establishes canonical admin dev origin as 3201

**Guardrails Locked:**
1. Do NOT re-add tenant to session payload
2. Do NOT change OIDC/cookie auth pipeline (approved Session 9)
3. Do NOT change Keycloak realm client config
4. Do NOT touch JwtBearer path
5. Tests must reflect tenant-free contract

**Orchestration:** Ripley → Hicks → Vasquez → Bishop → Coordinator followup (test updates)

---

### 2026-03-22 Session 9: Admin BFF Migration Re-Review (Post-Lockout, APPROVED)

**Role in Session 9 (Post-Repair):** Re-reviewed repaired implementation after lockout repairs by Vasquez (paths + CSRF), Bishop (test suite), and Hudson (Vite proxy). **Approved** — all five critical defects resolved, all four acceptance criteria met.

**Acceptance Verification:**
1. **50/50 tests pass**, 0 fail, 0 skip. `FrontendAuthContractTests` and `AdminBffSessionContractTests` fully green.
2. **Frontend BFF routes match backend exactly:** `buildLoginUrl` sends `returnUrl` query param matching backend binding; session/CSRF/login/logout paths identical.
3. **CSRF flow correct:** `AuthContext.tsx` calls `GET /admin/session/csrf` after session restore; `buildCsrfHeaders()` uses backend-provided header name dynamically (no hardcoded header).
4. **Vite proxy complete:** Covers `/admin`, `/auth`, `/signin-oidc-admin`, `/signout-callback-oidc-admin` with `changeOrigin: false` preserving browser host.

**Non-Blocking Residual Risks:**
1. Duplicate `/admin/session` endpoint (remove in cleanup pass).
2. `/auth/bff/admin/switch-tenant` deferred (track as follow-up).
3. In-memory ticket store (document multi-instance production plan).
4. MimeKit vulnerability advisory `NU1902` (upgrade independently).

**Lockout Lifted.** Ready for merge.

---

**Role in Session 8:** Led CORS investigation diagnosis across all agents. Confirmed architectural soundness of two-client model:
1. **Cost Analysis:** Zero infrastructure cost per client in Docker setup (Keycloak pricing is per-instance)
2. **Operational Benefits:** Session isolation (prevents `oidc-client-ts` collisions) + independent redirect URI scoping
3. **Root Cause of CORS Issue:** Stale Keycloak container state, NOT code/architecture defect

**Architectural Decision:** Keep two clients (`opplat-client`, `opplat-admin`). Consolidation would not fix login issues; would risk session collisions and increase attack surface for redirect URI sprawl.

**Recommendation:** No code changes required for CORS. Operator resets Keycloak container state using provided diagnostic checklist.

**Cross-Team Coordination:** Ripley led decision consolidation with Hudson (infrastructure), Vasquez (frontend), Hicks (backend), Bishop (validation).
Adjudicated architectural decision to keep two separate Keycloak clients (\opplat-client\ for tenant SPA, \opplat-admin\ for admin SPA). Confirmed decision is correct for:
1. Distinct redirect URI scoping (different ports per SPA)
2. Session isolation (prevents oidc-client-ts storage key collisions)
3. Same backend audience (\opplat-api\) validation model
4. Future per-client role/mapper flexibility

Coordinated with Hicks (backend validation), Vasquez (frontend callback fix), and Bishop (regression coverage). Root cause was frontend callback/protected-route auth-error handling, not two-client model.

### 2026-03-22 Session 10: Admin Auth 500 Docker Dev Proxy Root-Cause Diagnosis

**Role in Session 10:** Diagnosed HTTP 500 from `/admin/session/current-user` reported by elvis.crego on admin dev stack. Performed full request path trace: frontend `fetch()` → Vite dev proxy → backend.

**Investigation Summary:**
- Build succeeds with 0 errors, 1 NU1902 warning (MimeKit)
- 56/56 auth integration tests pass, including Bishop's new sparse-claim and full-claim coverage
- Backend auth code is sound — no null-reference paths in `BuildSessionAsync`, `BuildSessionUser`, or `ResolveAuthenticationMode`
- Auth pipeline works: PolicyScheme → AdminCookie → 401 unauthenticated / 200 authenticated SuperAdmin

**Root Cause: Docker dev-mode proxy networking gap (NOT auth design or backend code).**

The `docker-compose.override.yml` maps host:3201 → admin container:3001 (Vite dev server). Vite proxy resolves target as:
```
env.VITE_DEV_PROXY_TARGET || env.VITE_API_URL || 'http://localhost:8080'
```

- `VITE_DEV_PROXY_TARGET` is **not set** in Docker Compose
- `VITE_API_URL=http://localhost:8080` **is** set, but `localhost` inside the container = the container itself
- API runs in separate container reachable at `api:8080` on Docker bridge
- Result: ECONNREFUSED → Vite returns HTTP 500

Production Dockerfile (nginx) correctly proxies to `http://api:8080`; gap is dev-only.

**Required Fix:** Add `VITE_DEV_PROXY_TARGET=http://api:8080` to admin-frontend environment in `docker-compose.override.yml`. Also align `src/opplat-admin/Dockerfile.dev` EXPOSE to `3001`.

**Constraints for Implementation (Hudson):**
1. Do NOT change backend auth code — it is correct
2. Do NOT change frontend auth code — it is correct
3. Fix is Docker/Vite config only
4. After fix: `GET http://localhost:3201/admin/session/current-user` should return 401 (no session), not 500

**Key Lesson:** When Vite runs inside Docker, `loadEnv()` merges `process.env` (Docker Compose vars) with `.env` files. Browser-facing env vars like `VITE_API_URL` are correct for the browser but wrong for in-container proxy. The `VITE_DEV_PROXY_TARGET` env var exists to decouple these concerns — **must be set explicitly for Docker dev mode**.

### Multitenancy & Keycloak Architecture (2026-02-27 → 2026-03-21)
Designed comprehensive multitenancy using Finbuckle.MultiTenant 7.0.1 with per-database isolation. Dual-strategy tenant resolution: route-based and header fallback. 3-tier role model (SuperAdmin/TenantAdmin/TenantUser). Keycloak realm JSON seeds realm, clients, roles, and test users.

## Learnings

### Admin Auth Simplification — Tenant Removal from Auth Boundary (2026-07-17)

**Decision:** Approved simplified admin auth contract. Admin authenticates as SuperAdmin identity with NO tenant context at auth level. Tenant is operational context for management features, injected via `X-Tenant-Identifier` header and route segments.

**Key findings:**
1. Post-login redirect bug: `AllowedOrigins[0]` was `http://localhost:3001` but Docker dev uses 3201. `ResolveReturnUrl` falls back to first origin when no `Origin` header (browser navigations don't send it). Fix: reorder to put 3201 first.
2. `Auth__ClientIdAdmin` and `Auth__ClientIdClient` env vars in docker-compose are dead — they don't bind to any property in `AuthOptions`. The actual client ID comes from `Auth:AdminBff:ClientId` in appsettings.
3. Keycloak `opplat-admin` client is `publicClient: true` with PKCE — works for now but should be made confidential later.
4. Admin session payload and frontend auth context carried `tenantId`/`tenantIdentifier` unnecessarily. Removed from auth boundary. Tenant management features keep using route/header context.

**Files affected by decision:**
- `src/Opplat.MainApp/appsettings.json` — AllowedOrigins reorder
- `src/Opplat.MainApp/appsettings.Development.json` — AllowedOrigins reorder
- `src/Opplat.MainApp/Features/Admin/AdminContracts.cs` — AdminSessionUserDto tenant fields removed
- `src/Opplat.MainApp/Features/Admin/AdminEndpoints.cs` — BuildSessionUser tenant extraction removed, duplicate /admin/session endpoint removed
- `src/opplat-admin/src/types/index.ts` — User, AuthSessionUser, AuthSessionPayload tenant fields removed
- `src/opplat-admin/src/auth/AuthContext.tsx` — tenantId/tenantIdentifier removed from context
- `src/opplat-admin/src/auth/claims.ts` — tenant persistence/extraction removed

**Decision file:** `.squad/decisions/inbox/ripley-admin-auth-simplification.md`

---

### Admin Current-User 500 — Docker Dev Proxy Root-Cause Review (2026-07-17)

Reviewed the `/admin/session/current-user` 500 reported at `http://localhost:3201`. Traced the full request path: frontend `fetch()` → Vite dev proxy → backend. Build succeeds, 56/56 integration tests pass (including Bishop's new sparse-claim and full-claim current-user tests). Backend auth code is sound — no backend bug, no contract mismatch, no auth design issue.

**Root cause: Docker dev-mode proxy networking.** The `docker-compose.override.yml` maps host port 3201 to the admin-frontend container's port 3001 (Vite dev server). The Vite proxy target is resolved from:
```
env.VITE_DEV_PROXY_TARGET || env.VITE_API_URL || 'http://localhost:8080'
```
`VITE_DEV_PROXY_TARGET` is not set. `VITE_API_URL=http://localhost:8080` (from compose override). Inside the Docker container, `localhost:8080` is the container itself — the API runs in a separate container at `api:8080` on the Docker bridge network. Connection refused → Vite returns HTTP 500.

Production Dockerfile (nginx) correctly proxies to `http://api:8080`. The gap is dev-mode only.

**Fix:** Add `VITE_DEV_PROXY_TARGET=http://api:8080` to admin-frontend environment in `docker-compose.override.yml`.

**Key lesson:** When Vite runs inside Docker, `loadEnv()` merges `process.env` (including Docker Compose env vars) with `.env` files. But the Docker Compose env var `VITE_API_URL` carries a browser-facing URL (`localhost:8080`) that's correct for the browser but wrong for in-container server-to-server proxy. The `VITE_DEV_PROXY_TARGET` env var exists to decouple these two concerns — it must be set explicitly for Docker dev mode.

### Admin BFF Final Implementation Review — REJECTED (2026-07-16)
Reviewed the actual implementation of the admin BFF migration from Hicks (backend), Vasquez (frontend), and Bishop (tests). **Rejected** due to five critical integration defects:

1. **Route mismatch** — Frontend calls `/bff/auth/*`, backend serves `/admin/session` and `/auth/bff/admin/*`. All session/login/logout calls 404.
2. **Query param mismatch** — Frontend sends `returnTo`, backend binds `returnUrl`.
3. **CSRF header mismatch** — Frontend hardcodes `X-XSRF-TOKEN`, backend expects `X-Opplat-CSRF`.
4. **CSRF token never acquired** — Frontend expects token in session DTO; backend puts it in a separate `/admin/session/csrf` endpoint that frontend never calls.
5. **16 of 58 tests fail** — 7 crash on missing `oidc.ts`, 9 fail on stale pre-migration assertions.

Architecture is sound: PolicyScheme routing, cookie config, OIDC code+PKCE, antiforgery middleware, claims normalization, and the frontend session pattern are all correctly designed. The defects are integration-layer mismatches, not architectural flaws.

Lockout: Vasquez (paths + CSRF), Bishop (tests), Hudson (Vite proxy). Hicks confirms canonical path prefix. Re-review on fix submission.

### Admin BFF Integration Repair — APPROVED (2026-07-16)
Re-reviewed the repaired implementation after lockout repairs by Hudson (frontend/config seams) and Lambert (tests). **Approved** — all five critical defects resolved, all four acceptance criteria met:

1. **50/50 tests pass**, 0 fail, 0 skip.
2. **Route alignment** — Frontend paths (`/admin/session/current-user`, `/admin/session/csrf`, `/auth/bff/admin/login`, `/auth/bff/admin/logout`) match backend routes exactly.
3. **CSRF flow** — Frontend fetches token from `/admin/session/csrf`, reads `headerName` + `requestToken` dynamically (no hardcoded header), applies via `buildCsrfHeaders()` in axios interceptor.
4. **Vite proxy** — Covers `/admin`, `/auth`, `/signin-oidc-admin`, `/signout-callback-oidc-admin` with `changeOrigin: false`.

Non-blocking residual: duplicate `/admin/session` endpoint, deferred `/switch-tenant`, in-memory ticket store for future scale-out consideration. Lockout lifted.

### Admin BFF Migration Cut Approved (2026-07-16)
Reviewed full auth state of both backend (`Opplat.MainApp`) and frontend (`opplat-admin`). Approved the admin-first BFF migration cut. Key architectural decisions:

1. **Dual auth scheme:** `AddPolicyScheme` selector routes `/bff/*` to cookie auth, everything else to JwtBearer. Both coexist during migration.
2. **Backend hosts BFF:** No separate service. `Opplat.MainApp` adds `AddCookie` + `AddOpenIdConnect` + antiforgery alongside existing `AddJwtBearer`.
3. **Four BFF endpoints:** `/bff/auth/login`, `/bff/auth/session`, `/bff/auth/logout`, `/bff/auth/switch-tenant`.
4. **Frontend simplification:** Admin SPA drops `react-oidc-context`, `oidc-client-ts`, client-side JWT parsing, callback routes, and bearer injection. Becomes a session consumer via `/bff/auth/session` + cookies.
5. **CSRF mandatory:** All mutating BFF endpoints require antiforgery validation. Non-negotiable.
6. **CORS tightening required:** `AllowAnyOrigin()` incompatible with `AllowCredentials()`. Must use explicit origin allowlist for cookie endpoints.
7. **Keycloak:** New `opplat-bff` confidential client needed alongside existing public clients.

Rejection criteria documented: no JS-readable session cookie, no missing CSRF, no client-side JWT parsing, no broken JwtBearer path. Full spec in `.squad/decisions/inbox/ripley-admin-bff-review.md`.

### BFF Auth Migration Shape (2026-03-21)
For Opplat, moving the browser from SPA-managed OIDC to a BFF pattern is feasible and advisable once production hardening becomes the priority. The strongest target is a **shared BFF session layer** for both React apps: browser uses same-origin HTTP-only cookies plus a small session endpoint, while the server becomes the confidential OIDC client and handles login, refresh, logout, and downstream access tokens.

Keep the backend/provider contract provider-neutral (`Keycloak` locally, `Entra` in production), but stop making browser code depend on raw tokens or provider claim quirks. Preserve two app experiences (admin and tenant) while centralizing auth/session handling, and migrate the admin SPA first because it has the smaller tenant-routing surface area.

### Entra + Keycloak Provider Parity (2026-03-21)
For Opplat, the safest dual-provider OIDC architecture is provider-agnostic at the protocol layer: keep ASP.NET Core `JwtBearer` + OIDC discovery on the backend and `react-oidc-context` / `oidc-client-ts` in both SPAs, then switch providers through configuration (`Authority`, client IDs, scopes, metadata URL) rather than SDK swaps.

Do **not** let business-tenant membership depend on provider-specific token plumbing. Standardize authorization roles (`SuperAdmin`, `TenantAdmin`, `TenantUser`) as IdP app/realm roles in both Entra and Keycloak, but keep Opplat tenant membership as an application-owned mapping/enrichment concern so Entra production and Keycloak local dev stay aligned without custom-provider lock-in.

### OIDC Callback Navigation Pattern (2026-03-21)
When handling OIDC callbacks in React Router SPAs, use a three-layer navigation strategy:
1. `window.history.replaceState()` to update URL immediately
2. Dispatch `PopStateEvent` to notify BrowserRouter
3. Fallback `window.location.replace()` with timeout if still on callback path

This prevents the callback page from rendering error UI before navigation completes. Also: callback pages should use `useEffect` with `useNavigate()` as primary redirect, with hard redirect fallback.

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
