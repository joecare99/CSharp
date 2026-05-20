# Feature F-RepoMigrator-07 - Archive and web snapshot source ingestion

## Status

Draft

## Parent

- Epic `E-RepoMigrator-001 - RepoMigrator migration workbench`

## Goal

Allow RepoMigrator to ingest versioned source-code snapshots from either HTTP(S) download pages or local archive directories and reconstruct them into a repository-oriented history with a linear trunk plus release refs.

## Summary

This feature plans a new snapshot-source slice for RepoMigrator. The source may be an HTTP or HTTPS location that exposes downloadable archives, or a local absolute or relative directory path that contains archive files. The imported snapshots should be ordered into a time-oriented development line on `main` or `trunk`, create release tags for every archive-derived version, and optionally emit branches per release snapshot when explicitly requested.

The planning baseline assumes that version identifiers are useful but not always sufficient for unambiguous ordering. Archive-internal metadata should therefore participate in ordering, with file-system timestamps used only as fallback signals. Manual reordering must remain possible because both file names and outer file timestamps may be misleading after later file operations.

## In Scope

- Source detection based on HTTP(S) versus local directory paths
- A provider or adapter model for archive-backed snapshot sources
- Archive-driver abstraction with initial support targets for `.zip`, `.7z`, `.tar`, and `.tar.gz`
- Metadata extraction for ordering snapshots using archive-internal creation or entry timestamps where available
- Fallback ordering using the newest timestamp found inside an archive when stronger metadata is unavailable
- Manual snapshot reordering or explicit sequence overrides when inferred order is not trustworthy
- DevOps-backed persistence of archive import plans and execution checkpoints for resume scenarios
- Linear history reconstruction on a primary `main` or `trunk` branch
- Mandatory release-tag creation for each imported archive snapshot
- Optional release-branch creation derived from the same snapshot set
- Naming-policy planning for tags and optional branches based on archive file names
- Preview-oriented planning for optional regex-based name extraction or normalization

## Out of Scope

- Arbitrary web crawling beyond the explicitly configured archive page or feed
- Full package-format support for every existing archive type in the first slice
- Guaranteed semantic-version parsing for all legacy naming conventions
- Fine-grained reconstruction of original commit history inside a snapshot unless a later backlog item explicitly adds that behavior
- Remote publishing governance, protected-branch policy, or release workflow automation outside the migration scope

## Acceptance Criteria

- RepoMigrator planning clearly distinguishes HTTP(S) snapshot sources from local absolute or relative directory sources
- The archive-source design uses driver-style format handling instead of hardcoding all archive logic into one implementation
- The first supported archive formats are documented as `.zip`, `.7z`, `.tar`, and `.tar.gz`
- Snapshot ordering combines version cues with archive-internal metadata and a documented fallback strategy
- Operators can manually override the inferred order of archive snapshots
- Imported snapshots form one linear `main` or `trunk` history line
- A tag is planned for every imported archive snapshot
- Optional branch creation remains configurable instead of mandatory
- Tag naming is based on the archive file name without extension, with the planned prefix-normalization discussion recorded explicitly
- Open questions remain visible for regex-based naming and preview behavior instead of being silently assumed

## Related Backlog Items

- `BI-RepoMigrator-009` - `Define archive snapshot source detection and ordering contracts`
- `BI-RepoMigrator-010` - `Plan driver-based archive extraction and metadata inspection`
- `BI-RepoMigrator-011` - `Define release ref naming and manual archive ordering workflow`
- `BI-RepoMigrator-012` - `Define DevOps-backed archive import status persistence and resume`
- `BI-RepoMigrator-006` - `Expand automated test baseline for migration orchestration and tools`

## Architectural Direction

The preferred architecture is to model archive collections as a new migration-source provider type instead of treating them as another repository type. Archive feeds do not expose commits, branches, or native repository semantics, so they should not be forced into `IVersionControlProvider`. RepoMigrator.Core should stay provider-agnostic and should therefore contain only shared abstractions and shared models. Archive-specific files must live in archive-specific provider projects, and sub-provider concerns such as concrete compression formats must live in their own dedicated provider projects.

The current design direction separates:

