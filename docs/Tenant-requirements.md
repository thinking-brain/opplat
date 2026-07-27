# Tenant Administration System — Implementation Requirements

> **Version:** 1.2
> **Status:** Draft
> **Last Updated:** 2026-03-25
>
> **About this document:** Requirements are ordered by implementation dependency. Each module builds on the previous one. Agents should implement modules in sequence unless explicitly noted as parallelizable.

---

## Module 1 — Identity Provider Foundation
> **Depends on:** Nothing. This is the first thing to implement.
> **Blocks:** All other modules.

### 1.1 Entra ID App Registration
Register a backend application in the Azure Entra ID (single-tenant) portal. The service principal must be granted the `User.ReadWrite.All` application permission. Store the resulting `tenant_id`, `client_id`, and `client_secret` (or certificate) securely in environment configuration. No user-facing functionality is required in this step.

### 1.2 Microsoft Graph API Client
Implement a backend Graph API client with the following capabilities:
- Authenticate using client credentials flow (service principal, not delegated).
- **Create a user** — `POST /v1.0/users`. UPNs must follow the collision-safe format `{uuid}@yourtenant.onmicrosoft.com`. The user's real email must be stored in the `mail` or `otherMails` field.
- **Enable a user** — `PATCH /v1.0/users/{oid}` with `accountEnabled: true`.
- **Disable a user** — `PATCH /v1.0/users/{oid}` with `accountEnabled: false`.
- **Delete a user** — `DELETE /v1.0/users/{oid}`.
- **Trigger password reset** — force password change on next sign-in via `passwordProfile.forceChangePasswordNextSignIn: true`.

All Graph API calls must be asynchronous, include error handling, and implement retry logic for transient failures (HTTP 429, 503).

### 1.3 OIDC Authentication Endpoint
Implement the authentication entry point using Entra ID as the OIDC provider. Upon successful login:
- Validate the JWT token issued by Entra ID.
- Extract the stable `oid` claim (object ID).
- Return the token to the client for use in subsequent API calls.

No tenant resolution or database routing is required yet — that is handled in Module 4.

### 1.4 MFA Enforcement
Configure Multi-Factor Authentication as mandatory for all users in the Entra ID tenant via Conditional Access policy or Security Defaults. This must be enforced at the identity provider level, not in the application layer.

### 1.5 Self-Service Password Reset (SSPR)
Enable and configure Entra ID Self-Service Password Reset so all users can reset their own passwords without administrator intervention.

---

## Module 2 — Core Database Schema & Tenant Model
> **Depends on:** Module 1 (identity provider must exist before storing `oid`).
> **Blocks:** Modules 3, 4, 5, 6.
> **Can run in parallel with:** Module 1 schema design work.

### 2.1 Application Database — Core Tables
Design and provision the central application database (not to be confused with per-tenant databases). At minimum, the following entities must be defined:

- **`subscription_plans`** — plan name, user seat limit, resource limits per type (users, API calls, storage), pricing.
- **`tenants`** — tenant ID, name, status (`Active` / `Inactive`), plan ID, creation date, inactivation date, assigned database instance, schema name.
- **`tenant_users`** — entra `oid`, tenant ID, real email, role, `is_primary_admin` flag, active/inactive status, created date.
- **`database_instances`** — instance identifier, connection string reference, current tenant schema count, status (`active` / `archived`).
- **`audit_logs`** — actor `oid`, target tenant ID, target user `oid`, action type, before state (JSON), after state (JSON), timestamp.

### 2.2 Resource Limit Definitions
Resource types subject to plan enforcement must be explicitly defined in the `subscription_plans` schema. Initial resource types are: maximum active users, API calls per month, storage quota in GB. This list must be extendable without schema changes (e.g., stored as a JSON limits map).

### 2.3 Database Instance Sharding Configuration
Define a configurable threshold for the maximum number of tenant schemas allowed per database instance. This value must be stored in application configuration (not hardcoded). When the threshold is reached during provisioning, a new database instance must be automatically provisioned (see Module 3).

---

## Module 3 — Tenant Provisioning Engine
> **Depends on:** Module 2.
> **Blocks:** Module 4 (registration flow calls this engine).

### 3.1 Schema Provisioning
Implement a provisioning service that, given a tenant ID and assigned database instance, creates a dedicated schema within that instance. The service must be idempotent — running it twice for the same tenant must not cause errors or duplicate schemas.

### 3.2 Database Instance Auto-Scaling
Before provisioning a new tenant schema, the system must check the current tenant schema count for the active database instance against the configured threshold. If the threshold is reached, the system must automatically provision a new database instance and register it in `database_instances` before continuing with schema creation.

