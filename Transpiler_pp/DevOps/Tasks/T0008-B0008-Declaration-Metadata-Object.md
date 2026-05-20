# T0008-B0008-Declaration-Metadata-Object

## Parent
- Backlog Item: B0008-Shared-Transpiler-Semantics

## Description
Consolidate variable-declaration traits into a shared metadata object so declaration growth remains structured and future language-independent declaration features can be added without expanding each declaration node surface repeatedly.

## Goal
Introduce a single shared declaration metadata model for variable declarations and update current semantic consumers to read that model directly.

## Scope
- Add a shared declaration metadata type in the semantic layer
- Refactor variable declarations to expose declaration metadata as one object
- Preserve convenience accessors for current tests and callers
- Update declaration extraction and focused tests to use the new metadata object

## Out of Scope
- Full declaration hierarchy redesign
- Member-level behavior modeling beyond stored metadata
- Parser-wide semantic restructuring

## Assumptions
- A dedicated declaration metadata object will scale better than adding more direct declaration properties over time
- Semantic consumers should move toward the metadata object as the source of truth while compatibility accessors remain available during transition

## Open Questions
- Whether declaration metadata should later include accessibility, mutability, storage duration, or external-binding traits
- Whether future member declarations should share this model directly or derive specialized metadata types

## Implementation Notes
1. Keep the first declaration metadata object immutable after construction
2. Preserve current declaration extraction behavior while shifting storage into metadata
3. Validate extraction and compatibility through focused MSTest coverage

## Acceptance Criteria
- Variable declarations expose a shared declaration metadata object
- Existing declaration extraction behavior remains intact
- Focused declaration and semantic tests pass after the refactor
