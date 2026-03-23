---
name: "efcore-startup-schema-compatibility"
description: "How to keep an EF Core service working when EnsureCreated meets a live database that still has legacy columns and constraints"
domain: "database-compatibility"
confidence: "high"
source: "earned"
tools:
  - name: "docker"
    description: "Validate the real containerized service against the live relational schema"
    when: "When source tests pass but the deployed service still throws database exceptions"
  - name: "psql"
    description: "Inspect live columns and constraints in PostgreSQL"
    when: "When diagnosing runtime schema drift after a persistence refactor"
---

## Context

Use this when a service keeps calling `Database.EnsureCreated()` for bootstrapping, but the backing relational database already exists from an older model and now misses renamed columns or still enforces obsolete constraints.

## Patterns

- Treat `EnsureCreated()` as create-only bootstrap, not a migration mechanism; add an explicit startup compatibility pass for live databases.
- Inspect the actual relational schema (`information_schema.columns`) before deciding what to alter.
- Add new columns idempotently, then backfill them from legacy data before the first live query hits the new model.
- If the old schema still enforces a secret-bearing column that the new boundary removed, relax the constraint instead of repopulating the secret.
- Keep the compatibility shim near the seeder/startup path so fresh databases still work and legacy databases self-heal on boot.

## Examples

- `src\Opplat.AdminApi\Data\AdminCatalogSchemaCompatibility.cs`
- `src\Opplat.AdminApi\Data\AdminPortalDataSeeder.cs`
- Live failure shape: `GET /admin/tenants` failing because `AdminTenants` lacked `DatabaseName` / `DatabaseSchema`

## Anti-Patterns

- Do not assume passing EF InMemory tests proves the relational schema is compatible.
- Do not restore removed columns like `ConnectionString` to the write model just to satisfy legacy constraints.
- Do not stop after fixing reads; verify create/update paths too, because old NOT NULL constraints can still break inserts.
