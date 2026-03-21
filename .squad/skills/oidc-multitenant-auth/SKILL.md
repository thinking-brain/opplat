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

### Frontend Claim Normalization
Normalize both Auth0 namespaced claims (for example `https://opplat.com/tenant_identifier`) and Keycloak flat claims (`tenant_identifier`) into a single frontend auth model. Keep that logic in one helper so route guards, layouts, and API interceptors all agree on the active tenant and roles.

### Tenant-Scoped SPA Requests
For tenant-facing SPAs, persist the resolved tenant identifier after login, prefix tenant-scoped relative API URLs with `/{tenant}`, and also send `X-Tenant-Identifier`. The route prefix keeps URLs aligned with Finbuckle's primary strategy while the header provides a fallback for mixed backend surfaces and admin-style calls.

### Runtime Config for Static Frontends
If the SPA is shipped from nginx or another static server, write a small `runtime-config.js` file from container env on startup and read that before falling back to `import.meta.env`. This avoids rebuilding the bundle whenever Docker Compose, Keycloak, or Auth0 settings change.

### Testing During Incremental Migration
When auth/admin implementation is only partially landed, don't block on full end-to-end tests. Add executable unit tests for stable seams like tenant-validation middleware and claim normalization, then add lightweight source/contract guards for route prefixes, policy hooks, and tenant connection-string selection until the full host can be exercised reliably.

### Keycloak Realm Import (Scopes)
Don't repurpose Keycloak built-in scope names (`profile`, `email`, `roles`, `offline_access`) for app-specific mappers. Keep the standard OIDC scopes attached to the SPA clients, and add custom scopes such as `opplat-tenancy` or `opplat-api-audience` for tenant claims and API audience so `scope=openid profile email offline_access` keeps working.

For Opplat, the SPA clients should keep the built-in `profile`, `email`, `roles`, and optional `offline_access` scopes, while custom default scopes add `tenant_id`, `tenant_identifier`, and `aud=opplat-api`.

## References
- Finbuckle.MultiTenant 7.0.1 docs
- Microsoft.AspNetCore.Authentication.JwtBearer OIDC discovery
- react-oidc-context / oidc-client-ts
- Keycloak 26.0 realm export/import format
