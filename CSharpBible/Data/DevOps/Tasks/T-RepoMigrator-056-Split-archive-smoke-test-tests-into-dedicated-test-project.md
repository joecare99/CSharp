# T-RepoMigrator-056 Split ArchiveSmokeTest tests into dedicated test project

## Summary
Create a dedicated MSTest project for `RepoMigrator.Tools.ArchiveSmokeTest` coverage and remove the migrated tests from the aggregate `RepoMigrator.Tests` project without changing the solution file.

## Scope
- Create `RepoMigrator.Tools.ArchiveSmokeTest.Tests`
- Move `ArchiveSmokeTestOptionsTests`
- Move `ArchiveSmokeTestProgramTests`
- Move ArchiveSmokeTest command help coverage
- Remove the direct `RepoMigrator.Tools.ArchiveSmokeTest` reference from `RepoMigrator.Tests`
- Keep the solution file untouched

## Validation
- Run the dedicated `RepoMigrator.Tools.ArchiveSmokeTest.Tests` project
- Run a workspace build

## Outcome
- Created `RepoMigrator.Tools.ArchiveSmokeTest.Tests` as a dedicated MSTest project.
- Moved ArchiveSmokeTest option, program, and command help coverage out of `RepoMigrator.Tests`.
- Removed the direct `RepoMigrator.Tools.ArchiveSmokeTest` reference from `RepoMigrator.Tests`.
- Left the solution file untouched.

## Verification
- `RepoMigrator.Tools.ArchiveSmokeTest.Tests`: 9/9 tests passed, 0 failed, 0 skipped.
- Workspace build: succeeded.

## Status
Done
