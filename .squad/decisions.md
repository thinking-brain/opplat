# Squad Decisions

## Active Decisions

### Phase 1 — Tenant Command Handlers (Mother)

**Date:** Phase 1 implementation

#### 1. TenantProvisioningCoordinator interface and DI registration
- Implemented `ITenantProvisioningCoordinator` on the concrete class; fixed DI to use `AddScoped<ITenantProvisioningCoordinator, TenantProvisioningCoordinator>()`.
- Both `CreateTenantCommandHandler` and `ProvisionTenantSchemaCommandHandler` depend on the interface, so the scoped interface registration is required.
- Resolved ambiguity on `ITenantProvisioningReporter` by fully qualifying the singleton registrations with `Opplat.Infrastructure.Services.` prefix.

#### 2. AdminOnly policy enforcement
- Changed `RequireAssertion(_ => true)` → `RequireAuthenticatedUser().RequireRole(authOptions.AdminRole)` to enforce SuperAdmin role gating.

#### 3. UpsertTenantRequest gains SubscriptionPlanId
- Added `public Guid? SubscriptionPlanId { get; set; }` (nullable for backward compatibility).
- `CreateTenantCommandHandler` falls back to cheapest active plan when omitted.

#### 4. CreateTenantCommandHandler implementation
- Normalises identifier/name/databaseName via `AdminPortalMappings`.
- Defaults `DatabaseSchema` to `tenant_{identifier}` if blank.
- Guards on duplicate identifier.
- Resolves subscription plan: explicit ID → cheapest active plan → throws if none.
- Picks database instance with lowest `CurrentTenantSchemaCount`.
- Saves `Tenant` row, then calls `EnsureTenantProvisionedAsync`.

#### 5. UpdateTenantCommandHandler implementation
- Looks up tenant by normalised identifier; throws `KeyNotFoundException` if missing.
- Updates Name, DatabaseName, optionally DatabaseSchema and SubscriptionPlanId.
- Sets `ModifiedAt`, saves, returns DTO.

#### 6. DeactivateTenantCommandHandler rewritten
- Added `IGraphUserService` and `ILogger` to constructor.
- Idempotent: returns if tenant already `Inactive`.
- Iterates active `TenantUsers`, calls `DisableUserAsync` for each with Entra OID.
- Marks users inactive with `DeactivatedAt` timestamp.
- Sets tenant `Status = Inactive`, `InactivatedAt`, `ModifiedAt`.

#### 7. TenantUser.TenantId annotation cleanup
- Removed `[MaxLength(128)]` from `Guid` field (misleading; EF ignores it on value types).

---

### Phase 1 — Tenant User Commands (Cosmo)

**Date:** 2026-01-01

#### 1. Replace UserManager with AdminTenantCatalogDbContext + IGraphUserService
- Rewrote `CreateTenantUserCommandHandler` and `UpdateTenantUserCommandHandler` to use:
  - `AdminTenantCatalogDbContext` for persistence (the `tenant_users` catalog table)
  - `IGraphUserService` for identity provider lifecycle (create, enable, disable)
- The catalog is the single source of truth for tenant user records. Graph API handles Entra identity lifecycle.

#### 2. AuditLog failures must not fail business operations
- `AuditLogService.LogAsync` wraps all DB writes in try/catch and only logs errors.
- Audit logging is observability infrastructure; it must never cause a user-facing 500 error.

#### 3. Seat limit check uses SubscriptionPlan.MaxActiveUsers
- Count `TenantUsers.Count(u => u.IsActive)` against `SubscriptionPlan.MaxActiveUsers`.
- If at or over limit, return `null` (caller maps to 409-style response).
- Check skipped if `SubscriptionPlan` is null (no plan = no cap enforced at this layer).

#### 4. TenantUser.Id is the public UserId for update commands
- Parse `UserId` with `Guid.TryParse` in the handler.
- If parse fails, return `false` (bad input). Avoids exception-driven control flow.

#### 5. PrimaryAdminGuard as a static utility class
- Implemented as a `static class` with `async Task` methods that throw `InvalidOperationException` on violation.
- Callers catch the exception and convert to appropriate HTTP response.
- Placed in `Opplat.Application.Services` namespace for reusability.

---

### Phase 2 — AdminApi DI Fix & Self-Registration Endpoint (Mother)

**Date:** 2026-03-31

#### 1. AdminApi startup DI crash fix
- Replace `RegisterServicesFromAssembly` on full `Opplat.Application` assembly with reflection-based namespace filter.
- Selectively register only handlers from `Opplat.Application.Features.Admin.*` and `Opplat.Application.Features.Account.*`.
- Prevents loading Sales/Inventory/License/Menus handlers whose constructor dependencies are never registered in AdminApi.
- Files changed: `src\Opplat.AdminApi\Extensions\WebBuilderExtension.cs`.

