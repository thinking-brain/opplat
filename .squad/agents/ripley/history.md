## Core Context

**Project:** Opplat — Multi-platform business management system (café/restaurant)  
**Role:** Architect, senior reviewer, design validation, reviewer lockout enforcement  
**Root:** C:\projects\personal\opplat | **Branch:** develop

**Key Design Decisions:**
- **Admin-Auth-Tenant Design** — Separate admin app (port 3001) with dedicated OIDC flow, tenant-scoped backend routes, role-to-surface mapping
- **Temporary Admin Shell Mode** — Authenticated but feature-gated UI; allows auth stabilization without exposing broken business logic
- **Admin API MediatR + PostgreSQL** — Replace mock stores with MediatR handlers, dedicated postgres for admin-api; auth pipeline frozen
- **Application Layer Refactor** — Multiple Application libraries per bounded context + MediatR handlers replace IService pattern. Thin API hosts keep only startup/endpoints
- **Controller-to-MinimalAPI Conversion** — Microservices first (Sales, Inventory), MainApp Areas last. All archived with `_Archived` suffix + commented route attributes
- **Aspire Local Development** — Single AppHost orchestrating 4 backend services + SQL Server, PostgreSQL, Keycloak. Dual-mode: Aspire for dev, docker-compose for CI/prod. Frontend Vite apps remain separate.

**Patterns Established:**
- Role-to-Surface Mapping: SuperAdmin → admin portal; TenantAdmin/TenantUser → client app with admin features per tenant
- Dual route shapes preserved in MainApp (non-tenant + tenant-prefixed routes) via MapSalesGroup/MapInventoryGroup reuse
- Regression gates pin thin-host composition (no AddControllers/MapControllers), MediatR ownership, auth seams at HTTP boundary
- Aspire-safe host behavior: /health, /alive endpoints; forwarded headers in Development; conditional HTTPS redirection

**Status:** Aspire local development approved and implemented (Session 26). All 4 services ready for orchestration. Phase Gate 2 closeout pending.

---

### Session 28 Summary (2026-03-23)

**PostgreSQL Migration — Design & Approval — APPROVED**
- Designed and approved PostgreSQL-first migration shape across MainApp, Sales, Inventory
- Confirmed AdminApi remains on PostgreSQL (no change required)
- Multitenancy strategy locked: database-per-tenant via Finbuckle (no schema-per-tenant)
- Implementation guardrails documented per phase and agent (Hudson: packages, Hicks: runtime, Bishop: validation)
- Approved deferral of per-tenant schema strategy to future iteration
- Decision record merged into `.squad/decisions.md` — Session 28 PostgreSQL section
- Orchestration logs: `.squad/orchestration-log/2026-03-23T18-45-44Z-ripley.md`

---

**Aspire Local Development Architecture — APPROVED & IMPLEMENTED**
- Approved single AppHost orchestrating MainApp, AdminApi, Sales API, Inventory API
- Approved ServiceDefaults pattern: health checks, OpenTelemetry, service discovery, resilience
- Approved dual-mode operation: Aspire for dev (hot-reload), docker-compose for CI/prod (unchanged)
- Approved frontend strategy: React/Admin Vite apps remain separate (npm run dev); not orchestrated
- Assigned Hudson (setup), Hicks (runtime), Bishop (validation) with clear constraints
- Decision record merged into `.squad/decisions.md` — Session 26 Aspire section
- Orchestration logs: `.squad/orchestration-log/20260323T175012Z-ripley.md`

---

### Session Archive (Sessions 1–21, 2026-03-20–03-23)

**Sessions 19–21 Summary:**
- **Session 19 (Design Review):** Approved MediatR refactor + controller-to-minimal-API conversion strategy (Sales microservice → Inventory microservice → MainApp Areas, lowest to highest risk)
- **Session 20 (Phase Gate 1 Review):** Rejected due to defects (legacy IService injection, controllers not archived); lockout protocol enforced; Hicks excluded from remediation
- **Session 21 (Phase Gate 1 Re-Review):** Approved after defects corrected (Inventory/Sales both fully converted with archival, MediatR wiring, regression gates)

