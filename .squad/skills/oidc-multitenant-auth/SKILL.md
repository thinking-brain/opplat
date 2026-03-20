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

## Anti-Patterns
- **Don't** issue JWTs from the backend when using an external IdP
- **Don't** use `SymmetricSecurityKey` with OIDC — let the IdP manage signing keys
- **Don't** use `@auth0/auth0-react` if you need Keycloak compatibility — use `react-oidc-context`
- **Don't** store passwords in ASP.NET Identity when using external IdP — passwords live in the IdP

## References
- Finbuckle.MultiTenant 7.0.1 docs
- Microsoft.AspNetCore.Authentication.JwtBearer OIDC discovery
- react-oidc-context / oidc-client-ts
