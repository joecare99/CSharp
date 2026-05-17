# Task T-RepoMigrator-023 - Define concrete code target state for archive import

## Status

Draft

## Parent

- Feature `F-RepoMigrator-07` - `Archive and web snapshot source ingestion`
- Backlog Item `BI-RepoMigrator-009` - `Define archive snapshot source detection and ordering contracts`
- Backlog Item `BI-RepoMigrator-010` - `Plan driver-based archive extraction and metadata inspection`
- Backlog Item `BI-RepoMigrator-011` - `Define release ref naming and manual archive ordering workflow`
- Backlog Item `BI-RepoMigrator-012` - `Define DevOps-backed archive import status persistence and resume`

## Goal

Define the concrete technical target state for the first archive-import implementation slice so follow-up coding tasks can be assigned with clear file, interface, and orchestration boundaries while keeping RepoMigrator.Core provider-agnostic.

## Recommended First Slice

The first coding slice should stay intentionally narrow:

- archive source from local directory only
- Git as the only target type for archive-derived history generation
- `.zip` as the first implemented archive format
- one commit per archive snapshot
- mandatory tag creation for every imported archive snapshot
- optional release-branch creation from the tagged commit
- persisted plan and execution state under `DevOps`

HTTP sources, additional archive formats, and multi-commit reconstruction from archive entry timestamps should remain follow-up slices.

## Project-Boundary Correction

The original draft of this task was too Core-heavy. The corrected target state is:

- `RepoMigrator.Core` contains only provider-agnostic contracts and shared models.
- Archive-specific models, planners, and orchestration types live in an archive-provider project.
- Compression-format-specific implementations live in dedicated compression provider projects such as a Zip provider project.

The file lists below should therefore be read as ownership targets across the solution, not as a directive to place provider-specific files into `RepoMigrator.Core`.

## Proposed Shared Core Files

The target state should keep one primary type per file where practical.

### Shared source and destination models in `RepoMigrator.Core`

Recommended provider-agnostic files under `RepoMigrator/RepoMigrator.Core`:

- `MigrationSourceDefinition.cs`
- `MigrationSourceKind.cs`
- `MigrationSourcePlan.cs`
- `MigrationSourcePlanItem.cs`
- `MigrationDestinationDefinition.cs`
- `MigrationDestinationKind.cs`
- `MigrationDestinationCommit.cs`

### Archive-provider-owned models outside Core

Recommended files in an archive-provider project:

- `ArchiveMigrationSourceDefinition.cs`
- `ArchiveSourceLocationKind.cs`
- `ArchiveOrderingOptions.cs`
- `ArchiveRefNamingOptions.cs`
- `ArchiveBranchOptions.cs`
- `ArchiveManualPlanOptions.cs`
- `ArchiveSnapshotDescriptor.cs`
- `ArchiveOrderingEvidence.cs`
- `ArchiveOrderingSignal.cs`
- `ArchiveOrderingSignalKind.cs`
- `ArchiveOrderingDecision.cs`
- `ArchiveImportState.cs`
- `ArchiveImportSnapshotState.cs`
- `ArchiveImportCheckpoint.cs`
- `ArchiveImportCheckpointPhase.cs`
- `ArchiveImportRunStatus.cs`
- `ArchiveImportManifestVersion.cs`

### Archive and compression acquisition models outside Core

Recommended files in archive-provider or compression-provider projects:

- `DiscoveredArchiveItem.cs`
- `ArchiveInspectionResult.cs`
- `ArchiveEntryMetadata.cs`
- `ArchiveDriverSelectionResult.cs`
- `ArchiveDownloadResult.cs`

## Proposed Model Responsibilities

### `ArchiveMigrationSourceDefinition`

Represents the archive-backed source configuration and should be owned by the archive provider project.

Suggested fields:

- `ArchiveSourceLocationKind LocationKind`
- `string Location`
- `IReadOnlyList<string> AllowedExtensions`
- `bool RecursiveDirectoryScan`
- `bool AllowRelativeLinks`
- `ArchiveOrderingOptions Ordering`
- `ArchiveRefNamingOptions Naming`
- `ArchiveBranchOptions Branches`
- `ArchiveManualPlanOptions ManualPlan`

