---
name: "admin-react-contract-alignment"
description: "How to realign a React admin client after backend contract field renames without reintroducing removed API surface"
domain: "frontend-api"
confidence: "high"
source: "earned"
tools:
  - name: "rg"
    description: "Find stale client contract names and removed endpoint references quickly"
    when: "When diagnosing admin frontend breakage after backend API refactors"
  - name: "dotnet test"
    description: "Validate source-contract and minimal endpoint tests after aligning the client"
    when: "After changing frontend DTO names or updating contract assertions"
---

## Context

Use this when the dedicated admin API changes DTO field names or removes endpoints and the React admin client starts failing after a backend boundary refactor.

## Patterns

- Compare the frontend API wrapper and TypeScript DTOs directly against the backend contract source, not just the UI pages.
- Treat removed endpoints as hard deletions; do not add fallback calls that revive removed admin user CRUD.
- When a backend property name changes but the UI label should stay human-friendly, rename the data field in code and keep the visible label unchanged.
- Update contract-style tests that read frontend source so CI catches the next drift immediately.

## Examples

- Backend contract: `src\Opplat.AdminApi\Endpoints\AdminContracts.cs`
- Frontend DTOs: `src\opplat-admin\src\types\index.ts`
- Tenant form/list UI: `src\opplat-admin\src\pages\TenantsPage.tsx`
- Dashboard summary UI: `src\opplat-admin\src\pages\DashboardPage.tsx`
- Source contract test: `test\Opplat.MainApp.Test\Auth\FrontendAuthContractTests.cs`

## Anti-Patterns

- Do not keep old frontend aliases like `schema` when the live JSON contract is `databaseSchema`; it hides drift until runtime.
- Do not “fix” a contract mismatch by restoring removed `/admin/users` calls or outdated tenant fields.
- Do not stop after the UI compiles; also validate the source-contract tests that pin the integration.
