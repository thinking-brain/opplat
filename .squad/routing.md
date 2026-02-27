# Opplat Squad — Routing Rules

## Signal → Agent Mapping

| Signal | Agent | Why |
|--------|-------|-----|
| .csproj, NuGet packages, target framework, project references | Hudson | Project config specialist |
| EF Core migrations, DbContext, SQL Server, Identity | Hicks | Backend data layer |
| ASP.NET Core controllers, middleware, JWT, SignalR, Program.cs | Hicks | Backend API |
| Finbuckle.MultiTenant, multitenancy, tenant resolution | Hicks | Backend architecture |
| React, Vite, TypeScript, MUI, Axios, React Router, frontend pages | Vasquez | Frontend specialist |
| xunit tests, Moq, integration tests, test project | Bishop | Testing specialist |
| Architecture decisions, cross-cutting concerns, code review | Ripley | Lead |
| Build config, CI, project structure, solution-level changes | Hudson | DevOps |
| Session logs, decisions, memory | Scribe | Scribe |
| Backlog, issue queue, work monitoring | Ralph | Work Monitor |

## Phase-Specific Routing

### Phase 1 — .NET Upgrade
- Primary: Hudson (csproj/NuGet changes) + Hicks (code compatibility fixes)
- Reviewer: Ripley
- Tests: Bishop

### Phase 2 — React Migration
- Primary: Vasquez (full React app scaffold + pages)
- Support: Hicks (API contract verification)
- Reviewer: Ripley

### Phase 3 — Multitenancy
- Primary: Hicks (backend Finbuckle integration, DbContext, Identity)
- Architecture: Ripley (design approval first)
- Tests: Bishop
- Frontend: Vasquez (tenant context in React if needed)

## Fallback

If signal is ambiguous → route to Ripley (Lead)
