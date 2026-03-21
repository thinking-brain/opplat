# Vasquez — Entra + Keycloak frontend auth recommendation

## Decision

Keep `react-oidc-context` + `oidc-client-ts` as the standardized SPA auth stack for both Opplat frontends. Do **not** replace it with Entra-specific packages unless the product later needs Microsoft-only capabilities such as Graph-first helpers, broker integration, or Conditional Access / CAE features that standard OIDC libraries cannot cover.

## Why

- Both Azure Entra ID and Keycloak support Authorization Code + PKCE through standard OIDC discovery.
- Opplat already has the right abstraction seams: runtime-config authority/client/scope values, claim normalization helpers, logout fallback handling, and Axios token access.
- Entra-specific SDKs like `@azure/msal-react` would make the production provider easier at the cost of local-dev Keycloak parity and a second auth mental model.

## Frontend contract

Use one provider-agnostic env contract in both SPAs:

```ini
VITE_AUTH_AUTHORITY=
VITE_AUTH_CLIENT_ID=
VITE_AUTH_SCOPE=
VITE_AUTH_AUDIENCE=
VITE_AUTH_USE_AUDIENCE_QUERY_PARAM=
```

Recommended values:

### Keycloak local dev

```ini
VITE_AUTH_AUTHORITY=http://localhost:8180/realms/opplat
VITE_AUTH_CLIENT_ID=opplat-client|opplat-admin
VITE_AUTH_SCOPE=openid profile email offline_access
VITE_AUTH_AUDIENCE=opplat-api
VITE_AUTH_USE_AUDIENCE_QUERY_PARAM=false
```

### Azure Entra production

```ini
VITE_AUTH_AUTHORITY=https://login.microsoftonline.com/<tenant-id>/v2.0
VITE_AUTH_CLIENT_ID=<spa-app-client-id>
VITE_AUTH_SCOPE=openid profile email offline_access api://<api-app-id>/access_as_user
VITE_AUTH_AUDIENCE=
VITE_AUTH_USE_AUDIENCE_QUERY_PARAM=false
```

## Provider differences the frontend must tolerate

1. **Authority**
   - Keycloak: realm URL (`.../realms/opplat`)
   - Entra: tenant v2 authority (`.../<tenant-id>/v2.0`)

2. **Scopes vs audience**
   - Keycloak: standard scopes; backend audience usually comes from realm/client-scope mapping
   - Entra: backend API permission belongs in `scope`, not in a separate `audience` query parameter

3. **Claim shape**
   - Keycloak roles can be nested (`realm_access`, `resource_access`) or flattened
   - Entra roles usually arrive in `roles` or Microsoft role URI claims

4. **Logout**
   - Both can expose RP-initiated logout, but behavior differs by provider session policy and redirect allow-lists
   - Frontend should keep the current fallback: if provider logout fails or no endpoint is exposed, clear local session and redirect to app login

## Implementation guidance

- Keep both SPAs on the same `react-oidc-context` wrapper pattern.
- Prefer passing `UserManagerSettings` into `<AuthProvider {...oidcSettings}>` and let the library own lifecycle.
- Keep claim normalization centralized so roles/tenant identifiers are translated once and reused everywhere.
- Preserve separate SPA client IDs (`opplat-client`, `opplat-admin`) even when both hit the same API to avoid storage collisions and redirect URI sprawl.
