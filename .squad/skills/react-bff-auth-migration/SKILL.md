# React SPA to BFF auth migration

## When to use this

Use this pattern when a React SPA currently performs OIDC in the browser and you want to move to a Backend-for-Frontend model with HTTP-only cookies and server-managed sessions.

## Goal

Reduce frontend auth complexity by removing browser token handling, then re-bootstrap identity from a same-origin session endpoint.

## Frontend migration checklist

1. Replace browser OIDC login with navigation to a BFF login endpoint.
2. Remove browser token persistence (`localStorage`, `sessionStorage`, `oidc-client-ts` state).
3. Replace callback token processing with either:
   - no SPA callback page at all, or
   - a temporary spinner page that only waits for session restoration.
4. Make `AuthProvider` fetch `GET /bff/auth/session` on startup.
5. Drive route guards from the session payload, not decoded JWT claims.
6. Change Axios/fetch to `withCredentials: true` and remove `Authorization` header injection from the browser.
7. Add CSRF header handling for all mutating requests.
8. Replace frontend logout with `POST /bff/auth/logout`.

## Session payload shape

Have the BFF return a frontend-ready identity object so each app does not re-implement claim parsing:

```json
{
  "isAuthenticated": true,
  "user": {
    "userId": "string",
    "name": "string",
    "email": "string"
  },
  "roles": ["SuperAdmin"],
  "tenantId": "string|null",
  "tenantIdentifier": "string|null",
  "csrfToken": "string"
}
```

## What gets simpler in React

- No silent renew
- No token expiry bookkeeping
- No callback race conditions
- No browser bearer-token leakage
- No JWT claim decoding for route guards

## What gets harder outside React

- Session storage
- CSRF protection
- Downstream token acquisition/proxying
- Cookie policy across local/prod environments

## Opplat-specific guidance

- Pilot the admin app first because it has a smaller auth surface and does not tenant-prefix multiple API clients.
- Keep admin and client auth separation conceptually distinct even under BFF; they still have different routing and authorization expectations.
- If the admin portal does not need tenant context at sign-in time, keep session bootstrap tenant-agnostic and persist any tenant selection as page-level UI state instead of coupling it to auth restoration.
- For the tenant client app, preserve `tenantIdentifier` bootstrap in the session response so existing route/header behavior can be migrated without decoding browser tokens.

## Hosting pattern for cookie auth

- Keep the SPA talking to the BFF through a same-origin seam whenever possible.
- In local development, use a Vite proxy for `/bff/*` plus the protected API routes the app calls directly (for Opplat admin, `/admin/*`).
- In containerized/static hosting, mirror that setup in Nginx or the edge proxy so browser cookies stay first-party.
- If you still need a frontend callback route during rollout, make it a spinner/recovery page that only re-checks `/bff/auth/session` and redirects when the server session appears.

## Testing during incremental rollout

- If backend/frontend BFF work is not fully landed yet, do **not** flip the whole suite red just to describe the target.
- Add executable tests for seams that already exist and must remain stable during migration, such as tenant-isolation headers, route/header alignment, or current authorization boundaries.
- Add clearly skipped source-contract tests for the target BFF behavior (`/bff/auth/login`, `/bff/auth/session`, `/bff/auth/logout`, cookie credentials, CSRF headers, no browser bearer injection) so implementers have precise acceptance criteria ready to activate.
- When protecting login return-target behavior, include at least one executable backend test for `/auth/bff/admin/login` that sets `AuthOptions.AdminBff.AllowedOrigins`, sends an `Origin` header from the intended SPA dev port, and verifies a relative `returnUrl` is expanded against that origin. This catches fallback regressions such as `3201` requests being redirected to `3001`.
