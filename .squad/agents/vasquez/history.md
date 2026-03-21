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

### Phase 2: React Application Created (2026-02-27)

**What was built:**
- Complete React 18 application with TypeScript at `src/opplat-react/`
- Vite-based build system for fast development
- Material-UI v5 for modern, responsive UI components
- React Router v6 for client-side routing
- Axios HTTP client with JWT interceptor
- Authentication context using React Context API
- Protected routes with automatic redirect

**Vue app patterns discovered:**
- Vue app used sessionStorage for token storage (key: "token")
- Token format: `Bearer ${jwt}` 
- Base API URL from environment: `process.env.VUE_APP_SERVICE_URL`
- Routes: /auth/login, /home, /products, /sell, /users
- Vuex store for auth state management
- JWT payload parsing to extract username and roles from claims

**API endpoints discovered from Controllers:**
- Auth: `/auth/Account/Login` (POST) - returns { token, expiration, userId }
- Auth: `/auth/Account/user-list` (GET) - returns User[]
- Auth: `/auth/Account/profile/{name}` (GET) - returns User details
- Auth: `/auth/Account/add-user` (POST) - create new user
- Auth: `/auth/Account/edit-user` (POST) - update user name/lastName
- Auth: `/auth/Account/cambiar-estado` (GET) - toggle user active status
- Auth: `/auth/Account/cambiar-roles` (POST) - update user roles
- Products: `/sales/Products` (GET/POST/PUT/DELETE) - CRUD operations
- Sales: `/Sales` (GET/POST) - list and create sales

**Key implementation details:**
- Used localStorage instead of sessionStorage for token persistence
- Token stored as `opplat_token` with Bearer prefix
- User data stored as `opplat_user` (JSON)
- JWT claims: unique_name for username, role claim for roles array
- All API requests automatically include Authorization header via interceptor
- 401 responses trigger automatic logout and redirect to /login
- MUI DataGrid-style tables for Products and Users pages
- POS-style cart interface for Sell page with product selection and checkout
- All pages fully functional with create, edit, delete operations

**Technology decisions:**
- Vite over Create React App (faster, more modern)
- Material-UI v5 for consistent, professional UI
- React Context for auth state (simpler than Redux for this scale)
- Axios for HTTP client (familiar, good interceptor support)
- TypeScript for type safety and better developer experience
- Functional components with hooks (modern React patterns)

### Phase 2b: Feature Parity with Vue App (2026-02-27)

**Gap analysis completed:**
After comparing Vue and React apps side-by-side, identified specific missing features:

**ProductsPage gaps fixed:**
1. Added search TextField that filters product list by name (client-side)
2. Added activate/deactivate toggle buttons (Habilitar/Deshabilitar) per row
3. Added image column showing 60x60 Avatar with product image
4. Added file upload button for product images using hidden file input + ref
5. Replaced window.confirm with proper MUI Dialog for delete confirmation
6. Added History icon button (placeholder for future feature)
7. Edit button now disabled when product is inactive

**SellPage enhancements:**
1. Added "Detalles de Venta" Card with sale-level fields matching Vue:
   - Dependiente (Autocomplete with mock staff names)
   - Posición (Autocomplete with table positions: Mesa 1-5, Barra, Para Llevar)
   - Comanda (TextField for order reference)
   - Observaciones (multiline TextField for notes)
2. Renamed "Complete Sale" button to "Registrar Venta"
3. Added loading state to checkout button (submitting variable)
4. Changed "Total" label to "Importe Total" for consistency
5. All labels translated to Spanish

**UsersPage gaps fixed:**
1. Added profile picture column with Avatar component (shows first letter if no image)
2. Added Delete button with trash icon per row
3. Added confirm delete Dialog matching ProductsPage pattern
4. Added tooltips to all action buttons and Active switch for clarity
5. All labels translated to Spanish (Usuarios, Agregar Usuario, etc.)

**API updates:**
- products.api.ts: Added toggleActive and uploadImage methods
- uploadImage uses FormData with multipart/form-data header
- users.api.ts: Added delete method

**Type updates:**
- ProductForSale: Added active and imageUrl optional fields
- User: Added profilePicture optional field

**Key patterns established:**
- Confirm dialogs use consistent pattern: state for deletingId, open/close handlers, confirm action
- File uploads use hidden input + ref + click trigger pattern
- Image URLs constructed as `/api/uploads/${filename}`
- Tooltips on all icon buttons for accessibility
- Spanish labels throughout matching Vue app exactly
- Active state affects button states (Edit disabled when inactive)

### Phase 3a: Admin Frontend Recovery (2026-03-20)

