# Task T-RepoMigrator-017 - Draft archive source core architecture

## Status

Draft

## Parent

- Feature `F-RepoMigrator-07` - `Archive and web snapshot source ingestion`
- Backlog Item `BI-RepoMigrator-009` - `Define archive snapshot source detection and ordering contracts`
- Backlog Item `BI-RepoMigrator-010` - `Plan driver-based archive extraction and metadata inspection`
- Backlog Item `BI-RepoMigrator-011` - `Define release ref naming and manual archive ordering workflow`

## Goal

Propose a concrete architecture for archive-backed migration sources that keeps RepoMigrator.Core provider-agnostic without forcing archive sources into version-control-provider semantics.

## Architecture Summary

The first archive-import slice should treat archive collections as a new migration-source provider type rather than as a new version-control repository type. Archive feeds are not repositories, do not expose native branches or commits, and should therefore not be modeled through `IVersionControlProvider` directly.

The architecture should keep the existing repository target model for Git commits and tags, while introducing a broader provider family for migration sources. Archive-source providers should discover, inspect, order, and materialize archive snapshots for the existing snapshot-migration pipeline.

RepoMigrator.Core must remain provider-agnostic. Provider-specific files must therefore live in their dedicated provider projects. Archive-provider files belong into archive-provider projects, and concrete compression-format concerns such as Zip belong into dedicated compression-provider projects rather than into Core or into overly broad archive projects.

## Proposed Contract Direction

## Reviewed Architecture Update - capability-gated structured change model

The reviewed direction should no longer treat special execution paths as ad-hoc exceptions in orchestration code. Special paths should instead be selected through explicit capabilities.

### Execution principle

- If source and target capabilities allow a safe direct transfer path, orchestration may use that path.
- The prime first example is direct Git-to-Git transfer.
- Otherwise, orchestration should fall back to a provider-agnostic structured file-change model.

This keeps direct fast paths available without making them the default architectural shape for every provider combination.

### Structured change model direction

The reviewed long-term model is:

- a repository is treated as a network of commits,
- each commit is represented as a structured list of file changes,
- input drivers emit normalized change data,
- output providers consume that change data according to their own capabilities.

Suggested provider-agnostic change-model concepts:

- `MigrationChangeSet`
  - logical change identifier
  - message
  - author data
  - timestamp
  - ordered file changes
- `MigrationFileChange`
  - path before change
  - path after change
  - operation kind such as add, modify, delete, rename, copy, mode change
  - content payload or text hunk data
- `MigrationTextChange`
  - ordered hunk list
  - normalized line-ending information when needed
- `MigrationTextHunk`
  - start line in the source view
  - removed text block
  - added text block
- `MigrationBinaryChange`
  - explicit binary add, replace, delete, rename, or opaque delta payload reference

### Root-remapping requirement

Patch and archive-derived inputs must support source-root relocation explicitly.

Suggested model direction:

- `PathRewriteRule`
  - `FromPrefix`
  - `ToPrefix`
  - optional normalization flags
- change extraction applies path rewriting during normalization so downstream providers operate only on canonical paths

This is required for patch collections where the historical archive root differs from the currently materialized working root.

### Patch-driver direction

Patch files should be handled through a dedicated input-driver path rather than by embedding patch-specific behavior into destination providers.

Suggested responsibilities:

- inspect `.patch` inputs,
- parse text hunks and recognized binary change markers,
- emit normalized file-change data,
- apply configured root-remapping before the changes leave the driver layer.

Reviewed refinement:

- the patch path should be treated as a general patch-driver direction rather than as a one-off patch-input exception,
- the first concrete slice should remain read-only and focus on parsing `.patch` input,
- later patch-output capability should be anticipated in the model and capabilities,
- destination providers and compatibility adapters should not assume patch-output support exists in the first slice.

### Destination-provider direction

Destination providers should consume normalized file-change data according to capability.

Suggested capability split:

- direct-transfer capability
- structured-change-apply capability
- fallback materialized-workdir capability
- destination-ref capability for tags and branches

This allows a phased migration where existing targets can still receive materialized work directories while newer targets adopt structured changes directly.

### Compatibility-adapter direction

The reviewed bridge from structured changes to existing targets should be an explicit compatibility adapter.

Suggested responsibilities:

- consume normalized change sets from structured sources such as the read-first patch driver,
- materialize deterministic work state for current Git and SVN target flows,
- preserve ordering, metadata, and destination-ref intent,
- remain removable once provider-native structured-change sinks are introduced.

This adapter should be treated as a transition layer, not as a hidden permanent architecture dependency.

### 1. Provider Model Split

Introduce a broader migration-source provider contract that can represent either an existing repository source or a new archive source.

Suggested model direction:

- `IMigrationSourceProvider`
  - repository-backed implementations
  - archive-backed implementations
- `MigrationSourceDefinition`
  - `MigrationSourceKind Kind`
  - provider-agnostic shared configuration only
  - provider-specific configuration owned by the responsible provider project

This keeps `RepoType` focused on actual repository systems while still letting RepoMigrator.Core remain provider-agnostic.

### 2. Archive Source Definition Ownership

