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

For APIs that must support **different providers across environments** (for example Keycloak locally and Azure Entra ID in production), keep the backend on plain ASP.NET Core `AddJwtBearer` plus OIDC discovery. Prefer provider-neutral middleware over Entra-specific helpers such as `Microsoft.Identity.Web` unless the API also needs Entra-only features like downstream Graph token acquisition.

Provider-specific backend packages are usually unnecessary for bearer-token validation itself. Standard OIDC discovery already covers signing keys, issuer metadata, lifetime validation, and audience checks for both Entra and Keycloak.

### Entra vs Keycloak Backend Config
Keep one backend auth shape and vary only provider settings:

- `Authority`
  - Entra: `https://login.microsoftonline.com/<tenant-id>/v2.0`
  - Keycloak: `http://localhost:8180/realms/<realm>`
- `MetadataAddress`
  - Usually `null` for Entra
  - Often useful for Keycloak containers/backchannel discovery
- `Audience`
  - Entra: often `api://<app-id>` or the API app/client ID
  - Keycloak: typically logical audience such as `opplat-api`
- `ValidIssuers`
  - Entra may need multiple accepted issuers (`login.microsoftonline.com/.../v2.0` plus legacy `sts.windows.net/.../`)
  - Keycloak usually needs the single realm issuer
- `AdditionalRoleClaimTypes`
  - Entra: `roles`, optionally `groups`
  - Keycloak: `roles`, `realm_access.roles`, `resource_access.{client}.roles`

### Stable Claim Normalization
Keep provider-specific claim shapes out of authorization policies. Normalize both providers into the same internal contract before `UseAuthorization()`:

- `ClaimTypes.Role`
- `ClaimTypes.Name`
- `tenant_id`
- `tenant_identifier`

That lets `SuperAdmin`, `TenantAdmin`, tenant-validation middleware, and tenant-aware JWT logic survive IdP swaps without rewriting policies.

### Tenant Identity in Token
If you control one provider shape end-to-end, injecting `tenant_id` and `tenant_identifier` into the access token is a clean optimization. Finbuckle resolves tenant from route/header; TenantValidationMiddleware can then cross-check the request tenant against the token claim.

For **Entra in production + Keycloak locally**, treat those tenant claims as optional enrichment, not the portability contract. Keep the authoritative Opplat tenant membership in application data and derive/normalize it from stable identity claims (`sub`, `oid`, email) so the multitenant model does not depend on Entra-specific custom-claim plumbing.

### Runtime Tenant Catalog for Admin CRUD
If admin users must create or deactivate tenants before an EF-backed tenant store exists, swap Finbuckle's static configuration store for a small file-backed `IMultiTenantStore<TTenantInfo>`. Seed it from `appsettings.json` on first run, then persist admin changes in a backend-local JSON catalog so route/header tenant resolution and per-tenant DbContext selection keep working without project-file changes.

### Middleware Order (Critical)
```
UseMultiTenant()       // 1. Resolve tenant from route/header
UseAuthentication()    // 2. Validate OIDC token
TenantValidation       // 3. Cross-check token tenant vs request tenant
UseAuthorization()     // 4. Role/policy checks
```

### Admin BFF Exception for Tenant Validation
On a mixed-mode host where a SuperAdmin portal uses cookie-backed BFF auth but the rest of the API stays tenant-aware, do **not** force tenant claim matching during login/session bootstrap. Skip tenant validation for:

- `/auth/bff/admin/*`
- `/admin/session*`
- OIDC callback endpoints such as `/signin-oidc-admin` and `/signout-callback-oidc-admin`

Keep tenant validation active for tenant-scoped admin operations (for example `/admin/tenants/{tenantIdentifier}/users`) so auth remains simple while later tenant-specific features still have a guarded seam.

### Preferred Admin Return Origin
If the admin BFF accepts multiple local origins, add an explicit `Auth:AdminBff:DefaultOrigin` and use it as the safe fallback redirect target. Do **not** rely on the first `AllowedOrigins` entry, because missing `Origin` headers will otherwise send users to whichever port happens to be listed first.

### Splitting Admin BFF Out of a Shared Host
When a dedicated admin API takes ownership of `/admin/session*` and `/auth/bff/admin/*`, strip the old shared host back to JwtBearer-only auth. Remove the old `MapAdminEndpoints()` call, cookie/OIDC registration, antiforgery middleware, and any admin-only compose/env wiring from the shared host so only one backend owns the admin contract.

