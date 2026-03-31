# Project Context

- **Owner:** Elvis Crego
- **Project:** opplat — Multi-platform business management system for café and restaurant operations. Multi-tenant SaaS with ASP.NET Core (.NET 10), EF Core, PostgreSQL, Finbuckle.MultiTenant, React 18, TypeScript, MUI, Keycloak (local) / Entra ID (production).
- **Stack:** ASP.NET Core (.NET 10), Entity Framework Core, PostgreSQL, Finbuckle.MultiTenant, SignalR, OIDC, React 18, Vite, TypeScript, Material-UI, Docker, nginx
- **Created:** 2026-03-31

## Key Architecture Facts

- My domain: Domain logic, tenant provisioning, identity integrations
- TenantSchemaProvisioningService: idempotent schema creation (checks information_schema.schemata before CREATE SCHEMA)
- DatabaseInstanceAutoScalingService: auto-provisions when MaxTenantsPerInstance (default 100) threshold reached
- TenantSchemaMigrationRunner: batched bulk migrations (default batch 10, delay 5s), phased execution
- Identity: IGraphUserService (Entra), IKeycloakUserService (local dev), NoOp stubs for both
- GraphUserService has retry logic (429/503) with ODataError-based retry and configurable MaxRetries/BaseDelaySeconds
- Modules 4+ commands largely stub with NotImplementedException — these are primary targets
- IMPLEMENTATION-PLAN.md has full coverage map of all modules

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->

### 2026-01-01 — Phase 1: Tenant User Commands & Audit Log

- **TenantUser catalog is the truth store.** All user lifecycle goes through `AdminTenantCatalogDbContext.TenantUsers`. UserManager/per-tenant Identity tables are legacy dead code.
- **IGraphUserService is the Entra adapter.** `CreateUserAsync`, `EnableUserAsync`, `DisableUserAsync` are the three lifecycle methods used by tenant user commands. `IKeycloakUserService` only has `CreateUserAsync` + `DeleteUserAsync` — no enable/disable — so update operations use Graph only.
- **TenantUser.Id is Guid, UserId in commands is string.** Always `Guid.TryParse` the incoming string UserId. Return false/null on parse failure rather than throwing.
- **AuditLogService swallows its own exceptions.** This is intentional — audit failures must not surface as 500s.
- **AdminTenantCatalogDbContext can use InMemory for unit tests.** It takes `DbContextOptions<AdminTenantCatalogDbContext>` as primary constructor param, so `DbContextOptionsBuilder.UseInMemoryDatabase` works cleanly in tests.
- **AppTenantInfo.Id is a string GUID.** Parse with `Guid.TryParse` when you need the typed Guid for EF queries.
- **SubscriptionPlan.MaxActiveUsers** is the seat limit field — check `tenant.TenantUsers.Count(u => u.IsActive) >= plan.MaxActiveUsers` for the cap.
- **Temporary password pattern:** `$"Tmp!{Guid.NewGuid():N}1A"` meets Entra complexity (upper, lower, digit, special).
