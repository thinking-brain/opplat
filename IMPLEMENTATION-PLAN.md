# Opplat Tenant Administration System — Implementation Plan

> **Generated:** 2026-03-30
> **Based on:** `Tenant-requirements.md` v1.2
> **Scope:** Auth, Client/User Management, and Tenant system requirements (Modules 1–10)

---

## Executive Summary

The codebase has **strong foundational coverage** for Modules 1–3 (Identity, Schema, Provisioning). Module 4+ features are in early stages — command handlers exist but many are stubbed (`throw new NotImplementedException()` or commented-out code). The auth layer supports both Keycloak (local dev) and Entra ID (production) with a clean provider abstraction, but registration, billing, IAM, and notification flows are largely unbuilt.

---

## Coverage Legend

| Symbol | Meaning |
|--------|---------|
| ✅ | Fully implemented and functional |
| 🟡 | Partially implemented — structure exists, logic incomplete |
| 🔴 | Not implemented — needs to be built from scratch |

---

## Module 1 — Identity Provider Foundation

### 1.1 Entra ID App Registration — ✅ Configuration Ready

| What exists | Where |
|---|---|
| `GraphApiOptions` with TenantId, ClientId, ClientSecret, CertificateThumbprint | `Infrastructure/Identity/GraphApiOptions.cs` |
| `AuthOptions.Entra` for tenant, authority host, v2 endpoint, oid claim | `Application.Abstractions/Options/AuthOptions.cs` |
| DI registration via `AddGraphUserService` with conditional real/no-op binding | `Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs` |

**Status:** Configuration model is complete. The actual Azure portal registration (manual step) and `User.ReadWrite.All` permission grant are deployment tasks documented via `GraphApiOptions`.

**Action needed:** None at code level. Ensure deployment docs include the portal registration steps and secret rotation procedure.

---

### 1.2 Microsoft Graph API Client — ✅ Fully Implemented

| Requirement | Implementation | Status |
|---|---|---|
| Client-credentials flow | `ClientSecretCredential` in DI registration | ✅ |
| Create user with collision-safe UPN `{uuid}@domain` | `GraphUserService.CreateUserAsync` — generates UPN, stores real email in `Mail`+`OtherMails` | ✅ |
| Enable user | `GraphUserService.EnableUserAsync` — PATCH `accountEnabled: true` | ✅ |
| Disable user | `GraphUserService.DisableUserAsync` — PATCH `accountEnabled: false` | ✅ |
| Delete user | `GraphUserService.DeleteUserAsync` — DELETE | ✅ |
| Password reset | `GraphUserService.ResetPasswordAsync` — `forceChangePasswordNextSignIn: true` | ✅ |
| Async with error handling | All methods async, `ODataError` catch blocks | ✅ |
| Retry logic (429, 503) | `ExecuteWithRetryAsync` with exponential backoff, configurable retries | ✅ |
| No-op stub for local dev | `NoOpGraphUserService` + `NoOpKeycloakUserService` | ✅ |

**Keycloak dual-provider support (bonus):** `IKeycloakUserService` with `KeycloakUserService` implementation exists for local development — creates/deletes users via Keycloak Admin REST API.

**Action needed:** None. This module is production-ready.

---

### 1.3 OIDC Authentication Endpoint — ✅ Fully Implemented

| Requirement | Implementation | Status |
|---|---|---|
| Entra ID as OIDC provider | `AuthRuntimeConfigurationResolver` auto-builds authority, metadata, issuers from `Auth:Entra` config | ✅ |
| JWT validation | JWT Bearer handler with `ValidateIssuer`, `ValidateAudience`, `ValidateLifetime` | ✅ |
| Extract `oid` claim | `OidcClaimsNormalizer.NormalizeObjectId` — resolves `oid` or falls back to `sub` | ✅ |
| Token return to client | Admin BFF session endpoint returns access token; bearer path forwards token | ✅ |
| BFF cookie auth for admin portal | Cookie + OIDC scheme with PKCE, CSRF antiforgery middleware | ✅ |
| Dual auth scheme (cookie or bearer) | `AdminApiAuth` policy scheme selects by `Authorization` header presence | ✅ |

**Action needed:** None. Auth pipeline is complete.

---

### 1.4 MFA Enforcement — 🟡 Portal Configuration

