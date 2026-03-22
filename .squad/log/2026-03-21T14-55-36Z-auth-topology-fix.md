# Session Log: Auth Topology Fix
**Date:** 2026-03-21  
**Timestamp:** 2026-03-21T14-55-36Z  
**Topic:** Authentication Topology and Callback Flow Fix  
**Status:** ✅ COMPLETED

## Overview
Four-agent parallel investigation into Keycloak client architecture and post-login SPA callback failures. All agents confirmed the two-client model is correct and fixed the remaining frontend callback handling issues.

## Agents & Outcomes

### Ripley (Architect)
- **Task:** Adjudicate one client vs two clients in Keycloak
- **Outcome:** ✅ Confirmed two clients are intentional and architecturally sound
- **Key Decision:** Keep `opplat-client` and `opplat-admin` separate for redirect URI isolation and session isolation

### Hicks (Backend)
- **Task:** Inspect backend auth flow and topology impact
- **Outcome:** ✅ Confirmed backend/realm alignment
- **Key Implementation:** Updated `GetAdminUsersQueryHandler` to skip unreachable tenant databases instead of failing bootstrap
- **Finding:** Remaining issue is frontend recovered-session handling, not backend topology

### Vasquez (Frontend)
- **Task:** Trace and fix remaining SPA-side post-login failure
- **Outcome:** ✅ Fixed ProtectedRoute handling in both SPAs
- **Root Cause:** `react-oidc-context` keeps `error` populated after session restored
- **Fix:** Updated both `ProtectedRoute.tsx` to only show auth-error when `error && !isAuthenticated`

### Bishop (QA/Validation)
- **Task:** Validate chosen client model and callback flow
- **Outcome:** ✅ Added regression coverage for both callback pages
- **Implementation:** Extended `FrontendAuthContractTests.cs` with callback and two-client contract tests

## Architecture Decision
**Two-Client Keycloak Model Approved:**
- `opplat-client` for tenant-facing SPA (ports 3000/5173)
- `opplat-admin` for SuperAdmin portal (ports 3001/5174)

Rationale:
1. Distinct redirect URI scoping per application
2. Session isolation (prevents oidc-client-ts storage key collisions)
3. Future per-client role restrictions flexibility

## Root Cause Analysis
- **NOT the two-client model** (as some initially suspected)
- **Frontend callback handling:** Recovered sessions were not preferred over transient shared OIDC errors
- **Bootstrap resilience:** Admin dashboard was failing when any tenant DB became unreachable

## Fixes Applied
1. `ProtectedRoute.tsx` in both admin and client SPAs now properly handle recovered sessions
2. `AuthCallbackPage.tsx` in both SPAs now redirect authenticated users immediately
3. `GetAdminUsersQueryHandler` now resilient to unreachable tenant databases
4. Added comprehensive regression test coverage for callback flow

## Validation
- ✅ All builds passing
- ✅ All tests passing
- ✅ Backend auth tests passing
- ✅ Frontend contract tests guarding callback behavior
- ✅ Two-client realm contract validated

## Constraints for Future Work
- Maintain two separate Keycloak clients (not subject to consolidation)
- Both SPAs must always prefer recovered sessions over transient auth errors on protected routes
- Admin dashboard must handle tenant DB unavailability gracefully
- Regression tests must continue guarding callback return-target handling
