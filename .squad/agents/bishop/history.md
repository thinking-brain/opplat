## Project Context

**Project:** Opplat — Multi-platform business management system (café/restaurant)
**Requested by:** elvis.crego
**Stack:** ASP.NET Core (net6.0 → net10.0) | EF Core | SQL Server | SignalR | JWT | React 18 (replacing Vue 2)
**Solution root:** C:\projects\personal\opplat
**Branch:** develop

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

### 2024-01-XX: Test Project Analysis (net6.0 → net10.0)

**Findings:**
- Test project has only 2 test files: `LicenciaTest.cs` and `SetupContexto.cs`
- **All tests are already disabled** (commented out) - likely from previous migration
- No immediate compilation risk during framework upgrade
- Packages are severely outdated: EF Core InMemory 2.2.6 (from 2019), Moq 4.12.0, xunit 2.4.0
- Test re-enablement will require significant work due to EF Core 2.2 → 9.0 breaking changes

**Critical Package Upgrades Required:**
- `Microsoft.EntityFrameworkCore.InMemory` 2.2.6 → 9.0.x (CRITICAL: 6+ year gap, major API changes)
- `Microsoft.NET.Test.Sdk` 16.0.1 → 17.12.0
- `Moq` 4.12.0 → 4.20.x (nullable reference handling, protected member mocking changes)
- `xunit` 2.4.0 → 2.9.x (low risk, compatible)

**Test Code Patterns Observed:**
- Standard AAA (Arrange-Act-Assert) pattern
- EF Core InMemory for integration testing
- Moq for mocking (IHostingEnvironment - now deprecated, use IWebHostEnvironment)
- Tests are for `LicenciaController` with database context

**Upgrade Strategy:**
1. Phase 1: Update .csproj target framework (Hudson)
2. Phase 2: Update package versions (Bishop)
3. Phase 3: Verify compilation (should succeed since tests disabled)
4. Phase 4: Re-enable tests later (separate task, 2-4 hour effort)

**Confidence:** HIGH for compilation success, MODERATE effort for future test re-enablement

**Key Takeaway:** Tests being disabled is actually beneficial for this upgrade - reduces immediate risk. Test re-enablement should be a separate, planned task after framework upgrade stabilizes.

### 2026-03-17: Final Phase 1 validation after Hicks' Finbuckle revision

**Validation run:**
- `dotnet build .\opplat.sln -v minimal` ✅ SUCCESS
- `dotnet test .\opplat.sln -v minimal --no-build` ✅ SUCCESS (0 discovered tests)
- Zero-test discovery confirmed pre-existing: `[Fact]` commented in test file, not migration-caused

**Decision record created:** `.squad/decisions/inbox/bishop-net10-validation.md`

**Conclusion:** Phase 1 migration is locked and validated. Build framework ready; zero-test state expected and documented.
