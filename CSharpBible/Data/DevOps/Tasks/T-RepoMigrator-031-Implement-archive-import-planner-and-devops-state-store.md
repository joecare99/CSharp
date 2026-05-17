# Task T-RepoMigrator-031 - Implement archive import planner and DevOps state store

## Status

Done

## Parent

- Backlog Item `BI-RepoMigrator-012` - `Define DevOps-backed archive import status persistence and resume`

## Goal

Implement the archive import planner and the DevOps-backed plan/state persistence layer.

## Scope

- Implement a planner that combines source discovery, driver inspection, ordering, and naming into a durable import plan
- Implement a `DevOps` state store for `plan.json` and `state.json`
- Keep JSON output deterministic and reviewable
- Support loading and saving plans and execution state by stable plan id
- Prepare the implementation for cross-machine resume constraints without relying on temp paths
- Implement the planner and state store in an archive-provider project rather than in `RepoMigrator.Core`

## Detailed Work Packages

1. Implement `ArchiveImportPlanner`
2. Implement `DevOpsArchiveImportStateStore`
3. Implement deterministic file layout under `DevOps/Data/RepoMigrator/ArchiveImports`
4. Add tests for plan generation, manifest roundtrip, stable file layout, and override-friendly persistence behavior

## Deliverables

- Archive import planner implementation
- DevOps-backed plan/state persistence implementation
- Tests covering manifest roundtrip and plan generation behavior

## Dependencies

- `T-RepoMigrator-020` - `Define archive import planner and DevOps manifest layout`
- `T-RepoMigrator-027` - `Implement archive plan and state models`
- `T-RepoMigrator-028` - `Implement directory archive source provider`
- `T-RepoMigrator-029` - `Implement zip archive driver`
- `T-RepoMigrator-030` - `Implement archive ordering and naming services`

## Acceptance Criteria

- Archive import plans can be prepared and stored under `DevOps`
- Plan and state manifests are deterministic and readable
- Tests cover plan creation and persistence roundtrip scenarios
- The implementation respects the corrected provider-project boundaries

## Validation Evidence

- Added archive-provider-owned abstractions `IArchiveImportPlanner` and `IArchiveImportStateStore`
- Implemented `ArchiveImportPlanner` to combine normalized source discovery, archive inspection, ordering, naming, and durable plan generation
- Implemented `DevOpsArchiveImportStateStore` with deterministic manifest layout under `DevOps/Data/RepoMigrator/ArchiveImports/{planId}`
- Added targeted MSTest coverage in:
  - `RepoMigrator.Tests.ArchiveImportPlannerTests`
  - `RepoMigrator.Tests.DevOpsArchiveImportStateStoreTests`
- Targeted validation result: `3/3` tests passed
- Full workspace build passed after the implementation