### Dual IdP (Dev/Prod)
Configure the same `Auth:Authority` env var to point at Keycloak locally and Auth0 in production. The backend doesn't need provider-specific code — OIDC discovery handles it.

### Entra + Keycloak Standardization
If production uses Azure Entra ID but local development uses Keycloak, keep the stack **provider-neutral**:

- backend: `Microsoft.AspNetCore.Authentication.JwtBearer`
- frontend: `react-oidc-context` + `oidc-client-ts`
- config contract: `Authority`, optional `MetadataAddress`, `Audience`, SPA `clientId`, scopes, redirect URIs

Avoid Entra-only SDKs such as `Microsoft.Identity.Web` or `msal-react` unless you are willing to maintain a separate Keycloak path. They are fine for Entra-only systems, but they increase branching and reduce dev/prod parity in a dual-provider setup.

Standardize Opplat authorization roles as app/realm roles with the **same names in both providers** (`SuperAdmin`, `TenantAdmin`, `TenantUser`). Normalize incoming role claims centrally so Entra `roles`, Keycloak realm roles, and any namespaced claim variants all collapse into the same internal role model.

For business tenancy, prefer **application-owned membership mapping** over IdP-owned tenant claims. Keycloak can emit `tenant_id` / `tenant_identifier` easily, but Entra custom token shaping is a weaker portability seam. Treat those claims as optional enrichment; resolve the authoritative Opplat tenant from application data keyed by stable identity claims such as `sub`, `oid`, or email.

### Frontend
Use `react-oidc-context` (wraps `oidc-client-ts`) — works with any OIDC provider. Avoid provider-specific SDKs to keep dev/prod parity.

When using `react-oidc-context`, prefer the documented provider-owned setup:

```tsx
<AuthProvider {...oidcSettings} onSigninCallback={onSigninCallback}>
  <App />
</AuthProvider>
```

Passing `UserManagerSettings` directly keeps redirect processing and lifecycle management inside the library. If code outside React (for example an Axios interceptor) needs the current token, read and parse the documented `oidc.user:{authority}:{clientId}` browser-storage entry with `User.fromStorageString(...)` instead of sharing a singleton `UserManager` instance across the app.

For Keycloak local dev plus Azure Entra production, keep one shared config contract and only vary env values:

- `VITE_AUTH_AUTHORITY`
- `VITE_AUTH_CLIENT_ID`
- `VITE_AUTH_SCOPE`
- `VITE_AUTH_AUDIENCE` (optional)
- `VITE_AUTH_USE_AUDIENCE_QUERY_PARAM` (default false for Keycloak and Entra)

Provider notes:
- **Entra**: put API permissions directly in `scope` (for example `api://<api-app-id>/access_as_user`) and do not send an `audience` authorize-query param.
- **Keycloak**: request standard OIDC scopes, keep `audience` query params off, and let the realm/client scopes attach the backend audience and custom claims.
- **Auth0/custom OIDC**: only enable `VITE_AUTH_USE_AUDIENCE_QUERY_PARAM=true` when the provider explicitly expects `audience` on the authorize request.

For claim normalization, assume the same logical role can arrive as:
- Entra: `roles` or `http://schemas.microsoft.com/ws/2008/06/identity/claims/role`
- Keycloak: `realm_access.roles`, `resource_access.{client}.roles`, or flattened equivalents

Do not overload Entra's directory `tid` claim as the product tenant identifier. Keep Opplat tenant identity on custom app claims like `tenant_id` / `tenant_identifier` so tenant routing survives provider swaps.

### BFF Upgrade Path for Dual-SPA Apps
When a product has **multiple SPAs talking to the same backend** and browser token storage starts to become the main risk, move auth to a shared BFF rather than teaching each SPA to stay an OIDC client forever.

Preferred target:

- browser apps stop parsing/storing access tokens
- BFF becomes the confidential OIDC client
- browser receives an HTTP-only session cookie plus a normalized `GET /bff/auth/session` payload
- login/logout/refresh move to server endpoints such as `/bff/auth/login`, `/bff/auth/callback`, `/bff/auth/logout`
- tenant-switch flows become explicit server actions (`/bff/auth/switch-tenant`) instead of ad hoc claim parsing in the browser

