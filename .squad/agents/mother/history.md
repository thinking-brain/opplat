# Project Context

- **Owner:** Elvis Crego
- **Project:** opplat — Multi-platform business management system for café and restaurant operations. Multi-tenant SaaS with ASP.NET Core (.NET 10), EF Core, PostgreSQL, Finbuckle.MultiTenant, React 18, TypeScript, MUI, Keycloak (local) / Entra ID (production).
- **Stack:** ASP.NET Core (.NET 10), Entity Framework Core, PostgreSQL, Finbuckle.MultiTenant, SignalR, OIDC, React 18, Vite, TypeScript, Material-UI, Docker, nginx
- **Created:** 2026-03-31

## Key Architecture Facts

- My domain: AdminApi and MainApp endpoint handlers, EF Core models, DbContexts, migrations
- AdminApi: `Features/Admin/Commands/` and `Features/Admin/Queries/` for CQRS-style handlers
- AdminApi DbContexts: `AdminTenantCatalogDbContext` (Module 2+ models), `LegacyTenants` DbSet for backward compatibility
- NpgsqlConnection with SSL mode normalization used across provisioning services
- Build with: `dotnet build .\opplat.slnx -m:1 -v minimal`
- Focused build: `dotnet build .\src\Opplat.AdminApi\Opplat.AdminApi.csproj -nologo`

## Learnings

### Phase 1 — Tenant Command Handlers (2026-xx-xx)

- `TenantProvisioningCoordinator` in `Opplat.Infrastructure.Services` also defines a local `ITenantProvisioningReporter` interface (distinct from the one in `Opplat.Application.Abstractions.Services`). When both namespaces are imported in the same file, `ITenantProvisioningReporter` and `ITenantSchemaMigrationReporter` become ambiguous — qualify with full namespace when both usings are needed.
- `DeactivateTenantCommandHandler` constructor needed `IGraphUserService` (from `Opplat.Application.Abstractions.Identity`) and `ILogger` — primary constructor syntax was abandoned in favour of full constructor to support logging.
- `AdminOnly` policy was intentionally left open (`RequireAssertion(_ => true)`) — now enforces `RequireAuthenticatedUser` + `RequireRole(authOptions.AdminRole)`.
- DI: always register as `AddScoped<IInterface, ConcreteClass>()` for handlers that depend on interface abstractions; registering the concrete alone breaks MediatR DI resolution at runtime.

### Phase 2 — AdminApi DI fix + Self-Registration endpoint (2026-03-31)

- `RegisterServicesFromAssembly` on the full `Opplat.Application` assembly pulls in Sales/Inventory/License/Menus handlers that require repositories never registered in AdminApi, causing a startup `AggregateException`. Fix: keep the AdminApi assembly scan and add a reflection-based namespace filter to selectively register only `Opplat.Application.Features.Admin.*` and `Opplat.Application.Features.Account.*` handlers.
- The namespace filter must register against the concrete MediatR interfaces (`IRequestHandler<,>`, `IRequestHandler<>`, `INotificationHandler<>`) — MediatR resolves handlers by these exact generic definitions.
- `AddKeycloakUserService` is already registered in `WebBuilderExtension.AddAdminApi` — do not double-register.
- The `adminData` group (`/admin` without `RequireAuthorization`) is intentionally open — `GET /admin/subscription-plans` is accessible anonymously for registration wizard consumption.
- `RegisterTenantCommand` lives in `Opplat.Application.Features.Admin.Commands`, follows the same pattern as `CreateTenantCommand`: picks cheapest active plan if none specified, picks DB instance with lowest `CurrentTenantSchemaCount`, normalizes identifier via `AdminPortalMappings.NormalizeTenantIdentifier`.
- Schema provisioning failure after self-registration is non-fatal: log warning, return success. Tenant and user records are already persisted.


### 2026-04-01 19:11 - Entity Configuration Refactoring
- Successfully moved all EF Core entity configuration from data annotations to Fluent API using IEntityTypeConfiguration<T> pattern
- Created 47 configuration files organized by module (Core, Sales, Inventory, Accounting, Administration)
- Fixed GUID identity column issues - UseIdentityByDefaultColumn() only works with integer types in PostgreSQL
- Resolved FK type mismatches in Administration entities by using HasPrincipalKey() for alternate keys (e.g., Tenant.Identifier instead of Tenant.Id)
- Generated migrations for all 4 DbContexts: AdminTenantCatalogDbContext, OpplatDbContext, SalesDbContext, InventoryDbContext
- Applied namespace filters in ApplyConfigurationsFromAssembly to keep DbContext configurations modular and bounded
- Removed all DataAnnotations attributes from 26 entity files while preserving JsonIgnore for serialization

### 2026-04-01 20:02 — TPC Inheritance for BaseEntity

- Made `BaseEntity` abstract
- Updated `User` to inherit `BaseEntity` (removed duplicate `Guid Id`)
- Removed `required string CreatedBy` from `JournalEntry` entity (was shadowing `BaseEntity.CreatedBy`, pre-existing CS0108 warning eliminated)
- Created `Configurations/Common/BaseEntityConfiguration` using `UseTpcMappingStrategy()` (EF Core 10 API — NOT `UseTpc()`) with `gen_random_uuid()` default for `Id`, `CreatedAt`/`ModifiedAt` required, `CreatedBy`/`ModifiedBy` max 256
- Added `Microsoft.EntityFrameworkCore.Relational` as explicit package reference to `Opplat.Infrastructure.csproj` — `UseTpcMappingStrategy()` extension method was not resolvable without it even though Npgsql brings it transitively
- Updated all 4 DbContext `ApplyConfigurationsFromAssembly` filters to include `Common` namespace
- Removed `HasKey(e => e.Id)` from all BaseEntity-inheriting entity configurations (~27 files across Administration, Sales, Inventory, Accounting, Core)
- Additional cleanups in TenantConfiguration: removed `HasMaxLength(128)` on `Id` and `HasDefaultValue(DateTime.UtcNow)` on `CreatedAt` (static startup-time value, incorrect semantics)
- JournalEntryConfiguration: removed `builder.Property(j => j.CreatedBy).IsRequired()` (now handled by BaseEntityConfiguration)
- Deleted and regenerated all 4 migrations: InitialAdministration, InitialMain, InitialSales, InitialInventory
- Build: 0 errors, 36 pre-existing warnings

- Created session log: `.squad/log/2026-04-01T20-02-59Z-tpc-baseentity.md`
- Merged decision from inbox to `.squad/decisions/decisions.md`
- Scribe prepared git commit with comprehensive message documenting all changes
