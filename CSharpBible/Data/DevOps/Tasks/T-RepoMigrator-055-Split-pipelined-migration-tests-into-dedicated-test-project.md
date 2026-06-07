# T-RepoMigrator-055 Split PipelinedMigration tests into dedicated test project

## Summary
Create a dedicated MSTest project for `RepoMigrator.Tools.PipelinedMigration` coverage and remove the migrated tests from the aggregate `RepoMigrator.Tests` project without changing the solution file.

## Scope
- Create `RepoMigrator.Tools.PipelinedMigration.Tests`
- Move `PipelinedMigrationOptionsTests`
- Move `PipelinedMigrationProgramTests`
- Move `PipelinedMigrationServiceTests`
- Remove the direct `RepoMigrator.Tools.PipelinedMigration` reference from `RepoMigrator.Tests`
- Keep the solution file untouched

## Validation
- Run the dedicated `RepoMigrator.Tools.PipelinedMigration.Tests` project
- Run a workspace build

## Outcome
- Created `RepoMigrator.Tools.PipelinedMigration.Tests` as a dedicated MSTest project.
- Moved PipelinedMigration option, program, service, console progress, ordered buffer, pipeline work item, and command help coverage out of `RepoMigrator.Tests`.
- Removed the direct `RepoMigrator.Tools.PipelinedMigration` reference from `RepoMigrator.Tests`.
- Left the solution file untouched.

## Verification
- `RepoMigrator.Tools.PipelinedMigration.Tests`: 53/53 tests passed, 0 failed, 0 skipped.
- Workspace build: succeeded.

## Status
Done