Archive-specific source definitions should live in an archive-specific provider project instead of `RepoMigrator.Core`.

Suggested `ArchiveSnapshotSourceDefinition` ownership and fields:

- owned by an archive-provider project
- `ArchiveSourceLocationKind LocationKind`
  - `HttpIndex`
  - `LocalDirectory`
- `string Location`
- `IReadOnlyList<string> AllowedExtensions`
- `bool AllowRelativeLinks`
- `bool RecursiveDirectoryScan`
- archive-ordering and naming options that are specific to archive-based ingestion

### 3. Archive Snapshot Descriptor Ownership

Archive-specific snapshot descriptors should also live outside `RepoMigrator.Core` because they are provider-specific planning artifacts.

Suggested `ArchiveSnapshotDescriptor` fields:

- `string SnapshotId`
- `string ArchivePathOrUrl`
- `string ArchiveFileName`
- `string ArchiveBaseName`
- `string ArchiveExtension`
- `string DriverId`
- `string? DetectedVersionText`
- `VersionParseKind VersionParseKind`
- `DateTimeOffset? ArchiveCreatedTimestamp`
- `DateTimeOffset? NewestEntryTimestamp`
- `DateTimeOffset? ExternalLastWriteTimestamp`
- `int DiscoveryIndex`
- `int? ManualOrderIndex`
- `string ProposedTagName`
- `string? ProposedBranchName`
- `ArchiveOrderingEvidence OrderingEvidence`

### 4. Archive Ordering Evidence Ownership

Archive ordering evidence and related evidence signals belong with the archive provider implementation or an archive-specific shared project rather than in Core.

Suggested evidence model:

- `ArchiveOrderingEvidence`
  - `ArchiveOrderingDecision WinningSignal`
  - `IReadOnlyList<ArchiveOrderingSignal> Signals`
  - `string Explanation`
- `ArchiveOrderingSignal`
  - `ArchiveOrderingSignalKind Kind`
  - `string DisplayName`
  - `string? RawValue`
  - `bool Used`
  - `int Priority`

This keeps ordering explainable in the UI and testable in core logic.

## Proposed Service and Driver Model

### 1. Migration Source Providers

Introduce a broader source-provider abstraction with archive-specific providers beneath it.

Suggested source-provider direction:

- `IMigrationSourceProvider`
  - `bool CanHandle(MigrationSourceDefinition source)`
  - `Task<MigrationSourcePlan> PrepareAsync(MigrationSourceDefinition source, CancellationToken ct)`

Archive-backed implementations can still use a narrower discovery contract internally, but those contracts belong in provider-specific projects rather than in Core:

- `IArchiveSnapshotSource`
  - `Task<IReadOnlyList<DiscoveredArchiveItem>> DiscoverAsync(ArchiveSnapshotSourceDefinition source, CancellationToken ct)`

Initial archive implementations:

- `HttpArchiveSnapshotSourceProvider`
- `DirectoryArchiveSnapshotSourceProvider`

### 2. Archive Driver Services

Introduce a driver abstraction for inspecting and extracting archive formats:

- `IArchiveDriver`
  - `string Id { get; }`
  - `bool CanHandle(string archiveName)`
  - `Task<ArchiveInspectionResult> InspectAsync(string archiveFilePath, CancellationToken ct)`
  - `Task ExtractToAsync(string archiveFilePath, string targetDirectory, CancellationToken ct)`

Supporting registry:

- `IArchiveDriverRegistry`
  - resolves the first matching driver
  - reports unsupported archive formats clearly

Initial drivers:

- `ZipArchiveDriver` in a dedicated Zip compression provider project
- `SevenZipArchiveDriver` in a dedicated 7z compression provider project
- `TarArchiveDriver` in a dedicated Tar provider project
- `TarGzArchiveDriver` in a dedicated TarGz provider project

## Proposed Ordering Strategy

The first implementation should use a stable precedence pipeline instead of a fuzzy heuristic.

### Ordering precedence

1. `ManualOrderIndex`
2. Parsed version key when enabled and uniquely comparable
3. `ArchiveCreatedTimestamp`
4. `NewestEntryTimestamp`
5. `ExternalLastWriteTimestamp`
6. `ArchiveBaseName`
7. `DiscoveryIndex`

### Ordering notes

- Manual ordering fully overrides inferred ordering.
- Version parsing should be optional and diagnostic-driven because ambiguous naming is expected.
- External file timestamps must remain a fallback only.
- Final tie-breakers must be deterministic so repeated runs produce identical plans.

## Proposed Naming Strategy

### Tag naming

Default rule:

- Start with archive file name without extension.
- Treat compound extensions such as `.tar.gz` as one extension boundary.
- Preserve the raw base name by default.

Optional normalization rule for later refinement:

- Detect a shared non-version prefix across all selected archives.
- Replace that prefix with `v` only when the resulting name does not already start with `v` or `V`.
- Always show a preview before execution.

### Branch naming

Suggested initial rule when enabled:

- Keep the linear development line on `main` or `trunk`.
- Create optional release branches from the same commit that receives the tag.
- Derive the branch name from the final tag name through a dedicated branch prefix policy.