### `ArchiveSnapshotDescriptor`

Represents one discovered archive plus the metadata needed for planning and should be owned by the archive provider project.

Suggested fields:

- `string SnapshotId`
- `string ArchivePathOrUrl`
- `string ArchiveFileName`
- `string ArchiveBaseName`
- `string ArchiveExtension`
- `string DriverId`
- `string? DetectedVersionText`
- `DateTimeOffset? ArchiveCreatedTimestamp`
- `DateTimeOffset? NewestEntryTimestamp`
- `DateTimeOffset? ExternalLastWriteTimestamp`
- `int DiscoveryIndex`
- `int? ManualOrderIndex`
- `string ProposedTagName`
- `string? ProposedBranchName`
- `ArchiveOrderingEvidence OrderingEvidence`

### `ArchiveImportPlan`

Represents the durable reviewed plan stored under `DevOps` before and during execution and should be owned by the archive provider project.

Suggested fields:

- `int SchemaVersion`
- `string PlanId`
- `DateTimeOffset CreatedUtc`
- `ArchiveMigrationSourceDefinition Source`
- `RepositoryEndpoint Target`
- `IReadOnlyList<ArchiveImportPlanItem> Items`
- `ArchiveImportPlanStatus Status`
- `string? Notes`

### `ArchiveImportPlanItem`

Represents one ordered snapshot in the reviewed migration plan.

Suggested fields:

- `string SnapshotId`
- `int FinalOrderIndex`
- `ArchiveSnapshotDescriptor Snapshot`
- `string FinalTagName`
- `string? FinalBranchName`
- `bool CreateBranch`

### `ArchiveImportState`

Represents the resumable execution state for one plan and should be owned by the archive provider project.

Suggested fields:

- `int SchemaVersion`
- `string PlanId`
- `ArchiveImportRunStatus Status`
- `DateTimeOffset LastUpdatedUtc`
- `string? LastMachineName`
- `string? LastWorkspaceRoot`
- `ArchiveImportCheckpoint CurrentCheckpoint`
- `IReadOnlyList<ArchiveImportSnapshotState> Snapshots`
- `string? LastValidationSummary`

### `ArchiveImportSnapshotState`

Tracks idempotent progress for one planned snapshot.

Suggested fields:

- `string SnapshotId`
- `ArchiveImportCheckpointPhase Phase`
- `bool DownloadCompleted`
- `bool ExtractionCompleted`
- `bool CommitCompleted`
- `string? TargetCommitId`
- `bool TagCreated`
- `bool BranchCreated`
- `string? FailureSummary`
- `DateTimeOffset? LastAttemptUtc`

## Proposed New Abstractions

Recommended files under `RepoMigrator/RepoMigrator.Core/Abstractions`:

- `IMigrationSourceProvider.cs`
- `IMigrationSourceProviderFactory.cs`
- `IMigrationDestinationProvider.cs`
- `IMigrationDestinationProviderFactory.cs`

Recommended archive-provider-specific abstractions outside Core:

- `IArchiveSnapshotSource.cs`
- `IArchiveDriver.cs`
- `IArchiveDriverRegistry.cs`
- `IArchiveImportPlanner.cs`
- `IArchiveImportStateStore.cs`
- `IArchiveMigrationService.cs`
- `IArchivePlanIdFactory.cs`

### `IMigrationSourceProvider`

Responsibilities:

- provide a provider-style abstraction for non-target migration sources
- keep RepoMigrator.Core agnostic across repository-backed and archive-backed source types
- prepare a source-specific snapshot or work plan for downstream execution

Suggested members:

- `string Name { get; }`
- `bool CanHandle(MigrationSourceDefinition source)`
- `Task<MigrationSourcePlan> PrepareAsync(MigrationSourceDefinition source, CancellationToken ct)`

### `IMigrationSourceProviderFactory`

Responsibilities:

- resolve the matching source provider for a normalized source definition
- keep source-provider selection out of orchestration classes

Suggested members:

- `IMigrationSourceProvider Create(MigrationSourceDefinition source)`

