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

When debugging scope failures, inspect all four layers together: `runtimeConfig.ts`, `auth/oidc.ts`, `.env.example`, and Docker/runtime-config injection. A mismatch across those layers can produce a runtime scope string that is broader than the one documented in source.

### SPA Callback Navigation
In React Router SPAs, don't assume `window.history.replaceState(...)` is enough to leave an OIDC callback route. `BrowserRouter` may not observe that native history mutation, so the UI can stay stuck on `/auth/callback` even after the session is restored.

If your OIDC library callback runs outside React, follow `replaceState` with a router-observable navigation signal such as dispatching `popstate`, using router navigation from a component, or falling back to `window.location.replace(...)`. This is especially relevant for `react-oidc-context` / `oidc-client-ts` callback hooks.


## References
- Finbuckle.MultiTenant 7.0.1 docs
- Microsoft.AspNetCore.Authentication.JwtBearer OIDC discovery
- react-oidc-context / oidc-client-ts
- Keycloak 26.0 realm export/import format