Suggested default branch prefix:

- `releases/`

Example:

- Archive `Product_1.2.0.zip`
- Trunk commit on `main`
- Tag `Product_1.2.0`
- Optional branch `releases/Product_1.2.0`

## Proposed Execution Flow

1. Detect source kind from input.
2. Discover archive files from HTTP index or local directory.
3. Resolve a matching archive driver for each discovered archive.
4. Inspect each archive and collect ordering metadata.
5. Build a draft import plan with proposed order, tags, and optional branches.
6. Allow user review and manual reordering before execution.
7. For each ordered archive:
   - download if necessary
   - extract to a temp directory
   - commit the full snapshot to `main` or `trunk`
   - create a tag on the resulting commit
   - optionally create a release branch from the same commit
8. Flush and finalize the target provider.

## Integration with Existing RepoMigrator Core

### Recommended minimum-change approach

- Keep `RepositoryEndpoint` for targets.
- Keep `IVersionControlProvider` for target repository operations and genuine VCS behavior.
- Add a broader source-provider layer for source-side migration inputs.
- Plan a parallel destination-provider layer above VCS-specific target behavior.
- Implement archive collections as source providers beneath that layer in dedicated provider projects.
- Reuse the existing temp-directory snapshot commit flow once an archive has been extracted where the reviewed structured-change path is not yet in place.
- Introduce a compatibility adapter for structured changes so read-first patch-driver output can flow into existing Git and SVN targets without a big-bang rewrite.

### Why this direction is preferred

- Existing Git and SVN provider behavior stays isolated.
- RepoMigrator.Core stays agnostic through a provider model instead of hardcoded source kinds.
- Archive-source logic remains testable without pretending to be a repository provider.
- Future non-repository destinations such as sequential archive output can fit the same architecture.
- Provider-specific files and sub-providers can evolve independently in their own projects.
- The current migration service and target-provider abstractions can be extended incrementally instead of rewritten.
- The read-first patch-driver slice can be introduced without forcing immediate patch-output implementation.
- Structured-change adoption can progress incrementally through an explicit compatibility bridge.

## Initial Implementation Slice Recommendation

The first delivery slice should deliberately stay narrow.

### Recommended scope

- Git target only for archive-derived history generation
- One commit per archive snapshot
- Mandatory tag creation for every archive snapshot
- Optional release branch creation from the tagged commit
- Local-directory source first, HTTP index second if needed for delivery slicing
- Driver support for `.zip` first, then `.7z`, `.tar`, and `.tar.gz`

### Explicitly deferred from the first slice

- Splitting one archive into multiple commits from internal file timestamps
- Reconstructing partial deltas between archives before commit
- Non-Git release-ref generation targets
- Sequential archive-output destination implementation
- Arbitrary web crawling beyond a known page or feed
- Patch-output generation from normalized change sets

## Examples

### Example source interpretation

Input: `https://example.org/releases/`

- `MigrationSourceKind = ArchiveCollection`
- `LocationKind = HttpIndex`

Input: `C:\Archives\VendorDrop`

- `MigrationSourceKind = ArchiveCollection`
- `LocationKind = LocalDirectory`

Input: `..\..\Snapshots`

- `MigrationSourceKind = ArchiveCollection`
- `LocationKind = LocalDirectory`

### Example ordering signals

Archive `Product_2.0.0.zip`

- Parsed version: `2.0.0`
- Archive created timestamp: unavailable
- Newest entry timestamp: `2020-02-03`
- External last write timestamp: `2024-01-15`

Decision:

- Use parsed version if the surrounding set is consistently comparable.
- Otherwise use `2020-02-03`, not the copied outer file date.

## Validation Considerations

- Unit tests should cover source discrimination, archive-driver resolution, ordering precedence, tie-breaking, and tag-name derivation.
- Integration-style tests should cover full archive-to-Git snapshot flows with temp-directory extraction.
- UI or workflow tests should cover preview visibility for inferred order and naming output.
- Architecture tests and contract-focused tests should later cover capability selection, path-rewrite normalization, and structured file-change compatibility across drivers and targets.

## Open Questions

- Should HTTP sources cache downloaded archives before plan review, or only after execution starts?
- Should the manual plan be stored as part of app state or as an explicit reusable manifest file?
- Should branch creation default to off even when tag creation is always on?
- Which managed or external-tool dependency is acceptable for `.7z` support across the solution target frameworks?

## Next Refinement Steps

1. Map the proposed source wrapper onto the current migration-service API surface.
2. Refine the archive-driver contract against real format-library constraints and provider-project boundaries.
3. Define the target project split for archive providers and compression providers.
4. Create code-oriented tasks for models, orchestration, relocation, and tests after the API direction is accepted.
5. Review a provider-agnostic structured file-change model before any broad implementation.
6. Define explicit capability contracts for direct transfer versus structured-change execution.
7. Define path-rewrite handling for moved source roots in patch and archive-derived inputs.
8. Define the read-first patch-driver slice with separate read and write capabilities.
9. Define the compatibility adapter that bridges structured changes into current target providers.
