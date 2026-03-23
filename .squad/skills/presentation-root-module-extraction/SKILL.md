---
name: "presentation-root-module-extraction"
description: "Extract feature logic into module projects while keeping the web app as the single composition root"
domain: "backend-architecture"
confidence: "high"
source: "repo-work"
---

## Context
Use this pattern when a .NET solution needs module-first structure without changing externally visible API behavior. It works especially well when the web application must stay responsible for middleware, routing, DbContext setup, authentication, and dependency injection.

## Patterns

### Keep controllers in the web root
If preserving routes and startup behavior matters more than a full architectural split, leave controllers/endpoints in the web app and move only feature entities, services, repositories, and infrastructure implementations into module projects. Update controller `using` statements and DI registrations to point at module namespaces.

### Move code by feature slice
Move the entire feature slice together:
- Domain entities
- Domain DTOs
- Service interfaces and implementations
- Repository interfaces
- Infrastructure repository implementations

This avoids partial feature ownership spread across old and new locations.

### Point the composition root at module projects
The web app should reference the module Domain/Infrastructure projects directly and continue to compose dependencies there. This keeps startup, middleware order, auth, and tenant behavior centralized.

### Preserve known cross-module coupling during structure-only refactors
If a feature like Sales already depends on an Inventory entity, keep that dependency temporarily rather than redesigning it during a structure-only phase. Record the coupling as a follow-up instead of changing runtime behavior opportunistically.

### Neutralize duplicate application layers
If module `Application` projects already exist but the web app must remain the presentation root, keep those projects as placeholders and remove duplicate controllers/endpoints. That preserves the intended module skeleton without introducing two competing HTTP surfaces.

### Archive controllers during minimal API rollout
When converting an existing HTTP surface to minimal APIs, keep the old controller files as reference artifacts but rename the classes to `*Controller_Archived`, remove route attributes, and stop calling `MapControllers()` for that host. This preserves route knowledge during the migration without leaving two active endpoint stacks.

### Retire one area at a time in mixed hosts
If the web host still has other live MVC areas, archive only the converted area's controllers, remove only that area's conventional route registrations, and keep `MapControllers()` for the remaining surfaces. Pair the new minimal API module with both tenant-prefixed and legacy route groups so multitenant middleware keeps working without forcing every caller onto the tenant path immediately.

### Inject MediatR explicitly at the endpoint edge
For thin-host minimal APIs, inject `[FromServices] IMediator` in each endpoint lambda and send application commands/queries from there. This makes the mediator boundary obvious in source and helps architecture tests distinguish thin endpoint surfaces from direct service injection.

### Keep host DTO mapping at the edge
If module application handlers should stay free of host-specific contracts, let handlers return domain entities or simple command results, then translate those results to host DTOs (for example `ResponseDto`) inside the minimal endpoint file. That keeps the class library portable while still preserving the existing HTTP contract.

## Examples

```text
MainApp
  - Program.cs keeps DI + middleware
  - Areas/Sales/Controllers stays in place
  - Areas/Inventory/Controllers stays in place

Modules/Sales
  - Domain/Entities
  - Domain/Services
  - Domain/Repositories
  - Infrastructure/Repositories

Modules/Inventory
  - Domain/Entities
  - Domain/Services
  - Domain/Repositories
  - Infrastructure/Repositories
```

## Anti-Patterns
- **Moving controllers out prematurely** - creates route/composition churn during a structural refactor.
- **Splitting only entities but not services/repositories** - leaves ownership ambiguous and increases namespace confusion.
- **Redesigning cross-module contracts mid-move** - mixes refactor risk with behavior change.
- **Leaving legacy and module copies side-by-side** - invites duplicate type definitions and unclear source of truth.
