# Session Log: Admin BFF Migration & Repair Cycle (2026-03-21 to 2026-03-22)

## Overview
Completed admin-first BFF migration with one rejection/revision cycle. Moved `opplat-admin` from browser-managed OIDC (`react-oidc-context` + `oidc-client-ts`) to server-backed session authentication via Keycloak confidential client. Backend implements dual-auth scheme (cookie + JwtBearer) to preserve API compatibility during migration.

## Spawn Sequence

1. **Ripley** (Architect): Approved initial admin BFF architecture; defined work assignment for Hicks, Vasquez, Hudson, Bishop.
2. **Hicks** (Backend): Implemented BFF auth endpoints (`/admin/session/*`, `/auth/bff/admin/*`), cookie auth, OpenID Connect integration, and antiforgery middleware.
3. **Vasquez** (Frontend): Migrated admin SPA to session-based auth; removed OIDC client dependencies; integrated CSRF and cookie flow.
4. **Hudson** (DevOps): Added Keycloak confidential BFF client; wired Docker Compose env variables.
5. **Bishop** (QA): Created initial BFF contract test coverage; validated dual-auth coexistence.

## Rejection Cycle (Ripley → Lockout Protocol)

**Ripley's First Review:** Found five critical integration defects making auth non-functional:
1. **Route mismatch**: Frontend called `/bff/auth/session|login|logout` but backend exposed `/admin/session/*` + `/auth/bff/admin/login|logout`.
2. **Query param mismatch**: Frontend sent `returnTo` but backend expected `returnUrl`.
3. **CSRF header mismatch**: Frontend hardcoded `X-XSRF-TOKEN` but backend configured `X-Opplat-CSRF`.
4. **CSRF token never acquired**: Frontend never called `/admin/session/csrf` endpoint; token remained null.
5. **Test suite broken**: 16 of 58 tests failed (7 crashes from removed `oidc.ts`, 9 assertion failures).

**Lockout Protocol**: Vasquez, Bishop, Hudson locked out. No further integration work until defects resolved.

## Repair Cycle (Lockout Protocol Active)

### Hudson: Vite Proxy Repair
- Added proxy coverage for `/admin`, `/auth`, `/signin-oidc-admin`, `/signout-callback-oidc-admin`.
- Set `changeOrigin: false` to preserve browser host for cookie scope alignment.

### Vasquez: Frontend Path & CSRF Repair
- Updated `bff.ts` constants to match backend canonical paths.
- Changed `returnTo` query param to `returnUrl`.
- Rewrote CSRF acquisition: frontend now calls `GET /admin/session/csrf` and reads header name from response.
- Updated `axiosClient.ts` request interceptor to use backend-provided header name dynamically.
- Fixed `AuthContext.tsx` callback logic to handle recovered sessions correctly.

### Bishop: Test Suite Repair
- Rewrote `FrontendAuthContractTests` to target mixed-auth tree (opplat-react = OIDC, opplat-admin = BFF session).
- Un-skipped and rewrote `AdminBffSessionContractTests` to match actual implementation.
- Updated `KeycloakRealmContractTests` to validate BFF env config, not OIDC client config.
- Result: 50 tests pass, 0 skip, 0 fail.

### Ripley: Re-Review & Lockout Lifted
- Verified all five defects resolved.
- Confirmed 50/50 tests green.
- Approved migration; lifted lockout for team.

## Final Validation

**Backend:**
```
dotnet build .\opplat.sln -nologo --tl:off -v minimal ✅
dotnet test test\Opplat.MainApp.Test\Opplat.MainApp.Test.csproj -nologo --tl:off -v minimal ✅
  → 50 tests pass, 0 skip, 0 fail
  → Auth-filtered suite clean
```

**Frontend:**
```
cd src\opplat-admin && npm run lint ✅
npm run build ✅
```

## Non-Blocking Follow-Ups (Ripley)

1. **Duplicate session endpoint**: Backend exposes both `/admin/session` and `/admin/session/current-user`. Frontend only calls `/current-user`. Consider removing alias in cleanup pass.
2. **Deferred `/auth/bff/admin/switch-tenant`**: Approved endpoint not yet implemented; track as follow-up when tenant switching needed.
3. **In-memory ticket store**: `MemoryCacheTicketStore` sufficient for dev/staging. For multi-instance production, requires Redis/SQL.
4. **MimeKit vulnerability warning**: `NU1902` advisory on MimeKit 4.10.0 (moderate, unrelated to auth). Upgrade independently.

## Architecture Validated

✅ **PolicyScheme routing** — Bearer header detection, path-based cookie/JWT routing  
✅ **Cookie configuration** — HttpOnly, SameSite=Lax, server-side ticket store, sliding expiration  
✅ **OpenIdConnect configuration** — Code+PKCE, confidential client, claims normalization  
✅ **Antiforgery middleware** — Skips safe methods, skips Bearer, scoped to `/admin` + logout  
✅ **CORS tightening** — Origins configured with credentials allowed  
✅ **Claims normalization** — Provider-neutral, handles Keycloak realm_access  
✅ **Frontend AuthContext pattern** — Session-based, no JS-accessible tokens  
✅ **Frontend ProtectedRoute** — Correct error-vs-auth priority  
✅ **Admin API layer** — `withCredentials: true`, tenant header injection, 401 redirect  

## Outcome

**Admin-first BFF migration complete.** `opplat-admin` now uses server-backed session auth. `opplat-react` remains on browser-managed OIDC (migration pending). Backend dual-auth coexistence (cookie + JwtBearer) preserves API compatibility. All tests green. Ready for merge and production deployment.

---

**Session Coordinator:** Scribe  
**Date:** 2026-03-22  
**Status:** ✅ COMPLETED  
