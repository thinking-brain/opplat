---
name: "database-migrations"
description: "Use when creating, updating, reviewing, or troubleshooting .NET Entity Framework Core database migrations. Always generate migrations with dotnet-ef and never hand-author migration or model snapshot code."
domain: "database-migrations"
confidence: "high"
source: "manual"
---

## Context

Use this skill whenever an agent changes EF Core entities, configurations, relationships, indexes, owned types, seed data, or database schemas and a migration may be required.

Migration files are generated artifacts. The LLM must not write or reconstruct migration classes, designer files, or model snapshots by hand. Use the EF Core command-line tools so the generated operations and snapshot match the actual model.

Before running a command, identify:

- The `DbContext` that owns the changed model.
- The project where the `DbContext` and EF configurations are compiled.
- The startup project that supplies configuration, dependency injection, and database connection settings.
- The project and folder where migrations are stored.
- The target framework and EF Core tool/package version.

## Required workflow

1. Inspect the relevant `.csproj` files and existing migrations.
2. Confirm the correct `DbContext` with `--context` when more than one context exists.
3. Build the projects before generating the migration.
4. Run `dotnet ef migrations add` with the appropriate project, startup project, context, and output directory.
5. Review the generated migration, designer file, and model snapshot. Check that the operations represent the intended schema change and that no unrelated model changes were included.
6. Run the project’s focused tests and build again.
7. Apply the migration only when explicitly requested. Generation and application are separate actions.

If design-time creation fails, fix the design-time configuration or `IDesignTimeDbContextFactory<TContext>` rather than manually creating migration files. Do not bypass the tool by copying an old migration and editing it.

## Installation and versioning

Use a local or pinned tool where the repository has a tool manifest. Otherwise install or update the global tool as needed:

```powershell
dotnet tool install --global dotnet-ef --version <ef-core-version>
dotnet tool update --global dotnet-ef --version <ef-core-version>
dotnet ef --version
```

The `dotnet-ef` major version should match the project’s EF Core major version. Prefer the repository’s existing version and package management conventions.

## Examples

### Single project

When the context, entities, startup code, and migrations are in one project:

```powershell
dotnet build .\src\MyApp\MyApp.csproj
dotnet ef migrations add AddCustomerPhone `
  --project .\src\MyApp\MyApp.csproj \
  --startup-project .\src\MyApp\MyApp.csproj \
  --context AppDbContext \
  --output-dir Data\Migrations
```

From the project directory, the shorter equivalent is:

```powershell
dotnet ef migrations add AddCustomerPhone --context AppDbContext --output-dir Data\Migrations
```

Use the repository’s shell syntax. In PowerShell, a backtick can continue a command; the one-line form avoids shell-specific continuation issues.

### Multiple projects

When entities and the context are in a class library, migrations are in another project, and the main app owns configuration and connections:

```powershell
dotnet build .\src\MyApp.Api\MyApp.Api.csproj
dotnet ef migrations add AddCustomerPhone `
  --project .\src\MyApp.Migrations\MyApp.Migrations.csproj `
  --startup-project .\src\MyApp.Api\MyApp.Api.csproj `
  --context AppDbContext `
  --output-dir Migrations
```

In this arrangement:

- `--project` is the target project for generated migration files. It must reference the project containing the context and model, or otherwise be configured as the migrations assembly.
- `--startup-project` is the executable/main app used to load configuration, service registration, and the connection string.
- `--context` selects the exact context when the solution contains more than one.
- `--output-dir` is relative to the project passed to `--project`.

If the context lives in an entities/infrastructure project but migrations are stored there too, use that project for `--project` and the main API for `--startup-project`:

```powershell
dotnet ef migrations add AddInvoiceStatus `
  --project .\src\Opplat.Infrastructure\Opplat.Infrastructure.csproj `
  --startup-project .\src\Apis\Opplat.Api.Main\Opplat.Api.Main.csproj `
  --context OpplatDbContext `
  --output-dir Persistance\Migrations\Main
```

For a second context, generate against its own migration folder and specify its context explicitly:

```powershell
dotnet ef migrations add AddProductBarcode `
  --project .\src\Opplat.Infrastructure\Opplat.Infrastructure.csproj `
  --startup-project .\src\Apis\Opplat.Api.Main\Opplat.Api.Main.csproj `
  --context InventoryDbContext `
  --output-dir Persistance\Migrations\Inventory
```

### Inspecting and applying migrations

Use the tool for inspection and application as well:

```powershell
dotnet ef migrations list \
  --project .\src\MyApp.Migrations\MyApp.Migrations.csproj \
  --startup-project .\src\MyApp.Api\MyApp.Api.csproj \
  --context AppDbContext

dotnet ef migrations script `
  --project .\src\MyApp.Migrations\MyApp.Migrations.csproj \
  --startup-project .\src\MyApp.Api\MyApp.Api.csproj \
  --context AppDbContext

dotnet ef database update `
  --project .\src\MyApp.Migrations\MyApp.Migrations.csproj \
  --startup-project .\src\MyApp.Api\MyApp.Api.csproj \
  --context AppDbContext
```

Do not run `database update` against a shared or production database without explicit user authorization and the project’s deployment process.

## Validation checklist

- The migration was created by `dotnet ef`, not authored by the agent.
- The selected context is the one whose model changed.
- The generated migration contains only expected operations.
- The model snapshot changed consistently with the migration.
- The migration project compiles and references the model/context project.
- The startup project can create the context at design time.
- Existing migrations were not renamed, deleted, or rewritten without an explicit request.
- Any destructive operation such as dropping a column or table was called out for review.

## Anti-patterns

- Writing `Migration`, `.Designer.cs`, or `*ModelSnapshot.cs` files directly.
- Guessing migration operations from an entity diff.
- Running `dotnet ef` without `--context` in a solution with multiple contexts.
- Using the migrations project as `--startup-project` when connection settings live in the main app.
- Treating a successful build as proof that the migration is correct.
- Applying a newly generated migration automatically as part of code generation.