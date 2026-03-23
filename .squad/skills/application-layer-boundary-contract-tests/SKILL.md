# Application-layer boundary contract tests

## When to use

Use this when a repo has one shared application project plus separate bounded-context application projects, and the team wants the architectural boundary to stay explicit after a refactor review or rejected consolidation proposal.

## Pattern

1. **Pin the shared project as module-agnostic**
   - Read the shared application `.csproj`
   - Assert it does not reference module-specific domain/infrastructure/application projects that would absorb bounded-context ownership
   - Assert module-specific folders/namespaces (for example `Opplat.Application.Sales`) do not appear under the shared project tree

2. **Pin each bounded context as still owning its handlers**
   - Assert each module application project still exists
   - Assert request/handler source files remain under the module application directory
   - Assert source namespaces stay rooted at the module boundary (for example `Opplat.Modules.Sales.Application.*`)

3. **Pin host composition to explicit module assemblies**
   - Assert host `.csproj` files still reference the separate module application projects
   - Assert `Program.cs` still passes each module `AssemblyMarker` into shared MediatR registration
   - Assert any module-specific DI extension (`AddSalesApplication`, `AddInventoryApplication`) is still called explicitly when that is the approved arrangement

4. **Ignore build artifacts**
   - Exclude `bin/` and `obj/` when scanning `.cs` files
   - Generated files can otherwise create false namespace failures

## Opplat example

- Shared seam: `src\Opplat.Application\`
- Module owners: `src\Modules\Sales\Application\`, `src\Modules\Inventory\Application\`
- Hosts that must keep explicit module references:
  - `src\Opplat.MainApp\`
  - `src\Services\Sales\Opplat.Services.Sales.Api\`
  - `src\Services\Inventory\Opplat.Services.Inventory.Api\`
- Boundary test: `test\Opplat.MainApp.Test\Architecture\ApplicationLayerBoundaryArchitectureTests.cs`
