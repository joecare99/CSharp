# Task T-RepoMigrator-016 - Specify archive ordering, name preview, and manual override workflow

## Status

Completed

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

## Reviewed Workflow Proposal

The archive import workflow should introduce one explicit review step between archive inspection and migration execution. That step must present the inferred snapshot order, the evidence behind that order, the derived tag names, and the optional release-branch names in one place before any target commit is created.

The review step should work from a durable reviewed import plan rather than from transient UI-only state. The inferred plan is created from discovery, archive inspection, ordering, and naming services, then written to the archive-import plan manifest under runtime-defined storage. User-approved manual ordering and naming overrides become part of that reviewed plan so execution and resume use the same authoritative values.

## Proposed User Flow

1. The user selects an archive-backed source and a target repository.
2. RepoMigrator discovers candidate archives and inspects archive metadata.
3. RepoMigrator builds a draft import plan with:
   - inferred final order,
   - ordering evidence per snapshot,
   - default tag names from archive file names without extension,
   - optional branch names when release branches are enabled.
4. The user enters a dedicated review step before execution.
5. The review step shows one ordered list or grid of snapshots with the effective order, evidence summary, and derived refs.
6. The user may manually reorder snapshots, edit final tag names, enable or disable branch creation per snapshot when that later becomes supported, or edit final branch names when branch creation is enabled.
7. RepoMigrator validates the reviewed plan for collisions, invalid names, missing values, and incomplete ordering.
8. The approved reviewed plan is persisted.
9. Execution uses only the reviewed plan, not a re-inferred transient ordering.

## Review Screen Requirements

Each snapshot row should show at least the following columns:

- final order index,
- archive file name,
- detected version text when available,
- archive-created timestamp when available,
- newest-entry timestamp when available,
- external last-write timestamp when available as fallback only,
- winning ordering reason,
- final tag name,
- final branch name when branch creation is enabled,
- validation state.

The UI should not only show the winning signal. It should also show the supporting signals that were considered so the operator can understand why the order was inferred and why a manual override may be needed. A collapsed summary plus expandable details is sufficient for the first slice.

## Manual Override Rules

- Manual reordering fully overrides inferred ordering for the affected snapshots.
- A manual reorder operation should immediately renumber the final order for all snapshots so the resulting plan remains gap-free and deterministic.
- The user should be able to reset one snapshot or the whole list back to inferred ordering.
- Manual tag-name edits and branch-name edits should override derived defaults without mutating the original archive metadata.
- Execution and resume must use the persisted reviewed values from the import plan manifest.
- Manual ordering should be persisted as explicit reviewed plan data, not as a hidden application-only settings artifact.

This reviewed direction aligns with the existing durable archive import plan and state manifests. The plan is the operator-reviewed contract; the state file tracks execution progress against that contract.

## Naming Rules

### Default tag naming

The baseline rule is intentionally simple and transparent:

- start with the archive file name,
- remove the archive extension,
- treat compound extensions such as `.tar.gz` as one boundary,
- preserve the raw base name by default.

Examples:

- `Product_1.2.0.zip` -> tag `Product_1.2.0`
- `Product_1.2.0.tar.gz` -> tag `Product_1.2.0`
- `release-2024-04-15.7z` -> tag `release-2024-04-15`

### Optional prefix normalization

Prefix normalization should remain opt-in. RepoMigrator may later offer a previewed normalization mode that detects a shared non-version prefix and replaces it with `v` only when the result remains unambiguous and does not already start with `v` or `V`.

This must never happen silently. The operator must see the before and after result for every snapshot before approving the plan.

### Regex-based extraction or normalization preview

Regex-based naming is powerful but risky. The minimum safe preview should show, per snapshot:

- the original archive file name,
- the archive base name after extension removal,
- the configured regex pattern,
- the matched capture or transformed value,
- the resulting final tag name,
- the resulting final branch name when enabled,
- validation warnings for empty results, duplicate names, or invalid ref syntax.

The plan cannot proceed to execution while regex-based naming produces duplicate or invalid final refs.

## Release Branch Relationship To Trunk Commits And Tags

The history model remains one linear development line on `main` or `trunk`.

For each reviewed archive snapshot:

