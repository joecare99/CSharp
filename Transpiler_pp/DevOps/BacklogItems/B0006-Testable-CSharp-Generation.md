# B0006-Testable-CSharp-Generation

## Parent
- Feature: F0001-IEC-to-CSharp-Testability

## Description
Generate testable C# artifacts for the supported IEC subset with a structure that preserves deterministic execution and enables direct unit testing.

## Value
This item delivers the first usable bridge from IEC source semantics to executable C# code.

## Scope
- Generate C# for supported `FUNCTION`, `FUNCTION_BLOCK`, and `PROGRAM` shapes
- Preserve deterministic state transitions and cycle-based execution behavior
- Keep generated code suitable for direct MSTest-based assertions
- Allow backend-level configuration of generated wrapper structure such as namespace and containing type
- Allow backend-level selection of artifact roles so generated wrappers can represent function, function block, and program forms

## Out of Scope
- Full optimization or formatting customization
- Unsupported IEC language areas

## Acceptance Criteria
- Supported IEC input generates compilable C# output
- Generated code can be executed in tests with deterministic results
- Output structure matches the execution model defined earlier in the roadmap
- Backend output-shaping options can be tested without changing semantic analysis responsibilities
- Backend artifact roles can shape generated wrapper output for function, function block, and program forms

## Assumptions
- Semantic analysis provides the information needed for generation

## Open Questions
- Whether generation should target source files, Roslyn syntax trees, or both
- Which namespace and type layout conventions should be used for generated artifacts
- Which additional backend output options should be standardized after the initial namespace/type wrapper support
- When artifact roles should start carrying richer execution-model metadata beyond wrapper shape
