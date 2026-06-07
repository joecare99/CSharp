# T-RepoMigrator-057 Split archive provider tests into dedicated test project

## Summary
Create a dedicated MSTest project for `RepoMigrator.Providers.Archive` coverage and remove the migrated archive-focused tests from the aggregate `RepoMigrator.Tests` project without changing the solution file.

## Scope
- Create `RepoMigrator.Providers.Archive.Tests`
- Move archive model, planner, migration service, ordering, and ref naming tests
- Move archive directory source provider tests
- Move archive DevOps and file-system state store tests
- Remove the direct `RepoMigrator.Providers.Archive` reference from `RepoMigrator.Tests`
- Keep the solution file untouched

## Validation
- Run the dedicated `RepoMigrator.Providers.Archive.Tests` project
- Run a workspace build

## Outcome
- Created `RepoMigrator.Providers.Archive.Tests` as a dedicated MSTest project.
- Moved archive model, planner, migration service, ordering, ref naming, directory source provider, and archive state store coverage out of `RepoMigrator.Tests`.
- Removed the direct `RepoMigrator.Providers.Archive` reference from `RepoMigrator.Tests`.
- Left the solution file untouched.

## Verification
- `RepoMigrator.Providers.Archive.Tests`: 33/33 tests passed, 0 failed, 0 skipped.
- Workspace build: succeeded.

## Status
Done
