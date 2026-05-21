# Task T-RepoMigrator-036 - Create archive provider project and move archive-specific models out of core

## Status

Completed

## Parent

- Task `T-RepoMigrator-035` - `Relocate provider-specific archive and compression files out of core`
- Backlog Item `BI-RepoMigrator-009` - `Define archive snapshot source detection and ordering contracts`
- Backlog Item `BI-RepoMigrator-012` - `Define DevOps-backed archive import status persistence and resume`

## Goal

Create the dedicated archive-provider project and relocate archive-specific models and services from `RepoMigrator.Core` into that project.

## Scope

- Create a dedicated archive-provider project
- Move archive-specific source-definition files out of `RepoMigrator.Core`
- Move archive-specific import-plan and resume-state files out of `RepoMigrator.Core`
- Move `DirectoryArchiveSnapshotSourceProvider` out of `RepoMigrator.Core`
- Update namespaces, references, and tests to the new project boundary

## Detailed Work Packages

1. Project creation
   - create the archive-provider project with the appropriate target framework
   - add the required project references to `RepoMigrator.Core`
   - add the project to the solution and test references where needed
2. Model relocation
   - move `ArchiveMigrationSourceDefinition` and `ArchiveSourceLocationKind`
   - move archive import plan, checkpoint, and state models
   - keep provider-agnostic models in `RepoMigrator.Core`
3. Service relocation
   - move `DirectoryArchiveSnapshotSourceProvider`
   - update namespaces and references in tests and consuming code
   - confirm no archive-specific implementation files remain in `RepoMigrator.Core`
4. Validation
   - build after relocation
   - run targeted model and directory-source tests
   - update task notes that still mention provisional Core placement

## Deliverables

- New archive-provider project in the solution
- Archive-specific model and service files relocated out of `RepoMigrator.Core`
- Updated tests and references
- Validation evidence through build and targeted tests

## Dependencies

- `T-RepoMigrator-035` - `Relocate provider-specific archive and compression files out of core`

## Acceptance Criteria

- Archive-specific models no longer live in `RepoMigrator.Core`
- `DirectoryArchiveSnapshotSourceProvider` no longer lives in `RepoMigrator.Core`
- `RepoMigrator.Core` remains limited to provider-agnostic contracts and shared models
- Build and affected tests pass after relocation

## Implementation Summary

- Created the new archive-provider project:
  - `RepoMigrator\RepoMigrator.Providers.Archive\RepoMigrator.Providers.Archive.csproj`
- Added the new project to the RepoMigrator solution and referenced it from the test project.
- Moved archive-specific files out of `RepoMigrator.Core` into `RepoMigrator.Providers.Archive`:
  - `ArchiveMigrationSourceDefinition`
  - `ArchiveSourceLocationKind`
  - `ArchiveImportManifestVersion`
  - `ArchiveImportPlanStatus`
  - `ArchiveImportRunStatus`
  - `ArchiveImportCheckpointPhase`
  - `ArchiveImportCheckpoint`
  - `ArchiveImportPlanItem`
  - `ArchiveImportPlan`
  - `ArchiveImportSnapshotState`
  - `ArchiveImportState`
  - `Services\DirectoryArchiveSnapshotSourceProvider`
- Kept `RepoMigrator.Core` provider-agnostic by replacing archive-specific Core properties with generic provider-data dictionaries on:
  - `MigrationSourceDefinition`
  - `MigrationDestinationDefinition`
- Added conversion helpers in the archive-provider project so archive-specific definitions can translate to and from normalized provider data.
- Retargeted tests to reference the new archive-provider project and the relocated types.

## Validation

- `run_build` completed successfully.
- Targeted tests passed for:
  - `RepoMigrator.Tests.MigrationSourceDestinationModelsTests`
  - `RepoMigrator.Tests.ArchiveImportModelsTests`
  - `RepoMigrator.Tests.DirectoryArchiveSnapshotSourceProviderTests`
