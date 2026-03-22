# Repo root helper for source-contract tests

## When to use this
Use this pattern when tests read files directly from the repository tree and the repo may switch between `opplat.sln`, `opplat.slnx`, or other top-level markers over time.

## Pattern
1. Centralize repo-root discovery in one helper used by all source-contract tests.
2. Accept both `opplat.sln` and `opplat.slnx` as valid markers.
3. Fall back to checking for sibling `src/` and `test/` directories.
4. Keep individual tests focused on behavior contracts, not root-discovery boilerplate.

## Why it matters
This prevents large suites of source-contract tests from failing because the solution container format changed rather than because the implementation regressed. It also makes it safer to retire or replace solution files during migration work.
