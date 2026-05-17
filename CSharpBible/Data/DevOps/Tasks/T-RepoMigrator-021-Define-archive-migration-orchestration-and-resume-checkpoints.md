# Task T-RepoMigrator-021 - Define archive migration orchestration and resume checkpoints

## Status

Draft

## Parent

- Backlog Item `BI-RepoMigrator-012` - `Define DevOps-backed archive import status persistence and resume`

## Goal

Define the execution flow and resume boundaries for archive-backed migrations.

## Scope

- Define checkpoint boundaries across discovery, planning, commit, tag, and optional branch creation
- Define how resumed runs re-validate target state before continuing
- Define idempotency expectations for already completed commits, tags, and branches
- Define failure handling for partial completion inside one snapshot step
- Record how runtime-only temp paths and downloads are recreated when not portable
- Define how source providers and destination providers participate in orchestration and resume decisions

## Deliverables

- Execution-state transition model
- Resume rules per checkpoint type
- Failure and recovery notes for interrupted runs
- A responsibility map for orchestration logic versus provider-specific resume checks

## Open Questions

- Should commit completion be recorded before or after tag creation when tags are mandatory?
- How should resume behave if a commit exists but the corresponding tag does not?

## Detailed Work Packages

1. Orchestration flow draft
   - define prepare, review, execute, resume, restart, and abandon flows
   - map which steps belong to source providers, planners, destination providers, and state stores
   - define first-slice behavior for local-directory archive imports into repository-backed destinations
2. Checkpoint model draft
   - define snapshot-level checkpoint phases and run-level statuses
   - define required data captured at each checkpoint boundary
   - define which checkpoint details are provider-agnostic versus provider-specific
3. Resume validation rules
   - define source re-validation rules on resume
   - define target re-validation rules for commit, tag, and branch progress
   - define operator-facing stop conditions for unsafe divergence
4. Failure and idempotency rules
   - define recovery rules for interruption between commit, tag, and branch steps
   - define retry behavior for transient versus non-transient failures
   - define how recreated temp state and optional downloads behave after resume

## Acceptance Criteria

- The orchestration flow identifies source-provider, destination-provider, and state-store responsibilities clearly
- Checkpoint boundaries are explicit enough for deterministic resume behavior
- Resume validation and unsafe-divergence stop conditions are documented
- Failure handling covers partial completion across commit, tag, and branch steps
