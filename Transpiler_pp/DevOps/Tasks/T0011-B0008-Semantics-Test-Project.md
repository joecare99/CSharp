# T0011-B0008-Semantics-Test-Project

## Parent
- Backlog Item: B0008-Shared-Transpiler-Semantics

## Description
Create a dedicated semantics test project so shared AST, binder, and emitter tests are owned and validated alongside the shared semantics layer instead of remaining in the IEC frontend test project.

## Goal
Align semantic test ownership with semantic code ownership and improve focused branch coverage for binder and emitter logic.

## Scope
- Add `TranspilerLib.Semantics.Tests` as a dedicated MSTest project
- Move shared semantic unit tests out of `TranspilerLib.IEC.Tests`
- Keep IEC frontend tests focused on mapper, scanner, interpreter, and frontend integration concerns
- Add focused branch-oriented tests for binder and emitter logic
- Preserve existing multi-target test execution

## Out of Scope
- Broad parser refactoring
- Backend integration test redesign
- Renaming semantic namespaces or types

## Assumptions
- Semantic code should be validated in a project that mirrors its ownership boundary
- Extra branch coverage should target the highest-complexity semantic helpers first

## Open Questions
- Whether additional shared semantic tests such as declaration extraction and AST node tests should also move in a later step
- Whether a future shared fixture package is preferable to local fixture duplication between test projects

## Implementation Notes
1. Keep MSTest as the project test framework
2. Preserve realistic export-fixture-based test coverage
3. Validate both the new semantics tests and the remaining IEC frontend tests

## Acceptance Criteria
- A `TranspilerLib.Semantics.Tests` project exists in the solution
- Binder and emitter semantic tests run from the semantics test project
- `TranspilerLib.IEC.Tests` no longer owns the migrated semantic unit tests
- Focused semantic and frontend tests still pass after the split
