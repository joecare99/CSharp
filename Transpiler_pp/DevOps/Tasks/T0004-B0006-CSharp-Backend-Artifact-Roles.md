# T0004-B0006-CSharp-Backend-Artifact-Roles

## Parent
- Backlog Item: B0006-Testable-CSharp-Generation

## Description
Extend the AST-based C# backend so generated wrapper code can distinguish between function-oriented, function-block-oriented, and program-oriented output roles without pushing target-shape concerns into shared semantic analysis.

## Goal
Support the first explicit backend artifact roles for generated C# wrappers while preserving deterministic emission and clean separation between shared semantics and target-language output shaping.

## Scope
- Add backend artifact role options for function, function block, and program output
- Shape wrapper class and method modifiers according to the selected artifact role
- Keep the previous default output behavior stable
- Add focused backend tests for artifact role output

## Out of Scope
- Full IEC artifact metadata modeling in the shared semantic layer
- Behavioral runtime differences between function blocks and programs
- C# optimization extraction

## Assumptions
- Artifact role shaping belongs to the backend layer because it describes target-language wrapper form
- The first implementation can express role differences through wrapper structure before deeper lifecycle semantics are modeled

## Open Questions
- Whether future program and function block roles need dedicated instance lifecycle helpers
- Whether artifact roles should later influence field accessibility, partial types, or generated file naming

## Implementation Notes
1. Keep shared semantic emission unchanged
2. Apply role-specific wrapper shaping in the backend facade
3. Preserve compatibility with the previous default output shape

## Acceptance Criteria
- Backend options support distinct artifact roles
- Focused tests verify function, function block, and program wrapper shaping
- Existing default backend generation remains valid