### 3.3 Schema Migration Strategy
Implement a migration runner capable of applying database schema changes across all tenant schemas. Requirements:
- Must be triggerable per-tenant or for all tenants in bulk.
- Must support phased/rolling execution to minimise downtime.
- Must log success/failure per tenant schema.
- Must support rollback of a failed migration.

---

## Module 4 — User Registration & Tenant Activation
> **Depends on:** Modules 1, 2, 3.
> **Blocks:** Module 5.

### 4.1 Registration Flow
Implement the self-service registration portal. The flow must:
1. Collect user details and selected subscription plan.
2. Initiate the payment process via the integrated payment provider.
3. On payment confirmation, call the Graph API to create the user in Entra ID (Module 1.2) and capture the returned `oid`.
4. Create the tenant and tenant_user records in the application database (Module 2.1).
5. Trigger the provisioning engine (Module 3.1) to create the tenant schema.
6. Set the tenant status to **Active** and designate the registering user as **Primary Admin**.
7. Send a tenant activation notification to the user (see Module 7).

Registration must be transactional: if any step fails, the entire process must roll back cleanly, including disabling or deleting the Entra user if created before the failure.

### 4.2 Tenant Routing Middleware
Implement authentication middleware that runs on every authenticated request:
1. Validate the Entra ID JWT token.
2. Extract the `oid` claim.
3. Look up the tenant record and assigned database instance from the application database.
4. Establish (or retrieve from pool — see Module 8.2) the database connection for that tenant's schema.
5. Inject the resolved tenant context into the request lifecycle.

If the tenant status is **Inactive**, the middleware must reject the request with an appropriate error response regardless of token validity.

### 4.3 Primary Admin Protections
Enforce at the application layer that the Primary Admin account (`is_primary_admin: true`) cannot be deleted or have its access revoked by any other tenant-level administrator. Only a Super Admin (Module 6) or the Primary Admin themselves may modify this account. Any attempt by a non-Super Admin to delete or revoke access from the Primary Admin must return a clear error.

---

## Module 5 — Subscription & Billing Management
> **Depends on:** Module 4.
> **Can run in parallel with:** Module 6.

### 5.1 Payment Failure Grace Period
Implement a configurable grace period policy for failed payments. Default behavior:
- Retry payment a minimum of 3 times over 7 days.
- Send a payment failure notification after each failed attempt (Module 7).
- Only after all retries are exhausted, disable all tenant users in Entra ID and set tenant status to **Inactive**.

The retry count and interval must be configurable, not hardcoded.

### 5.2 Deliberate Cancellation
On deliberate subscription cancellation, immediately disable all tenant users in Entra ID, set the tenant status to **Inactive**, and trigger the access restriction notification.

### 5.3 Resource Monitoring & Enforcement
Implement enforcement for each defined resource type (Module 2.2):
- Track current consumption per tenant in the application database.
- Enforce limits at the point of action (e.g., block user invitation if seat limit is reached).
- Enforcement logic must be independent per resource type.

### 5.4 Subscription Tier Upgrade
On upgrade, immediately apply new plan limits to the tenant record and send a confirmation notification.

### 5.5 Subscription Tier Downgrade Guard
Before applying a downgrade:
1. Compare current active user count against the seat limit of the target tier.
2. If active users exceed the target limit, block the downgrade and return a message identifying the conflict and instructing the administrator to disable users manually before retrying.
3. If the count is within limits, apply the new plan and send a confirmation notification.

### 5.6 Approaching Limit Notifications
The system must proactively notify the tenant administrator when active user count reaches 80% and 100% of the plan's seat limit.

---

## Module 6 — Tenant-Level IAM (Roles, Permissions & User Management)
> **Depends on:** Module 4.
> **Can run in parallel with:** Module 5.

### 6.1 Role & Permission Model
Define and implement the application-level role model. Roles and permissions are owned entirely by the application layer — Entra ID issues identity only. At minimum, the following roles must exist: **Primary Admin**, **Admin**, **User**. The permission model must be documented and extensible.

### 6.2 User Invitation Flow
Administrators must be able to invite new users up to the plan's seat limit. The invitation flow must define:
- Invitation expiry duration (must be configurable).
- Ability to resend an expired invitation.
- Behavior when a user accepts an invitation after the seat limit has been reached (reject with a clear message).
- On acceptance: create the user in Entra ID via Graph API, store the `oid` and role in `tenant_users`.

### 6.3 Role & Permission Management
Administrators must be able to:
- Assign and revoke administrative privileges for non-Primary Admin users.
- Manage granular permissions within the application's authorization model.
- All role changes must be written to the audit log (Module 6.5).

### 6.4 Session Invalidation on Tenant Deactivation
When a tenant is set to Inactive (Module 5.1, 5.2), all active sessions for that tenant's users must be invalidated. Implement forced token revocation or short token expiry with blocked refresh to achieve this.

