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
- Test: `dotnet test .\test\Opplat.MainApp.Test\Opplat.MainApp.Test.csproj --no-build --logger "console;verbosity=minimal"`
- Frontend validate: `npm run lint && npm run build` in `src\opplat-react` and `src\opplat-admin`

## Learnings

### 2026-04-01 - Entity Configuration Architecture
- Mother refactored the entire persistence layer to Fluent API, moving all EF Core configuration from domain entities to 47 IEntityTypeConfiguration<T> classes
- This removes infrastructure coupling from domain models, enabling cleaner unit testing and better separation of concerns
- DbContexts now apply configurations via ApplyConfigurationsFromAssembly with namespace filters for bounded context isolation
- PostgreSQL has specific limitations: UseIdentityByDefaultColumn() only works with integer types (not GUIDs); resolved via alternate key strategies like HasPrincipalKey()
- FK relationships with type mismatches use HasPrincipalKey() to map to alternate keys (e.g., Tenant.Identifier as string principal key)
