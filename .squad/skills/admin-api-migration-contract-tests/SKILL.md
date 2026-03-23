---
name: "admin-api-migration-contract-tests"
description: "How to lock an API migration with executable endpoint tests plus source-contract assertions"
domain: "testing"
confidence: "high"
source: "earned"
tools:
  - name: "dotnet test"
    description: "Runs the backend regression suite"
    when: "After updating contract tests for API or infrastructure migrations"
---

## Context

Use this when a backend is changing internals (for example, store/service logic moving to MediatR handlers or SQL Server moving to PostgreSQL) but the public HTTP contract should remain stable.

## Patterns

- Keep the main contract tests at the HTTP layer so route shapes, status codes, payloads, and tenant isolation stay protected while implementation moves underneath.
- Seed an in-memory EF Core context with realistic migration-era data, including provider-shaped connection strings if the frontend or DTOs expose them.
- Add a small source-contract suite to pin critical infrastructure seams such as DI registration, provider selection, and key framework dependencies.
- When an API boundary removes or renames fields, add page-level source-contract tests for the consuming SPA screens as well as API-wrapper tests; otherwise stale feature code can keep calling removed endpoints or old DTO fields even while lower-level contracts stay green.
- For multitenancy, verify both positive isolation (`/admin/users?tenantIdentifier=...`) and negative isolation (tenant-scoped endpoints do not leak users from other tenants).
- For JWT/OIDC migrations, assert claim normalization on the exact auth component used by the target service.

## Examples

- `test/Opplat.MainApp.Test/Auth/AdminApiMinimalEndpointContractTests.cs`
- `test/Opplat.MainApp.Test/Auth/AdminApiMigrationContractTests.cs`
- `test/Opplat.MainApp.Test/Auth/AdminPortalDataSeeder.cs`

## Anti-Patterns

- Do not assert private handler method structure when endpoint-level behavior is the real contract.
- Do not rely only on source assertions; they miss runtime DTO/status regressions.
- Do not seed generic fake data when the migration changes provider-specific strings or tenant relationships that the contract exposes.