**Status:** This is an Entra ID Conditional Access policy or Security Defaults toggle — not application code. The `AuthOptions.Entra` config acknowledges that MFA/SSPR are "portal/policy configuration" via XML doc comment.

**Action needed:** Document the Conditional Access policy setup in a deployment runbook. No code changes required.

---

### 1.5 Self-Service Password Reset (SSPR) — 🟡 Portal Configuration

**Status:** Same as 1.4 — Entra ID admin portal configuration.

**Action needed:** Document SSPR enablement steps in the deployment runbook.

---

## Module 2 — Core Database Schema & Tenant Model

### 2.1 Application Database — Core Tables — ✅ Fully Implemented

| Entity | File | Status |
|---|---|---|
| `SubscriptionPlan` | `Domain/Entities/Administration/SubscriptionPlan.cs` | ✅ |
| `Tenant` | `Domain/Entities/Administration/Tenant.cs` | ✅ |
| `TenantUser` | `Domain/Entities/Administration/TenantUser.cs` | ✅ |
| `DatabaseInstance` | `Domain/Entities/Administration/DatabaseInstance.cs` | ✅ |
| `AuditLog` | `Domain/Entities/Administration/AuditLog.cs` | ✅ |
| EF Configurations | `Infrastructure/Persistance/Configurations/Administration/` (5 files) | ✅ |
| `AdminTenantCatalogDbContext` | Registers all 5 `DbSet<>` properties | ✅ |
| Seed data | `Module2DataSeeder` — 3 subscription plans + initial DB instance | ✅ |

**All required fields are present:**
- `subscription_plans`: name, max users, API calls, storage, pricing, JSON `ResourceLimits`
- `tenants`: identifier, name, status (Active/Inactive), plan ID, creation date, inactivation date, database instance, schema
- `tenant_users`: `EntraOid`, tenant ID, email, role enum (PrimaryAdmin/Admin/User), `IsPrimaryAdmin`, active status
- `database_instances`: identifier, connection string reference, tenant schema count, status (Active/Archived)
- `audit_logs`: actor OID, target tenant, target user, action type, before/after state (JSONB), timestamp

**Action needed:** None. Schema is complete and aligned with requirements.

---

### 2.2 Resource Limit Definitions — ✅ Implemented

**Status:** `SubscriptionPlan.ResourceLimits` is a `jsonb` column storing a flexible key-value map. `Module2DataSeeder` populates it with `max_active_users`, `api_calls_per_month`, `storage_quota_gb`. New resource types can be added without schema changes.

**Action needed:** None.

---

### 2.3 Database Instance Sharding Configuration — ✅ Implemented

| Requirement | Implementation | Status |
|---|---|---|
| Configurable threshold | `DatabaseInstanceOptions.MaxTenantsPerInstance` (default: 100) | ✅ |
| Stored in configuration, not hardcoded | Bound from `DatabaseInstance` config section | ✅ |
| Auto-provision new instance when threshold reached | `DatabaseInstanceAutoScalingService.EnsureAvailableInstanceAsync` | ✅ |

**Action needed:** None.

---

## Module 3 — Tenant Provisioning Engine

### 3.1 Schema Provisioning — ✅ Fully Implemented

| Requirement | Implementation | Status |
|---|---|---|
| Create dedicated schema per tenant | `TenantSchemaProvisioningService.ProvisionTenantSchemaAsync` | ✅ |
| Idempotent (re-run safe) | Checks `SchemaExistsAsync` before CREATE | ✅ |
| Schema drop for offboarding | `DropTenantSchemaAsync` | ✅ |
| Ensure database exists | `EnsureDatabaseExistsAsync` method | ✅ |

**Action needed:** None.

---

### 3.2 Database Instance Auto-Scaling — ✅ Fully Implemented

| Requirement | Implementation | Status |
|---|---|---|
| Check count against threshold | `EnsureAvailableInstanceAsync` compares current count vs `MaxTenantsPerInstance` | ✅ |
| Auto-provision new instance | `ProvisionNewDatabaseInstanceAsync` creates new DB, registers in catalog | ✅ |
| Increment/decrement counts | `IncrementTenantCountAsync` / `DecrementTenantCountAsync` | ✅ |
| Recalculate counts | `RecalculateTenantCountsAsync` | ✅ |

**Action needed:** None.

---

### 3.3 Schema Migration Strategy — ✅ Fully Implemented

