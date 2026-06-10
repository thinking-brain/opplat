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
- Files changed: `src\Opplat.Api.Admin\Extensions\WebBuilderExtension.cs`.

#### 2. Public /public/register self-registration endpoint
- Added `RegisterTenantCommand` + `RegisterTenantCommandHandler` following the `CreateTenantCommand` orchestration pattern.
- Keycloak failure returns a failure result (no throw). Schema provisioning failure is non-fatal (log + continue).
- DTOs: `TenantRegistrationRequest` and `TenantRegistrationResult` added as records to `AdminDtos.cs`.
- Endpoint: `POST /public/register` on new `publicApi` group (`/public`) with `.AllowAnonymous()`.
- Existing `GET /admin/subscription-plans` on `adminData` group is already anonymous.
- Files changed:
  - `src\Opplat.Application\Dtos\AdminDtos.cs`
  - `src\Opplat.Application\Features\Admin\Commands\RegisterTenantCommand.cs` (new)
  - `src\Opplat.Api.Admin\Endpoints\AdminEndpoints.cs`

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

### 2026-04-15 — Simplify SPA API URL Configuration (Bishop, Carl, Mother)

**Date:** 2026-04-15  
**Status:** Implemented  
**Deciders:** Bishop, Carl (QA), Mother  
**Context:** Client-app unable to reach AdminApi; registration and subscription plan fetching broken

#### Problem

1. **Wrong default port:** `runtimeConfig.ts` defaulted `VITE_ADMIN_API_URL` to `http://localhost:5160`; AdminApi runs on 8084
2. **Missing CORS origin:** AdminApi allowed 3101/3201/5174 but not 3200 (client-app)
3. **Redundant env vars:** Aspire set `VITE_AUTH_API_URL`, `VITE_SALES_API_URL`, `VITE_INVENTORY_API_URL` for non-existent separate backends

#### Decision — Strategy per SPA

