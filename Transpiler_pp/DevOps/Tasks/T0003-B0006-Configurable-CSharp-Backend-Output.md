# T0003-B0006-Configurable-CSharp-Backend-Output

## Parent
- Backlog Item: B0006-Testable-CSharp-Generation

## Description
Add a minimal set of backend generation options so the AST-based C# backend can shape namespace and container type output without changing semantic analysis responsibilities.

## Goal
Keep the shared semantic emitter deterministic while allowing the C# backend facade to expose stable output-shaping options for tests and future language-independent generation workflows.

## Scope
- Add configurable namespace output for generated C# source
- Add configurable containing type name output for generated C# source
- Keep existing default output unchanged when options are not provided
- Add focused backend tests for the new output shaping behavior

## Out of Scope
- Roslyn syntax tree output
- Formatting profiles beyond the selected options
- C# optimization-stage extraction

## Assumptions
- Namespace and container type selection belong to the backend layer rather than the shared semantic layer
- The first option set should remain intentionally small and deterministic

## Open Questions
- Whether future output options should include accessibility, file-scoped namespace style, or partial-type generation
- Whether backend options should later support PROGRAM, FUNCTION, and FUNCTION_BLOCK specific wrappers

## Implementation Notes
1. Preserve the current generated shape as the default behavior
2. Introduce backend options as a separate type instead of adding optional primitive parameters repeatedly
3. Validate the new options with focused MSTest coverage

## Acceptance Criteria
- The C# backend supports namespace and containing type options
- Default output remains compatible with existing focused tests
- Focused backend tests verify the configurable output behavior
