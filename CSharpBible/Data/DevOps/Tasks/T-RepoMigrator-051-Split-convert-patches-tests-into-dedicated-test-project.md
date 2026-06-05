# Task T-RepoMigrator-051 - Split convert_patches tests into dedicated test project

## Status

Draft

## Parent

- Task `T-RepoMigrator-011` - `Close priority test coverage gaps`
- Task `T-RepoMigrator-050` - `Fix convert_patches behavior with regression tests before refactor`

## Goal

Move the `convert_patches.ps1` regression coverage out of the broad `RepoMigrator.Tests` project into a dedicated MSTest project with a clear, narrow scope.

## Scope

- Create a dedicated MSTest project for `convert_patches.ps1` regression coverage
- Move the current `ConvertPatchesScriptTests` into the dedicated test project
- Keep solution-file handling out of scope for this slice
- Preserve the current black-box script validation behavior and keep the new project independently runnable

## Deliverables

- New dedicated test project for `convert_patches.ps1`
- Moved regression tests with unchanged observable coverage scope
- Validation evidence for the dedicated test project and workspace build

## Detailed Work Packages

1. Create a new MSTest project with only the required package references
2. Move the `ConvertPatchesScriptTests` file into the new scoped project
3. Remove the moved test file from `RepoMigrator.Tests`
4. Validate the dedicated test project directly without changing the solution file

## Acceptance Criteria

- `convert_patches.ps1` regression tests no longer live in `RepoMigrator.Tests`
- A dedicated MSTest project exists for the `convert_patches.ps1` regression scope
- The new test project can be built and run directly by project path
- The current solution file remains untouched

## Risks

- The script under test remains outside the repository root and therefore depends on the machine-local script path staying available
- Additional future per-project test splits may still be needed for other currently aggregated tests

## Open Questions

- Should future patch-driver code tests live beside this dedicated script test project, or should script-level tests remain isolated from code-level provider tests?
- Should a later aggregate-only test project reference the dedicated scoped test projects, or should aggregation remain at CI level only?
