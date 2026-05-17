# Task T-RepoMigrator-033 - Implement archive migration service and resume flow

## Status

Draft

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