### `IMigrationDestinationProvider`

Responsibilities:

- provide a provider-style abstraction for migration targets
- keep RepoMigrator.Core symmetric across source and destination extensibility
- allow future non-repository outputs such as sequential archive emission without redesigning the orchestration boundary

Suggested members:

- `string Name { get; }`
- `bool CanHandle(MigrationDestinationDefinition destination)`
- `Task InitializeAsync(MigrationDestinationDefinition destination, CancellationToken ct)`
- `Task WriteSnapshotAsync(string workDir, MigrationDestinationCommit metadata, IMigrationProgress progress, CancellationToken ct)`
- `Task FinalizeAsync(CancellationToken ct)`

### `IMigrationDestinationProviderFactory`

Responsibilities:

- resolve the matching destination provider for a normalized target definition
- keep destination-provider selection out of orchestration classes

Suggested members:

- `IMigrationDestinationProvider Create(MigrationDestinationDefinition destination)`

### `IArchiveSnapshotSource`

Responsibilities:

- discover archive candidates from a configured source
- normalize local path information into discovered archive items
- keep discovery independent from archive extraction

Suggested members:

- `Task<IReadOnlyList<DiscoveredArchiveItem>> DiscoverAsync(ArchiveMigrationSourceDefinition source, CancellationToken ct)`

### `IArchiveDriver`

Responsibilities:

- inspect one concrete archive format
- return timestamp and entry metadata
- extract an archive into a temp folder

Suggested members:

- `string Id { get; }`
- `bool CanHandle(string archiveName)`
- `Task<ArchiveInspectionResult> InspectAsync(string archiveFilePath, CancellationToken ct)`
- `Task ExtractToAsync(string archiveFilePath, string targetDirectory, CancellationToken ct)`

### `IArchiveDriverRegistry`

Responsibilities:

- select the matching archive driver
- fail clearly when no driver supports the archive

Suggested members:

- `IArchiveDriver Resolve(string archiveName)`
- `bool TryResolve(string archiveName, out IArchiveDriver? driver)`

### `IArchiveImportPlanner`

Responsibilities:

- inspect discovered archives
- derive ordering evidence
- generate tags and optional branch names
- build the durable import plan

Suggested members:

- `Task<ArchiveImportPlan> BuildPlanAsync(ArchiveMigrationSourceDefinition source, RepositoryEndpoint target, CancellationToken ct)`

### `IArchiveImportStateStore`

Responsibilities:

- read and write persisted plan and state files under `DevOps`
- keep manifest writes deterministic and portable
- provide resume-oriented load methods

Suggested members:

- `Task SavePlanAsync(ArchiveImportPlan plan, CancellationToken ct)`
- `Task<ArchiveImportPlan?> TryLoadPlanAsync(string planId, CancellationToken ct)`
- `Task SaveStateAsync(ArchiveImportState state, CancellationToken ct)`
- `Task<ArchiveImportState?> TryLoadStateAsync(string planId, CancellationToken ct)`

### `IArchiveMigrationService`

Responsibilities:

- execute a reviewed archive import plan
- resume from persisted checkpoints
- use an existing target provider for commit, tag, and branch creation

Suggested members:

- `Task<ArchiveImportPlan> PreparePlanAsync(ArchiveMigrationSourceDefinition source, RepositoryEndpoint target, CancellationToken ct)`
- `Task ExecutePlanAsync(string planId, IMigrationProgress progress, CancellationToken ct)`
- `Task ResumePlanAsync(string planId, IMigrationProgress progress, CancellationToken ct)`

## Recommended Concrete Implementations

Recommended files under `RepoMigrator/RepoMigrator.Core/Services` or a dedicated archive namespace:

Recommended implementation ownership outside Core:

- archive-provider project:
  - `DirectoryArchiveSnapshotSource.cs`
  - `ArchiveDriverRegistry.cs`
  - `ArchiveImportPlanner.cs`
  - `ArchiveOrderingService.cs`
  - `ArchiveRefNamingService.cs`
  - `DevOpsArchiveImportStateStore.cs`
  - `ArchiveMigrationService.cs`
  - `ArchivePlanIdFactory.cs`