#### 2. Public /public/register self-registration endpoint
- Added `RegisterTenantCommand` + `RegisterTenantCommandHandler` following the `CreateTenantCommand` orchestration pattern.
- Keycloak failure returns a failure result (no throw). Schema provisioning failure is non-fatal (log + continue).
- DTOs: `TenantRegistrationRequest` and `TenantRegistrationResult` added as records to `AdminDtos.cs`.
- Endpoint: `POST /public/register` on new `publicApi` group (`/public`) with `.AllowAnonymous()`.
- Existing `GET /admin/subscription-plans` on `adminData` group is already anonymous.
- Files changed:
  - `src\Opplat.Application\Dtos\AdminDtos.cs`
  - `src\Opplat.Application\Features\Admin\Commands\RegisterTenantCommand.cs` (new)
  - `src\Opplat.AdminApi\Endpoints\AdminEndpoints.cs`

#### 3. IKeycloakUserService DI registration
- No change needed. Already registered in `WebBuilderExtension.AddAdminApi`.

---

### Phase 2 — Registration Wizard Frontend (Liz)

**Date:** 2026-03-31

#### 1. MUI Stepper for step navigation
- Used MUI `Stepper` + `Step` + `StepLabel` components.
- Simple `useState` for step index — no external state management needed for 3-step form.

#### 2. Auto-slugify tenantIdentifier
- Implemented `slugify()` to auto-derive tenant identifier from businessName (lowercase, spaces→hyphens, strip non-alphanumeric).
- User can manually override; once manually edited, auto-generation stops.

#### 3. Username derivation
- Username auto-derived on submit as `${tenantIdentifier}-${firstName.toLowerCase()}`.
- Not shown to user — reduces form complexity.

#### 4. API clients for registration
- Used `adminPublicAxiosClient` (no auth headers) for both `GET /admin/subscription-plans` and `POST /public/register`.
- Correct since these are pre-auth endpoints.

#### 5. Per-step validation
- Each step has its own validity check. "Next" button disabled until current step is valid.
- Prevents advancing with incomplete data.

#### 6. LoginPage UX
- Added Divider + "Create Account" outlined button to LoginPage for obvious registration path.
- Used `useNavigate` for programmatic navigation.

---

### 2026-04-02 — Centralized Tenant Database Configuration (Mother)

**Date:** 2026-04-02  
**Status:** Implemented  
**Decider:** Mother (Elvis Crego)  
**Context:** Database connections refactoring

#### Decision
Refactored database connection management to use centralized `TenantDatabaseOptions` configuration class instead of storing full connection strings in the database.

#### Rationale — Problem
- **Security Risk**: Full connection strings (including credentials) stored in `DatabaseInstance.ConnectionStringReference`
- **Configuration Inflexibility**: Changing database credentials required updating every database instance record
- **Duplication**: Same credentials repeated across multiple connection strings
- **Separation of Concerns**: Connection credentials are infrastructure configuration, not catalog data

#### Solution
1. **Created TenantDatabaseOptions**: Stores Host, Port, Username, Password in appsettings
2. **Changed DatabaseInstance Schema**: `ConnectionStringReference` → `DatabaseName` (stores only DB name)
3. **Dynamic Connection String Building**: Services build connection strings from options + database name + schema
4. **Centralized Configuration**: One place to manage tenant database credentials

#### Benefits
- **Security**: Credentials in configuration (can use environment variables, Key Vault) not in database
- **Maintainability**: Change credentials in one place (appsettings) instead of updating DB records
- **Clarity**: DatabaseInstance now stores what it owns (database name), not infrastructure config
- **Flexibility**: Easy to integrate with secret management systems

#### Trade-offs
- **Migration Required**: Existing deployments need EF migration to rename column
- **Breaking Change**: Services reading ConnectionStringReference need updates
- **More Configuration**: appsettings files need TenantDatabase section
- **Code Complexity**: Connection strings built dynamically instead of read directly

#### Connection Building Pattern
```csharp
// Tenant DB: options + catalog data + schema
var connectionString = new NpgsqlConnectionStringBuilder
{
    Host = tenantDbOptions.Host,
    Port = tenantDbOptions.Port,
    Username = tenantDbOptions.Username,
    Password = tenantDbOptions.Password,
    Database = databaseInstance.DatabaseName,
    SslMode = SslMode.Disable
}.ConnectionString;
```

#### Affected Components
- AdminApi: Provisioning services, catalog sync, seeders
- MainApp: DbContext registration, tenant provisioning
- Infrastructure: All services that connect to tenant databases
- Application: TenantCatalogStore (SQL queries updated)
- Tests: Test data and assertions updated

#### Migration Path
1. Deploy code changes
2. Run EF migration: `20260402182510_RenameConnectionStringReferenceToDatabaseName`
3. Update appsettings.json with TenantDatabase section
4. Restart services

---

## Governance

- All meaningful changes require team consensus
- Document architectural decisions here
- Keep history focused on work, decisions focused on direction

