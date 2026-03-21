# Session Log: CORS Investigation & Admin Auth Fix
**Date:** 2026-03-21  
**Requested by:** elvis.crego  
**Duration:** Sessions 6/6b/7  

## Who Worked
- **Hudson** (DevOps/Infra) — Traced live Keycloak CORS/runtime path
- **Vasquez** (Frontend Dev) — Verified admin SPA auth wiring; fixed callback recovery
- **Hicks** (Backend) — Implemented bootstrap resilience; validated issuer topology
- **Bishop** (QA/Validation) — Confirmed repo contract already covers localhost:3201; validated auth flow
- **Ripley** (Architect) — Consolidated two-client OIDC cost decision; confirmed topology soundness

## What Was Done

### Phase 1: Problem Diagnosis (Session 6)
- **Symptom:** Admin SPA login at `http://localhost:3201` fails with CORS error on token endpoint
- **Investigation:** Hudson traced live Keycloak and confirmed:
  - Realm JSON already includes `opplat-admin` client
  - Client already whitelists `http://localhost:3201` in webOrigins and redirectUris
  - Live Keycloak state partially aligned but inconsistent with repo
- **Finding:** Either Keycloak container is stale (realm not re-imported) or admin dev server is sending wrong client_id at runtime

### Phase 2: Frontend/Backend Alignment (Session 6b)
- **Investigation:** Vasquez + Hicks verified:
  - Admin SPA `runtimeConfig.ts` correctly resolves `client_id=opplat-admin`
  - `oidc.ts` performs standard PKCE code exchange against `http://localhost:8180/realms/opplat`
  - Backend validation uses `audience=opplat-api` (shared by both clients), not client_id
  - Both clients use identical default scopes and token claims
- **Decision:** Keep two-client topology. No architectural defect.
- **Implementation:** Vasquez fixed auth callback recovery:
  - Updated `AuthCallbackPage.tsx` to prefer restored sessions to error state
  - Dispatches PopStateEvent after `replaceState()` to notify BrowserRouter
  - Only shows error UI when truly unauthenticated

### Phase 3: Consolidation & Validation (Session 7)
- **Ripley confirmed:** Two-client OIDC cost is zero in Docker deployment. No infrastructure penalty.
- **Bishop validated:** Repo contract already covers `http://localhost:3201` for `opplat-admin`. Focused auth tests pass.
- **Hudson concluded:** Live Keycloak issue is stale container/realm state, not repo defect.
  - **Recommended fix:** Recreate Keycloak container + clear browser storage

## Decisions Made

1. **Keep two-client topology** (`opplat-client` and `opplat-admin`)
   - Zero infrastructure cost; enables session isolation and independent redirect scoping
   - Covered by regression tests

2. **Frontend callback recovery** as primary fix for post-login failures
   - Treat restored sessions as override for transient OIDC state errors
   - Applied to both admin and client SPAs for consistency

3. **Operational reset checklist** (not repo change)
   - Recreate Keycloak container: `docker compose up -d --force-recreate keycloak admin-frontend`
   - Clear browser localStorage/sessionStorage for both `localhost:3201` and `localhost:8180`
   - Hard-refresh browser or restart tab
   - If realm changes still not picked, full: `docker compose down && docker compose up -d --build --force-recreate`

## Key Outcomes

- ✅ Confirmed repo contract and runtime topology are sound
- ✅ Callback recovery seam fixed in both SPAs
- ✅ Regression test coverage for two-client model in place
- ✅ Cross-tenant admin bootstrap now resilient to one or more offline tenant DBs
- ✅ Issued operational reset procedure (no repo changes needed for Keycloak CORS issue)

## Architectural Learnings

1. **Callback seam:** `react-oidc-context` holds `error` and `isAuthenticated` independently. Always guard auth-error UI with `error && !isAuthenticated`.
2. **Two-client isolation:** Browser OIDC libraries (e.g., `oidc-client-ts`) key sessions by `authority + client_id`. Separate clients for separate SPAs avoids collisions.
3. **Keycloak stale state:** Docker `start-dev --import-realm` imports the JSON on first boot, but existing persisted realms survive container recreation unless Keycloak data volume is removed. For full reset, delete the volume.
4. **Public issuer vs. backchannel discovery:** Browser clients see the public Keycloak URL; backend can discover metadata from internal Docker hostname. Hicks implemented split configuration for robustness.

## Files Touched

**Frontend:**
- `src/opplat-admin/src/auth/oidc.ts` — PopStateEvent dispatch
- `src/opplat-admin/src/auth/AuthCallbackPage.tsx` — Session recovery preference
- `src/opplat-react/src/auth/oidc.ts` — Callback consistency
- `src/opplat-react/src/auth/AuthCallbackPage.tsx` — Session recovery preference
- `src/opplat-admin/src/auth/ProtectedRoute.tsx` — Auth error guard

**Backend:**
- `src/Opplat.MainApp/Authentication.cs` — Issuer configuration
- `GetAdminUsersQueryHandler` — Bootstrap resilience

**Tests:**
- `test/Opplat.MainApp.Test/Authentication/FrontendAuthContractTests.cs` — Regression coverage
- `test/Opplat.MainApp.Test/Authentication/AuthCallbackContractTests.cs` — Callback recovery contract

**Configuration:**
- `docker-compose.yml` — Keycloak hostname flags + service dependencies
- `docker/keycloak/opplat-realm.json` — Two clients (unchanged; already correct)

## Recommendations for Operator

1. **If admin login still fails:** Follow Hudson's operational reset checklist (see above)
2. **If CORS persists after reset:** Check Docker logs for realm import errors: `docker compose logs keycloak`
3. **If auth tests fail after merge:** All regression tests should pass; rebuild with `dotnet test`
4. **For future auth changes:** Maintain session-recovery preference in callback pages and always test both SPAs running simultaneously