- compression-provider projects:
  - `ZipArchiveDriver.cs`
  - additional format drivers later

### Important design choice

Do not overload the existing `IVersionControlProvider` abstraction with archive-source behavior. Keep `IVersionControlProvider` for genuine repository operations and introduce `IMigrationSourceProvider` for source-side extensibility. Also introduce `IMigrationDestinationProvider` as the broader target-side abstraction so future destination types such as sequential archive output can fit the same model. Keep repository-to-repository migration in `MigrationService` and add `ArchiveMigrationService` beside it in an archive-provider project, or later converge both onto higher-level source-provider and destination-provider orchestration paths once the abstractions prove stable.

This keeps:

- repository migration stable,
- provider selection explicit,
- source and destination extensibility symmetric,
- archive import testable in isolation,
- project ownership explicit,
- and resume logic away from the repository-provider abstraction.

## Recommended Provider Changes

The provider model should split into source providers and destination providers, with version-control providers remaining the repository-specific implementation detail for the first destination slice.

### New source-provider direction

Recommended new shared models:

- `MigrationSourceDefinition.cs`
- `MigrationSourceKind.cs`
- `MigrationSourcePlan.cs`
- `MigrationSourcePlanItem.cs`
- `MigrationDestinationDefinition.cs`
- `MigrationDestinationKind.cs`
- `MigrationDestinationCommit.cs`

Recommended first source providers:

- `RepositoryMigrationSourceProvider`
- `DirectoryArchiveSnapshotSourceProvider` in an archive-provider project

Recommended first destination providers:

- `VersionControlMigrationDestinationProvider`
- `SequentialArchiveMigrationDestinationProvider` later

Recommended first compression providers:

- `Compression.Provider.Zip`
- other compression provider projects later

The repository-backed destination contract still needs a small extension for release refs.

### `IVersionControlProvider`

Recommended additions:

- `Task<string?> GetCurrentCommitIdAsync(CancellationToken ct)`
- `Task CreateTagAsync(string tagName, string? commitId, CancellationToken ct)`
- `Task CreateBranchAsync(string branchName, string? commitId, CancellationToken ct)`
- `Task<bool> TagExistsAsync(string tagName, CancellationToken ct)`
- `Task<bool> BranchExistsAsync(string branchName, CancellationToken ct)`

Reason:

Archive execution must be able to record idempotent checkpoints and resume after commit creation but before tag or branch creation, while repository operations stay on the VCS-specific provider contract beneath the broader destination-provider abstraction.

## DevOps Persistence Layout

Recommended folder layout:

- `DevOps/Data/RepoMigrator/ArchiveImports/<PlanId>/plan.json`
- `DevOps/Data/RepoMigrator/ArchiveImports/<PlanId>/state.json`
- `DevOps/Data/RepoMigrator/ArchiveImports/<PlanId>/ordering-overrides.json`
- `DevOps/Data/RepoMigrator/ArchiveImports/<PlanId>/events.jsonl` optional later

### File responsibilities

- `plan.json`
  - durable reviewed import plan
  - portable across machines
- `state.json`
  - latest checkpoint and per-snapshot execution status
  - portable logical state only
- `ordering-overrides.json`
  - optional user-maintained manual order and naming overrides
- `events.jsonl`
  - optional append-only audit stream for later diagnostics

## Resume Checkpoint Design

Recommended checkpoint phases:

1. `PlanPrepared`
2. `SnapshotReady`
3. `ExtractionCompleted`
4. `CommitCompleted`
5. `TagCreated`
6. `BranchCreated`
7. `SnapshotCompleted`
8. `RunCompleted`

### Resume rules

- If `CommitCompleted = true` and `TagCreated = false`, verify the commit id still exists and create only the missing tag.
- If `TagCreated = true` and `BranchCreated = false`, create only the missing branch when branch creation is enabled.
- If the target branch head diverged unexpectedly from the recorded checkpoint, stop and require operator review.
- Never rely on persisted temp extraction paths after resume.

## Recommended Dependency Injection Changes

Expected registration changes in composition roots:

