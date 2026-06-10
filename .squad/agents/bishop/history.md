# Project Context

- **Owner:** Elvis Crego
- **Project:** opplat — Multi-platform business management system for café and restaurant operations. Multi-tenant SaaS with ASP.NET Core (.NET 10), EF Core, PostgreSQL, Finbuckle.MultiTenant, React 18, TypeScript, MUI, Keycloak (local) / Entra ID (production).
- **Stack:** ASP.NET Core (.NET 10), Entity Framework Core, PostgreSQL, Finbuckle.MultiTenant, SignalR, OIDC, React 18, Vite, TypeScript, Material-UI, Docker, nginx
- **Created:** 2026-03-31

## Key Architecture Facts

- AdminApi owns the central tenant catalog and provisioning endpoints
- MainApp owns per-request tenant resolution and tenant-scoped runtime APIs
- Two frontend SPAs: `src/opplat-react/` (tenant-facing), `src/opplat-admin/` (admin portal)
- Modules 1-3 (Identity, Schema, Provisioning) are largely implemented. Module 4+ features have stubs.
- Tenant schema isolation via per-tenant PostgreSQL schemas
- DatabaseInstanceAutoScalingService auto-provisions overflow instances when MaxTenantsPerInstance (default 100) is reached
- Auth: provider-neutral via AuthClaimTypes + OidcClaimsNormalizer; Keycloak local, Entra ID production
- Build: `dotnet build .\opplat.slnx -m:1 -v minimal`
- Test: `dotnet test .\test\Opplat.Api.Main.Test\Opplat.Api.Main.Test.csproj --no-build --logger "console;verbosity=minimal"`
- Frontend validate: `npm run lint && npm run build` in `src\opplat-react` and `src\opplat-admin`

## Learnings

### 2026-04-01 - Entity Configuration Architecture
- Mother refactored the entire persistence layer to Fluent API, moving all EF Core configuration from domain entities to 47 IEntityTypeConfiguration<T> classes
- This removes infrastructure coupling from domain models, enabling cleaner unit testing and better separation of concerns
- DbContexts now apply configurations via ApplyConfigurationsFromAssembly with namespace filters for bounded context isolation
- PostgreSQL has specific limitations: UseIdentityByDefaultColumn() only works with integer types (not GUIDs); resolved via alternate key strategies like HasPrincipalKey()
- FK relationships with type mismatches use HasPrincipalKey() to map to alternate keys (e.g., Tenant.Identifier as string principal key)

### 2026-04-02 — SPA API URL Simplification
- Root cause of client-app failure: `VITE_ADMIN_API_URL` defaulted to port 5160 (wrong) — AdminApi runs on 8084. CORS also blocked port 3200.
- Both SPAs now use Vite dev proxy for AdminApi routes (`/admin`, `/public`). `VITE_ADMIN_API_URL` is empty (same-origin).
- opplat-react uses cross-origin absolute URLs only for MainApp (port 8080) because dynamic `/{tenantId}/*` paths can't be statically proxied.
- opplat-admin uses cookie-based BFF auth — same-origin proxy is mandatory, not optional.
- Removed redundant env vars from Aspire: `VITE_AUTH_API_URL`, `VITE_SALES_API_URL`, `VITE_INVENTORY_API_URL`, `VITE_AUTH_USE_AUDIENCE_QUERY_PARAM`. These either defaulted to `VITE_API_URL` or were auto-computed.
- Key files: `src/Opplat.AppHost/Program.cs` (Aspire env vars), `src/opplat-react/vite.config.ts` (proxy), `src/opplat-react/src/runtimeConfig.ts` (defaults)
- AdminApi CORS origins are in `Auth__AdminBff__AllowedOrigins__N` — defined in Aspire Program.cs
- Decision written to `.squad/decisions/inbox/bishop-url-config.md`

### 2026-04-15 — Team Validation & Decision Merge
- Carl (QA) confirmed regression surface: frontend builds green, backend unit failures are baseline noise
- Mother verified Aspire as source of truth for URL values; confirmed client-app must `WaitFor(admin-api)` for registration/catalog calls
- Decision merged from inbox → `.squad/decisions.md`; orchestration logs created for Bishop, Carl, Mother
- Session log: `.squad/log/2026-04-15T19-13-58Z-url-config.md`
