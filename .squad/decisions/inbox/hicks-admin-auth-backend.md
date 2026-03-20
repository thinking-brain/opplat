# Hicks Decision Inbox: Admin Auth Backend

**Date:** 2026-03-20  
**Author:** Hicks

## Decision

Use a file-backed Finbuckle tenant store in `src/Opplat.MainApp/Data/tenant-catalog.json`, seeded from the existing configuration tenants, so admin tenant CRUD can persist changes while preserving route/header tenant resolution and per-tenant DbContext selection.

## Why

- Ripley's approved design needs admin tenant management before an EF-backed tenant catalog exists.
- Finbuckle's configuration store is fine for static bootstrap data, but admin CRUD needs persistence without adding a new project or touching project files.
- A backend-local catalog file keeps Hudson's compose work and Vasquez's admin frontend unblocked while remaining easy to replace with a database-backed store later.

## Impact

- `Program.cs` now uses a custom `IMultiTenantStore<AppTenantInfo>` instead of the static configuration store.
- Tenant CRUD under `/admin/tenants` updates the same store used for runtime tenant resolution.
- Existing tenant bootstrap data in `appsettings.json` remains the seed source for first run.
