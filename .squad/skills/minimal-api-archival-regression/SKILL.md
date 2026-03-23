# Minimal API archival regression gate

## When to use

Use this when a host migrates from controllers to minimal APIs but intentionally keeps archived controller files in the repo for reference.

## Pattern

1. **Pin the host contract**
   - Assert `Program.cs` maps endpoint modules, not inline route handlers
   - Assert `AddControllers` and `MapControllers` are absent

2. **Pin the endpoint-module contract**
   - Read each endpoint module source directly
   - Assert handlers are invoked through `IMediator`
   - Assert legacy `IService`/`DbContext`/`UserManager`-style business logic dependencies do not appear in endpoint files

3. **Pin the archive contract**
   - Scan controller files and fail only on live markers:
     - uncommented `[ApiController]`
     - uncommented non-archived `public class *Controller`
   - Allow archived files to retain action methods or commented route attributes if the host no longer maps controllers

4. **Pin the seam at runtime**
   - Add focused integration tests for auth/tenant behavior on representative minimal endpoints
   - For tenant-scoped routes, assert resolved tenant header and normalized tenant claims must agree

## Why it matters

This catches the real regression modes after a controller-to-minimal migration: reintroducing MVC host wiring, bypassing MediatR from endpoints, or weakening tenant/auth boundaries. It also avoids false failures from archived reference code that is intentionally preserved but no longer routable.
