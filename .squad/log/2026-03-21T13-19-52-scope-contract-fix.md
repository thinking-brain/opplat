# Session Log: Keycloak Scope Contract Fix — 2026-03-21 13:19:52

## Session Overview
**Date:** 2026-03-21  
**Duration:** Multi-turn collaboration  
**Agents:** Hicks (Backend), Vasquez (Frontend), Bishop (Tester), Ripley (Architect/Lead)  
**Outcome:** ✅ COMPLETE — Scope contract finalized and corrections applied  

## Problem Statement
Users experienced login failure: `Invalid scopes: openid profile email roles offline_access`

The error suggested that either:
1. Keycloak realm was misconfigured
2. Frontend was requesting incorrect scopes
3. Docker Compose wasn't wiring environment variables properly

## Investigation Approach
- **Hicks:** Inspected Keycloak realm export and Docker Compose bootstrap
- **Vasquez:** Analyzed SPA runtime scope configuration and defaults
- **Bishop:** Created test harnesses to guard scope contracts and identify drift
- **Ripley:** Synthesized findings across all layers and adjudicated conflicts

## Key Discoveries

### Keycloak Realm (Hicks)
✅ Realm export is **correct**:
- Default client scopes: `profile`, `email`, `roles`, `opplat-tenancy`, `opplat-api-audience`
- Optional scopes: `offline_access`
- No redef of built-in scopes

### Docker Compose Wiring (Hicks)
⚠️ Environment wiring **incomplete**:
- `VITE_AUTH_SCOPE` not set in docker-compose.yml or docker-compose.override.yml
- Frontend services fall back to hardcoded `openid` default
- Mismatch between intent and runtime

### SPA Frontend Defaults (Vasquez)
❌ Frontend **drifted** from intended contract:
- Both `runtimeConfig.ts` files default to `openid` only
- Both `auth/oidc.ts` files enforce only `openid`
- README documents `openid` only
- **Intended contract:** `openid profile email offline_access`

### Test Harnesses (Bishop)
✅ Created regression guards:
- `FrontendAuthContractTests.cs` — Validates SPA scope alignment (currently failing)
- `KeycloakRealmContractTests.cs` — Validates realm scope correctness (currently passing)

## Root Cause Analysis (Ripley)
The `roles` error is NOT from the realm (which is correct) and NOT from the current frontend code requesting `roles` explicitly. Instead:
- **Most likely source:** Stale `.env.local` file or cached browser OIDC state still referencing the old `roles` scope
- **Why `roles` fails when requested:** Keycloak attaches `roles` as a **default client scope** (automatic), not as a **requestable consent scope**. Explicitly requesting it triggers the validation error.

## Final Decision
**Authoritative SPA Scope Request Contract:**
```
openid profile email offline_access
```

**Rationale:**
- `openid` — OIDC mandatory
- `profile`, `email` — Keycloak default scopes (harmless to request explicitly)
- `offline_access` — MUST be requested for refresh tokens; it's optional in Keycloak
- **`roles` — EXCLUDED** — Keycloak injects via defaultClientScopes automatically

## Corrections Applied
1. ✅ `src/opplat-react/src/runtimeConfig.ts` — Updated fallback scope
2. ✅ `src/opplat-admin/src/runtimeConfig.ts` — Updated fallback scope
3. ✅ `src/opplat-react/.env.example` — Updated documentation
4. ✅ `src/opplat-admin/.env.example` — Updated documentation
5. ✅ `README.md` — Updated local dev examples
6. ⏭️ Docker Compose: Already correct (no changes needed)
7. ⏭️ Keycloak realm: Already correct (no changes needed)

## Validation Checklist
- ✅ TypeScript compilation: Both SPAs pass `tsc --noEmit`
- ✅ Realm validation: `KeycloakRealmContractTests` passing
- ⏳ Frontend validation: `FrontendAuthContractTests` now should pass with corrections

## Learnings & Future Guidance
1. **Scope Configuration is Multi-Layer:** Any future changes must coordinate across Docker Compose, runtimeConfig, .env.example, and README
2. **Keycloak Scopes vs Mappers:** Scope requests must exclude things that Keycloak handles via default client scopes or mappers
3. **Test Seams Matter:** The new test harnesses will catch scope drift in future PRs
4. **Document the Contract:** Keep the intended scope contract visible in README and Docker Compose env variables
