# Backlog Item BI-RepoMigrator-011 - Define release ref naming and manual archive ordering workflow

## Status

Draft

## Parent

- Epic `E-RepoMigrator-001` - `RepoMigrator migration workbench`
- Feature `F-RepoMigrator-07` - `Archive and web snapshot source ingestion`

## Goal

Define how archive snapshots become release tags, optional release branches, and a user-adjustable ordered import plan.

## Description

Every imported archive snapshot should produce a release tag, while the same snapshots also populate one linear development history on `main` or `trunk`. Optional release-branch creation may additionally expose snapshot states as branches when requested.

The current naming direction is to derive the tag name from the archive file name without extension. There is also interest in replacing a common shared prefix with `v` when the resulting name does not already start with `v`, but this rule is still under discussion and should not be silently locked in. A regex-based extraction and normalization option with example preview output is a likely follow-up.

Because inferred snapshot order may still be wrong even with archive metadata, the workflow must also define how users review and manually reorder the archive plan before execution.

The reviewed direction should also stay compatible with later mixed plans where a reviewed migration sequence may contain both archive-derived snapshots and patch-derived change sets. Naming, review, and ordering workflow decisions should therefore stay as destination-agnostic planning artifacts for as long as possible, with repository-specific ref creation remaining downstream behavior.

## Scope

- Define the mandatory tag-creation rule for every archive snapshot
- Define optional branch creation from archive snapshots
- Define the baseline naming rule using archive file names without extension
- Record the open prefix-to-`v` normalization discussion explicitly
- Define requirements for optional regex-based naming with example preview behavior
- Define how users inspect and manually reorder the archive import plan
- Clarify whether tags and optional branches are created before, during, or after the linear trunk commit flow
- Clarify how repository-backed destinations and future sequential archive destinations affect naming and ordering artifacts
- Clarify how later mixed plans with patch-derived changes should remain compatible with the same review workflow

## Acceptance Criteria

- Mandatory tag creation for each archive snapshot is documented
- Optional branch creation remains configurable and bounded by a clear workflow
- Baseline naming from archive file names without extension is documented
- The prefix-normalization discussion is recorded as an open design decision rather than assumed as final
- Regex-based naming preview requirements are visible for later refinement
- Manual reordering is represented as a first-class workflow requirement
- The reviewed workflow remains compatible with later mixed plans that include patch-derived changes
- The workflow notes which parts are destination-specific versus destination-agnostic

## Assumptions

- The linear `main` or `trunk` history remains the canonical reconstructed development line
- Tags should map closely to release artifacts and therefore be created for every imported archive snapshot
- Users need previewable naming output before running a long migration

## Risks

- Aggressive normalization may produce unexpected tag names or collisions
- Optional branch creation for every release may create clutter in repositories with many snapshots
- Manual reordering without a durable stored representation may be hard to reproduce later
- Destination-specific naming assumptions may leak into destination-agnostic planning if boundaries are not documented clearly

## Open Questions

- Should the default branch name always be `main`, or should `trunk` be equally first-class for archive migrations?
- Should prefix normalization apply only when all files share a detectable prefix pattern?
- How should naming collisions between inferred tags be resolved and previewed?
- Should the manual ordering plan be exportable for reuse or review?

## Next Refinement Steps

1. Draft naming examples from realistic archive sets to test the baseline rule
2. Distinguish destination-agnostic naming artifacts from repository-specific tag and branch behaviors
3. Define the review screen or manifest format for manual reordering
4. Split follow-up work into UI, orchestration, and test tasks after the workflow is agreed

## Planned Implementation Tasks

- `T-RepoMigrator-024` - `Define migration destination provider abstractions`
- `T-RepoMigrator-030` - `Implement archive ordering and naming services`
- `T-RepoMigrator-032` - `Implement repository-backed destination provider and Git ref operations`
