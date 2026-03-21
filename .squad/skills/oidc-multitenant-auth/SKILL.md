# SKILL: OIDC Auth with Finbuckle MultiTenant

## When to Apply
When adding OIDC-based authentication (Auth0, Keycloak, Entra ID, etc.) to an ASP.NET Core app that already uses Finbuckle.MultiTenant for tenant resolution.

## Pattern

### Backend Token Validation
Use `Authority`-based OIDC discovery instead of hardcoded symmetric keys:

```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Auth:Authority"];
        options.Audience = builder.Configuration["Auth:Audience"];
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            NameClaimType = "preferred_username",
        };
    });
```

### Tenant Identity in Token
Configure the IdP to inject `tenant_id` and `tenant_identifier` as custom claims in the access token. Finbuckle resolves tenant from route/header; TenantValidationMiddleware cross-checks against the token claim.

### Runtime Tenant Catalog for Admin CRUD
If admin users must create or deactivate tenants before an EF-backed tenant store exists, swap Finbuckle's static configuration store for a small file-backed `IMultiTenantStore<TTenantInfo>`. Seed it from `appsettings.json` on first run, then persist admin changes in a backend-local JSON catalog so route/header tenant resolution and per-tenant DbContext selection keep working without project-file changes.

### Middleware Order (Critical)
```
UseMultiTenant()       // 1. Resolve tenant from route/header
UseAuthentication()    // 2. Validate OIDC token
TenantValidation       // 3. Cross-check token tenant vs request tenant
UseAuthorization()     // 4. Role/policy checks
```

### Dual IdP (Dev/Prod)
Configure the same `Auth:Authority` env var to point at Keycloak locally and Auth0 in production. The backend doesn't need provider-specific code — OIDC discovery handles it.

### Frontend
Use `react-oidc-context` (wraps `oidc-client-ts`) — works with any OIDC provider. Avoid provider-specific SDKs to keep dev/prod parity.

### SPA Callback Completion
For `BrowserRouter`-based SPAs, treat the post-signin callback as a navigation seam, not just a URL rewrite. After `react-oidc-context` / `oidc-client-ts` finishes processing the signin response, prefer `window.location.replace(returnTo)` over `window.history.replaceState(...)` so the app deterministically leaves `/auth/callback` instead of relying on router-observed history mutation.

When deriving frontend auth loading state, do not let `activeNavigator` alone keep the UI stuck once `oidc.user` or `isAuthenticated` is already present. Gate the loading spinner on unresolved navigation only, for example while navigator state exists **and** no authenticated user has been restored yet.

### Frontend Claim Normalization
Normalize both Auth0 namespaced claims (for example `https://opplat.com/tenant_identifier`) and Keycloak flat claims (`tenant_identifier`) into a single frontend auth model. Keep that logic in one helper so route guards, layouts, and API interceptors all agree on the active tenant and roles.

For Keycloak roles specifically, support **both** token shapes:
- nested objects such as `realm_access.roles` / `resource_access.{client}.roles`
- flat dotted claim keys such as `"realm_access.roles"` / `"resource_access.opplat-admin.roles"`

Different libraries can materialize the same Keycloak token differently, so auth helpers should collect roles from both representations before evaluating `SuperAdmin`, `TenantAdmin`, or `TenantUser`.

### Tenant-Scoped SPA Requests
For tenant-facing SPAs, persist the resolved tenant identifier after login, prefix tenant-scoped relative API URLs with `/{tenant}`, and also send `X-Tenant-Identifier`. The route prefix keeps URLs aligned with Finbuckle's primary strategy while the header provides a fallback for mixed backend surfaces and admin-style calls.

### Admin Bootstrap Resiliency
For cross-tenant admin dashboards, do not make the first post-login bootstrap depend on every tenant database being online. If an aggregate endpoint like `/admin/users` fans out across all tenant connection strings, treat individual tenant database failures as partial-data warnings: log them and skip the broken tenant so a successful `SuperAdmin` login does not look like an OIDC callback failure.

When the product has two distinct SPAs (for example a tenant app and a platform-admin app), keep separate Keycloak public clients if they run on different origins or need independent redirect URI allow-lists. Sharing the same realm roles and API audience does **not** require collapsing those SPAs into one client, and separate client IDs avoid storage-key collisions in browser OIDC libraries.

