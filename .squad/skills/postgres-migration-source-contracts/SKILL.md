---
name: "postgres-migration-source-contracts"
description: "Lock a SQL Server to PostgreSQL migration with source-only regression tests across runtime, config, orchestration, and docs."
domain: "testing"
confidence: "high"
source: "earned"
---

## When to use

- A .NET repo is migrating from SQL Server to PostgreSQL.
- Runtime code has moved faster than Docker/AppHost/docs, or vice versa.
- You need regression coverage without launching real database containers.

## Pattern

1. Add source-contract tests for the runtime seams that choose the EF Core provider (`UseNpgsql`, design-time factories, tenant/bootstrap query paths).
2. Add source-contract tests for local orchestration (`docker-compose.yml`, AppHost `Program.cs`) so every backend points at PostgreSQL and no `AddSqlServer` or SQL Server connection builder remains.
3. Lock local defaults and docs together (`appsettings*.json`, `.env.docker`, `README.md`) so validation catches stale SQL Server guidance after the provider swap.
4. When tenant resolution moves behind a helper (for example `PostgresTenantConnectionStringResolver.Resolve(...)`), update architecture tests to assert the helper seam rather than the older inline string-selection code.
5. Validate with `dotnet build .\src\Opplat.AppHost\Opplat.AppHost.csproj -m:1 -v minimal` and `dotnet test .\test\Opplat.MainApp.Test\Opplat.MainApp.Test.csproj -m:1 -v minimal`.

## Opplat-specific examples

- `test\Opplat.MainApp.Test\Auth\PostgresMigrationContractTests.cs`
- `test\Opplat.MainApp.Test\Auth\AspireLocalDevelopmentContractTests.cs`
- `test\Opplat.MainApp.Test\Architecture\MultitenancyConfigurationTests.cs`

## Anti-patterns

- Do not only swap EF Core package references and stop there; stale README or `.env` templates will keep local-dev guidance wrong.
- Do not hard-code old source substrings in architecture tests once provider selection moves into a helper seam.
