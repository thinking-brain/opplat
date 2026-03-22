# Session Log — 2026-03-22 — Admin API Startup Fix

**Date:** 2026-03-22  
**Focus:** Admin API compose startup failure and health endpoint disambiguation

## Problem
Docker Compose health check repeatedly failed because the admin API `/health` endpoint threw `AmbiguousMatchException`. Both `HealthController.Get()` and a minimal API `app.MapGet("/health", ...)` mapping were matching the same route, causing routing ambiguity during startup.

## Solution
1. **Backend (Hicks):** Removed duplicate minimal API `/health` mapping from `Program.cs`. Kept the endpoint served exclusively by `HealthController`.
2. **Infrastructure (Hudson):** Made the health endpoint route explicit (`[Route("health")]`) and anonymous (`[AllowAnonymous]`) to align with Docker/K8s health probe patterns.
3. **Testing (Bishop):** Added focused startup regression tests to pin the admin API compose port, health probe path, and Dockerfile entrypoint.

## Outcome
✅ Admin API compose service reaches healthy state on first startup  
✅ Health endpoint is explicit, anonymous, and deterministic  
✅ Admin BFF/session contract (`/admin/session/*`, `/auth/bff/admin/*`) preserved  
✅ Startup regression tests pass  
✅ Full compose stack healthy (all 8 containers)

## Key Decisions Merged
- Hicks: Admin API docker startup debug
- Hudson: Admin API health endpoint route disambiguation
- Bishop: Admin API startup validation + split test strategy