### 6.5 Audit Logging — Tenant Actions
All administrative actions performed by tenant-level administrators must be written to `audit_logs` (Module 2.1), including: user invitations, role changes, permission changes, subscription modifications, and user enable/disable actions. Each log entry must include: timestamp, actor `oid`, target entity, action type, before state, after state.

---

## Module 7 — Notification System
> **Depends on:** Module 2 (needs tenant and user data).
> **Can run in parallel with:** Modules 5 and 6.

### 7.1 Notification Delivery
Implement a notification service capable of delivering messages to tenant administrators. The delivery mechanism (e.g., email via SMTP/SES, webhook) must be defined during technical design. The service must be decoupled from the business logic that triggers it (e.g., via an event queue or pub/sub).

### 7.2 Required Notification Events
The following events must trigger a notification to the relevant tenant administrator:
- Tenant activation confirmed.
- Payment failed (with retry number and next retry date).
- Final access restriction after grace period exhaustion.
- Active user count at 80% of seat limit.
- Active user count at 100% of seat limit.
- Subscription upgrade confirmed.
- Subscription downgrade confirmed.
- Subscription downgrade blocked (with reason).

---

## Module 8 — Super Admin Console
> **Depends on:** Modules 4, 5, 6.
> **Can run in parallel with:** Module 9.

### 8.1 Global Tenant Management
Build a dedicated Super Admin management application (separate from the tenant-facing application). The Super Admin must be able to:
- View all tenants and their status, plan, and administrator details.
- Activate or deactivate any tenant.
- View subscription and billing status.

### 8.2 Support Intervention — Tenant User Management
The Super Admin must be able to perform the following on any individual user across all tenants:
- View user profile and current status (read access).
- Trigger a password reset via Graph API.
- Enable or disable a user account (both in application DB and Entra ID via Graph API).
- Modify role assignments in the application layer.

Impersonation (acting as a user within their tenant session) is explicitly out of scope.

### 8.3 Audit Logging — Super Admin Actions
All Super Admin actions must be written to `audit_logs` with: timestamp, Super Admin actor identity, target tenant ID, target user `oid` (if applicable), action type, before state, and after state. Audit logs must be immutable.

---

## Module 9 — Data Governance & Retention
> **Depends on:** Module 3 (tenant schemas must exist).
> **Can run in parallel with:** Module 8.

### 9.1 Tenant Data Isolation Enforcement
Verify and enforce that no cross-tenant data access is possible at the application or query layer. Each tenant's schema must be accessible only via the tenant routing middleware (Module 4.2) for the corresponding tenant session.

### 9.2 Retention Policy for Inactive Tenants
Data for inactive tenants must be preserved for a minimum of **one year** from the date the tenant status was last set to Inactive. No automated deletion may occur within this window.

### 9.3 Cold Storage Archival
Implement an automated process to identify database instances where all tenant schemas belong to inactive tenants that are within the retention window (not yet eligible for deletion). Eligible instances must be moved to cold storage to reduce costs.

Before implementing, a recovery time SLA must be defined (e.g., data restorable within 48 hours). This SLA must be honoured by the chosen cold storage solution.

### 9.4 Tenant Offboarding & Data Deletion
Implement a formal offboarding process for tenants requesting full data removal. The process must:
- Accept and record a verified deletion request from the Primary Admin or a Super Admin.
- Delete all tenant data from the application database and tenant schema after the retention period has elapsed, unless a legal or regulatory hold is active.
- Delete the associated users from Entra ID via Graph API.
- Document the process for handling GDPR data subject access requests (DSAR) and right-to-erasure requests.

### 9.5 Regulatory Compliance Documentation
Before going to production, the following must be documented:
- Data residency region for all storage and processing.
- Legal basis for data retention under applicable regulations (e.g., GDPR).
- Process for responding to data subject access requests within regulatory deadlines.

---

## Module 10 — Infrastructure & Performance Hardening
> **Depends on:** Modules 3, 4.
> **Recommended before production launch.**

### 10.1 Connection Pooling
Implement a connection pooling strategy to prevent connection pool exhaustion from dynamic per-tenant database connections. Evaluate and implement one of: PgBouncer, per-tenant pool limits, or application-level connection management. The chosen strategy must be load-tested before production.

### 10.2 Token & Session Configuration
Define and enforce the following session parameters:
- Access token expiry duration.
- Refresh token expiry duration.
- Refresh token rotation on every use.
- Maximum concurrent session policy (if applicable).

### 10.3 Provisioning Resilience
Ensure the tenant provisioning engine (Module 3) handles partial failure scenarios gracefully. All provisioning steps must be logged so that a failed provisioning can be resumed or cleanly rolled back by a Super Admin or automated retry process.