For Opplat-style admin + tenant SPAs, prefer **one shared BFF/auth session layer** over one BFF per SPA when all of these are true:

- same backend/API estate
- same core role model (`SuperAdmin`, `TenantAdmin`, `TenantUser`)
- same identity providers, just different app routes or UX
- no compliance requirement to isolate the apps operationally

Use separate BFFs only when origins, policies, or provider behavior must diverge materially. Otherwise a shared BFF reduces duplicated auth code, centralizes provider switching (Keycloak local / Entra prod), and gives one place to enforce CSRF, refresh-token rotation, and tenant-membership validation.

### Mixed OIDC + BFF Contract Testing
When a repo is mid-migration, do **not** keep asserting that every SPA uses the same auth stack. Split regression coverage by app:

- keep tenant/client SPA tests pinned to browser OIDC files (`oidc.ts`, redirect callbacks, scope config)
- move admin SPA tests to BFF seams (`/admin/session/*`, `/auth/bff/admin/*`, CSRF bootstrap, cookie-backed Axios, same-origin proxying)
- remove or un-skip placeholder migration tests as soon as the real BFF files land

This avoids false negatives like `FileNotFoundException` against deleted OIDC files while still protecting the new backend/frontend contract.

During migration, keep the backend role/claim normalization seam provider-neutral, but move the **authoritative** tenant-membership decision into application/server logic. Let IdP tenant claims remain enrichment, not the source of truth, so the BFF can survive provider swaps without requiring identical token shaping.

#### ASP.NET Core backend target shape
For ASP.NET Core APIs already using `AddJwtBearer`, the lowest-risk migration path is usually **dual mode**:

- add cookie auth + `AddOpenIdConnect` for browser/BFF sessions
- keep `AddJwtBearer` for service-to-service calls and incremental endpoint migration
- expose a small BFF contract such as `/bff/auth/login`, `/bff/auth/logout`, `/bff/auth/session`, and `/bff/auth/switch-tenant`
- protect cookie-authenticated mutating requests with antiforgery/CSRF validation

Prefer the existing main backend host for the first BFF cut when it already owns tenant resolution, claim normalization, and admin/auth orchestration. Split to a separate BFF service later only if scale, deployment isolation, or origin/policy differences become material.

#### Dual Auth Scheme Selection Pattern
Use `AddPolicyScheme` to route requests to the correct authentication handler at runtime:

```csharp
.AddPolicyScheme("AutoSelect", "Route to Cookie or JWT", options =>
{
    options.ForwardDefaultSelector = context =>
    {
        if (context.Request.Path.StartsWithSegments("/bff"))
            return CookieAuthenticationDefaults.AuthenticationScheme;
        
        var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return JwtBearerDefaults.AuthenticationScheme;
        
        return CookieAuthenticationDefaults.AuthenticationScheme;
    };
});
```

Set `DefaultScheme` to the policy scheme. This avoids per-endpoint `[Authorize(AuthenticationSchemes = "...")]` annotations and keeps the routing concern in one place.

Key rules:
- Cookie `HttpOnly = true`, `SameSite = Lax`, `Secure` per environment
- `SaveTokens = true` on `AddOpenIdConnect` — server holds access/refresh tokens
- Antiforgery required on all mutating cookie-authenticated endpoints
- `AllowAnyOrigin()` is **incompatible** with `AllowCredentials()` — use explicit origin allowlist for cookie endpoints
- `IClaimsTransformation` fires for both schemes, so existing claim normalization works without changes
- Middleware order: `UseAuthentication()` → tenant validation → `UseAntiforgery()` → `UseAuthorization()`

For Opplat-style migrations where only the admin portal moves to BFF first, do **not** forward every non-bearer request to the cookie scheme. Keep JwtBearer as the default for non-admin APIs, then explicitly route `/admin`, `/auth/bff/admin`, and the admin OIDC callback paths to the cookie handler. That preserves legacy tenant/client API behavior while the admin SPA migrates incrementally.

#### Development proxy + HTTPS redirect trap
If a local SPA dev server proxies same-origin BFF traffic to ASP.NET Core over **HTTP**, `UseHttpsRedirection()` can break the BFF bootstrap contract even when production is correct. A route like `/admin/session/current-user` may get turned into an HTTP 307 redirect instead of the intended 401/200 JSON session response, and the proxy/browser layer can surface that as a generic 500.

