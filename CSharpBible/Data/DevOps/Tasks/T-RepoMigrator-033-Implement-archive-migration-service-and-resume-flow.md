# Task T-RepoMigrator-033 - Implement archive migration service and resume flow

## Status

Completed

## Parent

- Backlog Item `BI-RepoMigrator-012` - `Define archive import status persistence and resume`

## Goal

Implement the first-slice archive migration orchestration service with deterministic resume checkpoints.

## Scope

- Implement plan preparation, execution, and resume entry points
- Use the broader source-provider and destination-provider abstractions
- Recreate runtime temp state safely on resume
- Persist checkpoint progression under runtime-defined storage
- Stop on unsafe divergence instead of guessing through inconsistent target state
- Implement the orchestration service in an archive-provider project rather than in `RepoMigrator.Core`

## Detailed Work Packages

1. Implement `ArchiveMigrationService`
2. Integrate plan loading, state loading, execution checkpoints, and state updates
3. Implement first-slice resume behavior for commit completed, tag missing, and branch missing scenarios
4. Add tests covering successful execution and interruption/resume scenarios

## Deliverables

- Archive migration service implementation
- Resume-aware checkpoint progression
- Tests for first-slice resume paths and unsafe-divergence stops

## Dependencies

- `T-RepoMigrator-021` - `Define archive migration orchestration and resume checkpoints`
- `T-RepoMigrator-031` - `Implement archive import planner and file-system state store`
- `T-RepoMigrator-032` - `Implement repository-backed destination provider and Git ref operations`

## Acceptance Criteria

- Archive plans can be executed and resumed through a dedicated service
- Checkpoints persist to runtime-defined storage and are honored during resume
- Tests cover interruption between commit, tag, and branch steps
- The implementation respects the corrected provider-project boundaries

## Validation Evidence

- Added provider-agnostic destination ref abstraction `IMigrationDestinationRefOperations` for commit-addressable destination resume steps
- Added archive-provider-owned `IArchiveMigrationService` and implemented `ArchiveMigrationService`
- Integrated durable plan/state persistence, extraction, snapshot write, tag creation, optional branch creation, and resumable checkpoint progression
- Integrated the existing archive migration service into the current WPF start workflow for local archive-directory to Git target scenarios
- Corrected the WPF composition root to resolve runtime-defined storage and relative archive source paths instead of using the application bin directory
- Implemented first-slice resume handling for:
  - commit already completed but tag and branch missing
  - tag already created but branch missing
  - unsafe divergence surfaced during tag creation
- Added targeted MSTest coverage in `RepoMigrator.Tests.ArchiveMigrationServiceTests` and `RepoMigrator.Tests.MainViewModelStartWorkflowTests`
- Targeted validation result: `6/6` tests passed
- Full workspace build passed after the implementation

## Follow-up Maintenance Note

- A later validation against a local bare Git remote (`C:\Projekte\Cpp\xpdf.git`) exposed a race in the archive-to-Git target path when detailed logging was enabled and the slower timing made overlapping remote operations visible.
- Root cause: `GitProvider.CommitSnapshotAsync(...)` scheduled remote branch pushes in the background, while archive resume/ref steps (`EnsureTagAsync` / `EnsureBranchAsync`) could immediately start additional pushes against the same remote repository.
- `GitProvider` now tracks already scheduled background branch-push tasks and waits for them before starting archive tag or branch ref pushes. This keeps remote Git operations deterministic and prevents concurrent object migration against the same bare target.
- Added regression coverage in `RepoMigrator.Tests.GitProviderRemoteTests` to verify that a tag push waits for a pending background branch push.
- Validation after the fix:
  - workspace build passed,
  - full `RepoMigrator.Tests` execution passed with `238/238` tests successful and `0` skipped.

## Follow-up Timestamp Note

- Archive-import commits no longer default unconditionally to the current runtime timestamp.
- The planner now persists a selected commit timestamp per archive snapshot with the following priority:
  1. newest internal archive entry timestamp,
  2. archive file last-write timestamp,
  3. runtime fallback only if no persisted timestamp is available.
- `ArchiveMigrationService` now uses the planned archive timestamp when writing destination commits so imported history better reflects the time basis contained in the archive set.
- Added regression coverage in `RepoMigrator.Tests.ArchiveImportPlannerTests` and `RepoMigrator.Tests.ArchiveMigrationServiceTests` for timestamp selection, fallback, and propagation.
- Validation after the timestamp update:
  - workspace build passed,
  - full `RepoMigrator.Tests` execution passed with `239/239` tests successful and `0` skipped.
