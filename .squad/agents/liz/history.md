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
