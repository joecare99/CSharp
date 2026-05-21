# Task T-RepoMigrator-045 - Define compatibility adapter for structured changes to existing target providers

## Status

In Progress

## Parent

- Task `T-RepoMigrator-042` - `Review provider-agnostic structured file-change model`
- Task `T-RepoMigrator-043` - `Define capability contracts for direct transfer and structured change execution`

## Goal

Define a compatibility adapter that bridges normalized structured file changes into the existing target-provider flow so RepoMigrator can adopt the new model without a big-bang provider rewrite.

## Scope

- Review an adapter that materializes normalized changes for current destination providers
- Define interaction with existing Git and SVN target flows
- Define boundaries between temporary compatibility behavior and later provider-native structured sinks

## Architecture Review Summary

The reviewed compatibility adapter should bridge provider-agnostic structured change sets into the currently existing target-provider flows without forcing an immediate rewrite of Git and SVN destination implementations.

The adapter is a transition component.

It should:

- consume normalized change sets from reviewed structured sources such as the read-first patch driver,
- materialize deterministic work state for current target providers,
- preserve migration ordering and commit metadata,
- and remain replaceable once provider-native structured change sinks are introduced.

The adapter should not become a hidden permanent architecture layer.

## Reviewed Adapter Responsibilities

### 1. Structured-change consumption

Reviewed responsibilities:

- accept provider-agnostic normalized change sets,
- support text-hunk-based file changes and conservative binary operations,
- consume mixed plans where future migration slices may combine snapshots and patch-derived changes,
- preserve enough metadata for downstream commit creation and diagnostics.

### 2. Deterministic materialization

Reviewed responsibilities:

- apply normalized file changes in deterministic order,
- produce a materialized work state that existing Git and SVN targets can consume,
- keep path rewrites and normalization decisions upstream rather than reinterpreting them in the adapter,
- avoid provider-specific patch parsing in the compatibility layer.

### 3. Metadata preservation

Reviewed responsibilities:

- preserve commit ordering,
- preserve commit timestamps and authoring metadata when supplied by the migration plan,
- preserve destination-ref intent for downstream ref operations,
- preserve diagnostics that explain unsupported change kinds or fallback decisions.

### 4. Replacement boundary

Reviewed responsibilities:

- make the adapter removable once a target provider supports native structured-change sinks,
- keep provider-specific optimizations out of the adapter where possible,
- avoid coupling Core orchestration to workdir-only assumptions.

## Reviewed Interaction with Patch-Driver Direction

The reviewed compatibility layer should align with the patch-driver direction defined in related tasks.

Implications:

- the first read-only patch-driver slice should be able to feed normalized change sets into this adapter,
- the adapter should not assume that patch output exists yet,
- future patch-driver write capability should remain independent from this bridge layer,
- root-remapping should already be resolved before the adapter receives changes.

## Reviewed Target Interaction Direction

### Git target interaction

- materialize deterministic work state for the existing Git destination flow,
- keep destination commit creation and ref operations in the Git provider path,
- avoid duplicating Git-specific optimization logic in the adapter.

### SVN target interaction

- materialize deterministic work state for the existing SVN destination flow,
- keep SVN-specific commit or synchronization behavior in the SVN provider path,
- avoid introducing SVN-specific change interpretation into Core.

### Destination refs

- keep destination refs outside the core materialization responsibility,
- allow downstream destination-ref operations to consume preserved branch or tag intent after content application.

## Reviewed Validation Considerations

- Compatibility tests should verify deterministic replay of normalized changes through the adapter into current target flows.
- Regression tests should verify that existing snapshot-oriented targets remain stable while structured-change support is introduced incrementally.
- Metadata tests should verify that commit order, timestamps, and ref intent survive the bridge layer.
- Boundary tests should verify that upstream normalization concerns such as root remapping are not silently reinterpreted in the adapter.

## Detailed Work Packages

1. Define adapter responsibilities
   - consume normalized change sets
   - materialize target work state deterministically
   - preserve commit ordering and metadata
2. Define target-provider interaction
   - Git target compatibility
   - SVN target compatibility
   - destination-ref handling boundaries
3. Define migration path
   - short-term bridge for current providers
   - later replacement by provider-native structured-change sinks
4. Define validation strategy
   - compatibility tests
   - regression safety against current snapshot flows

## Deliverables

- Reviewed compatibility-adapter architecture
- Reviewed integration path for existing target providers
- Reviewed bridge alignment with the read-first patch-driver slice
- Follow-up-ready basis for implementation and test tasks

## Acceptance Criteria

- The reviewed adapter allows phased adoption of structured changes without rewriting all existing target providers immediately
- Existing Git and SVN target behavior can be integrated through the reviewed bridge
- The adapter preserves metadata, ordering, and deterministic replay requirements
- The adapter is explicitly framed as a transitional bridge rather than a permanent hidden architecture layer
- The reviewed direction remains consistent with a read-first patch driver and later optional patch output support
- The design makes later replacement by provider-native structured-change sinks possible

## Notes

- This task defines the bridge strategy first; implementation should follow only after the reviewed model is accepted.

## Implementation Progress

- Implemented provider-native structured compatibility sinks for Git and SVN that materialize normalized structured changes into deterministic work directories and delegate commit creation to the existing target-provider flows.
- Registered both structured repository sinks in the WPF composition root so factory-based sink resolution can select the matching bridge per destination provider.
- Added regression tests for SVN structured sink replay, copy metadata override, mode metadata projection, commit delegation, and multi-provider sink-factory resolution.
- Validated the slice with targeted structured sink tests and a successful workspace build.

## Reviewed Rollout Guidance

### Phase 1

- define the adapter contract and its boundary to Core orchestration
- define how normalized changes are materialized for existing Git and SVN targets
- keep current runtime paths unchanged until the reviewed contracts are accepted

### Phase 2

- connect the first structured-change source from the read-only patch-driver slice
- validate deterministic compatibility replay into current target-provider flows
- preserve commit metadata and destination-ref intent through the bridge

### Phase 3

- introduce provider-native structured sinks where justified
- remove bridge usage selectively where native sinks supersede it
- keep the adapter only for providers that still need compatibility materialization