1. materialize and commit the full snapshot to the linear branch,
2. create the mandatory release tag on that resulting commit,
3. optionally create the release branch from that same commit.

The optional release branch is therefore a parallel ref created from the already tagged snapshot commit. It does not represent a separate imported history line and must not change the order of the linear mainline reconstruction.

## Naming And Execution Examples

### Example 1 - Default naming without branch creation

Input archives:

1. `Product_1.0.0.zip`
2. `Product_1.1.0.zip`
3. `Product_2.0.0.zip`

Reviewed result:

- commits are created on `main` in the reviewed order,
- tags are `Product_1.0.0`, `Product_1.1.0`, and `Product_2.0.0`,
- no release branches are created.

### Example 2 - Default naming with release branches enabled

Input archives:

1. `Product_1.0.0.zip`
2. `Product_2.0.0.tar.gz`

Reviewed result:

- commit 1 on `main`, tag `Product_1.0.0`, optional branch `releases/Product_1.0.0`,
- commit 2 on `main`, tag `Product_2.0.0`, optional branch `releases/Product_2.0.0`.

### Example 3 - Manual reorder because timestamps are misleading

Input archives after inspection:

1. `Product_2.0.0.zip` with newer external timestamp
2. `Product_1.0.0.zip` with older external timestamp

If the operator determines that the reviewed order should be `1.0.0` then `2.0.0`, the manual order becomes authoritative. The persisted reviewed plan must store that final order so execution and resume do not fall back to the misleading timestamp signal.

### Example 4 - Optional normalization preview

Input archives:

1. `Product_1.0.0.zip`
2. `Product_1.1.0.zip`

Optional previewed normalization proposal:

- raw tags: `Product_1.0.0`, `Product_1.1.0`
- normalized preview: `v1.0.0`, `v1.1.0`

The operator must explicitly approve that normalization. Otherwise the raw tag names remain authoritative.

## Validation And Diagnostics Requirements

The review workflow should block execution when any of the following apply:

- duplicate final order indices,
- duplicate final tag names,
- duplicate final branch names among enabled branches,
- invalid tag or branch ref syntax,
- branch name present while branch creation is disabled,
- branch creation enabled but final branch name missing,
- unresolved regex output,
- no deterministic final order.

Warnings that should remain visible but not necessarily block execution include:

- weak ordering based only on fallback timestamps,
- conflicting metadata signals,
- missing version text,
- inferred order changed after a configuration option toggle.

## Implied UI Follow-up Tasks

- Add a dedicated archive-plan review step to the WPF workflow between inspection and execution.
- Add a snapshot review grid with ordering evidence, final tag preview, final branch preview, and validation state.
- Add reorder commands such as move up, move down, reset to inferred order, and reset all.
- Add edit affordances for final tag and branch names.
- Add a preview mode for optional normalization and regex-based extraction.
- Add inline validation and collision highlighting before execution can start.

## Implied Orchestration And Model Follow-up Tasks

- Extend the durable archive import plan so reviewed manual order and reviewed final refs are always persisted as authoritative execution input.
- Ensure archive execution consumes reviewed plan values without re-deriving order or names during resume.
- Add validation services for tag collisions, branch collisions, and invalid ref names before execution.
- Add tests that cover manual reorder persistence, reset behavior, naming overrides, and regex preview validation.
- Add per-snapshot diagnostics so weak ordering evidence remains visible in persisted plans and execution logs.

## Validation Evidence

- Reviewed `F-RepoMigrator-07`, `BI-RepoMigrator-009`, `T-RepoMigrator-017`, `T-RepoMigrator-023`, `T-RepoMigrator-031`, and `T-RepoMigrator-041` for consistency with the archive-import direction.
- Aligned the workflow decision with the existing reviewed direction that archive imports use a durable import plan plus separate execution state.
- Recorded the operator review step, manual override persistence rule, naming-preview behavior, and branch-creation timing in this task.

## Open Questions

- Should branch creation remain all-or-nothing for the first UI slice, or should the review step support per-snapshot branch enablement immediately?
- Should optional normalization offer only one reviewed built-in strategy at first, or also allow custom regex transformations in the same slice?
