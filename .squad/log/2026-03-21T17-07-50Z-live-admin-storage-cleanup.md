# Session Log: Live Admin OIDC Storage Cleanup

**Date:** 2026-03-21T17:07:50Z  
**Agents:** Hudson (Infrastructure), Vasquez (Frontend), Bishop (Testing)  
**Topic:** Admin SPA localStorage/sessionStorage pruning for stale oidc-client-ts entries

## Summary

Completed operational diagnosis of live admin OIDC auth flow and implemented runtime cleanup for stale browser storage entries.

**Hudson's finding:** Live Keycloak realm validation showed `opplat-admin` client already includes `http://localhost:3201` in both `webOrigins` and `redirectUris`. Direct token probe with `Origin: http://localhost:3201` and `client_id=opplat-admin` returns valid `Access-Control-Allow-Origin` header. Same probe for `client_id=opplat-client` returns no ACAO, proving the observed browser CORS error matches stale frontend state (wrong client ID or old storage entries) rather than realm configuration.

**Vasquez's implementation:** Added `clearStaleOidcStorage()` utility in `src/opplat-admin/src/auth/oidc.ts` to prune entries from both `localStorage` and `sessionStorage` before `UserManager` initialization. Cleanup removes:
- Stale `oidc.user:*` entries for mismatched authority/client pairs
- Orphaned `oidc.*` state payloads from previous client/authority experiments
- Keeps only entries matching active VITE_AUTH_AUTHORITY + VITE_AUTH_CLIENT_ID

**Bishop's verification:** Confirmed both admin app lint and build pass post-fix.

## Decisions Made

1. **Not a repo-config defect:** Live Keycloak state is correct. Treat as operator-runbook issue (reset container state + clear browser cache).
2. **Startup pruning is hygiene, not bug fix:** Removes noise from browser debugging; doesn't change backend behavior or scope contract.

## Validation

- ✅ `src/opplat-admin/src/auth/oidc.ts` builds without errors
- ✅ Admin lint passes  
- ✅ Live token probe confirms Keycloak realm origin allowlist includes `http://localhost:3201`

## Next Steps

Before further live debugging:
1. Recreate Keycloak and admin-frontend containers  
2. Clear browser site storage for `http://localhost:3201` and `http://localhost:8180`  
3. Retry login; observe browser/network state for unexpected client IDs or auth errors