| Requirement | Implementation | Status |
|---|---|---|
| Per-tenant migration | `TenantSchemaMigrationRunner.ExecuteMigrationForTenantAsync` | ✅ |
| Bulk migration | `ExecuteMigrationBulkAsync` with configurable batch size | ✅ |
| Phased/rolling execution | Batches with configurable `delayBetweenBatches` | ✅ |
| Per-tenant success/failure logging | `TenantMigrationResult` per tenant, reporter pattern | ✅ |
| Rollback support | `TryRollbackAsync` runs `rollbackSql` on failure | ✅ |
| Migration log tracking | `LogMigrationExecutionAsync` interface defined | ✅ |
| HTTP endpoints | `POST /admin/core/migrations/tenants/{id}` and `POST /admin/core/migrations/bulk` | ✅ |

**Action needed:** None. Fully operational.

---

## Module 4 — User Registration & Tenant Activation

### 4.1 Registration Flow — 🔴 Mostly Stubbed

**What exists:**
- `RegisterUserCommand` + handler — creates user in Keycloak only (not Entra ID)
- `POST /auth/register` endpoint with validation
- `CreateTenantCommand` + handler — has DB context and provisioning coordinator injected but **`throw new NotImplementedException()`**

**What's missing (all work items):**

| # | Work Item | Priority |
|---|---|---|
| 1 | **Full registration orchestrator** — a new command/service that coordinates: collect user details → payment initiation → Graph API user create → tenant + tenant_user record creation → schema provisioning → set Active + PrimaryAdmin → notification | Critical |
| 2 | **Payment provider integration** — abstract `IPaymentService` + implementation (Stripe, etc.) | Critical |
| 3 | **Transactional rollback** — if any step fails, disable/delete the Entra user, reverse DB records | Critical |
| 4 | **Implement `CreateTenantCommandHandler.Handle`** — currently throws `NotImplementedException` | Critical |
| 5 | **Implement `UpdateTenantCommandHandler.Handle`** — currently throws `NotImplementedException` | High |
| 6 | **Implement `DeactivateTenantCommandHandler.Handle`** — currently throws `NotImplementedException` | High |
| 7 | **Wire registration to use `IGraphUserService`** instead of only Keycloak — the current `RegisterUserCommandHandler` only calls `IKeycloakUserService` | High |

---

### 4.2 Tenant Routing Middleware — 🟡 Partially Implemented

**What exists:**
- `TenantValidationMiddleware` in MainApp — validates tenant from route/header/claim, rejects Inactive tenants with 403
- `TenantCatalogStore` — multi-tenant store using Finbuckle, looks up tenants from central catalog (file or DB)
- `PostgresTenantConnectionStringResolver` — resolves per-tenant connection string
- Cross-tenant claim validation — blocks requests where token tenant_id doesn't match route tenant

**What's missing:**

| # | Work Item | Priority |
|---|---|---|
| 1 | **OID-based tenant resolution** — current middleware resolves tenant from route/header/claim but does NOT look up tenant by `oid` from `tenant_users` table as required by spec (Module 4.2 step 3) | High |
| 2 | **Connection pooling integration** — middleware retrieves connection strings but no pool management (see Module 10.1) | Medium |
| 3 | **Inject resolved tenant context** — currently uses Finbuckle `IMultiTenantContextAccessor` which is functional but doesn't inject the full resolved context (database instance, schema) explicitly | Low |

---

### 4.3 Primary Admin Protections — 🟡 Partial

**What exists:**
- `TenantUser.IsPrimaryAdmin` flag in domain model
- `TenantUserRole.PrimaryAdmin` role enum
- `Module2CatalogSync.EnsurePrimaryAdminAsync` ensures a primary admin exists for every synced tenant

**What's missing:**

| # | Work Item | Priority |
|---|---|---|
| 1 | **Delete/revoke guard** — no enforcement preventing non-SuperAdmin users from deleting or revoking the PrimaryAdmin. Needs a domain rule or middleware check. | High |
| 2 | **Clear error response** when attempting to modify the PrimaryAdmin without authority | High |

---

## Module 5 — Subscription & Billing Management

### 5.1 Payment Failure Grace Period — 🔴 Not Implemented

| # | Work Item | Priority |
|---|---|---|
| 1 | **Payment retry engine** — configurable retry count/interval, scheduled job | Critical |
| 2 | **Grace period policy options** — retry count, interval, grace window as configuration | Critical |
| 3 | **Auto-disable on exhaustion** — disable all tenant users in Entra + set tenant Inactive | Critical |
| 4 | **Payment failure notification trigger** (depends on Module 7) | High |

