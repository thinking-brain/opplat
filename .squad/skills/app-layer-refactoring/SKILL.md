# Skill: Modular-to-Flat Application Layer Refactoring

**Purpose:** Safely consolidate module-specific application logic (handlers, DTOs, DI) from distributed class libraries into a centralized application project while maintaining module organization through folder structure.

**Context:** Multi-module ASP.NET Core applications where handlers (MediatR), DTOs, and requests are currently distributed across separate projects per module.

---

## Pattern: Flat Structure with Module Folders

### Target Structure
```
src/Opplat.Application/                    [Central Application Library]
├─ Sales/                                   [Module folder—was separate project]
│  ├─ Products/                             [Handler groups by bounded context]
│  ├─ Toppings/
│  └─ Common/
├─ Inventory/                               [Module folder—was separate project]
│  ├─ Products/
│  ├─ Storages/
│  └─ Common/
├─ DependencyInjection/                     [Central Coordinator]
│  └─ ServiceCollectionExtensions.cs        [Registers all modules + MediatR]
└─ AssemblyMarker.cs

src/Modules/Sales/Application/              [Retained wrapper project]
├─ DependencyInjection/                     [Module repo/service registration]
└─ AssemblyMarker.cs                        [Optional placeholder / compatibility seam]

src/Modules/Inventory/Application/          [Retained wrapper project]
├─ DependencyInjection/
└─ AssemblyMarker.cs
```

### Benefits
- **Single assembly**: All handlers in one DLL → faster assembly loading, unified MediatR scan
- **Clear discoverability**: All app logic in `src/Opplat.Application/`, modules as subfolders
- **Thinner handler scanning**: Hosts scan one application assembly for MediatR while module wrappers still own service/repository registration
- **Flexible scaling**: New modules added as new folders, no new projects needed

---

## Safety Checks (Pre-Migration)

### 1. Dependency Graph Validation
```
Global Opplat.Application must be allowed to reference the module Domain/Infrastructure projects needed by the moved handlers.
Module Application projects may continue to reference Opplat.Application if they stay as wrappers only.

✅ Safe if: moved handlers compile against module domain/repository abstractions without introducing domain↔application cycles.
```

### 2. Assembly Scanning Impact
```
MediatR scans using Assembly[] passed to AddMediatR()
✅ Safe if all handlers are in target assembly post-move
❌ Hazard: If handlers left behind, they won't register (handlers not found errors)
```

### 3. Namespace Collision Check
```
Inventory.Products.* (namespace) ≠ Sales.Products.* (namespace)
✅ Safe: Qualified names prevent collisions even with same folder names
```

### 4. Cross-Reference Validation
```
Grep all host/test projects for old namespaces
✅ Safe if all imports are mechanical (find/replace safe)
❌ Hazard: If hardcoded string references or reflection exist
```

---

## Migration Steps

### Phase 1: Structural (Folder/File Moves)
```powershell
# 1. Create module folders in target app project
mkdir src/Opplat.Application/Sales
mkdir src/Opplat.Application/Inventory

# 2. Move MediatR request/handler slices and command-result types
cp src/Modules/Sales/Application/*Requests.cs src/Opplat.Application/Sales/
cp src/Modules/Inventory/Application/*Requests.cs src/Opplat.Application/Inventory/

# 3. Leave module Application projects in place with only DI/marker files
```

### Phase 2: Project References (csproj)
**Update target app project (Opplat.Application.csproj):**
```xml
<!-- Add module domain/infra references so central DI can wire them -->
<ProjectReference Include="..\Modules\Sales\Domain\Opplat.Modules.Sales.Domain.csproj" />
<ProjectReference Include="..\Modules\Sales\Infrastructure\Opplat.Modules.Sales.Infrastructure.csproj" />
<ProjectReference Include="..\Modules\Inventory\Domain\Opplat.Modules.Inventory.Domain.csproj" />
<ProjectReference Include="..\Modules\Inventory\Infrastructure\Opplat.Modules.Inventory.Infrastructure.csproj" />
```

**Host project references may stay in place** if they still call module DI wrapper extensions such as `AddSalesApplication()` / `AddInventoryApplication()`.

