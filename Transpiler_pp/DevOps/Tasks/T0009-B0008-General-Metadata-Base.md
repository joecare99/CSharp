# T0009-B0008-General-Metadata-Base

## Parent
- Backlog Item: B0008-Shared-Transpiler-Semantics

## Description
Introduce a general shared metadata base model so artifact and declaration metadata can align around one semantic abstraction before more traits are generalized.

## Goal
Create a lightweight common metadata root that gives the semantic layer a stable foundation for broader language-independent metadata growth.

## Scope
- Add a shared metadata base type in the semantic layer
- Align artifact and declaration metadata to inherit from the shared base
- Keep existing metadata shape and semantic consumers stable
- Add focused tests for the shared base alignment

## Out of Scope
- Full unification of all metadata properties into one class
- Large semantic hierarchy redesign
- Broad backend behavior changes

## Assumptions
- A lightweight common base is the safest next step toward a general model
- Existing specialized metadata types should remain intact while aligning structurally

## Open Questions
- Whether later metadata bases should also model naming, source identity, or diagnostic annotations
- Whether future declaration and artifact metadata should remain separate specializations or converge further

## Implementation Notes
1. Keep the base intentionally small in the first increment
2. Preserve all current focused tests and compatibility surfaces
3. Validate the alignment with focused semantic tests

## Acceptance Criteria
- A shared metadata base type exists
- Artifact and declaration metadata align to the shared base
- Focused tests validate the alignment without breaking current behavior
