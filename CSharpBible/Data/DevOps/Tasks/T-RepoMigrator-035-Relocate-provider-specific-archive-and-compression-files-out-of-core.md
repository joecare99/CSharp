# Task T-RepoMigrator-035 - Relocate provider-specific archive and compression files out of core

## Status

Done

## Parent

- Feature `F-RepoMigrator-07` - `Archive and web snapshot source ingestion`
- Backlog Item `BI-RepoMigrator-009` - `Define archive snapshot source detection and ordering contracts`
- Backlog Item `BI-RepoMigrator-010` - `Plan driver-based archive extraction and metadata inspection`
- Backlog Item `BI-RepoMigrator-012` - `Define DevOps-backed archive import status persistence and resume`

## Goal

Restore the intended project boundaries by moving provider-specific archive and compression files out of `RepoMigrator.Core` into dedicated provider projects.

## Scope

- Identify archive-specific files currently placed in `RepoMigrator.Core`
- Identify compression-format-specific files that should live in dedicated sub-provider projects
- Define the target project map for archive providers and compression providers
- Move provider-specific files while preserving provider-agnostic contracts in `RepoMigrator.Core`
- Update tests, references, and planning notes to reflect the corrected project boundaries

## Detailed Work Packages

1. Inventory and classification
   - list the currently added `Archive*` files and archive-specific services in `RepoMigrator.Core`
   - classify them into provider-agnostic versus provider-specific responsibilities
   - identify which compression concerns need a dedicated project boundary
2. Target project map
   - define the archive-provider project that owns archive source discovery, archive planning, archive state, and archive orchestration
   - define dedicated compression-provider projects such as a Zip compression provider project
   - define which shared contracts remain in `RepoMigrator.Core`
3. Relocation execution
   - move provider-specific files into the selected provider projects
   - adjust namespaces, project references, and test references
   - keep `RepoMigrator.Core` limited to provider-agnostic abstractions and shared models
4. Validation and follow-up
   - update affected DevOps task notes that previously used provisional Core placement
   - build the solution and run the relevant tests after relocation
   - identify remaining follow-up items for additional archive and compression projects

## Deliverables

- A corrected provider-project map for archive and compression concerns
- Relocated provider-specific files outside `RepoMigrator.Core`
- Updated tests and project references
- Updated DevOps documentation noting the corrected architecture

## Current Inventory Snapshot

The following provider-specific archive files are currently placed in `RepoMigrator.Core` and should be treated as relocation candidates:

- `RepoMigrator.Core\ArchiveMigrationSourceDefinition.cs`
- `RepoMigrator.Core\ArchiveSourceLocationKind.cs`
- `RepoMigrator.Core\ArchiveImportManifestVersion.cs`
- `RepoMigrator.Core\ArchiveImportPlanStatus.cs`
- `RepoMigrator.Core\ArchiveImportRunStatus.cs`
- `RepoMigrator.Core\ArchiveImportCheckpointPhase.cs`
- `RepoMigrator.Core\ArchiveImportCheckpoint.cs`
- `RepoMigrator.Core\ArchiveImportPlanItem.cs`
- `RepoMigrator.Core\ArchiveImportPlan.cs`
- `RepoMigrator.Core\ArchiveImportSnapshotState.cs`
- `RepoMigrator.Core\ArchiveImportState.cs`
- `RepoMigrator.Core\Services\DirectoryArchiveSnapshotSourceProvider.cs`

The following shared files currently remain valid Core ownership:

- `MigrationSourceKind.cs`
- `MigrationSourceDefinition.cs`
- `MigrationSourcePlan.cs`
- `MigrationSourcePlanItem.cs`
- `MigrationDestinationKind.cs`
- `MigrationDestinationDefinition.cs`
- `MigrationDestinationCommit.cs`
- `Abstractions\IMigrationSourceProvider.cs`
- `Abstractions\IMigrationSourceProviderFactory.cs`
- `Abstractions\IMigrationDestinationProvider.cs`
- `Abstractions\IMigrationDestinationProviderFactory.cs`

## Proposed Target Project Map

### 1. Shared provider-agnostic project

- `RepoMigrator.Core`
  - shared source and destination abstractions
  - shared normalized source and destination models
  - shared orchestration contracts that do not depend on one provider family

### 2. Archive-provider project

Recommended new project:

- `RepoMigrator.Providers.Archive`

Proposed ownership:

- archive source definitions
- archive import plan and state models
- archive planning and archive-specific state persistence
- archive source provider implementations such as local-directory and later HTTP-index sources
- archive migration orchestration that is specific to archive-based ingestion

