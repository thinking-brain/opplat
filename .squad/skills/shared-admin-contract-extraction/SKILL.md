---
name: "shared-admin-contract-extraction"
description: "Extract exact admin host contracts into shared application abstractions without flattening module handlers"
domain: "backend-architecture"
confidence: "high"
source: "repo-work"
---

## Context
Use this when two ASP.NET hosts share the same admin/client contract types or auth constants, but their runtime implementations still diverge enough that moving whole features would over-couple the hosts.

## Pattern

### Extract only exact duplicates first
If two hosts expose the same DTO shape or constant set, move that canonical contract into `Opplat.Application.Abstractions` and have both hosts consume it.

### Leave divergent behavior in the host
Keep host-local option models, claim normalization, persistence, and middleware when the implementations are not yet identical or active source-contract tests pin host-specific code.

### Preserve bounded contexts
Do not use the shared extraction as a reason to flatten `Modules/*/Application` into one assembly. Shared host contracts belong in the shared abstractions project; module handlers still belong to their module application projects.

## Example
- `AdminSessionDto`, `AdminSessionUserDto`, and `AdminCsrfTokenDto` move to `src\Opplat.Application.Abstractions\Admin\`
- `AuthRoles` and `AuthClaimTypes` move to `src\Opplat.Application.Abstractions\Auth\`
- `MainApp` and `AdminApi` keep their own `AuthOptions` / OIDC normalization until those behaviors genuinely converge

## Anti-Patterns
- Moving tenant-specific write models into the shared project just because the endpoint names match
- Flattening Sales/Inventory handlers into `Opplat.Application` while doing contract cleanup
- Forcing shared runtime code when only the contract, not the behavior, is actually common
