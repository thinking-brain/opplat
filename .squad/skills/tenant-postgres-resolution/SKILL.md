# Tenant PostgreSQL resolution

## When to use
When an Opplat ASP.NET host needs to resolve a tenant database during the SQL Server -> PostgreSQL migration without embedding provider logic directly in `Program.cs`.

## Pattern
1. Read the default PostgreSQL connection string from `DefaultConnection` with `MainConnection` fallback.
2. If the tenant record already has a full `ConnectionString`, normalize it with `NpgsqlConnectionStringBuilder`.
3. If the tenant record only has `DatabaseName` (and optionally `DatabaseSchema`), clone the default connection string and override `Database` / `SearchPath`.
4. Hand the final string to `UseNpgsql(...)`.
5. Reuse the same resolver in request-time DbContext registration, design-time factories, provisioning flows, and cross-tenant admin queries.

## Why it works
This keeps hosts thin, preserves compatibility with older tenant records that still carry full connection strings, and lets the admin/catalog side evolve toward metadata-driven tenant database definitions.

## Opplat examples
- `src\Opplat.MainApp\Data\PostgresTenantConnectionStringResolver.cs`
- `src\Opplat.MainApp\Program.cs`
- `src\Opplat.Microservices.Shared\Extensions\ServiceCollectionExtensions.cs`
