# Project Context

- **Owner:** Elvis Crego
- **Project:** opplat — Multi-platform business management system for café and restaurant operations. Multi-tenant SaaS with ASP.NET Core (.NET 10), EF Core, PostgreSQL, Finbuckle.MultiTenant, React 18, TypeScript, MUI, Keycloak (local) / Entra ID (production).
- **Stack:** ASP.NET Core (.NET 10), Entity Framework Core, PostgreSQL, Finbuckle.MultiTenant, SignalR, OIDC, React 18, Vite, TypeScript, Material-UI, Docker, nginx
- **Created:** 2026-03-31

## Key Architecture Facts

- My domain: `src/opplat-react/` (tenant SPA), `src/opplat-admin/` (admin portal SPA)
- Frontend validate: `npm run lint && npm run build` in each app directory
- Auth: frontend uses normalized identity fields (not provider-specific claims). runtimeConfig.ts has Keycloak config in dev
- MUI Material-UI is the component library — all new components use MUI primitives
- React Router 6 for routing, Axios for HTTP, Vite as build tool
- TypeScript strict mode — no `any` without justification
- Provider-neutral identity: frontend should rely on normalized fields, not Keycloak/Entra-specific claims

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->
- Implemented multi-step registration wizard (RegisterPage) with MUI Stepper. Steps: plan selection, business info, account details. Auto-slugify tenantIdentifier from businessName.
- MUI Stepper components: `Stepper`, `Step`, `StepLabel`, `StepContent` for visual progress tracking. Validation happens per-step; form state managed in component.
- Subscription plan fetching from GET /admin/subscription-plans (anonymously accessible). TenantRegistrationRequest POST payload includes selectedPlanId, businessName, tenantIdentifier, email, password.
- LoginPage updated with prominent "Create Account" button linking to RegisterPage. Provider-neutral auth fields preserved (email, not Keycloak/Entra-specific claims).
- Angular-to-React inventory migration: ported Warehouses, ProductClassifications, and ProductGroups CRUD pages. All follow ProductsPage.tsx dialog+table pattern with MUI Paper/TableContainer, Snackbar toasts, LoadingSpinner, and Spanish UI text.
- New types added to types/index.ts: `Warehouse`, `ProductClassification`, `ProductGroup`, `MovementType`.
- `CreateMovementData` type changed to `Omit<ProductMovement, 'id' | 'date'>` — the `date` field is server-set and should not be sent in the payload.
- InventoryPage enhanced with "Nuevo Movimiento" dialog that loads warehouses and products from the API dynamically. State lifted within the page component; warehouses loaded in the same `Promise.all` as products/movements to avoid extra requests.
- Layout nav extended with `Store` (WarehouseIcon), `Category`, and `GroupWork` MUI icons for the three sub-inventory pages.
- Pre-existing build error: `LicensePage.tsx` imports from `'../api/license.api'` which does not exist. This is unrelated to inventory migration work and was present before this task.
