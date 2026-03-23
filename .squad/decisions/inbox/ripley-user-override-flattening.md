# Application Layer Flattening: User Override — Migration Guardrails

**Decision Date:** 2026-03-24  
**Agent:** Ripley (Lead/Architect)  
**Status:** 📋 GUARDRAILS SET — AWAITING IMPLEMENTATION  
**Trigger:** User explicitly overrode prior rejection (Session 23)

---

## Context

The user requested all application/business logic move into the global `Opplat.Application` project under module-specific folders. The prior architectural decision to keep module Application projects separate was explicitly overridden.

This document establishes the migration guardrails. The team treats user direction as the new target.

---

## Target Architecture

```
src/
  Opplat.Application/
    Sales/              ← MediatR handlers for Sales
      Products/
      ProductTags/
      Sales/
      Toppings/
      CostTabs/
      Common/
    Inventory/          ← MediatR handlers for Inventory
      Products/
      ProductGroups/
      ProductClassifications/
      Storages/
      MovementTypes/
      ProductMovements/
      Inventories/
      UnitsOfMeasurement/
      Common/
    DependencyInjection/
      ServiceCollectionExtensions.cs   ← Combined module registration
    AssemblyMarker.cs
  Modules/
    Sales/
      Domain/           ← Stays (entities, repository interfaces)
      Infrastructure/   ← Stays (EF implementations)
      Application/      ← REMOVED (handlers moved to Opplat.Application/Sales/)
    Inventory/
      Domain/           ← Stays
      Infrastructure/   ← Stays
      Application/      ← REMOVED
  Services/
    Sales/Api/          ← Thin host — MediatR injection only
    Inventory/Api/      ← Thin host — MediatR injection only
  Opplat.MainApp/       ← Thin host — MediatR injection only
```

---

## Migration Guardrails (MANDATORY)

### 1. Namespace Convention
All migrated handlers use `Opplat.Application.{Module}.{Feature}` namespace.
```csharp
// Before (separate project)
namespace Opplat.Modules.Sales.Application.Products;

// After (global Application project)
namespace Opplat.Application.Sales.Products;
```

### 2. Hosts Remain Thin
Hosts inject `IMediator` and dispatch. No direct service injection for business logic.
```csharp
// Correct: MediatR dispatch
app.MapGet("/products", async ([FromServices] IMediator mediator) =>
    await mediator.Send(new ListProductsQuery()));

// WRONG: Direct service injection
app.MapGet("/products", async ([FromServices] IProductService service) =>
    await service.List());
```

### 3. Domain/Infrastructure Stay in Modules
Do NOT move domain entities, repository interfaces, or EF implementations. Module boundaries at Domain/Infrastructure level remain intact.

### 4. Remove Legacy IService Pattern
The migration must remove the dual registration of both IService + ICommandHandler for the same operations. Final state: MediatR handlers only for application logic.

### 5. Opplat.Application Must Reference Module Domain Projects
```xml
<!-- Opplat.Application.csproj must add -->
<ProjectReference Include="..\Modules\Sales\Domain\Opplat.Modules.Sales.Domain.csproj" />
<ProjectReference Include="..\Modules\Inventory\Domain\Opplat.Modules.Inventory.Domain.csproj" />
```

### 6. Host Assembly Scanning Simplifies
Hosts now scan only `Opplat.Application`:
```csharp
// Before
builder.Services.AddOpplatApplication(
    Assembly.GetExecutingAssembly(),
    typeof(Opplat.Modules.Sales.Application.AssemblyMarker).Assembly,
    typeof(Opplat.Modules.Inventory.Application.AssemblyMarker).Assembly);

// After
builder.Services.AddOpplatApplication(
    Assembly.GetExecutingAssembly(),
    typeof(Opplat.Application.AssemblyMarker).Assembly);
```

### 7. Architecture Tests Must Be Updated or Removed
`ApplicationLayerBoundaryArchitectureTests.cs` enforces the old pattern. The migration must either:
- Remove these tests, OR
- Update them to enforce the new pattern (handlers in `Opplat.Application/{Module}/`)

### 8. Sequencing Constraint
Execute in this order to avoid breaking builds:
1. Add module domain references to `Opplat.Application.csproj`
2. Create folder structure (`Sales/`, `Inventory/` under `Opplat.Application/`)
3. Move handler files, update namespaces
4. Update `ServiceCollectionExtensions` in `Opplat.Application` to register module handlers
5. Remove old module Application projects from solution
6. Update host `.csproj` references (remove module Application, ensure Opplat.Application referenced)
7. Update host `Program.cs` assembly scanning
8. Delete or update architecture tests
9. Remove legacy `IService` registrations from DI
10. Build + test all hosts

---

## No-Go Conditions (STOP if any apply)

1. **External consumers of module Application assemblies** — If any package or service outside this repo references `Opplat.Modules.Sales.Application.dll` directly, flattening breaks them.
2. **Module-specific MediatR pipelines** — If Sales/Inventory need different validation, logging, or behaviors registered per-module, a single Application assembly complicates this.
3. **Independent module versioning** — If modules will be versioned/released separately in the future, shared assembly prevents that.

**Current state:** None apply. ✅ Migration is safe.

---

## Validation Criteria (Definition of Done)

- [ ] `Opplat.Application/Sales/` contains all Sales handlers
- [ ] `Opplat.Application/Inventory/` contains all Inventory handlers
- [ ] `Opplat.Modules.Sales.Application` project removed from solution
- [ ] `Opplat.Modules.Inventory.Application` project removed from solution
- [ ] No host injects legacy `IProductService`, `ISalesService`, etc.
- [ ] All hosts scan only `Opplat.Application.AssemblyMarker.Assembly` for handlers
- [ ] `dotnet build opplat.slnx` succeeds
- [ ] All tests pass (update/remove architecture tests as needed)
- [ ] `docker compose up` starts all services with `/health` endpoints responding

---

## Notes

This overrides the rejection documented in Session 23. The user is the final authority on architecture direction. My role shifts from blocking to ensuring safe execution.

The legitimate concern about cross-context coupling (Sales handlers accidentally referencing Inventory types) remains. Mitigation: Code review vigilance and potential analyzer rules post-migration.

---

*Ripley — Lead / Architect*
