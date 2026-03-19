---
title: "Phase 1 Sales and Inventory module extraction"
date: "2026-03-18"
author: "Hicks"
status: "proposed"
---

## What
Moved Sales and Inventory backend implementation out of legacy `src/Opplat.Domain/*` and `src/Opplat.Infrastructure/*` folders into the module projects under:

- `src/Modules/Sales/Domain`
- `src/Modules/Sales/Infrastructure`
- `src/Modules/Inventory/Domain`
- `src/Modules/Inventory/Infrastructure`

Kept `src/Opplat.MainApp` as the presentation/composition root by leaving area controllers, routing, and DI composition there.

## Why
The requested Phase 1 work was primarily a structural refactor, not an API redesign. Keeping controllers in MainApp preserves route behavior and avoids an invasive composition rewrite while still moving feature implementation into module-owned projects.

## Decision
1. MainApp references module Domain/Infrastructure projects directly.
2. MainApp controllers now depend on `Opplat.Modules.*` namespaces.
3. Legacy Sales/Inventory source trees in `Opplat.Domain` and `Opplat.Infrastructure` were removed after the move.
4. Existing module `Application` projects were intentionally left as placeholders only; duplicate controllers were removed so MainApp remains the single HTTP surface.
5. The existing Sales-to-Inventory coupling (`CostTab` -> Inventory product entity) was preserved for Phase 1 instead of being redesigned.

## Consequences
- Feature code now lives in the module projects where future backend work should land.
- MainApp still owns routing, middleware, DbContext composition, Swagger, auth, and tenant pipeline.
- A later phase can decide whether the placeholder `Application` projects should host handlers/endpoints or be retired entirely.