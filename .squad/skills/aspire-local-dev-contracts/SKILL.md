# Skill: Aspire local-dev contract validation

## When to use
- A repo adds or updates a .NET Aspire AppHost for local development.
- You need regression coverage without starting Docker containers or long-lived orchestrated processes.

## Pattern
1. Read the approved architecture decisions first so tests enforce the intended AppHost scope (for Opplat: backends + infra only, not the Vite SPAs).
2. Add source-contract tests that read `src\Opplat.AppHost\Program.cs` and assert:
   - required resources exist (`AddSqlServer`, `AddPostgres`, `AddContainer("keycloak")`, all backend `AddProject(...)` calls)
   - expected local ports and OIDC env vars are present
   - Keycloak reuses the committed realm/config mounts
3. Pair those tests with README assertions so local-dev instructions and limitations stay honest.
4. Validate with `dotnet build .\opplat.slnx -m:1 -v minimal` and `dotnet test .\test\Opplat.MainApp.Test\Opplat.MainApp.Test.csproj -m:1 -v minimal`.
5. If frontends participate in the local-dev story, separately build them with `npm run build` and lock any expected dev-port defaults with contract tests.

## Opplat-specific notes
- Hicks already added shared Aspire-friendly runtime seams (`/health`, `/alive`, forwarded headers, conditional HTTPS redirection), so Bishop only needs to lock the orchestration/documentation contracts.
- Opplat's current limitation is intentional: SQL Server, PostgreSQL, and Keycloak still require Docker, and the Vite SPAs stay outside Aspire.
