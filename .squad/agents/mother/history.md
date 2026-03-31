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

