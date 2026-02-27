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

