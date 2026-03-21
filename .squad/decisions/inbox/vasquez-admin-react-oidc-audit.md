## Decision

Align the admin SPA with the documented `react-oidc-context` pattern by passing `oidc-client-ts` settings directly to `<AuthProvider>` and letting the library create/manage its own `UserManager`.

## Why

The admin app was working around the library with a shared singleton `UserManager` that was also imported by Axios helpers. Official `react-oidc-context` guidance expects the provider to own redirect processing and session state, while non-React consumers should read the persisted `oidc.user:{authority}:{clientId}` record from browser storage.

## Consequences

- Keeps our callback handling on the supported `onSigninCallback` seam.
- Preserves existing SuperAdmin gating, callback recovery, and stale-storage cleanup.
- Reduces the risk of custom manager wiring fighting the provider lifecycle.