---

### 5.2 Deliberate Cancellation — 🔴 Not Implemented

| # | Work Item | Priority |
|---|---|---|
| 1 | **Cancellation command** — disable all Entra users, set tenant Inactive, trigger notification | High |
| 2 | **Cancellation endpoint** in AdminApi | High |

---

### 5.3 Resource Monitoring & Enforcement — 🟡 Data Model Ready

**What exists:**
- `SubscriptionPlan` has `MaxActiveUsers`, `MaxApiCallsPerMonth`, `MaxStorageGb` + extensible `ResourceLimits` JSON
- `Tenant.UserCount` computed property

**What's missing:**

| # | Work Item | Priority |
|---|---|---|
| 1 | **Seat-limit enforcement at invitation point** — check active user count vs plan limit before allowing user creation | High |
| 2 | **Per-resource-type independent enforcement logic** | Medium |
| 3 | **API call / storage tracking infrastructure** | Medium |

---

### 5.4 Subscription Tier Upgrade — 🔴 Not Implemented

| # | Work Item | Priority |
|---|---|---|
| 1 | **Upgrade command** — update plan on tenant, apply new limits immediately | Medium |
| 2 | **Confirmation notification** | Medium |

---

### 5.5 Subscription Tier Downgrade Guard — 🔴 Not Implemented

| # | Work Item | Priority |
|---|---|---|
| 1 | **Downgrade command** with pre-check — compare active user count to target plan's seat limit | Medium |
| 2 | **Block with conflict message** if users exceed target limit | Medium |
| 3 | **Confirmation notification** on success | Medium |

---

### 5.6 Approaching Limit Notifications — 🔴 Not Implemented

| # | Work Item | Priority |
|---|---|---|
| 1 | **Threshold check service** — fires at 80% and 100% of seat limit | Medium |
| 2 | **Notification trigger** (depends on Module 7) | Medium |

---

## Module 6 — Tenant-Level IAM (Roles, Permissions & User Management)

### 6.1 Role & Permission Model — 🟡 Partially Implemented

**What exists:**
- `AuthRoles`: SuperAdmin, TenantAdmin, TenantUser
- `TenantUserRole` enum: PrimaryAdmin, Admin, User
- `AuthRoles.TenantAssignable` array for validation
- Claims normalization resolves roles from multiple claim types

**What's missing:**

| # | Work Item | Priority |
|---|---|---|
| 1 | **Granular permission model** — document and implement fine-grained permissions beyond role-level (e.g., `CanManageInventory`, `CanViewReports`) | Medium |
| 2 | **Permission documentation** | Medium |

---

### 6.2 User Invitation Flow — 🔴 Not Implemented

**What exists:**
- `CreateTenantUserCommand` handler skeleton — validates roles, creates local `User` record, but Identity (`UserManager`) code is fully commented out. Does NOT create user in Entra/Keycloak.

**What's missing:**

| # | Work Item | Priority |
|---|---|---|
| 1 | **Invitation entity** — `TenantInvitation` with token, expiry, status, target email, role | High |
| 2 | **Invitation send flow** — generate invite, send email/notification, enforce seat limit | High |
| 3 | **Invitation accept flow** — validate token, check expiry, check seat limit, create Entra user via Graph, store `oid` + role in `tenant_users` | High |
| 4 | **Resend expired invitation** | Medium |
| 5 | **Reject acceptance after seat limit reached** with clear message | High |
| 6 | **Configurable invitation expiry duration** | Medium |

---

### 6.3 Role & Permission Management — 🟡 Partial

**What exists:**
- `ChangeRolesCommand` — listed in the features but implementation code references commented-out UserManager

**What's missing:**

| # | Work Item | Priority |
|---|---|---|
| 1 | **Working role assignment/revocation** — update `TenantUser.Role` via catalog DB, not per-tenant Identity DB | High |
| 2 | **Audit log entry** on every role change | High |
| 3 | **PrimaryAdmin self-protection** (see Module 4.3) | High |

---

### 6.4 Session Invalidation on Tenant Deactivation — 🔴 Not Implemented