### Containerized Keycloak Backchannel Discovery
When Keycloak serves browser logins on a public host (for example `http://localhost:8180`) but the API runs in Docker, keep the JWT bearer `Authority` on the **public issuer** and add a separate `MetadataAddress` that points to the internal container URL (for example `http://keycloak:8180/.../.well-known/openid-configuration`).

For Keycloak 26, pair that with `--hostname=<public-url> --hostname-backchannel-dynamic=true` so discovery/JWKS requests from containers can use the private network while tokens still carry the browser-visible issuer. Otherwise, successful browser sign-in can still fail at the API with issuer/discovery mismatches that look like a redirect bug.

### Runtime Config for Static Frontends
If the SPA is shipped from nginx or another static server, write a small `runtime-config.js` file from container env on startup and read that before falling back to `import.meta.env`. This avoids rebuilding the bundle whenever Docker Compose, Keycloak, or Auth0 settings change.

When the same SPA also has a hot-reload or dev-server Compose path, pin the OIDC scope env there too. Nginx runtime-config defaults do not protect Vite dev servers, so an old fallback like `openid profile email roles` can survive and trigger Keycloak `Invalid scopes` errors even after the realm export is fixed.

### Testing During Incremental Migration
When auth/admin implementation is only partially landed, don't block on full end-to-end tests. Add executable unit tests for stable seams like tenant-validation middleware and claim normalization, then add lightweight source/contract guards for route prefixes, policy hooks, and tenant connection-string selection until the full host can be exercised reliably.

For SPA OIDC scope regressions, guard **both** `runtimeConfig.ts` and `auth/oidc.ts`. One file often owns the fallback `VITE_AUTH_SCOPE`, while the other adds "required" scopes; validating only one seam can miss a broken combined request.

### Keycloak Realm Import (Scopes)
Don't repurpose Keycloak built-in scope names (`profile`, `email`, `roles`, `offline_access`) for app-specific mappers. Keep the standard OIDC scopes attached to the SPA clients, and add custom scopes such as `opplat-tenancy` or `opplat-api-audience` for tenant claims and API audience so `scope=openid profile email offline_access` keeps working.

With Keycloak 26 full-model imports, don't assume referenced built-in scopes are auto-created inside a fresh imported realm. If SPA clients list `profile`, `email`, `roles`, `web-origins`, or other built-ins in `defaultClientScopes` / `optionalClientScopes`, explicitly include those client-scope definitions in the realm export or Keycloak can silently ignore the references and later reject the scope request.

For Opplat, treat `openid profile email offline_access` as the intended SPA-requested scope contract for local login flows. Keycloak should still contribute `roles`, `tenant_id`, `tenant_identifier`, and `aud=opplat-api` through default client scopes, so the frontend must not auto-append `roles` on top of that request.

**Critical: Never request `roles` as a scope parameter.** Keycloak does not expose `roles` as a requestable scope — it's a protocol mapper configuration attached via `defaultClientScopes`. Requesting `scope=...roles...` triggers `Invalid scopes` errors. Realm roles flow into tokens automatically via the client's default scope configuration.

When debugging a live CORS or redirect-URI failure after the realm export was fixed in source, distinguish **repo contract** from **running realm state**. A Docker Compose setup that bind-mounts the realm JSON and starts Keycloak with `--import-realm` can still serve an older already-imported realm until the container or realm is recreated, so verify the live client origins in Keycloak before assuming the repo export is wrong.

When debugging scope failures, inspect all four layers together: `runtimeConfig.ts`, `auth/oidc.ts`, `.env.example`, and Docker/runtime-config injection. A mismatch across those layers can produce a runtime scope string that is broader than the one documented in source.

### Keycloak Token Endpoint CORS Diagnostics
For Keycloak SPA flows, a missing `Access-Control-Allow-Origin` on `/protocol/openid-connect/token` is often a **client-selection** clue, not just a missing `webOrigins` entry in source control.

Keycloak computes token-endpoint CORS from the client tied to the token request. In a multi-SPA setup, probe the live endpoint with an `Origin` header and alternate `client_id` values:

