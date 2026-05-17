# Task T-RepoMigrator-027 - Implement archive plan and state models

## Status

Completed

## Parent

- Backlog Item `BI-RepoMigrator-012` - `Define DevOps-backed archive import status persistence and resume`

## Goal

Implement the archive import plan, checkpoint, and persisted execution state models needed for durable `DevOps` manifests.

## Scope

- Add archive plan types such as `ArchiveImportPlan` and `ArchiveImportPlanItem`
- Add archive execution-state types such as `ArchiveImportState` and `ArchiveImportSnapshotState`
- Add checkpoint enums and status types
- Keep provider-agnostic root data separate from extension-oriented fields
- Prepare the models for deterministic JSON serialization later

## Detailed Work Packages

1. Add plan, checkpoint, and state model files in `RepoMigrator.Core`
2. Add XML documentation for persisted-state responsibilities
3. Ensure identifiers and status fields support resume-safe progression
4. Add tests for defaults and status transitions where behavior is expressed in model helpers

## Deliverables

- Archive plan and state model files
- Resume-oriented checkpoint types
- Initial model tests where applicable

## Dependencies

- `T-RepoMigrator-018` - `Define archive core models and persisted state schema`
- `T-RepoMigrator-025` - `Implement normalized migration source and destination models`

## Acceptance Criteria

- RepoMigrator.Core contains durable plan and state models for archive imports
- The models support provider-agnostic persistence plus targeted extension data
- The models are ready for a DevOps-backed state store implementation

## Implementation Summary

- Added archive import manifest and lifecycle types in `RepoMigrator.Core`:
  - `ArchiveImportManifestVersion`
  - `ArchiveImportPlanStatus`
  - `ArchiveImportRunStatus`
  - `ArchiveImportCheckpointPhase`
- Added durable archive plan and state model files:
  - `ArchiveImportCheckpoint`
  - `ArchiveImportPlan`
  - `ArchiveImportPlanItem`
  - `ArchiveImportState`
  - `ArchiveImportSnapshotState`
- Kept provider-agnostic root sections explicit through normalized `MigrationSourceDefinition` and `MigrationDestinationDefinition` usage.
- Added targeted provider-extension dictionaries on plan, plan-item, state, and snapshot-state models so later provider-specific data can be persisted without distorting the shared schema.
- Added `ArchiveImportModelsTests` covering defaults and representative assigned-value preservation.

## Architecture Correction Note

- A later architectural clarification established that archive-specific files should live in archive-specific provider projects rather than in `RepoMigrator.Core`.
- The archive plan, checkpoint, and state types introduced here should therefore be treated as provisional Core placement and relocated under `T-RepoMigrator-035 - Relocate provider-specific archive and compression files out of core`.
- The provider-agnostic persistence concepts remain valid, but the archive-specific concrete models should not remain in Core long term.

## Validation

- `run_build` completed successfully.
- Targeted tests passed:
  - `ArchiveImportPlan_Defaults_AreInitialized`
  - `ArchiveImportState_Defaults_AreInitialized`
  - `ArchiveImportModels_AssignedValues_ArePreserved`