- a broader source-provider layer for migration inputs,
- archive-source discovery for HTTP(S) index pages and local directories,
- archive-driver inspection and extraction per supported format,
- project boundaries that keep provider-specific implementation files out of `RepoMigrator.Core`,
- dedicated sub-provider projects for concrete compression or packaging formats such as Zip,
- explicit ordering evidence and manual override support,
- DevOps-backed plan and checkpoint persistence for interruption-safe resume workflows,
- and target-side repository commit, tag, and optional branch creation.

This direction is captured in task `T-RepoMigrator-017 - Draft archive source core architecture` and should guide the next implementation-oriented refinement pass.

The current recommended code target state is captured in task `T-RepoMigrator-023 - Define concrete code target state for archive import`. That target state must be interpreted with the corrected project-boundary rule: provider-agnostic contracts stay in `RepoMigrator.Core`, archive-specific implementations and archive-specific models live in archive-specific provider projects, and concrete compression-format implementations live in dedicated compression provider projects.

The planning direction now also includes a symmetric destination-provider layer above repository-specific target behavior so future outputs such as sequential archive emission can fit without redesigning RepoMigrator.Core. This destination-provider work is refined in task `T-RepoMigrator-024 - Define migration destination provider abstractions`.

The correction of already planned or implemented provider-specific file placement is tracked in task `T-RepoMigrator-035 - Relocate provider-specific archive and compression files out of core`.

## Persistence and Resume Direction

The archive-import workflow should persist its plan and execution state under `DevOps` so work can continue after interruption, including on another machine when source accessibility and target validation still match the recorded plan.

The preferred persistence direction is:

- keep a durable, reviewable import-plan manifest under `DevOps`,
- keep a separate execution-state or checkpoint manifest under `DevOps`,
- persist logical snapshot identity, ordering, naming, and target progress,
- avoid persisting credentials or non-portable temporary extraction paths as authoritative state,
- and require re-validation of source and target assumptions before resuming execution.

This direction is further refined in backlog item `BI-RepoMigrator-012 - Define DevOps-backed archive import status persistence and resume`.

## Risks

- Archive file names may suggest a misleading version order when vendors reused or changed naming conventions
- Outer file timestamps may be distorted by copying, extracting, or backup operations
- Archive-internal timestamps may be incomplete, inconsistent, or timezone-ambiguous across formats
- HTTP pages may expose inconsistent link structures that require tighter source-page assumptions than a local directory feed
- Tag-name normalization rules may become surprising if common prefixes are stripped too aggressively
- Resume state may become unsafe if source content or target repository state changes after the persisted checkpoint
- Provider-specific files placed in `RepoMigrator.Core` may weaken long-term architecture boundaries if not corrected early

## Open Questions

- Should the first implementation support Git only as the generated repository target for archive-derived histories?
- Should a future destination-provider layer support emitting sequential archives as an alternative migration target beside repositories?
- Which archive-internal date should win when multiple signals exist, for example archive comment metadata, entry creation timestamps, or entry modification timestamps?
- Should the default tag naming rule keep the raw archive base name, or normalize a common shared prefix into `v...` only when a version segment can be identified confidently?
- How should regex-based naming previews be presented so users can verify tag or branch output before migration starts?
- Should optional release branches be created from every archive tag, or only from a selected subset?
- How should manual reordering be represented: drag-and-drop in the UI, an explicit ordered list, or an importable mapping file?
- Which `DevOps` subfolder and manifest strategy should be the canonical location for portable archive-import status persistence?

## Next Refinement Steps

1. Define the source contract that distinguishes HTTP(S) archive feeds from local directory feeds
2. Specify the archive-driver abstraction and the metadata fields each driver must expose
3. Refine the ordering policy, including precedence, fallback, and manual override behavior
4. Define the naming policy for mandatory tags and optional branches, including preview examples
5. Define the DevOps-backed plan and checkpoint persistence model for interruption-safe resume
6. Confirm the symmetric source-provider and destination-provider abstractions above repository-specific implementations
7. Confirm the corrected project boundaries so provider-specific files and sub-providers live in dedicated provider projects
8. Confirm the concrete code target state, including new shared contracts, archive provider projects, destination providers, and Git target ref operations
9. Map the agreed archive orchestration path onto the current composition roots and UI workflow boundaries
10. Add dedicated implementation and test tasks once the first delivery slice is agreed
