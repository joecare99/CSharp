# Task T-RepoMigrator-043 - Define capability contracts for direct transfer and structured change execution

## Status

Draft

## Parent

- Task `T-RepoMigrator-042` - `Review provider-agnostic structured file-change model`
- Feature `F-RepoMigrator-01` - `Core migration orchestration and provider contracts`

## Goal

Define explicit capability contracts that decide when RepoMigrator may use a direct transfer path and when it must use the structured file-change model.

## Scope

- Review capability concepts for direct transfer, structured change application, materialized-workdir fallback, and destination refs
- Define how orchestration should select the execution path from source and target capabilities
- Keep direct Git-to-Git transfer as an explicit capability-based exception

## Architecture Review Summary

The reviewed target direction should make execution-path selection explicit through capabilities rather than through provider-name checks or orchestration-local exceptions.

The intended rule is:

- use a direct transfer path only when source and target capabilities explicitly allow it,
- otherwise use the provider-agnostic structured file-change model,
- and only use workdir materialization as a compatibility fallback when the target cannot yet consume structured changes directly.

This preserves high-value fast paths such as direct Git-to-Git transfer without making them the default architectural shape for all migrations.

## Reviewed Capability Categories

### 1. Direct transfer capability

Represents the ability to bypass normalized file changes and let the source-target combination perform a safe direct migration.

Reviewed intent:

- explicit capability, never implicit assumption
- source and target must both participate in the decision
- expected first example: Git source to Git target native transfer

Suggested reviewed shape:

- capability indicates supported target provider kinds or transfer families
- capability may require specific option combinations
- capability may reject flows that need path rewriting, patch normalization, or cross-model conversion

### 2. Structured change source capability

Represents the ability of an input side to emit normalized change sets rather than only snapshots or provider-native direct transfers.

Reviewed intent:

- general patch drivers should emit structured change sets from patch input
- the first reviewed patch-driver slice should expose read capability first and defer write capability
- repository providers may later emit structured change sets when direct transfer is not selected
- archive-driven mixed plans may later combine full snapshots with patch-derived change sets

Reviewed capability direction:

- the capability contract should allow a format-oriented driver to advertise read and write support separately
- the first reviewed patch-driver slice should be modeled as `SupportsRead = true` and `SupportsWrite = false`

### 3. Structured change sink capability

Represents the ability of a destination side to consume normalized change sets directly.

Reviewed intent:

- newer target providers may consume structured changes natively
- capability should describe supported change kinds, including text hunks and conservative binary operations
- capability should remain separate from destination-ref support

### 4. Materialized-workdir fallback capability

Represents the ability of a destination side to accept a temporary materialized working tree instead of direct structured changes.

Reviewed intent:

- existing Git and SVN target flows can remain operational through this compatibility path
- this capability is a bridge, not the long-term architectural target for all providers
- orchestration should select it only when direct transfer is unavailable and native structured-change sink support is unavailable

### 5. Destination-ref capability

Represents optional support for tags, branches, or equivalent destination references.

Reviewed intent:

- keep ref creation outside the core content-application capability
- allow archive-import and similar flows to query ref support explicitly
- preserve provider-specific details outside Core while keeping the high-level capability provider-agnostic

## Reviewed Orchestration Decision Rules

### Execution-path precedence

Reviewed precedence order:

1. direct transfer when explicitly supported by both sides and compatible with the requested operation
2. structured change path when the source can emit normalized changes and the target can consume them directly
3. structured change plus compatibility materialization when the source can emit normalized changes but the target only supports workdir-based application
4. existing snapshot-only path only where the reviewed migration slice has not yet adopted the structured change contracts

### Negotiation rules

Reviewed decision inputs should include:

- source capabilities
- target capabilities
- requested migration options
- source-kind constraints such as root rewriting or patch normalization
- destination ref requirements

Reviewed decision outputs should include:

- selected execution path
- rationale or diagnostic explanation
- rejected alternatives when useful for diagnostics and tests

### Guard rails

- direct transfer must not be chosen merely because provider keys look compatible
- root remapping or patch-derived normalization should normally disable direct transfer
- patch-driver read capability must not be treated as implicit patch-output support
- capability negotiation should be deterministic and testable without invoking actual provider side effects

