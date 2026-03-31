# Work Routing

How to decide who handles what.

## Routing Table

| Work Type | Route To | Examples |
|-----------|----------|---------|
| Architecture, design decisions, code review | Bishop | ADR drafts, PR reviews, module design, multitenant patterns |
| .NET backend — APIs, services, data layer | Mother | EF Core models, repositories, endpoint handlers, DB migrations |
| .NET backend — domain logic, integrations | Cosmo | Business rules, tenant provisioning, Graph/Keycloak integrations |
| React UI, frontend components | Liz | SPA features, MUI components, routing, API hooks, tenant UX |
| Tests, quality gates, edge cases | Carl | Unit/integration tests, test plans, CI quality checks |
| Auth, secrets, threat modeling, hardening | Whistler | OIDC config, claims, secret rotation, pen-test findings |
| Scope & priorities | Bishop | What to build next, trade-offs, architectural decisions |
| Session logging | Scribe | Automatic — never needs routing |

## Issue Routing

| Label | Action | Who |
|-------|--------|-----|
| `squad` | Triage: analyze issue, assign `squad:{member}` label | Bishop |
| `squad:bishop` | Lead architecture/design work | Bishop |
| `squad:mother` | Backend API/data layer work | Mother |
| `squad:cosmo` | Domain/integration work | Cosmo |
| `squad:liz` | Frontend/React work | Liz |
| `squad:carl` | QA/testing work | Carl |
| `squad:whistler` | Security/auth work | Whistler |

### How Issue Assignment Works

1. When a GitHub issue gets the `squad` label, **Bishop** triages it — analyzing content, assigning the right `squad:{member}` label, and commenting with triage notes.
2. When a `squad:{member}` label is applied, that member picks up the issue in their next session.
3. Members can reassign by removing their label and adding another member's label.
4. The `squad` label is the "inbox" — untriaged issues waiting for Bishop's review.

## Rules

1. **Eager by default** — spawn all agents who could usefully start work, including anticipatory downstream work.
2. **Scribe always runs** after substantial work, always as `mode: "background"`. Never blocks.
3. **Quick facts → coordinator answers directly.** Don't spawn an agent for "what port does the server run on?"
4. **When two agents could handle it**, pick the one whose domain is the primary concern.
5. **"Team, ..." → fan-out.** Spawn all relevant agents in parallel as `mode: "background"`.
6. **Anticipate downstream work.** If a feature is being built, spawn Carl to write test cases simultaneously.
7. **Security review** — any auth, claims, or secrets change routes a copy to Whistler.
8. **Issue-labeled work** — when a `squad:{member}` label is applied to an issue, route to that member.
