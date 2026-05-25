---
name: "finbuckle-store-di"
description: "How Opplat should consume Finbuckle tenant stores from DI"
domain: "multitenancy"
confidence: "high"
source: "earned"
---

## Context

Use this when working on MainApp multitenancy, especially endpoint handlers and middleware that need to read tenant metadata from the catalog after Finbuckle has been configured in startup.

## Patterns

### Consume the registered store abstraction

If startup uses `AddMultiTenant<AppTenantInfo>().WithStore<TenantCatalogStore>(...)`, downstream code should request `IMultiTenantStore<AppTenantInfo>` from DI. Do not inject `TenantCatalogStore` directly in endpoints or middleware.

### Call the interface surface

When you only have `IMultiTenantStore<AppTenantInfo>`, use `GetByIdentifierAsync` and `GetAsync`. The `TryGetByIdentifierAsync` and `TryGetAsync` helpers belong to the concrete `TenantCatalogStore` and are not part of the Finbuckle interface contract.

### Guard the pattern with source-level tests

For architecture-style regression coverage in this repo, use source assertions in `test\Opplat.UnitTest\Architecture\MultitenancyConfigurationTests.cs` to verify tenant consumers keep using the abstraction.

## Examples

```csharp
group.MapGet("/tenant-context",
    async ([FromServices] IMultiTenantStore<AppTenantInfo> tenantStore) =>
    {
        var tenant = await tenantStore.GetByIdentifierAsync("mojocafe");
        return Results.Ok(tenant);
    });
```

```csharp
var tenantStore = context.RequestServices.GetService<IMultiTenantStore<AppTenantInfo>>();
var tenant = tenantStore is null ? null : await tenantStore.GetAsync(claimTenantId);
```

## Anti-Patterns

- Injecting `TenantCatalogStore` directly into request handlers, endpoints, or middleware
- Calling `TryGetByIdentifierAsync` / `TryGetAsync` on an `IMultiTenantStore<AppTenantInfo>` reference
- Assuming `WithStore<TStore>()` automatically makes the concrete `TStore` resolvable everywhere
