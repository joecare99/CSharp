# Task T-RepoMigrator-042 - Review provider-agnostic structured file-change model

## Status

Draft

## Parent

- Task `T-RepoMigrator-017` - `Draft archive source core architecture`
- Task `T-RepoMigrator-023` - `Define concrete code target state for archive import`

## Goal

Review and refine a provider-agnostic structured file-change model that can become the default migration path whenever direct transfer is not explicitly enabled by capabilities.

## Scope

- Review shared change-set and file-change concepts for text and binary content
- Review path-rewrite handling for moved source roots
- Review capability boundaries for direct transfer versus structured-change execution
- Review how existing source and destination providers could adopt the model incrementally

## Architecture Review Summary

The reviewed structured file-change model should become the default provider-agnostic migration shape whenever direct transfer is not explicitly selected by capabilities.

The reviewed direction should:

- keep direct Git-to-Git transfer as an explicit capability-based exception,
- normalize non-direct inputs into shared change-set models,
- allow a read-first patch driver to feed those models without requiring immediate patch-output support,
- and allow a temporary compatibility adapter to bridge the models into existing Git and SVN target flows.

The reviewed model should stay provider-agnostic in Core and keep format-specific parsing or provider-specific application behavior outside the shared abstractions.

## Detailed Work Packages

1. Review shared Core models
   - `MigrationChangeSet`
   - `MigrationFileChange`
   - `MigrationTextChange`
   - `MigrationTextHunk`
   - `MigrationBinaryChange`
   - `PathRewriteRule`
2. Review capability model
   - direct-transfer capability
   - structured-change-apply capability
   - materialized-workdir fallback capability
   - destination-ref capability
3. Review patch-input first slice
   - define a general patch-driver direction with separate read and write capabilities
   - normalize `.patch` files into file-change lists through the first read-only slice
   - apply root remapping during normalization
   - keep patch-specific parsing out of destination providers
4. Review phased rollout path
   - compatibility adapter for existing targets
   - later provider-native structured-change sinks
   - repository-source adoption after the first patch slice

## Reviewed Model Direction

### Shared change-set model

The reviewed Core model should cover:

- ordered migration change sets,
- file-level operations such as add, modify, delete, rename, and copy where representable,
- text changes with ordered hunks,
- conservative binary change representation,
- large-binary payload handling without forcing every payload into memory,
- explicit path-rewrite rules for moved source roots,
- metadata needed for deterministic replay and diagnostics.

### Binary payload strategy

The reviewed model should distinguish between small binary payloads that may be carried inline and larger binary payloads that should be referenced indirectly.

Suggested reviewed direction:

- use a provider-agnostic binary payload reference model rather than a raw machine-local file path,
- allow an inline mode for small payloads,
- allow a file-link or artifact-reference mode for larger payloads,
- keep the threshold configurable rather than baking a fixed size such as `100 KB` into Core,
- preserve payload metadata such as length, hash, and source format hints for diagnostics and validation.

Suggested reviewed shape:

- `MigrationBinaryChange`
  - operation kind
  - payload mode such as `Inline` or `FileReference`
  - optional inline payload for small binaries
  - optional payload reference for large binaries
  - payload length
  - payload hash

- `BinaryPayloadReference`
  - reference kind
  - relative artifact path or provider-owned handle
  - portability flag or lifetime semantics when needed

The reviewed direction should avoid storing large binary content in memory by default when the source already provides a stable file-backed representation.

If binary payload references are persisted beyond transient execution, the reviewed direction should prefer a runtime-defined artifact location or a provider-owned storage abstraction rather than an arbitrary temp path or a repository-local planning folder.

### Input-side responsibilities

The reviewed model should expect input-side components to normalize source-specific representations before downstream execution.

Examples:

- archive-oriented preparation may continue to produce snapshot-oriented plans until later slices adopt structured changes,
- the first patch-driver slice should read `.patch` input and emit normalized change sets,
- future patch-driver slices may later add output support without changing the Core model shape.

### Output-side responsibilities

The reviewed model should expect output-side components either to:

- consume structured changes natively,
- or receive them through a compatibility adapter that materializes deterministic work state for existing providers.

Patch-specific parsing and patch-output generation should not be embedded in destination providers.

## Reviewed Validation Considerations

- Model reviews should verify that path rewrites remain explicit rather than implicit side effects.
- Capability reviews should verify that direct transfer stays an exception path, not the default architectural assumption.
- Patch-driver reviews should verify that the first slice remains read-only while preserving data needed for later optional output support.
- Compatibility-adapter reviews should verify that existing Git and SVN target flows can adopt structured changes incrementally.
- Binary-payload reviews should verify that large binary content can flow through the model via references without unnecessary in-memory buffering.

## Deliverables

- Reviewed target architecture for provider-agnostic change-set processing
- Capability-based execution rules for special paths
- Reviewed first-slice plan for a read-first patch driver with root remapping and later optional output direction

## Acceptance Criteria

- The reviewed model keeps direct Git-to-Git transfer as a capability-based exception rather than a general architectural shortcut
- Root remapping is represented explicitly in the reviewed change normalization path
- The reviewed model separates input-driver responsibilities from output-provider responsibilities
- The reviewed direction allows a general patch driver with read-first rollout and later optional output support
- The reviewed binary-change direction allows large payloads to be referenced indirectly instead of requiring in-memory storage
- The rollout path avoids a big-bang rewrite and preserves compatibility with current providers

## Notes

- This task is intentionally review-oriented and should be validated before any broad implementation starts.
- Patch handling should follow input drivers that emit file-change lists rather than patch-specific execution in destination providers.

## Reviewed Rollout Guidance

### Phase 1

- review the shared Core model for provider-agnostic change sets
- review capability-gated execution-path selection
- review the general patch-driver shape with separate read and write capabilities

### Phase 2

- implement the first read-only patch-driver slice
- normalize moved roots through explicit path rewrites
- bridge structured changes into current target providers through the compatibility adapter

### Phase 3

- add provider-native structured-change sinks where justified
- revisit repository-backed sources as structured-change emitters when direct transfer is not selected
- consider patch output only after the read path and compatibility bridge are validated
