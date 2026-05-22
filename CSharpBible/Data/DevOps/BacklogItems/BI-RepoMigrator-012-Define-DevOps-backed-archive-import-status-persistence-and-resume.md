# Backlog Item BI-RepoMigrator-012 - Define DevOps-backed archive import status persistence and resume

## Status

Draft

## Parent

- Epic `E-RepoMigrator-001` - `RepoMigrator migration workbench`
- Feature `F-RepoMigrator-07` - `Archive and web snapshot source ingestion`

## Goal

Define how archive-import planning and execution state is persisted inside the `DevOps` workspace so interrupted migrations can be resumed reliably, including on another machine.

## Description

Archive-backed migration may involve long-running discovery, metadata inspection, ordering review, download, extraction, commit, tag, and optional branch steps. The workflow needs durable status persistence so the current import plan and execution checkpoint survive interruptions.

The persisted state should live in the repository-local `DevOps` area rather than only in transient application state. That allows an interrupted import to continue later in the same workspace and, when the relevant source location is still accessible, on another machine that has the same workspace content.

The design should persist portable logical state, not machine-specific temp paths. Absolute local source paths, credentials, and transient extraction directories must be treated carefully so the persisted state remains useful without becoming misleading or unsafe.

## Scope

- Define a durable archive-import plan manifest stored under `DevOps`
- Define an execution-state manifest with resumable checkpoints
- Define which fields must stay machine-independent versus machine-local
- Define how completed commits, tags, and optional branches are recorded for idempotent resume
- Define validation and recovery behavior when the source, target, or manifest content changed since the last run
- Define operator workflow for resuming, restarting, or abandoning a persisted import plan
- Clarify which persisted artifacts are source-provider-specific, destination-provider-specific, or provider-agnostic

## Acceptance Criteria

- Archive-import planning and execution state is represented through durable `DevOps` artifacts
- Resume checkpoints are explicit enough to continue after interruption without repeating already completed target operations unnecessarily
- The design distinguishes portable state from machine-specific runtime state
- Cross-machine continuation constraints are documented clearly
- Recovery behavior is defined for changed manifests, missing sources, or diverging target repository state
- The persistence model documents how symmetric source and destination provider abstractions participate in resume behavior

## Assumptions

- The `DevOps` directory is available to users as a shared, reviewable planning and status area for this repository
- Resume support should prefer deterministic re-validation over blindly trusting stale runtime artifacts
- Sensitive values such as credentials should not be persisted in plain status artifacts

## Risks

- Persisting too little state may force expensive rework after interruption
- Persisting too much machine-specific state may make cross-machine resume brittle
- Resume logic may become unsafe if target-side divergence is not detected before continuing
- Provider-specific checkpoint details may become coupled too tightly to one destination type if the schema is not normalized carefully

## Open Questions

- Should persisted status live under `DevOps/Data`, a dedicated `DevOps/State` folder, or a more specific RepoMigrator subfolder?
- Should source paths be stored exactly as entered, normalized, or both?
- How should resume behavior react when the target branch head no longer matches the previously recorded checkpoint?
- Should downloaded HTTP archives be cached as optional reusable artifacts or always re-fetched on resume?

## Next Refinement Steps

1. Draft the manifest schema for import plans and runtime checkpoints
2. Define checkpoint boundaries across discovery, review, download, extraction, commit, tag, and branch steps
3. Separate provider-agnostic persisted state from provider-specific extension fields
4. Split follow-up work into core-model, orchestration, and test tasks once the persistence model is agreed

## Planned Implementation Tasks

- `T-RepoMigrator-027` - `Implement archive plan and state models`
- `T-RepoMigrator-031` - `Implement archive import planner and DevOps state store`
- `T-RepoMigrator-033` - `Implement archive migration service and resume flow`
- `T-RepoMigrator-034` - `Test archive import first slice`
