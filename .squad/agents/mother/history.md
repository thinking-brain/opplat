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
- Focused build: `dotnet build .\src\Opplat.Api.Admin\Opplat.Api.Admin.csproj -nologo`

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

### 2026-04-01 21:00 — AdminApi Startup Refactoring

- Split monolithic `WebBuilderExtension.cs` (335 lines) into focused extension files by concern:
  - `AdminDatabaseExtensions.cs`: DbContext + Module 3 provisioning services registration
  - `AdminAuthExtensions.cs`: Authentication (JWT, Cookie, OIDC), authorization policies, antiforgery
  - `AdminMediatRExtensions.cs`: MediatR registration with selective handler filtering
  - `AdminCorsExtensions.cs`: CORS policy configuration
- Moved infrastructure services registration to `Opplat.Infrastructure.DependencyInjection.AdminInfrastructureExtensions`: Graph/Keycloak user services, audit logging
- Kept Aspire dev support (`AddOpplatAspireDevelopmentSupport`) in AdminApi since it requires ASP.NET Core dependencies (health checks, forwarded headers) not available in Infrastructure project
- Refactored `WebBuilderExtension.AddAdminApi()` to thin orchestrator (80 lines) that resolves auth config and delegates to focused extensions
- Added EF Core migrations on startup in `Program.cs` with `db.Database.MigrateAsync()` before `app.Run()`
- Created `DevDataSeeder` to seed subscription plans and database instances in Development environment only
- Seeder uses actual entity structure: `PricingMonthly` (not `PricePerMonth`), `ResourceLimits` JSON string (not separate properties), `DatabaseInstance.Identifier` + `ConnectionStringReference` (not individual host/port/name fields)
- Build: 0 errors after fixing `AuthRuntimeConfiguration` type name and matching entity properties

### 2026-04-02 — Database Connection Refactoring

- Created `TenantDatabaseOptions` class to centralize tenant database credentials (Host, Port, Username, Password)
- Renamed `DatabaseInstance.ConnectionStringReference` to `DatabaseName` - now stores only the database name (e.g., "opplat_tenants_db1"), not full connection strings
- Created EF migration `20260402182510_RenameConnectionStringReferenceToDatabaseName` to rename column and reduce max length from 512 to 256
- Updated `DatabaseInstanceConfiguration` with `HasColumnName("database_name")` mapping
- Services now inject `TenantDatabaseOptions` and build connection strings dynamically using `NpgsqlConnectionStringBuilder`
- Pattern: `new NpgsqlConnectionStringBuilder { Host = options.Host, Port = options.Port, Username = options.Username, Password = options.Password, Database = databaseName, SslMode = SslMode.Disable }`
- Updated all SQL queries in `TenantCatalogStore` from `di."ConnectionStringReference"` to `di."DatabaseName"`
- Updated services: TenantSchemaProvisioningService, TenantSchemaMigrationRunner, DatabaseInstanceAutoScalingService, TenantProvisioningCoordinator, TenantProvisioningService, Module2CatalogSync
- Removed `DefaultConnectionString` property from `DatabaseInstanceOptions`
- Updated appsettings.json files: `ConnectionStrings:DefaultConnection` → `ConnectionStrings:AdminDatabase` in AdminApi, added `TenantDatabase` section to both AdminApi and MainApp
- Updated seeders (DevDataSeeder, Module2DataSeeder) to use `DatabaseName` instead of full connection strings
- Fixed AdminPortalMappings: removed `ReadDatabaseName` helper, directly access `DatabaseName` property
- Updated MainApp Program.cs: build default connection string from `TenantDatabaseOptions` instead of reading from ConnectionStrings
- Fixed all test files: updated test data and assertions to use `DatabaseName` instead of `ConnectionStringReference`
- Build succeeded with 0 errors after fixing all references

### 2026-04-02 — AdminApi Startup Migration Runner Fix
 
- Fixed double-rename bug in migration `20260402182510_RenameConnectionStringReferenceToDatabaseName.cs`: removed redundant `migrationBuilder.Sql()` calls from both `Up()` and `Down()` methods that were attempting to rename the same column already handled by `migrationBuilder.RenameColumn()`
- Verified migration runner already exists in `Program.cs` (lines 12-27): applies pending migrations with `db.Database.MigrateAsync()` and seeds dev data via `DevDataSeeder.SeedAsync()`
- No changes needed to `WebApplicationExtensions.cs` - startup flow was already correct
- Build succeeded with 0 errors after fixing migration

### 2026-04-15 — Aspire SPA URL Wiring Review

- `src\Opplat.AppHost\Program.cs` should use Aspire `GetEndpoint(...)` references when feeding `VITE_API_URL`, `VITE_DEV_PROXY_TARGET`, and Admin BFF origin env vars so frontend URL config follows the real backend/app ports instead of duplicated localhost strings.
- Under Aspire local dev, both React apps intentionally keep `VITE_ADMIN_API_URL` empty and rely on Vite same-origin proxying for `/admin` and `/public`; the client app still needs `VITE_API_URL` pointed at MainApp for tenant-scoped APIs.
- `client-app` should `WaitFor(admin-api)` as well as MainApp/Keycloak because registration and subscription-plan calls flow through AdminApi.
- Release builds validated the AppHost/MainApp/AdminApi wiring; the debug AppHost build failure was only a locked `Opplat.AppHost.exe` from an alOpplat.Api.Mainlocal process, not a code regression.Opplat.Api.MainOpplat.Api.Main
- Current `Opplat.UnitTest` failures are baseline noise unrelated to this URL change: several tests assert against moved/deleted files like `src\Opplat.MainApp\Auth\OidcClaimsTransformation.cs`, `src\Opplat.Api.Admin\Endpoints\AdminContracts.cs`, and `test\Opplat.MainApp.Test\Opplat.MainApp.Test.csproj`.

### 2026-04-15 — Scribe Post-Session Tasks
- Orchestration logs created for Bishop, Carl, Mother with agent-specific work summaries and findings
- Session log: `.squad/log/2026-04-15T19-13-58Z-url-config.md` (brief summary of URL config session)
- Decision inbox merged into `.squad/decisions.md`; deduplicated across Bishop/Carl/Mother documents
- Agent history files updated to include team validation notes and decision merge
- Ready for git commit with squad/* staging

### 2026-04-28 — MainApp tenant-context DI fix
Opplat.Api.MainOpplat.Api.Main
- `src\Opplat.MainApp\Endpoints\AccountEndpoints.cs` and `src\Opplat.MainApp\Middleware\TenantValidationMiddleware.cs` must resolve the Finbuckle tenant store through `IMultiTenantStore<AppTenantInfo>`, not the concrete `TenantCatalogStore`.
- `builder.Services.AddMultiTenant<AppTenantInfo>().WithStore<TenantCatalogStore>(...)` registers the store abstraction for Finbuckle resolution; injecting `TenantCatalogStore` directly causes runtime failures like "No service for type 'TenantCatalogStore' has been registered."
- When consuming the abstraction, use Finbuckle store methods `GetByIdentifierAsync` / `GetAsync` (the interface surface), not the concrete helper methods `TryGetByIdentifierAsync` / `TryGetAsync`.
- Added a source-level guard in `test\Opplat.UnitTest\Architecture\MultitenancyConfigurationTests.cs` so future endpoint or middleware changes do not regress back to concrete store injection.