| # | Work Item | Priority |
|---|---|---|
| 1 | **Token revocation or block-refresh mechanism** when tenant goes Inactive | High |
| 2 | **Short token lifetime + refresh token gating** — if refresh token is requested for an Inactive tenant, deny | High |
| 3 | `TenantValidationMiddleware` already rejects requests for Inactive tenants (403), which provides a basic layer, but active sessions remain alive until token expiry | Medium |

---

### 6.5 Audit Logging — Tenant Actions — 🟡 Schema Ready, Logic Missing

**What exists:**
- `AuditLog` entity with all required fields (actor, target, action, before/after JSONB, timestamp)
- `AuditLogConfiguration` EF mapping
- `DbSet<AuditLog>` on `AdminTenantCatalogDbContext`

**What's missing:**

| # | Work Item | Priority |
|---|---|---|
| 1 | **Audit log service/interceptor** — an `IAuditLogService` that writes logs on admin actions | High |
| 2 | **Integration points** — call audit service from every admin command handler (user invite, role change, subscription mod, enable/disable) | High |
| 3 | **Immutability enforcement** — disable UPDATE/DELETE on `audit_logs` table at DB level | Medium |

---

## Module 7 — Notification System

### 7.1 Notification Delivery — 🟡 Basic Infrastructure Exists

**What exists:**
- `Notification` + `UserNotification` models in domain (per-tenant DB)
- `NotificationsHub` (SignalR) for real-time in-app notifications in MainApp
- `SmtpSettings` model exists in Domain
- `EmailSender` service in Application

**What's missing:**

| # | Work Item | Priority |
|---|---|---|
| 1 | **Decoupled notification service** — current hub is coupled to MainApp. Need an `INotificationService` abstraction usable from AdminApi/Application layer | High |
| 2 | **Event queue / pub-sub integration** — notifications should be triggered via events, not direct calls | Medium |
| 3 | **Email delivery channel** — `EmailSender` exists but needs to be wired for tenant admin notifications | Medium |

---

### 7.2 Required Notification Events — 🔴 Not Implemented

All 8 notification event types are unimplemented:

