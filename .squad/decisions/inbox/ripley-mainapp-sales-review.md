---
decision_id: 8
title: MainApp Sales Conversion Wave - Approved
author: ripley
date: 2026-03-23
status: approved
---

# MainApp Sales Conversion Wave — Phase Gate Review

## Decision

**APPROVED.** MainApp Sales conversion satisfies all architectural requirements.

## Evidence

### Thin Host ✅
- Program.cs keeps `MapControllers()` only for unconverted areas (expected during wave-based migration)
- Sales surface routed via `app.MapSalesEndpoints()` minimal API extension

### Minimal API Endpoint Module ✅
- `SalesEndpoints.cs` in `Features/Sales/`
- All endpoints inject `[FromServices] IMediator mediator`
- No legacy `IProductService`, `IToppingService`, etc. injection

### MediatR-Backed Application Logic ✅
- Sales module Application project contains real handlers:
  - `ListProductsQueryHandler`, `CreateProductCommandHandler`, `UpdateProductCommandHandler`, `DeleteProductCommandHandler`
  - Full handler coverage for Toppings, ProductTags, CostTabs, Sales
- Handlers inject repositories, not legacy services

### Tenant and Non-Tenant Routes ✅
- `/sales` + `/{__tenant__}/sales` both mapped via `MapSalesGroup()` pattern
- Multi-tenant middleware unchanged

### Controller Archival ✅
- All 5 controllers archived: SalesController, ProductsController, ToppingsController, ProductTagsController, CostTabsController
- Naming: `*Controller_Archived`
- Route attributes commented
- Replacement file referenced in header comments

### Regression Coverage ✅
- 81/81 tests passing
- `ConvertedSurfaceArchitectureTests.cs` validates:
  - Endpoint wiring to MediatR
  - No legacy service injection
  - Controller archival format
  - Route surface coverage
- `EndpointSurfaceTests.cs` validates:
  - Route shapes for all Sales endpoints
  - Auth seam: `/sales` list protected, child reads unannotated

## Next Wave Authorization

**AUTHORIZED:** Remaining MainApp Areas conversion.

**Target candidates (priority order):**
1. **Admin area** — If active controllers exist beyond `AdminEndpoints.cs`
2. **Any remaining MVC areas** — Check for unconverted controllers

**Note:** MainApp still uses `MapControllers()` for unmanaged areas. Final wave should remove this call entirely.

## Review Metadata

- Build: ✅ Succeeded (7 warnings, 0 errors)
- Tests: ✅ 81/81 passing
- Reviewer: Ripley