- register `IMigrationSourceProvider` implementations
- register `IMigrationSourceProviderFactory`
- register `IMigrationDestinationProvider` implementations
- register `IMigrationDestinationProviderFactory`
- register `IArchiveSnapshotSource` implementations
- register `IArchiveDriver` implementations
- register `IArchiveDriverRegistry`
- register `IArchiveImportPlanner`
- register `IArchiveImportStateStore`
- register `IArchiveMigrationService`

For the first slice, only `DirectoryArchiveSnapshotSource`, `ZipArchiveDriver`, and a repository-backed `VersionControlMigrationDestinationProvider` need concrete registrations.

Those registrations should come from the owning provider projects rather than from `RepoMigrator.Core`.

## Recommended Test Surface

### New unit-test groups

- `ArchiveOrderingServiceTests`
- `ArchiveRefNamingServiceTests`
- `ArchiveDriverRegistryTests`
- `DirectoryArchiveSnapshotSourceTests`
- `DevOpsArchiveImportStateStoreTests`
- `ArchiveImportPlannerTests`
- `ArchiveMigrationServiceTests`

### First high-value scenarios

- local path source discrimination
- `.zip` driver selection
- compound-extension-safe base-name derivation
- stable ordering with manual override
- durable plan and state roundtrip under `DevOps`
- resume after interruption between commit and tag creation
- resume after interruption between tag and branch creation

## Explicit Non-Goals for the First Coding Slice

- HTTP download cache
- `.7z`, `.tar`, and `.tar.gz` implementation
- multi-commit reconstruction from archive entries
- SVN target support for archive imports
- sequential archive destination implementation
- append-only detailed event log

## Suggested Follow-up Coding Tasks

1. Keep only provider-agnostic shared contracts and shared models in `RepoMigrator.Core`.
2. Add archive-specific abstractions and models in an archive-provider project.
3. Implement `DirectoryArchiveSnapshotSource` in the archive-provider project.
4. Implement `ZipArchiveDriver` in a dedicated Zip compression provider project.
5. Implement archive ordering, naming, planning, and state-store services in the archive-provider project.
6. Implement `ArchiveMigrationService` with commit, tag, branch, and resume checkpoints in the archive-provider project.
7. Extend Git provider support for tag and branch idempotency operations.
8. Add targeted MSTest coverage for provider projects, plan building, and resume behavior.

## Open Questions

- Should `state.json` record machine identity only as diagnostics, or should it participate in safety validation?
- Should `CreateTagAsync` and `CreateBranchAsync` accept a null commit id meaning current HEAD, or should a concrete commit id always be required for resume safety?
- Should the first slice expose plan preparation and execution separately in the UI, or only as one guided workflow with persisted intermediate files?

## Detailed Work Packages

1. Core type layout
   - finalize the list of normalized source and destination models that truly belong in Core
   - group models into shared, provider-specific, destination-specific, persistence-specific, and sub-provider categories
   - identify which existing files can remain unchanged in the first slice
2. Abstraction layout
   - finalize `IMigrationSourceProvider`, `IMigrationDestinationProvider`, and their factories
   - move archive-specific contracts beneath the broader provider layers into provider projects
   - define how existing `IVersionControlProvider` implementations are wrapped by destination providers
3. Service layout
   - decide whether `ArchiveMigrationService` remains a dedicated orchestration service in the first slice and in which provider project it belongs
   - define planner, naming, ordering, and state-store service boundaries outside Core
   - define which service responsibilities remain deferred for later slices
4. Composition-root impact
   - identify DI registration changes in app and tool entry points
   - identify where provider factories are resolved today and how the broader provider layers fit across project boundaries
   - identify minimum non-breaking changes for the first code slice
5. Validation and follow-up slicing
   - map the target state to implementation-ready coding tasks
   - separate first-slice `.zip` plus local-directory work from later HTTP and archive-output work
   - identify the first validation checkpoints for builds and tests after implementation starts

## Acceptance Criteria

- The concrete target state is detailed enough to break directly into implementation tasks
- Shared, source-side, destination-side, and persistence responsibilities are separated clearly
- First-slice versus later-slice work remains explicit
- Composition-root and test impact are identified clearly
