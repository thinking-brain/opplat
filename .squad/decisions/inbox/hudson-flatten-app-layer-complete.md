# Application Layer Flattening — Implementation Complete

**Date:** 2026-03-23  
**Agent:** Hudson (DevOps/Infrastructure)  
**Status:** ✅ IMPLEMENTED AND VERIFIED  
**Impact:** High (architecture restructure; simplifies host dependencies and project organization)

## Decision

Flatten the module-specific application layers (`Opplat.Modules.Sales.Application`, `Opplat.Modules.Inventory.Application`) by moving all MediatR handlers, DTOs, and dependency injection registrations into the shared `Opplat.Application` project while preserving module organization through subfolders.

## Rationale

- **User Request:** "I can not see the business logic in Application project please move all logic to that project as MediatR handlers"
- **Simplification:** Eliminates 2 intermediate project files; all hosts now depend on a single shared application layer instead of module-specific versions
- **Clarity:** Namespace pattern `Opplat.Application.{Module}.*` preserves module ownership while centralizing logic
- **Maintainability:** Single source of truth for handler registrations; DI orchestration centralized in `Opplat.Application.DependencyInjection`

## Implementation Details

### What Was Moved
- **Sales Module (8 files):** CostTabRequests.cs, ToppingRequests.cs, ProductTagRequests.cs, SaleRequests.cs, ProductRequests.cs + Common DTOs + DI registrations
- **Inventory Module (11 files):** UnitsOfMeasurementRequests.cs, StorageRequests.cs, ProductMovementRequests.cs, MovementTypeRequests.cs, ProductClassificationRequests.cs, InventoryRequests.cs, ProductRequests.cs, ProductGroupRequests.cs + Common DTOs + DI registrations

### Namespace Updates
All moved files updated from:
- `Opplat.Modules.Sales.Application.*` → `Opplat.Application.Sales.*`
- `Opplat.Modules.Inventory.Application.*` → `Opplat.Application.Inventory.*`

### DI Orchestration Pattern
```csharp
// src/Opplat.Application/DependencyInjection/ServiceCollectionExtensions.cs
public static IServiceCollection AddOpplatApplication(this IServiceCollection services, params Assembly[] additionalAssemblies)
{
    var assemblies = new[] { typeof(AssemblyMarker).Assembly }
        .Concat(additionalAssemblies)
        .Distinct()
        .ToArray();

    services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblies));
    services.AddSalesApplication();      // src/Opplat.Application/Sales/DependencyInjection/...
    services.AddInventoryApplication();  // src/Opplat.Application/Inventory/DependencyInjection/...

    return services;
}
```

### Project Reference Updates
Removed module Application references from:
- `Opplat.MainApp.csproj` (line 38 removed)
- `Opplat.Services.Sales.Api.csproj` (line 16 removed)
- `Opplat.Services.Inventory.Api.csproj` (line 16 removed)

Kept references to:
- Module Domain projects (for entities)
- Module Infrastructure projects (via Domain → Infrastructure transitive deps)
- `Opplat.Application` (shared, new home for handlers)

### Solution File Cleanup
Removed from `opplat.slnx`:
- `src/Modules/Sales/Application/` folder entry
- `src/Modules/Inventory/Application/` folder entry

Deleted from disk:
- `src/Modules/Sales/Application/` directory tree (2 project files + folders)
- `src/Modules/Inventory/Application/` directory tree (2 project files + folders)

### Dockerfile Updates
Removed module Application COPY/restore steps from:
- `src/Opplat.MainApp/Dockerfile` (lines 14, 17)
- `src/Services/Sales/Opplat.Services.Sales.Api/Dockerfile` (line 10)
- `src/Services/Inventory/Opplat.Services.Inventory.Api/Dockerfile` (line 10)

Reason: Projects no longer exist; `dotnet restore` would fail on missing projects during containerized builds.

## Verification

- ✅ **Build Status:** `dotnet build` → 0 errors (4 pre-existing MimeKit CVE warnings)
- ✅ **Handler Visibility:** All MediatR handlers now in `Opplat.Application` root with module namespace prefixes
- ✅ **DI Functional:** `AddOpplatApplication()` correctly registers handlers from shared assembly
- ✅ **No Circular Dependencies:** Domain/Infrastructure still in modules; Application has no backward refs
- ✅ **Repository Resolution:** Module DI extensions still wire to domain services/repositories correctly
- ✅ **Docker Build Compatible:** Dockerfiles no longer reference deleted projects

## Architecture Changes

**Before:**
```
Opplat.MainApp → Opplat.Application + Opplat.Modules.Sales.Application + Opplat.Modules.Inventory.Application
Opplat.Services.Sales.Api → Opplat.Application + Opplat.Modules.Sales.Application
Opplat.Services.Inventory.Api → Opplat.Application + Opplat.Modules.Inventory.Application
```

**After:**
```
Opplat.MainApp → Opplat.Application (contains Sales/ and Inventory/ subfolders)
Opplat.Services.Sales.Api → Opplat.Application (contains Sales/ subfolder)
Opplat.Services.Inventory.Api → Opplat.Application (contains Inventory/ subfolder)
```

## Side Effects & Mitigations

| Effect | Mitigation |
|--------|-----------|
| Module-specific handlers now in shared layer | Namespace pattern preserves module ownership; architecture tests can enforce |
| DI wiring simpler but more centralized | Documented in `AddOpplatApplication()` with clear extension calls |
| No longer possible to load module handlers independently | By design (handlers always shared across MainApp and APIs) |

## Acceptance Criteria

- ✅ All handlers moved from module Application projects to shared layer
- ✅ Solution builds with zero errors
- ✅ Module Application projects removed from solution and disk
- ✅ Dockerfiles updated to reflect removed projects
- ✅ DI registrations remain functional
- ✅ No circular dependencies introduced

## Future Considerations

- **Architecture Tests:** Bishop should add tests verifying handlers remain in `Opplat.Application.*` namespaces and domain logic stays in module Domain projects
- **Admin API:** Verify `Opplat.AdminApi` still correctly references shared handlers if needed
- **Module Isolation:** If future modules need completely independent handler lifecycles, revisit this decision and add parallel Application.* projects

---

**Signed Off By:** Hudson (DevOps/Infrastructure)  
**Date:** 2026-03-23  
**Session:** 23
