# Session Log — Admin Callback Fix — 2026-03-21T13-48-01Z

## Manifest
- **Vasquez** — Fixed admin SPA post-login callback handoff
- **Hicks** — Verified/aligned admin role and claim contract
- **Bishop** — Validated post-login regression and added coverage

## Problem Statement
Elvis reported that the admin SPA successfully redirected from Keycloak but remained stuck on the `/auth/callback` loading screen with the message:
```
Completando inicio de sesión
Validando permisos y recuperando la sesión administrativa.
```

The backend auth alignment was correct (SuperAdmin role seeding, audience/issuer), but the frontend callback handoff had two seams in failure:

1. **Callback URL rewrite issue (Vasquez):** The callback handler used `window.history.replaceState()` to update the URL, but this does not notify `BrowserRouter` of the route change, leaving React Router still viewing the active route as `/auth/callback`.

2. **Claim shape mismatch (Hicks):** Keycloak can emit role claims in nested or flat dotted shapes depending on the library/runtime path, and both backend and frontend claim readers needed to accept both shapes to prevent token processing failures.

3. **Loading gate seam (Bishop):** The auth context loading gate checked `activeNavigator` state, which could linger briefly even after `oidc.user` was restored, blocking the UI from exiting the callback screen.

## Resolutions Applied

### 1. Router Navigation Event (Vasquez)
**Files:** `src/opplat-admin/src/auth/oidc.ts`, `src/opplat-react/src/auth/oidc.ts`

After `window.history.replaceState()`, dispatch a `PopStateEvent` to notify `BrowserRouter`:
```typescript
window.history.replaceState({}, document.title, getReturnTo(user));
window.dispatchEvent(new PopStateEvent('popstate'));
```

This ensures the router re-evaluates the current location and renders the correct component for the return path.

### 2. Claim Shape Normalization (Hicks)
**Backend Files:** `src/Opplat.MainApp/Auth/AuthClaimTypes.cs`, `src/Opplat.MainApp/Auth/OidcClaimsTransformation.cs`

**Frontend Files:** `src/opplat-admin/src/auth/claims.ts`, `src/opplat-react/src/auth/claims.ts`

Both backend and frontend now normalize Keycloak role claims to handle:
- Nested structure: `realm_access.roles`, `resource_access.{client}.roles`
- Flat dotted structure: `realm_access.roles`, `resource_access.{client}.roles`

The normalization ensures `SuperAdmin` role is consistently readable across all OIDC claim processing paths.

### 3. Loading Gate Refinement (Bishop)
**Files:** `src/opplat-admin/src/auth/AuthContext.tsx`, `src/opplat-react/src/auth/AuthContext.tsx`

Narrowed the loading gate condition to not block on `activeNavigator` state once `oidc.user` and `isAuthenticated` are available. This prevents a restored user from remaining visually stuck behind callback/loading chrome when navigator bookkeeping lingers briefly.

Added source-contract test coverage in `test/Opplat.MainApp.Test/Auth/FrontendAuthContractTests.cs` to prevent regression.

## Validation
- ✅ `dotnet test .\test\Opplat.MainApp.Test\Opplat.MainApp.Test.csproj --filter Auth` — All auth tests pass
- ✅ `dotnet build .\opplat.sln` — Build succeeds
- ✅ `npm --prefix .\src\opplat-admin run build` — Admin frontend builds
- ✅ `npm --prefix .\src\opplat-react run build` — Client frontend builds

## Outcome
✅ Admin SPA post-login callback now completes successfully with proper routing to the dashboard. Users authenticated with `SuperAdmin` role exit `/auth/callback` and land on their intended route.
