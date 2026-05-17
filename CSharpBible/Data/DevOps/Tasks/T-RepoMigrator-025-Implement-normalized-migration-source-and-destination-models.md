# Task T-RepoMigrator-025 - Implement normalized migration source and destination models

## Status

Completed

## Parent

- Backlog Item `BI-RepoMigrator-009` - `Define archive snapshot source detection and ordering contracts`

## Goal

Implement the shared source and destination model types that sit above provider-specific source and target behavior.

## Scope

- Add normalized source models such as `MigrationSourceDefinition` and `MigrationSourceKind`
- Add normalized destination models such as `MigrationDestinationDefinition` and `MigrationDestinationKind`
- Add shared write metadata such as `MigrationDestinationCommit`
- Keep source-specific and destination-specific extension points explicit
- Preserve compatibility with the existing repository endpoint model for first-slice integration

## Detailed Work Packages

1. Add the shared model files in `RepoMigrator.Core`
2. Define nullable-safe relationships between normalized models and repository-specific models
3. Document model responsibilities in XML documentation
4. Add tests for default values and model invariants where behavior exists

## Deliverables

- New normalized source and destination model types in `RepoMigrator.Core`
- A clear mapping note for first-slice repository-backed targets
- Initial tests covering model defaults or invariants

## Dependencies

- `T-RepoMigrator-023` - `Define concrete code target state for archive import`

## Acceptance Criteria

- RepoMigrator.Core contains normalized source and destination models above provider-specific details
- The new models do not overload `RepoType` with non-repository concepts
- The models are ready to be consumed by source-provider and destination-provider abstractions

## Implementation Summary

- Added normalized source and destination model files in `RepoMigrator.Core`:
  - `MigrationSourceKind`
  - `MigrationDestinationKind`
  - `MigrationSourceDefinition`
  - `MigrationDestinationDefinition`
  - `MigrationDestinationCommit`
- Added first archive-specific and future archive-output extension models:
  - `ArchiveSourceLocationKind`
  - `ArchiveMigrationSourceDefinition`
  - `SequentialArchiveDestinationDefinition`
- Kept the new models nullable-safe and compatible with the existing `RepositoryEndpoint` model for first-slice integration.
- Added `MigrationSourceDestinationModelsTests` covering defaults and representative assigned-value preservation.

## Validation

- `run_build` completed successfully.
- Targeted tests passed:
  - `MigrationSourceDefinition_Defaults_AreInitialized`
  - `MigrationDestinationDefinition_Defaults_AreInitialized`