**Admin API Sessions (6–18 archive):**
Sessions covering admin API design, MediatR+PostgreSQL migration, boundary refactor, shell mode, auth simplification, and auth removal. All architectural patterns established and approved. See `.squad/orchestration-log/` for detailed outcomes.

---

---

**Session 16: Admin API MediatR + PostgreSQL Migration — APPROVED**
Reviewed and approved MediatR handlers + PostgreSQL migration. Auth pipeline & endpoint contracts frozen.

**Session 13: Admin API MediatR + PostgreSQL Architecture Review — APPROVED**
Design approval for separate postgres container, dedicated AdminDbContext, handler pattern.

**Session 12: Admin API Startup Fix Verification — ✅ CONFIRMED**
Spot-checked implementation. All tests (65/65) passing, docker-compose valid, builds green.

### Prior Sessions (Sessions 1–11, 2026-03-20 and earlier)

**Sessions 6–11:** Admin-auth-tenant design approval, BFF migration, OIDC two-client assessment, shell mode validation
**Sessions 1–5:** Initial architecture setup, module structure refactoring, design patterns

See `.squad/orchestration-log/` for detailed session outcomes and `.squad/decisions.md` for architectural decisions.

## Learnings

### Session 23 (2026-03-24): Application Layer Flatten Proposal — REJECTED → OVERRIDDEN

**Proposal:** Move module Application projects into global `Opplat.Application/{Module}/` folders.

**Initial Decision:** Rejected. Convenience does not justify breaking bounded context encapsulation.

**Override:** User explicitly directed flattening to proceed. User authority supersedes architect preference.

**Key Factors (original rejection):**
1. Cross-context coupling — Sales handlers could accidentally reference Inventory types
2. Microservice bloat — Single Application assembly forces all handlers into all hosts
3. Prior decision explicitly rejected this pattern (decisions.md @ line 2141)
4. Module autonomy matters for parallel team work

**Guardrails Set (post-override):**
- Hosts remain thin (MediatR dispatch only)
- Domain/Infrastructure stay in module folders
- Legacy IService pattern must be removed
- Architecture tests updated to match new structure
- 10-step sequencing constraint documented

**Lesson:** User is final authority. When overridden, shift role from blocking to safe execution. Document guardrails, not objections.

### Session 24 (2026-03-24): Aspire Local Development Architecture — APPROVED

**Request:** Add .NET Aspire for local development orchestration.

**Analysis:**
- Current stack: 4 API hosts (MainApp, AdminApi, Sales, Inventory), PostgreSQL + SQL Server databases, Keycloak OIDC
- Docker Compose exists and must remain functional for CI/prod-like runs

**Design Decision:**
1. **AppHost** — Single orchestrator for all 4 APIs + container resources (DBs, Keycloak)
2. **ServiceDefaults** — Shared OpenTelemetry, health checks, service discovery, resilience
3. **Dual-mode operation** — Aspire for fast dev loop, Docker Compose for CI/integration
4. **Frontend excluded** — React/Admin SPAs run via native Vite dev servers, not Aspire

**Constraints Documented:**
- Hudson: Project creation, package refs, solution updates
- Hicks: Program.cs wiring, connection string injection, Keycloak env vars
- Bishop: AppHost smoke tests, dual-mode validation

**Lesson:** Aspire integrates best when treating .NET apps as native projects and external deps (DBs, auth) as containers. Frontend SPAs benefit from their own HMR tooling.

### Session 27 (2026-03-23): PostgreSQL Migration Design — APPROVED

**Request:** Migrate all services from SQL Server to PostgreSQL.

**Analysis:**
- Current state: MainApp/Sales/Inventory use SQL Server; AdminApi already on PostgreSQL
- Multitenancy: Finbuckle with per-tenant database isolation (not schema-per-tenant)
- Npgsql packages already referenced across most projects
- Existing SQL Server migrations must be archived, not converted

**Design Decision:**
1. **Provider swap** — `UseSqlServer()` → `UseNpgsql()` in all DbContext registrations
2. **Multitenancy unchanged** — Per-tenant databases; ConfigurationStore resolves connection strings
3. **Fresh migrations** — Archive SQL Server migrations, generate new PostgreSQL-native migrations
4. **Identity columns** — Remove `UseIdentityColumns()`; let Npgsql handle auto-increment

