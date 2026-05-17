# Task T-RepoMigrator-020 - Define archive import planner and DevOps manifest layout

## Status

Draft

## Parent

- Backlog Item `BI-RepoMigrator-011` - `Define release ref naming and manual archive ordering workflow`
- Backlog Item `BI-RepoMigrator-012` - `Define DevOps-backed archive import status persistence and resume`

## Goal

Define the archive import planner output and the `DevOps` folder layout used for portable plan and state persistence.

## Scope

- Define the draft import-plan structure produced after discovery and inspection
- Define folder and file naming conventions under `DevOps` for archive-import runs
- Define where manual ordering overrides and naming previews are stored
- Define how checkpoints are updated during execution without losing auditability
- Define cleanup and retention expectations for completed or abandoned plans
- Define how provider-agnostic plan sections and provider-specific extension sections are stored side by side

## Deliverables

- Proposed `DevOps` manifest layout for archive-import plans and state
- Update strategy for checkpoint writes during execution
- Notes on reviewability, diffability, and cross-machine portability
- A concrete proposal for how source-provider and destination-provider data is partitioned inside the manifest layout

## Open Questions

- Should one import plan have its own subfolder keyed by a generated plan id or by a source-derived stable name?
- Should checkpoint updates be append-only, overwrite-based, or split into summary plus event log?

## Detailed Work Packages

1. Manifest folder layout
   - define the `DevOps/Data/RepoMigrator/...` folder structure
   - define naming rules for plan folders and durable file names
   - define retention behavior for completed, failed, and abandoned runs
2. Plan manifest structure
   - define provider-agnostic root sections for source, destination, ordering, and items
   - define provider-extension sections for archive-specific and repository-specific data
   - define where manual overrides and naming previews are persisted
3. State manifest structure
   - define checkpoint summary shape and per-snapshot progress entries
   - define how idempotent destination progress is recorded
   - define overwrite-versus-append strategy for checkpoint updates
4. Reviewability and portability
   - define JSON formatting and deterministic ordering expectations
   - define what must remain diff-friendly in versioned workspaces
   - define cross-machine assumptions and unsafe-state detection hints

## Acceptance Criteria

- The manifest layout is concrete enough to guide implementation
- Shared and provider-specific manifest sections are separated clearly
- The layout remains reviewable, diffable, and portable across machines where feasible
- Checkpoint update behavior is defined explicitly