```powershell
Invoke-WebRequest -Method Post `
  -Uri 'http://localhost:8180/realms/opplat/protocol/openid-connect/token' `
  -Headers @{ Origin = 'http://localhost:3201' } `
  -ContentType 'application/x-www-form-urlencoded' `
  -Body 'grant_type=password&client_id=opplat-admin&username=x&password=y'
```

If the intended client returns ACAO but another client does not, the browser's CORS failure usually means the SPA is still using stale runtime config, stale browser OIDC state, or an outdated dev server/container — not that the realm export is missing the origin.

Debug in this order:
1. repo realm export (`redirectUris`, `webOrigins`)
2. live Keycloak admin API values
3. live SPA runtime config actually served to the browser
4. browser storage/session state

For Opplat's local Docker setup, a `docker compose up -d --force-recreate keycloak admin-frontend` plus clearing `localhost` site storage is the right first operational reset before making more config edits.

### SPA Callback Navigation
In React Router SPAs, don't assume `window.history.replaceState(...)` is enough to leave an OIDC callback route. `BrowserRouter` may not observe that native history mutation, so the UI can stay stuck on `/auth/callback` even after the session is restored.

Use a **three-layer navigation strategy** in `onSigninCallback`:

```typescript
export const onSigninCallback = (user?: User): void => {
  if (window.self !== window.top) return; // Skip iframe silent renew
  
  const returnTo = getReturnTo(user);
  
  // Layer 1: Update URL immediately
  window.history.replaceState({}, '', returnTo);
  
  // Layer 2: Notify BrowserRouter
  window.dispatchEvent(new PopStateEvent('popstate', { state: {} }));
  
  // Layer 3: Fallback hard redirect if still on callback
  setTimeout(() => {
    if (window.location.pathname.includes('/auth/callback')) {
      window.location.replace(returnTo);
    }
  }, 50);
};
```

For callback page components, use `useEffect` with `useNavigate()` as primary redirect mechanism, with a fallback hard redirect:

```tsx
useEffect(() => {
  if (isAuthenticated && !loading && !hasRedirected.current) {
    hasRedirected.current = true;
    navigate('/', { replace: true });
    setTimeout(() => window.location.replace('/'), 100); // Fallback
  }
}, [isAuthenticated, loading, navigate]);
```

When the callback screen itself reads shared auth error state, guard that UI against already-restored sessions. `react-oidc-context` can surface `error` while keeping `user` / `isAuthenticated` populated, so callback pages should redirect once auth is resolved and only render the error while the user is still unauthenticated.

For regression coverage, source-contract tests can pin three separate callback seams without needing a live IdP: the hard redirect in `auth/oidc.ts`, the safe relative `returnTo` filter, and the callback page's preference for a resolved session over transient shared `error` state.

For admin-style portals where `onSigninCallback` already handles deep-link return, a callback-page fallback redirect to `/` is an acceptable safety net to ensure valid users do not remain stuck on the callback route.

The same recovered-session preference must also exist in protected route guards. If `react-oidc-context` still exposes `error` after `isAuthenticated` becomes true, guard components should only render the auth error UI while the user is still unauthenticated; otherwise a valid login can land on a full-screen error after leaving `/auth/callback`.

In the same recovery window, `react-oidc-context` can restore a non-expired `user` before its derived `isAuthenticated` flag flips to true. Auth context wrappers should therefore expose `isAuthenticated = oidc.isAuthenticated || Boolean(oidc.user && !oidc.user.expired)` so callback completion and protected-route guards do not redirect a valid session back to `/login`.

### Multi-SPA Client Configuration
When multiple SPAs (e.g., admin portal + tenant client) share the same backend API, use **separate OIDC clients with the same audience**:
- Each client has distinct `redirectUris` scoped to its port/domain
- Both clients target the same `audience` (backend validates audience, not client ID)
- Separate client IDs prevent `oidc-client-ts` session storage collisions when both apps run simultaneously
- Both clients share identical `defaultClientScopes` for consistent token claims

Do NOT consolidate to a single client just because both apps call the same API. The overhead is minimal, and separation provides clean redirect URI management, session isolation, and future flexibility for per-client role restrictions.


## References
- Finbuckle.MultiTenant 7.0.1 docs
- Microsoft.AspNetCore.Authentication.JwtBearer OIDC discovery
- react-oidc-context / oidc-client-ts
- Keycloak 26.0 realm export/import format
