# T0007-B0008-Artifact-Metadata-Object

## Parent
- Backlog Item: B0008-Shared-Transpiler-Semantics

## Description
Consolidate compilation-unit artifact traits into a shared metadata object so semantic growth remains structured and future language-independent declaration traits can be added without expanding root node constructor signatures repeatedly.

## Goal
Introduce a single shared artifact metadata model for compilation units and update backend mapping to consume that model directly.

## Scope
- Add a shared artifact metadata type in the semantic layer
- Refactor the compilation unit to expose artifact metadata as one object
- Preserve convenience accessors or compatibility where useful for current tests and callers
- Update backend mapping and focused tests to use the shared metadata object

## Out of Scope
- Member-level metadata consolidation
- Full semantic renaming away from IEC-prefixed types
- Large parser refactors

## Assumptions
- A dedicated metadata object will scale better than adding more root-level flags and enums over time
- Backends should read shared artifact metadata from one semantic source of truth

## Open Questions
- Whether later declaration-level metadata should reuse this object shape or a related parallel model
- Whether artifact metadata should eventually include namespace identity or source-module identity

## Implementation Notes
1. Keep the first metadata object immutable after construction
2. Preserve existing focused behavior while shifting consumers to the metadata object
3. Validate semantic and backend usage with focused MSTest coverage

## Acceptance Criteria
- Compilation units expose a shared artifact metadata object
- Backend mapping consumes the shared metadata object
- Focused semantic and backend tests pass after the refactor
