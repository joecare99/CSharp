# Task T-RepoMigrator-033 - Implement archive migration service and resume flow

## Status

Done

## Parent

- Backlog Item `BI-RepoMigrator-012` - `Define DevOps-backed archive import status persistence and resume`

## Goal

Implement the first-slice archive migration orchestration service with deterministic resume checkpoints.

## Scope

- Implement plan preparation, execution, and resume entry points
- Use the broader source-provider and destination-provider abstractions
- Recreate runtime temp state safely on resume
- Persist checkpoint progression under `DevOps`
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
- `T-RepoMigrator-031` - `Implement archive import planner and DevOps state store`
- `T-RepoMigrator-032` - `Implement repository-backed destination provider and Git ref operations`

## Acceptance Criteria

- Archive plans can be executed and resumed through a dedicated service
- Checkpoints persist to `DevOps` and are honored during resume
- Tests cover interruption between commit, tag, and branch steps
- The implementation respects the corrected provider-project boundaries

## Validation Evidence

- Added provider-agnostic destination ref abstraction `IMigrationDestinationRefOperations` for commit-addressable destination resume steps
- Added archive-provider-owned `IArchiveMigrationService` and implemented `ArchiveMigrationService`
- Integrated durable plan/state persistence, extraction, snapshot write, tag creation, optional branch creation, and resumable checkpoint progression
- Implemented first-slice resume handling for:
  - commit already completed but tag and branch missing
  - tag already created but branch missing
  - unsafe divergence surfaced during tag creation
- Added targeted MSTest coverage in `RepoMigrator.Tests.ArchiveMigrationServiceTests`
- Targeted validation result: `5/5` tests passed
- Full workspace build passed after the implementation
