# Opplat Squad

## Project Context

**Project:** Opplat — Multi-platform business management system (café/restaurant)
**Owner:** elvis.crego
**Stack:** ASP.NET Core → net10.0 | EF Core | SQL Server | SignalR | JWT | React (migrating from Vue 2)
**Repo Root:** C:\projects\personal\opplat
**Branch:** develop
**Universe:** Aliens (1986)

## Issue Source

_Not connected to GitHub Issues. Can be added later._

## Members

| Name | Role | Skills | Badge |
|------|------|--------|-------|
| Ripley | Lead / Architect | Architecture decisions, code review, .NET patterns, multitenancy design | 🏗️ Lead |
| Hicks | Backend Dev | ASP.NET Core, EF Core, SQL Server, JWT, SignalR, Finbuckle.MultiTenant | 🔧 Backend |
| Vasquez | Frontend Dev | React 18, Vite, TypeScript, MUI, Axios, React Router | ⚛️ Frontend |
| Bishop | Tester | xunit, Moq, EF InMemory, integration tests, .NET test upgrade | 🧪 Tester |
| Hudson | DevOps / Infra | .csproj files, NuGet, build config, solution structure, migrations | ⚙️ DevOps |
| Scribe | Session Logger | Memory, decisions, session logs | 📋 Scribe |
| Ralph | Work Monitor | Backlog, issue tracking, keep-alive | 🔄 Monitor |

## Upgrade Plan (Phase Order)

1. **Phase 1 — .NET Upgrade:** All projects net6.0 → net10.0, bump all NuGet packages
2. **Phase 2 — React Client:** Replace opplat-vue with opplat-react (Vite + React 18 + TS + MUI + React Router + Axios). Pages: Login, Home, Products, Sell, Users
3. **Phase 3 — Multitenancy:** Finbuckle.MultiTenant, tenant resolution by route/host, per-tenant DbContext, tenant-aware Identity/Auth
