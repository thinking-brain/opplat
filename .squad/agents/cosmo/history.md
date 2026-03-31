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