**What was recovered:**
- Rebuilt `src/opplat-admin` as a Vite + React 18 + TypeScript + MUI admin portal
- Added provider-agnostic OIDC wiring with `react-oidc-context` and shared claim parsing patterns
- Implemented MVP pages for Login, Dashboard, Tenants, Users, and Settings
- Wired tenant CRUD to `/admin/tenants` and tenant-scoped user management to `/admin/tenants/{tenantIdentifier}/users`
- Validated the admin bundle with `npm run build`

**Admin app implementation notes:**
- Admin auth mirrors the client OIDC flow: redirect login, silent renew, logout fallback, and bearer token interceptor
- Tenant selection is stored locally as `opplat_admin_tenant` and only applied to tenant-scoped admin user endpoints through `X-Tenant-Identifier`
- Dashboard is API-backed and computes summary metrics client-side from tenants and admin user listings
- Settings is currently an environment/session reference page instead of calling non-existent backend settings endpoints
- The local build script explicitly invokes shared toolchain binaries from `src/opplat-react/node_modules` to keep `npm run build` reliable in this workspace

### Phase 3b: Admin App MVP Delivery (2026-03-20)
**What was added:**
- Built the missing admin UI shell (Layout, navigation, and theming) in `src/opplat-admin`
- Implemented MVP pages: Login, Dashboard, Tenants, Users, Settings
- Connected admin pages to `/admin/tenants`, `/admin/users`, and `/admin/settings` via the admin API client
- Added MUI dialogs for tenant create/edit/deactivate and user role updates
- Confirmed `npm run build` succeeds in `src/opplat-admin`

**Notes:**
- Admin login follows the provider-agnostic OIDC flow used by the client app
- Dashboard aggregates tenant + user counts with API-backed summaries

### Phase 3c: Client/Admin OIDC Alignment (2026-03-20)
**What changed:**
- Migrated `src/opplat-react` from custom JWT login to `react-oidc-context` / `oidc-client-ts`
- Normalized Auth0 namespaced claims and Keycloak flat claims into one frontend auth model
- Persisted the derived tenant identifier and applied it to tenant-scoped API routes plus `X-Tenant-Identifier`
- Added runtime `runtime-config.js` loading so nginx containers can consume compose env vars without a rebuild
- Revalidated both frontends with `npm run lint` / `npm run build`

**Implementation notes:**
- Client callback handling now uses `/auth/callback` and `/auth/silent-renew`
- Axios reads access tokens from OIDC-managed storage instead of custom local JWT keys
- Admin package scripts call local toolchain entrypoints directly because this shared Windows workspace had intermittent `.bin` resolution issues after package installation

### Phase 3d: Role-gated Keycloak alignment (2026-03-21)
**What changed:**
- Canonicalized frontend roles to `SuperAdmin`, `TenantAdmin`, and `TenantUser` in both React apps via `src/opplat-admin/src/auth/roles.ts` and `src/opplat-react/src/auth/roles.ts`
- Gated `src/opplat-admin` behind `SuperAdmin` and moved tenant user administration responsibility to `src/opplat-react/src/pages/UsersPage.tsx`
- Updated `docker/keycloak/opplat-realm.json` with seeded SuperAdmin / TenantAdmin / TenantUser accounts plus localhost redirect URIs for ports 3000/3001, 3100/3101, and 3200/3201
- Reduced the default SPA OIDC scope to `openid` and disabled `loadUserInfo` in both apps to avoid local Keycloak scope mismatches

**Implementation notes:**
- `ProtectedRoute` now supports role-based gating plus an unauthorized-state UI in both frontends
- The client `Users` route is visible and accessible only to `TenantAdmin`; `TenantUser` sees the rest of the tenant app without user-management entry points
- Root docs at `README.md` now document the seeded Keycloak users, role model, and the local scope change

### 2026-03-21: Frontend Auth Gating & OIDC Scope Alignment (Session 5)

**Task:** Implement role-based auth gating for frontend applications and align OIDC scope requests with Keycloak configuration.

**Work Performed:**
1. **Frontend Route/Component Gating**
   - `opplat-admin`: All root and nested admin routes now require `SuperAdmin` role
   - `opplat-react`: `/users` route, Users nav item, and dashboard quick-access gated to `TenantAdmin` role
   - Both apps now properly enforce role-based access control

2. **OIDC Scope Configuration**
   - Both frontends now request: `openid profile email roles`
   - Preserved extras like `offline_access` for offline session support
   - Removed audience query param for Keycloak realm URLs (kept for custom providers)

3. **Claims Parsing Enhancement**
   - Roles now extracted from both ID/access tokens
   - Includes `realm_access.roles` and `resource_access.*.roles` from Keycloak claims
   - Claims parsing properly handles both Auth0 and Keycloak token structures

