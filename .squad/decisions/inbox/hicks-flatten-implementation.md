# Hicks flatten implementation

## Decision

Honor the user's override and flatten Sales/Inventory MediatR request-handler slices into `src\Opplat.Application\{Sales|Inventory}\` while keeping module `Application` projects alive only as thin dependency-registration wrappers.

## Why

- The user explicitly wants business logic discoverable in the global Application project.
- MainApp and the Sales/Inventory service hosts already reference `Opplat.Application`, so moving handlers there improves discoverability without pushing endpoint logic back into hosts.
- Keeping `Modules\{Sales|Inventory}\Application\DependencyInjection\ServiceCollectionExtensions.cs` preserves existing repository/service registration seams and avoids mixing infrastructure composition into the shared application project.

## Implementation notes

- `Opplat.Application.csproj` now references Sales/Inventory domain and infrastructure projects so the moved handlers compile in the shared application assembly.
- Endpoints now import `Opplat.Application.Sales.*` / `Opplat.Application.Inventory.*`.
- Host `Program.cs` files now scan only `Opplat.Application` for MediatR handlers and no longer register module application assemblies for handler discovery.
- Architecture tests were updated to lock the new flattened-handler contract in place.
