## Project Context

**Project:** Opplat — Multi-platform business management system (café/restaurant)
**Requested by:** elvis.crego
**Stack:** ASP.NET Core (net6.0 → net10.0) | EF Core | SQL Server | SignalR | JWT | React 18 (replacing Vue 2)
**Solution root:** C:\projects\personal\opplat
**Branch:** develop

## Session Logs

### 2026-03-23: Admin Tenant Boundary Refactor (Session 14)
**Date:** 2026-03-23T00:04:41Z  
**Requested By:** elvis.crego  
**Team:** Ripley (review), Hicks (backend), Vasquez (frontend), Bishop (validation)  
**Status:** ✅ COMPLETE

Orchestrated completion of admin API boundary refactor. Merged 6 decision documents from inbox into decisions.md. Wrote orchestration logs for all 4 agents. Wrote session summary log. Updated all agent histories with task summaries. Coordinator validation: all checks passed (dotnet build, dotnet test 65/65, npm lint, npm build, docker compose config). Refactor complete — admin API now owns tenant catalog only, not users. Users belong to tenant-owned databases. Admin tracks MaxUsers/CurrentUserCount for subscription enforcement.

**Scribe Tasks Completed:**
1. ✅ Wrote orchestration logs: ripley.md, hicks.md, vasquez.md, bishop.md
2. ✅ Wrote session log: admin-boundary-refactor.md
3. ✅ Merged decisions inbox → decisions.md (6 files)
4. ✅ Updated agent histories (ripley, hicks, vasquez, bishop)
5. ✅ Updated identity/now.md if applicable
6. ✅ Staged .squad/ for commit

---

## Solution Structure

- src/Opplat.MainApp/           — ASP.NET Core Web API (net6.0 → net10.0)
- src/Opplat.Domain/            — Business logic (net6.0 → net10.0)
- src/Opplat.Infrastructure/    — Data access, EF Core (net6.0 → net10.0)
- src/Opplat.Shared/            — Common utilities (net6.0 → net10.0)
- src/opplat-vue/               — Old Vue 2 client (keep but inactive)
- src/opplat-react/             — NEW React 18 client (to be created)
- test/Opplat.MainApp.Test/     — xunit tests (net6.0 → net10.0)

## Key Architecture

- Clean Architecture (Domain / Infrastructure / MainApp)
- EF Core DbContext with ASP.NET Core Identity (OpplatDbContext)
- JWT Bearer auth
- SignalR hubs
- Swagger/OpenAPI

## Phase Plan

1. Phase 1 — .NET Upgrade (Hudson + Hicks + Bishop)
2. Phase 2 — React Client (Vasquez + Hicks for API verification)
3. Phase 3 — Multitenancy with Finbuckle.MultiTenant (Hicks + Ripley design)

## Decisions

- net10.0 target framework
- Finbuckle.MultiTenant for multitenancy
- React app at src/opplat-react/ (Vite + React 18 + TypeScript + MUI + React Router + Axios)
- Pages required: Login, Home, Products, Sell, Users
- Old Vue app kept at src/opplat-vue/ but inactive

## Learnings