| SPA | API Connection | Why |
|-----|----------------|-----|
| **opplat-admin** | Vite dev proxy (same-origin) | Cookie-based BFF auth; same-origin required |
| **opplat-react** | Mixed: absolute URL for MainApp, Vite proxy for AdminApi | MainApp uses dynamic `/{tenantId}/*` paths (can't proxy statically); AdminApi uses fixed `/admin`, `/public` prefixes (proxy cleanly) |

#### Implementation

**Aspire env vars** (AppHost):
- Keep: `VITE_API_URL` (MainApp on 8080)
- Add: `VITE_DEV_PROXY_TARGET` (AdminApi on 8084)
- Add: `VITE_ADMIN_API_URL` (empty string → same-origin proxy)
- Remove: `VITE_AUTH_API_URL`, `VITE_SALES_API_URL`, `VITE_INVENTORY_API_URL`, `VITE_AUTH_USE_AUDIENCE_QUERY_PARAM`

**Vite proxy rules:**
- `opplat-react/vite.config.ts` and `opplat-admin/vite.config.ts`: added `/admin` and `/public` proxy routes
- Empty `VITE_ADMIN_API_URL` forces same-origin; requests routed through Vite dev server to AdminApi

**Config defaults** (`runtimeConfig.ts`):
- `adminApiUrl`: `'http://localhost:5160'` → `''` (empty = same-origin)
- Others default to `VITE_API_URL` (backward compatible)

#### Trade-offs

- **MainApp calls remain cross-origin in dev** — fixing would require regex proxying of dynamic paths (fragile, over-engineered)
- **Production:** Both SPAs behind reverse proxy (nginx/Front Door) → same-origin anyway
- **Future:** If Sales/Inventory split to separate services, re-add env vars and point to new ports; runtimeConfig already supports this

#### Files Changed

- `src/Opplat.AppHost/Program.cs`
- `src/opplat-react/vite.config.ts`
- `src/opplat-react/src/runtimeConfig.ts`
- `src/opplat-react/src/vite-env.d.ts`
- `src/opplat-react/.env.example`
- `src/opplat-admin/vite.config.ts`

#### Validation

- Frontend: linted & built ✅
- Release builds: AppHost/MainApp/AdminApi ✅
- Backend tests: Baseline failures (unrelated, pre-existing)

#### QA Coverage Required

- Client app loads subscription plans in Aspire
- Registration posts to live admin API
- Auth/login/logout redirects resolve correctly
- Tenant-scoped API calls work (path/header behavior)
- Admin app same-origin `/admin/*` proxy works in dev
- BFF/auth cookie flows succeed from expected origin

---

### 2026-04-28 — MainApp Tenant Store DI Consumption (Mother)

**Date:** 2026-04-28  
**Status:** Implemented  
**Decider:** Mother

#### Decision
Consume the tenant catalog through `IMultiTenantStore<AppTenantInfo>` in MainApp endpoints and middleware. Do not inject `TenantCatalogStore` directly outside multitenancy registration/composition code.

#### Context
`MainApp` configures multitenancy with `builder.Services.AddMultiTenant<AppTenantInfo>().WithStore<TenantCatalogStore>(ServiceLifetime.Singleton)`. `/auth/account/tenant-context` failed at runtime because the endpoint requested `TenantCatalogStore` directly from DI. `TenantValidationMiddleware` used the same concrete lookup pattern.

#### Why
- Finbuckle resolves and exposes the store through the `IMultiTenantStore<AppTenantInfo>` abstraction.
- Requesting the concrete class creates a registration mismatch and runtime activation failure even though the multitenant store is configured correctly.
- Depending on the abstraction keeps MainApp aligned with the Finbuckle contract and makes tests easier to stub.

#### Notes
- When using the abstraction, call `GetByIdentifierAsync` / `GetAsync`; the `Try*` helpers are concrete convenience methods on `TenantCatalogStore`, not part of the interface contract.

---

### 2026-04-28 — QA Regression Scope for MainApp Tenant Context (Carl)

**Date:** 2026-04-28  
**Status:** Implemented  
**Decider:** Carl (QA)

#### Decision
Treat `test\Opplat.UnitTest\Auth\AuthEndpointAuthorizationIntegrationTests.cs` as the regression harness for `/auth/account/tenant-context`. Cover the runtime fix in two ways:
1. Source-level architecture assertion that MainApp consumers depend on `IMultiTenantStore<AppTenantInfo>`
2. Integration coverage that proves `/auth/account/tenant-context` resolves a tenant from claims when the accessor is empty but the multitenant store can resolve it

#### Why
- The production failure was a DI activation bug, not a business-rule failure. Coverage must prove both the abstraction choice and the live endpoint behavior.
- The fallback test is the edge case most likely to regress during future multitenancy refactors because it only happens when middleware cannot pre-resolve tenant context.

---

### 2026-05-26 — Angular-to-React Migration: Inventory Sub-Pages (Bishop)

**Date:** 2026-05-26  
**Status:** Implemented  
**Decider:** Bishop (Lead Engineer)  
**Context:** Liz implementing Warehouses, ProductClassifications, ProductGroups pages in `opplat-react`

#### 1. Sub-routes use the sibling pattern, not nested outlet

Add `/inventory/warehouses`, `/inventory/classifications`, `/inventory/groups` as sibling routes inside the existing layout wrapper — NOT as React Router nested children of the `inventory` route.

`InventoryPage` is a leaf component; refactoring it to render an `<Outlet>` would be scope creep. Sibling routes follow the existing pattern (`products`, `sell`, `users`) and require no changes to `InventoryPage` itself.

#### 2. API endpoint casing must match the backend exactly

| Resource | API path (exact) |
|----------|------------------|
| Warehouses/Storages | `/inventory/storages` |
| Classifications | `/inventory/ProductClassifications` |
| Product Groups | `/inventory/ProductGroups` |

Linux deployments are case-sensitive. `ProductClassifications` and `ProductGroups` are PascalCase in the Angular client. No trailing slashes.

#### 3. React route paths are lowercase kebab-case

UI route paths (`/inventory/warehouses`, `/inventory/classifications`, `/inventory/groups`) remain lowercase regardless of API path casing.

---

### 2026-05-26 — Angular-to-React Migration: Frontend Implementation Decisions (Liz)

**Date:** 2026-05-26  
**Status:** Implemented  
**Decider:** Liz (Frontend Dev)

#### 1. `CreateMovementData` excludes `date`
Changed to `Omit<ProductMovement, 'id' | 'date'>`. The `date` field is assigned server-side.

#### 2. Warehouses loaded eagerly on InventoryPage
Added `inventoryApi.listWarehouses()` to the existing `Promise.all` in `InventoryPage`'s `useEffect`. Avoids a secondary fetch when the user opens the "Nuevo Movimiento" dialog.

#### 3. Sub-inventory nav items are flat in the sidebar (not nested)
Three new items (Almacenes, Clasificaciones, Grupos) added as flat entries. A collapsible parent group can be introduced separately if the nav grows further.

#### 4. `Store` icon used for WarehousesPage nav item
MUI's `Warehouse` is already aliased to `InventoryIcon`. Using `Store` avoids confusion. Cosmetic change only.

#### 5. Page-level Spanish text convention followed
All UI strings are in Spanish, consistent with the Angular app and existing SPA. No i18n abstraction added.

#### 6. Pre-existing `license.api.ts` missing file not in scope
`LicensePage.tsx` imports from `'../api/license.api'` which does not exist. Pre-existing issue; tracked separately.

---

## Governance

- All meaningful changes require team consensus
- Document architectural decisions here
- Keep history focused on work, decisions focused on direction