## Reviewed Mapping to Current Solution State

### Current direct-transfer example

- Git provider native history transfer is the canonical reviewed example for direct transfer capability

### Current compatibility-fallback examples

- Git target snapshot commits through materialized work directories
- SVN target snapshot synchronization through materialized work directories

### Near-term reviewed extension points

- read-first patch driver as a structured change source
- compatibility adapter that materializes structured changes for current Git and SVN targets
- later repository providers as optional structured change sources when direct transfer is unavailable

## Reviewed Core Ownership Direction

Provider-agnostic capability contracts should live in `RepoMigrator.Core`.

Reviewed Core ownership candidates:

- execution-path selection result models
- provider-agnostic capability summaries
- compatibility-fallback capability contracts
- structured source and sink capability contracts

Provider-specific details should remain outside Core.

Reviewed provider-project ownership examples:

- Git-native transfer specifics
- patch-format parsing specifics
- destination-specific application limits or optimizations
- provider-local explanations for unsupported change kinds

## Reviewed Validation Considerations

- Contract tests should verify deterministic execution-path selection for representative source-target combinations.
- Capability tests should verify that direct Git-to-Git transfer remains capability-driven rather than hardcoded by orchestration shortcuts.
- Mapping tests should verify that structured-change sources can fall back to compatibility materialization when direct sinks are unavailable.
- Diagnostic tests should verify that rejected execution paths remain explainable.

## Detailed Work Packages

1. Define capability model
   - direct-transfer capability
   - structured-change-apply capability
   - materialized-workdir fallback capability
   - destination-ref capability
2. Define orchestration decision rules
   - source and target capability negotiation
   - explicit direct-transfer exception path
   - fallback selection rules
3. Define Core ownership
   - provider-agnostic contracts in `RepoMigrator.Core`
   - provider-specific capability details in provider projects

## Deliverables

- Reviewed capability-contract direction for Core
- Reviewed execution-path selection rules
- Follow-up-ready task basis for implementation

## Follow-up Candidates

- Introduce provider-agnostic capability summary models in Core.
- Introduce execution-path selection models and tests.
- Introduce the first structured change source contract implementation for patch inputs.
- Introduce the compatibility adapter for existing workdir-based destination providers.

## Acceptance Criteria

- Direct transfer is modeled as a capability-based path rather than a hardcoded architectural shortcut
- Structured-change execution can be selected explicitly through reviewed capabilities
- Existing providers can be mapped to the reviewed capability model without immediate rewrite
- The reviewed direction remains provider-agnostic in Core

## Notes

- This task should remain review-first before implementation begins.

## Reviewed Rollout Guidance

The reviewed rollout should stay incremental.

### Phase 1

- document and validate capability categories in Core
- add deterministic execution-path selection tests
- keep current runtime behavior unchanged where capabilities are not yet wired through

### Phase 2

- introduce the first structured change source through the read-only patch-driver slice
- route that source through capability negotiation
- use compatibility materialization for existing Git and SVN targets

### Phase 3

- add provider-native structured change sinks where justified
- reduce reliance on compatibility materialization where target providers can consume normalized change sets directly

### Phase 4

- revisit repository-backed sources so they can emit structured change sets when direct transfer is not selected
- keep direct transfer available only as an explicit capability path

## Reviewed Examples

### Example A - direct Git-to-Git transfer

- source advertises direct transfer to Git target
- target accepts direct transfer from Git source
- no root remapping or patch normalization is required
- selected path: direct transfer

### Example B - patch input to Git target

- source advertises structured change output through a patch driver with `SupportsRead = true` and `SupportsWrite = false`
- target does not yet advertise native structured change sink support
- target advertises materialized-workdir fallback
- selected path: structured changes plus compatibility materialization

### Example C - archive snapshot import with release refs

- source does not advertise direct transfer
- source currently provides snapshot-oriented preparation
- target advertises workdir-based snapshot application and destination refs
- selected path: snapshot compatibility path until the reviewed structured-change contracts are implemented for this slice