### Phase 3: Namespace Updates (Code)
**Find/Replace Pattern:**
```
OLD:  using Opplat.Modules.Sales.Application.*
NEW:  using Opplat.Application.Sales.*

OLD:  using Opplat.Modules.Inventory.Application.*
NEW:  using Opplat.Application.Inventory.*
```

Files affected:
- All API endpoints
- Host Program.cs
- Integration test setup (if present)
- Dockerfile COPY statements (unlikely, but verify)

### Phase 4: DI & startup wiring
**Keep MediatR centralized in the target app but preserve module DI wrappers:**
```csharp
using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Opplat.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOpplatApplication(
        this IServiceCollection services,
        params Assembly[] additionalAssemblies)
    {
        var assemblies = new[] { typeof(AssemblyMarker).Assembly }
            .Concat(additionalAssemblies)
            .Distinct()
            .ToArray();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblies));

        return services;
    }
}
```

**Update all hosts (Program.cs):**
```csharp
// OLD:
builder.Services.AddOpplatApplication(
    Assembly.GetExecutingAssembly(),
    typeof(Opplat.Modules.Sales.Application.AssemblyMarker).Assembly,
    typeof(Opplat.Modules.Inventory.Application.AssemblyMarker).Assembly);
builder.Services.AddSalesApplication();
builder.Services.AddInventoryApplication();

// NEW:
builder.Services.AddOpplatApplication(Assembly.GetExecutingAssembly());
builder.Services.AddSalesApplication();
builder.Services.AddInventoryApplication();
```

---

## Validation Checklist

### Build Verification
- [ ] `dotnet build` succeeds with zero errors
- [ ] Solution opens in IDE without unresolved references
- [ ] Nuget restore succeeds
- [ ] No warnings about missing assemblies

### Runtime Verification
- [ ] MediatR handlers load without "Handler not found" errors
- [ ] All DI registrations resolve successfully
- [ ] Endpoints respond correctly (GET /health, etc.)
- [ ] Tests pass (if present)
- [ ] Docker Compose builds and starts (if containerized)

### Code Review Checklist
- [ ] All namespace imports updated
- [ ] No hardcoded assembly names remain in reflection/string references
- [ ] Module DI wrapper projects no longer contain live `*Requests.cs` handler files
- [ ] AssemblyMarker in target app is in root namespace for MediatR scanning

---

## Common Hazards & Mitigations

### Hazard: "Handler not found" at Runtime
**Cause:** MediatR scans wrong assembly, or handler assembly not passed to `AddMediatR()`  
**Prevention:** Verify `typeof(AssemblyMarker).Assembly` points to NEW location post-move

### Hazard: Flattening too much
**Cause:** Moving DI/service-registration code into the shared app along with handlers  
**Prevention:** Keep `Modules/{Module}/Application/DependencyInjection` as the module-specific composition seam unless the team explicitly wants to centralize infrastructure/service registration too.

### Hazard: Namespace Collisions
**Cause:** Two modules with identical class names in same folder hierarchy  
**Prevention:** Ensure fully qualified names are unique. `Inventory.Products.GetProductQuery` ≠ `Sales.Products.GetProductQuery`

### Hazard: Dockerfile Breaks
**Cause:** COPY layers reference old module project paths  
**Prevention:** Search Dockerfiles for `Modules/*/Application/*.csproj` paths. Update or remove as needed.

---

## When NOT to Use This Pattern

- **Multi-version scenarios:** If modules must ship with different versions, keep separate handler assemblies
- **External consumption:** If module app logic is published as NuGet package, keep it as a separate project
- **Strict service isolation:** If each microservice must avoid carrying unrelated handlers entirely, keep handlers in their own module assembly
- **Build optimization:** If monolithic handler assembly conflicts with incremental build strategy

---

## References

- **MediatR Assembly Scanning:** https://github.com/jbogard/MediatR.Extensions.Microsoft.DependencyInjection
- **ASP.NET Core DI:** https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection
- **Modular Architecture:** Patterns of Enterprise Application Architecture (Martin Fowler)