### 3. Compression sub-provider projects

Recommended first dedicated sub-provider project:

- `RepoMigrator.Providers.Compression.Zip`

Likely later projects:

- `RepoMigrator.Providers.Compression.SevenZip`
- `RepoMigrator.Providers.Compression.Tar`
- `RepoMigrator.Providers.Compression.TarGz`

Proposed ownership:

- concrete format-specific inspection and extraction logic
- format-specific driver classes and helper models
- external dependency integration per compression format where needed

### 4. Repository-backed destination provider ownership

Repository-backed destination provider implementations should live in provider projects that already own the relevant target technology, for example the Git provider project, or in a dedicated migration-destination provider project only if shared logic grows enough to justify it.

## Classification Rules

Use the following classification during relocation:

### Keep in `RepoMigrator.Core`

- abstractions that mention no concrete provider family
- normalized shared models consumed across multiple provider families
- shared orchestration contracts independent of archive, Git, SVN, or compression details

### Move to `RepoMigrator.Providers.Archive`

- any type whose name or semantics are archive-specific
- archive import plan and archive resume state models
- archive ordering and naming services
- archive-specific state stores and archive-specific orchestration services
- directory and HTTP archive source providers

### Move to dedicated compression provider projects

- concrete `.zip`, `.7z`, `.tar`, or `.tar.gz` extraction and inspection logic
- format-specific helper types
- dependencies that are only required for one compression format

## Proposed Relocation Sequence

1. Create the new provider projects without moving code yet
   - `RepoMigrator.Providers.Archive`
   - `RepoMigrator.Providers.Compression.Zip`
2. Move archive-specific model files from Core into `RepoMigrator.Providers.Archive`
3. Move `DirectoryArchiveSnapshotSourceProvider` from Core into `RepoMigrator.Providers.Archive`
4. Add new archive-specific tests or retarget existing tests to reference the archive-provider project
5. Keep Zip implementation work blocked until the dedicated Zip project exists
6. Update remaining task plans to target the new projects explicitly
7. Build and test after each relocation slice to keep the correction low-risk

## Immediate Follow-up Task Split

Recommended follow-up tasks after this planning refinement:

- `T-RepoMigrator-036` - `Create archive provider project and move archive-specific models out of core`
- `T-RepoMigrator-037` - `Create Zip compression provider project and implement zip driver there`
- `T-RepoMigrator-038` - `Retarget archive-provider tests and project references after relocation`

## Validation Strategy

- Validate project creation before moving files
- Validate each relocation slice with build and targeted tests
- Update completed task notes that currently mention provisional Core placement
- Keep relocation commits or work slices small enough to isolate project-reference issues quickly

## Open Questions

- Should the archive-provider project be named `RepoMigrator.Providers.Archive`, `RepoMigrator.Providers.Archives`, or `RepoMigrator.ArchiveMigration`?
- Should compression providers sit under `RepoMigrator.Providers.Compression.*` or under a shorter `RepoMigrator.Compression.*` naming scheme?
- Should archive plan and resume-state models live directly in the archive-provider project, or in a small archive-shared project if multiple archive providers later reuse them?

## Dependencies

- `T-RepoMigrator-023` - `Define concrete code target state for archive import`
- `T-RepoMigrator-024` - `Define migration destination provider abstractions`

## Acceptance Criteria

- `RepoMigrator.Core` contains only provider-agnostic abstractions and shared models
- Archive-specific files live in an archive-specific provider project
- Compression-format-specific files live in dedicated compression provider projects
- The build and affected tests pass after relocation

## Validation Evidence

- Archive-specific models, planning, state, ordering, naming, persistence, and orchestration now live in `RepoMigrator.Providers.Archive`
- Compression-format-specific Zip inspection and extraction now live in `RepoMigrator.Providers.Compression.Zip`
- `RepoMigrator.Core` remains limited to provider-agnostic shared models and abstractions, including normalized source/destination contracts and destination ref abstraction needed by orchestration
- Repository-backed destination behavior was kept in the owning Git provider project via `VersionControlMigrationDestinationProvider` and Git-specific ref operations
- Tests and project references were retargeted to the owning provider projects instead of outdated Core placement assumptions
- Relocation and follow-up implementation slices were validated through successful targeted archive-import test coverage (`39/39`) and successful targeted Git/destination compatibility coverage (`31/31`)
- Full workspace build passed after the relocation and follow-up implementation work
