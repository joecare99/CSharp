# Task T-RepoMigrator-018 - Define archive core models and persisted state schema

## Status

Draft

## Parent

- Backlog Item `BI-RepoMigrator-009` - `Define archive snapshot source detection and ordering contracts`
- Backlog Item `BI-RepoMigrator-012` - `Define DevOps-backed archive import status persistence and resume`

## Goal

Define the core data models for archive sources, discovered snapshots, ordering evidence, import plans, and persisted execution state.

## Scope

- Draft normalized migration source and destination models plus archive-specific source extensions
- Draft archive source and snapshot descriptor models
- Draft ordering-evidence and naming-preview models
- Draft import-plan and execution-state manifests for `DevOps` persistence
- Distinguish portable fields from machine-specific runtime hints
- Record versioning expectations for the persisted schema
- Identify provider-agnostic schema sections versus provider-specific extension sections

## Deliverables

- Proposed core model list with responsibilities
- Manifest schema outline for plan and state persistence
- Compatibility notes for future schema evolution
- A field-level boundary note for source-provider, destination-provider, and shared schema ownership

## Open Questions

- Should plan and state be separate files or one combined manifest with staged sections?
- Which fields need stable identifiers so resume logic remains deterministic across machines?

## Detailed Work Packages

1. Shared model inventory
   - list the shared source and destination concepts needed above provider-specific implementations
   - separate normalized identifiers from provider-extension metadata
   - identify which existing models can be reused without distortion
2. Manifest partitioning
   - propose `plan.json`, `state.json`, and optional override-file responsibilities
   - define which fields belong in provider-agnostic root sections
   - define where provider-specific extension data can live without breaking portability
3. Identifier and schema-version design
   - define stable `PlanId`, `SnapshotId`, and destination-write identifiers
   - define schema-version handling and forward-compatibility expectations
   - define validation behavior for partial or older manifests
4. Portability analysis
   - mark machine-local hints versus portable authoritative state
   - define rules for source paths, workspace roots, and machine names in persisted manifests
   - document cross-machine resume constraints

## Acceptance Criteria

- The proposed model set includes normalized source and destination definitions above provider-specific details
- The persistence schema distinguishes shared and provider-specific data cleanly
- Stable identifiers for resume logic are defined explicitly
- Portability and schema-evolution expectations are documented
