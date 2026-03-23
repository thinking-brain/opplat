# Bishop — flatten boundary regression decision

- Date: 2026-03-23
- Requested by: elvis.crego

## Decision

Rewrite the boundary architecture tests to enforce the user-directed flattening of Sales and Inventory MediatR handlers into the shared `src\Opplat.Application` project.

## Rationale

- The prior gate intentionally blocked flattening; the user explicitly reversed that boundary.
- Regression coverage now needs to prove the new visible business-logic seam instead of protecting the old modular application split.
- Hosts must remain thin, so the tests still preserve minimal-API/MediatR composition while only relaxing the application-project location.

## Encoded test contract

1. `src\Opplat.Application\Sales\**` and `src\Opplat.Application\Inventory\**` contain the MediatR request/handler slices.
2. `src\Modules\{Sales|Inventory}\Application\` remains wrapper-only (assembly marker + DI/composition), with no `*Requests.cs` handler sources.
3. MainApp and microservice endpoints import request contracts from `Opplat.Application.*`, not `Opplat.Modules.*.Application.*`.
4. Hosts register `AddOpplatApplication(Assembly.GetExecutingAssembly())` without scanning module application assemblies, while keeping thin-host endpoint mapping intact.
