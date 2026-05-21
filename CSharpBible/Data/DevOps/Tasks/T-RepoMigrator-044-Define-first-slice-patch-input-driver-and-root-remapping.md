# Task T-RepoMigrator-044 - Define first-slice patch input driver and root remapping

## Status

Draft

## Parent

- Task `T-RepoMigrator-042` - `Review provider-agnostic structured file-change model`
- Feature `F-RepoMigrator-07` - `Archive and web snapshot source ingestion`

## Goal

Define the first implementation slice for a patch input driver that parses `.patch` files into normalized file-change lists and applies explicit root remapping during normalization.

## Scope

- Review patch-driver responsibilities and boundaries
- Review a general patch-driver direction that can later support both reading and writing patches
- Define root-remapping behavior for moved source roots
- Define the first structured output shape for text and binary changes emitted from `.patch` inputs
- Keep patch parsing out of destination providers

## Architecture Review Summary

The reviewed direction should treat patch handling as a dedicated patch-driver concern rather than as logic embedded in source orchestration or destination providers.

The reviewed long-term direction is a general patch driver that can:

- read patch files into normalized file-change models,
- later write normalized file-change models back into patch output,
- and keep patch-format specifics isolated from the rest of the migration pipeline.

For the first concrete slice, the driver should remain read-only.

## Reviewed Patch-Driver Direction

### General driver intent

The patch driver should be reviewed as a format-oriented component with two long-term responsibilities:

- patch input parsing
- patch output generation

The first implementation slice should include only the input-parsing capability.

### First-slice boundary

The reviewed first slice should:

- discover `.patch` inputs,
- parse supported patch formats into normalized change sets,
- apply root remapping during normalization,
- emit diagnostics for unsupported or ambiguous patch constructs,
- and stop before implementing patch output emission.

The reviewed first slice should explicitly defer:

- generating `.patch` output,
- round-trip guarantees,
- provider-native patch writing,
- and advanced binary delta reconstruction beyond conservative normalization.

## Reviewed Responsibilities

### 1. Patch input parsing

Reviewed responsibilities:

- detect supported patch-file formats,
- parse commit-oriented metadata when present,
- parse file-level changes and text hunks,
- normalize path pairs for before/after states,
- preserve enough source information for diagnostics and later output support.

### 2. Root remapping

Reviewed responsibilities:

- apply configured path-rewrite rules before normalized changes leave the driver layer,
- support moved source-root scenarios explicitly,
- reject rewrites that are ambiguous or escape the intended normalized root.

### 3. Binary handling

Reviewed first-slice strategy:

- support conservative binary representation,
- treat binary changes as add, replace, delete, rename, or opaque binary payload references where safely possible,
- avoid inventing unsupported delta semantics,
- keep format-specific binary markers available for later output-capable slices.

### 4. Output foresight

Even though the first slice is read-only, the reviewed model should already preserve enough structure to support later patch output.

Reviewed examples of forward-compatible preserved data:

- normalized file path pairs,
- ordered text hunks,
- file-level operation kind,
- binary markers or opaque payload references,
- original patch metadata useful for diagnostics or later round-trip comparisons.

## Reviewed Contract Direction

Suggested review direction:

- a patch-driver abstraction should represent patch-format behavior as a dedicated concern,
- patch input should emit provider-agnostic normalized change models,
- later patch output should consume the same normalized models when the output slice is implemented.

Suggested review candidates:

- `IPatchDriver`
  - parse patch input into normalized change sets
  - later write normalized change sets back into patch output
- `PatchDriverCapabilities`
  - `SupportsRead`
  - `SupportsWrite`
  - supported patch families or binary marker support

For the first slice, the reviewed capability expectation is:

- `SupportsRead = true`
- `SupportsWrite = false`

## Reviewed Validation Considerations

- Contract tests should verify that supported `.patch` inputs become deterministic normalized file-change output.
- Root-remapping tests should verify that moved source roots are rewritten before changes reach downstream consumers.
- Diagnostic tests should verify that unsupported patch constructs fail clearly and locally in the driver layer.
- Forward-compatibility review should verify that the normalized model preserves enough information for a later output-capable patch-driver slice.

## Detailed Work Packages

1. Define patch-driver contract
   - patch input discovery
   - parsed commit/file-change output
   - diagnostics for unsupported constructs
2. Define root-remapping contract
   - path rewrite rules
   - normalization order
   - validation for ambiguous or escaping rewrites
3. Define first-slice patch coverage
   - text hunks
   - file add, modify, delete, rename where representable
   - conservative binary handling strategy
4. Define first-slice integration path
   - patch driver as input-stage component
   - compatibility with mixed archive snapshot and patch commit plans

## Deliverables

- Reviewed first-slice target for patch-input normalization
- Reviewed root-remapping design for moved source roots
- Reviewed long-term patch-driver direction that foresees patch output
- Follow-up-ready basis for implementation tasks

## Acceptance Criteria

- `.patch` handling is designed as an input-driver responsibility rather than destination-specific logic
- The reviewed patch-driver direction already foresees later patch output support
- Root remapping is explicitly represented and applied during normalization
- The first slice clearly distinguishes text-hunk and binary-change handling
- The first slice is explicitly limited to reading patches
- The design supports later mixed plans with full snapshots and patch-derived commits

## Notes

- The initial focus is review and scoping, not broad implementation.

## Reviewed Rollout Guidance

### Phase 1

- define the general patch-driver abstraction
- define read versus write capabilities
- review the normalized patch-output shape without implementing writing yet

### Phase 2

- implement the read-only patch-driver slice
- integrate root remapping during normalization
- connect normalized output to the reviewed structured change path

### Phase 3

- revisit patch output generation once normalized change models and compatibility adapters are stable
- add output-capable patch-driver behavior only after the read path is validated in practice
