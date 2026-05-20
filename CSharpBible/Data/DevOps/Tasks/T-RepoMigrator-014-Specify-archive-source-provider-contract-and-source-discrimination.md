# Task T-RepoMigrator-014 - Specify archive source provider contract and source discrimination

## Status

Draft

## Parent

- Backlog Item `BI-RepoMigrator-009` - `Define archive snapshot source detection and ordering contracts`

## Goal

Define the concrete contract changes needed to represent archive-backed sources in RepoMigrator.

## Scope

- Review whether the current endpoint model can represent HTTP(S) and local directory archive sources cleanly
- Propose source-discrimination rules for URLs, absolute paths, and relative paths
- Draft the archive snapshot descriptor shape required by orchestration and UI
- Record validation expectations for ambiguous or unsupported source inputs

## Deliverables

- A contract proposal covering source discrimination and normalized archive-source descriptors
- Example inputs and the expected normalized interpretation
- Identified follow-up code touchpoints in core models, provider selection, and UI state

## Open Questions

- Should archive sources extend `RepoType`, use a new source-kind model, or be introduced through a separate provider family?
- Which path forms should be supported in the first implementation beyond standard Windows absolute and relative paths?