**Phased Approach:**
- Phase 1 (Hudson): Package cleanup — remove SqlServer packages
- Phase 2 (Hicks): Runtime wiring — Program.cs, ServiceCollectionExtensions
- Phase 3 (Hicks): AppHost/Docker — connection strings, database resources
- Phase 4-5 (Bishop): Migration generation + validation

**Constraints Documented:**
- No raw SQL in handlers (flag any for review)
- Preserve connection string key names
- Database naming convention: underscore (`opplat_main` not `opplat-main`)

**Lesson:** Database provider migrations are cleanest when EF Core migrations are regenerated fresh rather than converted. Archive don't delete — rollback path must remain viable.

### Session 29 (2026-03-25): Module 1 — Identity Provider Foundation — APPROVED & IMPLEMENTED

**Request:** Implement Tenant-requirements.md Module 1.

**Analysis:**
- Module 1 has 5 sub-requirements: 3 Azure portal tasks (no code), 2 code deliverables
- OIDC authentication (1.3) was already complete via provider-neutral JwtBearer stack
- Graph API client (1.2) was completely missing — core deliverable
- `oid` claim normalization gap: Entra uses `oid`, Keycloak uses `sub`

**Design Decisions:**
1. **Graph API client in Infrastructure** — `IGraphUserService` interface in Abstractions, `GraphUserService` implementation in Infrastructure. Clean Architecture boundary preserved.
2. **Conditional registration** — `GraphApi:Enabled` flag toggles between real Graph SDK client and `NoOpGraphUserService` for local dev (Keycloak has no Graph API).
3. **OID claim normalization** — Added `NormalizeObjectId()` to `OidcClaimsNormalizer`. Entra `oid` preserved if present; Keycloak `sub` promoted to `oid` as fallback.
4. **UUID-based UPN** — `{Guid.NewGuid()}@{TenantDomain}` for collision-safe user creation.
5. **AdminApi wired first** — Graph service registered in AdminApi (super admin operations). MainApp will consume it when Module 4 (registration flow) is implemented.

**Key File Paths:**
- Interface: `src/Opplat.Application.Abstractions/Identity/IGraphUserService.cs`
- Implementation: `src/Opplat.Infrastructure/Identity/GraphUserService.cs`
- No-op: `src/Opplat.Infrastructure/Identity/NoOpGraphUserService.cs`
- Options: `src/Opplat.Infrastructure/Identity/GraphApiOptions.cs`
- Registration: `src/Opplat.Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs`
- Claim types: `src/Opplat.Application.Abstractions/Auth/AuthClaimTypes.cs` (ObjectId, Subject)
- Normalizer: `src/Opplat.MainApp/Auth/OidcClaimsNormalizer.cs` (NormalizeObjectId)

**Packages Added:** Microsoft.Graph 5.103.0, Azure.Identity 1.19.0, Microsoft.Extensions.Http.Resilience 10.0.0

**Test Results:** 128/128 passing (13 new tests added).

**Risks Documented:**
- GraphServiceClient not mockable with Moq — use contract + no-op tests
- User.ReadWrite.All is high-privilege — document in deployment runbooks
- Certificate-based auth not yet implemented (ClientSecret only)
- Retry pipeline for 429/503 not yet wired into Graph SDK HttpClient

**Lesson:** When a requirements module mixes code deliverables with manual Azure config tasks, decompose first and document the manual steps in the decision record. Don't try to automate Azure portal configuration from application code.

---

### Session 29 Summary (2026-03-25)

**Module 1 Identity Foundation — Architecture Review — ✅ APPROVED**
- Reviewed scope decomposition: 5 sub-requirements (3 manual Azure, 2 code deliverables)
- Approved GraphUserService with 429/503 retry handling
- Approved oid claim normalization for Entra tokens
- Validated AdminApi as OIDC entry point
- Validated client app integration with Aspire
- **Status:** Ready for production Entra ID configuration

**Key Approvals:**
- Entra-aware auth config/runtime resolution
- AdminApi as sole Module 1 interactive auth host
- GraphUserService for user lifecycle operations
- Client apps as Aspire resources with preserved HMR

**Session Complete:** All manual Azure tasks documented; code foundation verified.