For admin-first BFF migrations, keep production HTTPS enforcement intact but consider skipping HTTPS redirection for the BFF/auth callback paths in **Development only**:

- `/admin/*`
- `/auth/bff/admin/*`
- `signin-oidc` / signout callback paths used by the BFF

That keeps local same-origin proxying stable without changing the production security posture.

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

When the admin SPA is running from the Vite dev container, an empty `/runtime-config.js` is expected and does **not** prove auth config is missing. In that mode, confirm the live client selection from the running container env (`docker exec opplat-admin-frontend /bin/sh -lc "printenv | sort | grep '^VITE_'"`) and pair it with token-endpoint probes for both `opplat-admin` and `opplat-client` before blaming the realm export.

If the repo contract and container env both point at the right authority/client, add a browser-storage check next. `oidc-client-ts` can leave behind `oidc.user:*` and `oidc.*` entries from earlier client/authority experiments; pruning mismatched entries from both `localStorage` and `sessionStorage` at SPA startup keeps live CORS/OIDC debugging focused on the current runtime rather than stale browser artifacts.

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

When writing regression tests for `react-oidc-context`, also pin the provider configuration seam itself: assert that `AuthProvider` passes `onSigninCallback`, and that the OIDC settings use `WebStorageStateStore({ store: window.localStorage })` (or `globalThis.localStorage`) so callback payload cleanup and restored sessions remain aligned with upstream expectations.

### Multi-SPA Client Configuration
When multiple SPAs (e.g., admin portal + tenant client) share the same backend API, use **separate OIDC clients with the same audience**:
- Each client has distinct `redirectUris` scoped to its port/domain
- Both clients target the same `audience` (backend validates audience, not client ID)
- Separate client IDs prevent `oidc-client-ts` session storage collisions when both apps run simultaneously
- Both clients share identical `defaultClientScopes` for consistent token claims

Do NOT consolidate to a single client just because both apps call the same API. The overhead is minimal, and separation provides clean redirect URI management, session isolation, and future flexibility for per-client role restrictions.

### Admin-First BFF SPA Alignment
For an admin portal that migrates from browser-managed OIDC to a backend-managed cookie session, split the SPA contract into two endpoint families:

- `/auth/bff/admin/*` for login/logout handoff
- `/admin/session/*` for authenticated bootstrap data such as current user and CSRF metadata

Do not keep legacy `/bff/auth/*` paths alive in the SPA once the backend exposes the admin-specific contract; a mixed prefix is easy to ship and guarantees 404s at runtime.

For CSRF specifically, do **not** hardcode a header like `X-XSRF-TOKEN` and do not depend on reading the antiforgery cookie from JavaScript. Prefer a dedicated bootstrap call like `/admin/session/csrf` that returns both the request token and the required header name, then lazily reacquire it before mutating requests if the in-memory copy is missing.

### Cookie BFF Dev Proxy Pattern
For Vite-hosted local development against a cookie/OIDC BFF, proxy every backend path involved in the browser round trip, not just the JSON API:

- `/admin`
- `/auth`
- `/signin-oidc-admin`
- `/signout-callback-oidc-admin`

Keep `changeOrigin: false` for those proxies so the backend sees the SPA host when it computes redirect URIs and sets host-scoped session cookies. If the proxy rewrites the Host header to the backend origin, local login can fail even though backend routes, cookies, and OIDC config are otherwise correct.

### Current-User Endpoint Regression Pattern
When an ASP.NET Core admin BFF endpoint builds its response with an explicit `AuthenticateAsync("AdminCookie")` call, integration tests must register that exact named scheme in the TestServer host, not just a generic default auth scheme.

Recommended regression pair:

- a **sparse-claim** authenticated cookie request against `/admin/session/current-user` that proves the endpoint returns `200 OK` with empty optional fields instead of throwing
- a **happy-path** cookie request that proves the payload includes tenant claims plus session bootstrap metadata like login path, logout path, CSRF header name, and cookie expiration

This catches the real admin SPA bootstrap seam and prevents fake-auth test hosts from masking 500s that only appear once the endpoint tries to read the cookie-auth result directly.


## References
- Finbuckle.MultiTenant 7.0.1 docs
- Microsoft.AspNetCore.Authentication.JwtBearer OIDC discovery
- react-oidc-context / oidc-client-ts
- Keycloak 26.0 realm export/import format