| # | Event | Priority |
|---|---|---|
| 1 | Tenant activation confirmed | High |
| 2 | Payment failed (retry #, next retry date) | High |
| 3 | Final access restriction after grace period | High |
| 4 | Active user count at 80% of seat limit | Medium |
| 5 | Active user count at 100% of seat limit | Medium |
| 6 | Subscription upgrade confirmed | Medium |
| 7 | Subscription downgrade confirmed | Medium |
| 8 | Subscription downgrade blocked (with reason) | Medium |

---

## Module 8 — Super Admin Console

### 8.1 Global Tenant Management — 🟡 Partially Implemented

**What exists:**
- `opplat-admin` React frontend (separate SPA)
- Admin BFF with cookie + bearer auth
- `GET /admin/tenants`, `GET /admin/core/tenants`, `GET /admin/subscription-plans`, `GET /admin/database-instances` — list endpoints work
- `POST /admin/tenants` — create tenant (handler throws NotImplementedException)
- `PUT /admin/tenants/{id}` — update (throws NotImplementedException)
- `DELETE /admin/tenants/{id}` — deactivate (throws NotImplementedException)
- Session/CSRF management endpoints
- Schema provisioning + migration endpoints work

**What's missing:**

| # | Work Item | Priority |
|---|---|---|
| 1 | **Implement `CreateTenantCommandHandler`** — actual tenant creation logic | Critical |
| 2 | **Implement `UpdateTenantCommandHandler`** — actual update logic | High |
| 3 | **Implement `DeactivateTenantCommandHandler`** — set Inactive + disable Entra users | High |
| 4 | **View subscription/billing status per tenant** — endpoint + frontend | Medium |
| 5 | **Activate tenant from deactivated state** — endpoint | Medium |

---

### 8.2 Support Intervention — Tenant User Management — 🔴 Not Implemented

| # | Work Item | Priority |
|---|---|---|
| 1 | **GET tenant user profile** — SuperAdmin queries `tenant_users` by OID or tenant | High |
| 2 | **Trigger password reset** — SuperAdmin calls `IGraphUserService.ResetPasswordAsync` for any user | High |
| 3 | **Enable/disable user** — SuperAdmin updates `tenant_users.IsActive` + calls Graph API `Enable/DisableUserAsync` | High |
| 4 | **Modify role assignments** — SuperAdmin updates `TenantUser.Role` in the catalog | High |
| 5 | **Explicit no-impersonation** — ensure no session-as-user capability exists (out of scope) | Low |

---

### 8.3 Audit Logging — Super Admin Actions — 🟡 Schema Ready

**Status:** Same `AuditLog` entity covers Super Admin actions. Missing the write logic (see Module 6.5).

**Action needed:** Extend the audit service to cover Super Admin operations with same schema.

---

## Module 9 — Data Governance & Retention

### 9.1 Tenant Data Isolation — 🟡 Enforced at Middleware Level

**What exists:**
- `TenantValidationMiddleware` blocks mismatched token tenant claims
- Per-tenant connection string resolution via `PostgresTenantConnectionStringResolver`
- Schema-per-tenant isolation in PostgreSQL

**What's missing:**

| # | Work Item | Priority |
|---|---|---|
| 1 | **Formal verification** — integration tests proving cross-tenant data inaccessibility | High |
| 2 | **PostgreSQL row-level security or role-based schema access** as defense-in-depth | Medium |

---

### 9.2 Retention Policy for Inactive Tenants — 🔴 Not Implemented

| # | Work Item | Priority |
|---|---|---|
| 1 | **Retention policy configuration** — minimum 1 year from inactivation date | Medium |
| 2 | **Scheduled job to enforce** — prevent any automated deletion within retention window | Medium |
| 3 | `Tenant.InactivatedAt` field exists (✅) to support date tracking | — |

---

### 9.3 Cold Storage Archival — 🔴 Not Implemented

| # | Work Item | Priority |
|---|---|---|
| 1 | **Identify eligible DB instances** — all tenant schemas are Inactive + within retention | Low |
| 2 | **Cold storage migration process** — move to Azure Cool/Archive Blob or equivalent | Low |
| 3 | **Define recovery-time SLA** | Low |

---

### 9.4 Tenant Offboarding & Data Deletion — 🔴 Not Implemented

| # | Work Item | Priority |
|---|---|---|
| 1 | **Deletion request entity + command** | Medium |
| 2 | **Full data purge workflow** — app DB records, tenant schema, Entra ID users | Medium |
| 3 | **GDPR DSAR support process** | Medium |

---

### 9.5 Regulatory Compliance Documentation — 🔴 Not Created

| # | Work Item | Priority |
|---|---|---|
| 1 | **Data residency documentation** | Low |
| 2 | **Legal basis for retention** | Low |
| 3 | **DSAR response process** | Low |

---

## Module 10 — Infrastructure & Performance Hardening

### 10.1 Connection Pooling — 🔴 Not Implemented

| # | Work Item | Priority |
|---|---|---|
| 1 | **PgBouncer or app-level pool strategy** for dynamic per-tenant connections | High |
| 2 | **Load testing** before production | High |

---

### 10.2 Token & Session Configuration — 🟡 Partially Configured

**What exists:**
- `AdminBffOptions.SessionHours` configures cookie expiry (default 8h)
- Sliding expiration on admin cookies
- `MemoryCacheTicketStore` for server-side session storage
- JWT `ClockSkew` set to 30 seconds

**What's missing:**

| # | Work Item | Priority |
|---|---|---|
| 1 | **Refresh token rotation** — enforce rotation on every use | Medium |
| 2 | **Refresh token expiry** — explicit configuration | Medium |
| 3 | **Max concurrent sessions policy** — optional, evaluate need | Low |

---

### 10.3 Provisioning Resilience — 🟡 Partially Covered

**What exists:**
- `TenantProvisioningCoordinator` wraps provisioning in try/catch, populates error result
- `ITenantProvisioningReporter` pattern for reporting success/failure
- Schema provisioning is idempotent (safe to retry)

**What's missing:**

| # | Work Item | Priority |
|---|---|---|
| 1 | **Resume/retry for partially failed provisioning** — currently fails and reports, no auto-resume | Medium |
| 2 | **Admin UI for retrying failed provisions** — SuperAdmin triggers re-provision (endpoint exists: `POST /admin/core/tenants/{id}/provision`) | Low |

---

## Prioritized Implementation Roadmap

### Phase 1 — Complete Core Write Operations (Critical)

These are blocking all downstream modules:

1. **Implement `CreateTenantCommandHandler`** — wire up tenant creation with plan assignment, database instance allocation, and schema provisioning
2. **Implement `UpdateTenantCommandHandler`** — update tenant metadata
3. **Implement `DeactivateTenantCommandHandler`** — set Inactive, disable Entra users
4. **Fix `CreateTenantUserCommand` handler** — uncomment/rewrite Identity logic to use `IGraphUserService` + `tenant_users` catalog table instead of per-tenant ASP.NET Identity
5. **Implement `UpdateTenantUserCommand` handler** — same pattern

### Phase 2 — Registration & Billing Foundation

6. **Design `IPaymentService` abstraction** with Stripe/payment provider adapter
7. **Build Registration Orchestrator** — the transactional flow from Module 4.1 (payment → Entra user → tenant → schema → notification)
8. **Implement Primary Admin protection guard** — domain rule preventing delete/revoke of PrimaryAdmin
9. **Add OID-based tenant lookup** to `TenantValidationMiddleware`

### Phase 3 — IAM & Invitation System

10. **Create `TenantInvitation` entity** + EF configuration
11. **Invitation send/accept/expiry flow** with seat-limit enforcement
12. **Working role management** — assign/revoke roles via catalog DB
13. **Audit log service** — `IAuditLogService` + integration in all command handlers

### Phase 4 — Subscription Management

14. **Payment retry engine** (scheduled background job)
15. **Cancellation flow** — disable all users, set Inactive
16. **Subscription upgrade/downgrade commands** with guards
17. **Seat-limit enforcement at point of action**
18. **Approaching-limit threshold checks**

### Phase 5 — Notification System

19. **`INotificationService` abstraction** decoupled from MainApp
20. **Wire all 8 required notification events**
21. **Email delivery channel** via existing `EmailSender`

### Phase 6 — Super Admin Console Completion

22. **Tenant user management endpoints** (view/reset/enable/disable/role)
23. **Billing status view per tenant**
24. **Activate from Inactive state**

### Phase 7 — Data Governance & Hardening

25. **Session invalidation** on tenant deactivation
26. **Retention policy enforcement job**
27. **Connection pooling strategy** (PgBouncer evaluation)
28. **Cross-tenant isolation integration tests**
29. **Offboarding / deletion workflow**
30. **Compliance documentation**

---

## Architecture Decisions to Make

| Decision | Options | Recommendation |
|---|---|---|
| **Payment provider** | Stripe, PayPal, custom | Stripe — best API for subscriptions + webhooks |
| **Notification transport** | In-app (SignalR), Email (SMTP/SES), Webhook | Start with email + in-app; abstract for webhook later |
| **Event bus for notifications** | MediatR notifications (in-process), Azure Service Bus, RabbitMQ | MediatR `INotification` for now; extract to message bus when needed |
| **Invitation delivery** | Email-only, in-app link | Email with accept link; in-app notification as supplement |
| **Connection pooling** | PgBouncer, Npgsql internal pooling, per-tenant pool limits | Evaluate Npgsql built-in multiplexing first; PgBouncer if needed |
| **Refresh token storage** | Cookie-based (BFF), database-backed | Cookie for admin BFF; database for API clients |
| **Identity user management** | Keep ASP.NET Identity per-tenant, or centralize in `tenant_users` | **Centralize in `tenant_users` catalog** — per-tenant Identity is being phased out based on the commented-out code |

---

## Key Code Debt to Address

| Item | Location | Issue |
|---|---|---|
| Commented-out `UserManager` code | `CreateTenantUserCommand`, `UpdateTenantUserCommand`, `GetTenantUsersQuery`, etc. | Legacy per-tenant Identity pattern being replaced by centralized `tenant_users` + Graph API |
| `TenantCommandsClient.cs` | `Features/Admin/Commands/` | Unknown purpose — review and clean up |
| Concrete class DI for provisioning services | `WebBuilderExtension.cs` registers `TenantSchemaProvisioningService` as concrete, not via interface | Should register as `ITenantSchemaProvisioningService` for testability |
| `TenantProvisioningCoordinator` takes concrete deps | Constructor takes `TenantSchemaProvisioningService` not `ITenantSchemaProvisioningService` | Should depend on abstractions |
| `AdminOnly` policy is `RequireAssertion(_ => true)` | `WebBuilderExtension.cs` | Placeholder — needs to enforce actual SuperAdmin role check |
| `TenantUser.TenantId` has `[MaxLength(128)]` on a `Guid` property | `TenantUser.cs` | Misleading — `Guid` doesn't use max length. Clean up annotation. |
