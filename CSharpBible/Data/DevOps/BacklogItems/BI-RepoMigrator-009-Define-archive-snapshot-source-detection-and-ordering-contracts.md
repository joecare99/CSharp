# Backlog Item BI-RepoMigrator-009 - Define archive snapshot source detection and ordering contracts

## Status

Draft

## Parent

- Epic `E-RepoMigrator-001` - `RepoMigrator migration workbench`
- Feature `F-RepoMigrator-07` - `Archive and web snapshot source ingestion`

## Goal

Specify how RepoMigrator identifies archive-backed snapshot sources and how it derives a reliable import order for archive snapshots.

## Description

RepoMigrator should accept both HTTP(S)-based sources and local directory paths as archive-backed snapshot inputs. The contract must clearly describe how the application distinguishes web addresses from absolute or relative file-system paths without making users choose an unrelated repository type manually.

The same contract must also define how snapshot order is inferred. Version identifiers in archive names may help but are not always unambiguous. File-system timestamps are also weak signals because later copy operations may rewrite them. The ordering model therefore needs an explicit precedence strategy that prefers archive-internal metadata and uses outer timestamps only as fallback information. Manual user-defined reordering must remain available.

The reviewed direction should also prepare this source contract for the later structured-change path. Archive-backed planning remains snapshot-oriented in the first slice, but the source-side model should already leave room for explicit path rewrites and mixed plans where later slices can combine full snapshots with patch-derived changes.

## Scope

- Define source-type detection rules for `http://`, `https://`, absolute paths, and relative paths
- Define the normalized source model shared by UI, orchestration, and source-provider code
- Specify snapshot descriptor fields needed for ordering, including file name, detected version text, archive-internal timestamps, and fallback timestamps
- Define ordering precedence across version cues, archive-internal metadata, and fallback timestamps
- Define how manual reordering or explicit sequence overrides replace inferred ordering
- Identify validation messages for ambiguous or incomplete ordering input
- Clarify how the source model fits into a broader `IMigrationSourceProvider` abstraction without overloading `RepoType`
- Review how explicit root-remapping intent can be preserved for later structured-change slices without polluting target providers

## Acceptance Criteria

- The source contract distinguishes HTTP(S) archive feeds from local directory archive feeds
- Absolute and relative local paths are both represented without requiring a different source concept
- Snapshot descriptors include enough metadata to explain inferred ordering decisions
- The ordering policy documents precedence, fallback rules, and ambiguity handling
- Manual override capability is represented in the planning model
- The source contract is explicitly aligned with a broader source-provider model in RepoMigrator.Core
- The reviewed direction is compatible with explicit path-rewrite intent for later structured-change execution
- Open questions around incomplete metadata remain explicit

## Assumptions

- The first archive-source slice builds on the existing snapshot-migration concept instead of inventing a separate migration engine
- Source detection should stay simple and transparent rather than trying to infer every exotic URI or shell path format
- The ordering explanation must be visible enough to support troubleshooting when imported history looks unexpected

## Risks

- Overloading the existing repository endpoint model may blur the boundary between repository providers and archive-source providers
- Ambiguous version strings may lead to unstable ordering if no manual override exists
- Different archive formats may expose different internal timestamp quality levels
- A source contract that is too archive-specific may make future non-repository providers harder to fit cleanly
- Hidden moved-root assumptions may leak into downstream execution if path-rewrite intent is not represented explicitly enough

## Open Questions

- Should ordering use a scored heuristic, or a strict field-precedence pipeline with diagnostics for ties?
- Should manual overrides be stored inside RepoMigrator settings, a sidecar file, or a dedicated import manifest?
- How much path normalization is required for Windows relative paths and UNC paths in the first slice?

## Next Refinement Steps

1. Review the current endpoint and migration-option models for source-provider extensibility impact
2. Define a draft normalized source definition and snapshot descriptor with explicit ordering fields
3. Align the source contract with the symmetric destination-provider planning direction
4. Split implementation work into contract, UI, and test tasks once the model is stable

## Planned Implementation Tasks

- `T-RepoMigrator-025` - `Implement normalized migration source and destination models`
- `T-RepoMigrator-026` - `Implement source and destination provider abstractions`
- `T-RepoMigrator-028` - `Implement directory archive source provider`
