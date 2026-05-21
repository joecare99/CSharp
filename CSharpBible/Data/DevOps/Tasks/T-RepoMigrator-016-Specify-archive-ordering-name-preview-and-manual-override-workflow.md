# Task T-RepoMigrator-016 - Specify archive ordering, name preview, and manual override workflow

## Status

Draft

## Parent

- Backlog Item `BI-RepoMigrator-011` - `Define release ref naming and manual archive ordering workflow`

## Goal

Define the user-visible workflow for reviewing inferred archive order and derived ref names before migration execution.

## Scope

- Describe how inferred ordering signals are shown to users
- Define how users manually reorder snapshots or override inferred ordering
- Define baseline tag-name derivation from archive file names without extension
- Record optional prefix normalization and regex preview requirements
- Clarify when optional release branches are generated in relation to tags and trunk commits

## Deliverables

- A workflow proposal for preview and override behavior
- Naming examples with expected tag and optional branch results
- A list of UI and orchestration follow-up tasks implied by the workflow

## Open Questions

- Should manual ordering be persisted as an internal settings artifact or as an explicit import manifest?
- How much preview detail is needed to make regex-based naming safe and understandable?