4. **Documentation Updates**
   - Root README.md now explains frontend auth flow
   - Added role expectations documentation
   - Documented SuperAdmin/TenantAdmin/TenantUser access patterns

5. **Validation Results**
   - ✅ `src/opplat-admin`: `npm run lint` PASS, `npm run build` PASS
   - ✅ `src/opplat-react`: `npm run lint` PASS, `npm run build` PASS
   - ✅ Both apps build without errors; pre-existing dev dependencies stable

**Key Changes:**
- `src/opplat-admin/src/auth/oidc.ts` → scope request aligned
- `src/opplat-admin/src/auth/claims.ts` → role claim parsing enhanced
- `src/opplat-admin/src/runtimeConfig.ts` → runtime config ready
- `src/opplat-react/src/auth/oidc.ts` → scope request aligned
- `src/opplat-react/src/auth/claims.ts` → role claim parsing enhanced
- `src/opplat-react/src/runtimeConfig.ts` → runtime config ready
- README.md → auth flow and role documentation

**Status:** ✅ COMPLETE — Both frontend apps properly gated, scopes aligned, builds passing

### 2026-03-21: Runtime Scope Mismatch Fix (Session 6)

**Task:** Diagnose the `Invalid scopes: openid profile email roles offline_access` login failure and align runtime/frontend scope handling.

**Work Performed:**
1. Inspected both SPAs' runtime scope sources:
   - `src/opplat-admin/src/runtimeConfig.ts`
   - `src/opplat-admin/src/auth/oidc.ts`
   - `src/opplat-admin/.env.example`
   - `src/opplat-admin/Dockerfile`
   - `src/opplat-react/src/runtimeConfig.ts`
   - `src/opplat-react/src/auth/oidc.ts`
   - `src/opplat-react/.env.example`
   - `src/opplat-react/Dockerfile`
2. Confirmed the actual requested scope set was being expanded in two places:
   - runtime config defaulted to `openid ... roles`
   - OIDC setup force-added `profile email offline_access`
3. Reduced the shared default request to `openid` only in both apps and kept `oidc-client-ts` from re-appending invalid extras.
4. Updated root documentation to match the new runtime behavior.
5. Revalidated both SPAs after the fix.

**Validation Results:**
- ✅ `src/opplat-admin`: `npm run lint` PASS, `npm run build` PASS
- ✅ `src/opplat-react`: `npm run lint` PASS, `npm run build` PASS

**Key Learning:**
- For these SPAs, the effective OIDC scope comes from the combination of runtime-config defaults, `.env` examples, Docker runtime injection, and `oidc.ts`; all four must agree or login can still request stale invalid scopes.

### 2026-03-21: Scope Contract Adjudication & Final Resolution (Team Sync)

**Cross-Agent Coordination:** Ripley synthesized findings from Hicks, Vasquez, and Bishop's independent investigations into a single authoritative scope contract.

**What Happened:**
1. **Hicks** confirmed the Keycloak realm export was correct and identified Docker Compose wiring as the gap.
2. **Vasquez** found that both SPAs were requesting overly broad scopes and aligned them to openid only (later refined).
3. **Bishop** created regression test harnesses and confirmed the drift between intended contract and frontend implementation.
4. **Ripley** adjudicated the conflict, ruling that:
   - The correct contract is openid profile email offline_access (NOT just openid, and NOT including oles)
   - oles must NEVER be in the scope request (Keycloak injects via defaultClientScopes automatically)
   - Your openid-only fix was too minimal (loses claims and refresh tokens)
   - All frontend untimeConfig.ts, .env.example, and README examples must align with Docker Compose's environment

**Key Learnings:**
- Keycloak does not expose oles as a requestable scope; it's a mapper configuration attached as a default client scope.
- Requesting oles in the scope parameter triggers "Invalid scopes" error.
- Scope configuration is multi-layer: changes must coordinate across Docker Compose, runtimeConfig, .env files, Dockerfile, and documentation.

**Files Updated:**
- src/opplat-react/src/runtimeConfig.ts — scope → openid profile email offline_access
- src/opplat-admin/src/runtimeConfig.ts — scope → openid profile email offline_access
- src/opplat-react/.env.example — documented correct scope
- src/opplat-admin/.env.example — documented correct scope
- README.md — local dev examples and env reference table aligned

**Test Harnesses:** Both FrontendAuthContractTests and KeycloakRealmContractTests are now permanent regression guards for scope contracts.

**Status:** ✅ COMPLETE — Your frontend scope alignment was correct in principle but too minimal in scope set. Team consensus finalized and applied.
