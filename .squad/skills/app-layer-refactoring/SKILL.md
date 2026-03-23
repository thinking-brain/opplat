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
│  ├─ DependencyInjection/                  [Private DI extensions]
│  └─ Common/
├─ Inventory/                               [Module folder—was separate project]
│  ├─ Products/
│  ├─ Storages/
│  ├─ DependencyInjection/                  [Private DI extensions]
│  └─ Common/
├─ DependencyInjection/                     [Central Coordinator]
│  └─ ServiceCollectionExtensions.cs        [Registers all modules + MediatR]
└─ AssemblyMarker.cs
```

### Benefits
- **Single assembly**: All handlers in one DLL → faster assembly loading, unified MediatR scan
- **Clear discoverability**: All app logic in `src/Opplat.Application/`, modules as subfolders
- **Simpler host projects**: Hosts reference 1 app project instead of N (hosts only do minimal API wiring)
- **Flexible scaling**: New modules added as new folders, no new projects needed

---

## Safety Checks (Pre-Migration)

### 1. Dependency Graph Validation
```
Module Applications must NOT reference global Opplat.Application
(If they do, moving them creates cycles—STOP.)

✅ Safe if: Module Infra/Domain form isolated DAG and global app only references modules.
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

# 2. Copy all files from source modules
cp -Recurse src/Modules/Sales/Application/* src/Opplat.Application/Sales/
cp -Recurse src/Modules/Inventory/Application/* src/Opplat.Application/Inventory/

# 3. Delete source module app projects
rm -Recurse src/Modules/Sales/Application/
rm -Recurse src/Modules/Inventory/Application/
rm src/Modules/Sales/Application/*.csproj
rm src/Modules/Inventory/Application/*.csproj
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

**Update host projects (API, MainApp csproj):**
```xml
<!-- Remove old module app references -->
<!-- <ProjectReference Include="..\Modules\Sales\Application\..." /> -->
<!-- <ProjectReference Include="..\Modules\Inventory\Application\..." /> -->

<!-- Keep module domain references if needed (usually not) -->
```

**Update solution file (*.slnx):**
```xml
<!-- Remove project entries for deleted modules.Sales.Application, modules.Inventory.Application -->
```

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

### Phase 4: DI Consolidation
**Centralize in target app's DependencyInjection/ServiceCollectionExtensions.cs:**
```csharp
using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Opplat.Modules.Sales.Domain.Repositories;
using Opplat.Modules.Sales.Domain.Services;
using Opplat.Modules.Sales.Infrastructure.Repositories;
// ... inventory imports

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
        services.AddSalesApplication();       // Private helper
        services.AddInventoryApplication();   // Private helper

        return services;
    }

    private static IServiceCollection AddSalesApplication(
        this IServiceCollection services)
    {
        // Register all Sales domain services/repos
        services.AddScoped<IProductService, ProductService>();
        // ... etc
        return services;
    }

    private static IServiceCollection AddInventoryApplication(
        this IServiceCollection services)
    {
        // Register all Inventory domain services/repos
        // ... etc
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
// All modules auto-registered; no separate extension calls
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
- [ ] DI extensions are private (not public) to avoid re-registration
- [ ] AssemblyMarker in target app is in root namespace for MediatR scanning

---

## Common Hazards & Mitigations

### Hazard: "Handler not found" at Runtime
**Cause:** MediatR scans wrong assembly, or handler assembly not passed to `AddMediatR()`  
**Prevention:** Verify `typeof(AssemblyMarker).Assembly` points to NEW location post-move

### Hazard: Circular Dependencies
**Cause:** Module app was referenced by global app before migration  
**Prevention:** Pre-migration verify: `grep -r "Modules\.(Sales|Inventory)\.Application" src/Opplat.Application/`  
If any found, break those imports first.

### Hazard: Namespace Collisions
**Cause:** Two modules with identical class names in same folder hierarchy  
**Prevention:** Ensure fully qualified names are unique. `Inventory.Products.GetProductQuery` ≠ `Sales.Products.GetProductQuery`

### Hazard: Dockerfile Breaks
**Cause:** COPY layers reference old module project paths  
**Prevention:** Search Dockerfiles for `Modules/*/Application/*.csproj` paths. Update or remove as needed.

---

## When NOT to Use This Pattern

- **Multi-version scenarios:** If modules must ship with different versions, keep separate projects
- **External consumption:** If module app logic is published as NuGet package, keep it as separate project
- **Team boundaries:** If different teams own modules and need strict boundaries, separate projects are clearer
- **Build optimization:** If monolithic assembly conflicts with incremental build strategy

---

## References

- **MediatR Assembly Scanning:** https://github.com/jbogard/MediatR.Extensions.Microsoft.DependencyInjection
- **ASP.NET Core DI:** https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection
- **Modular Architecture:** Patterns of Enterprise Application Architecture (Martin Fowler)
