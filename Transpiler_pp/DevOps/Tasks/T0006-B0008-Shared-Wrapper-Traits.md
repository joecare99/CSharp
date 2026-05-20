# T0006-B0008-Shared-Wrapper-Traits

## Parent
- Backlog Item: B0008-Shared-Transpiler-Semantics

## Description
Move wrapper-level traits such as static and partial intent into the shared semantic model so target backends map language-independent declaration characteristics instead of inferring them ad hoc.

## Goal
Represent wrapper-level declaration traits in shared semantics to keep the framework AST-first and reduce backend-owned structural decisions.

## Scope
- Add shared static intent metadata to the compilation unit model
- Add shared partial intent metadata to the compilation unit model
- Update the C# backend to map the shared traits to C# modifiers
- Add focused tests for shared trait storage and backend mapping

## Out of Scope
- Member-level partial or static modeling
n- Rich lifecycle semantics beyond wrapper traits
- Broad refactoring of current emitter internals

## Assumptions
- Static and partial intent are language-independent enough to belong in shared semantics at the wrapper level
- Backends should translate shared traits into target-language modifiers rather than infer them from artifact kind alone

## Open Questions
- Whether future semantic traits should distinguish wrapper-level and member-level static intent separately
- Whether partial intent should later influence file partitioning or generated companion types

## Implementation Notes
1. Keep the first shared wrapper trait slice compilation-unit-scoped
2. Preserve existing default backend behavior for current tests
3. Validate both shared semantics and backend mapping with focused MSTest coverage

## Acceptance Criteria
- Shared semantics define wrapper static and partial traits
- The compilation unit model exposes the new traits with stable defaults
- The C# backend maps the shared traits into wrapper modifiers
- Focused semantic and backend tests pass
