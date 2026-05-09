# T0015-B0006-IEC-SourceText-To-CSharp-Backend-Regression

## Parent
- Backlog Item: B0006-Testable-CSharp-Generation

## Description
Add focused regression coverage that proves the current IEC source-text frontend bridge can feed the C# backend with a realistic typed compilation unit from export-based input.

## Goal
Protect the first end-to-end typed IEC to C# path for the currently supported assignment, function-call, IF, and RETURN subset.

## Scope
- Reuse the IEC export fixture through `IecFrontendCompilationUnitFactory`
- Emit C# through `CSharpBackend`
- Assert representative generated members and control-flow output
- Keep the increment test-focused without expanding backend language coverage

## Out of Scope
- New backend statement kinds
- Full compilability testing of generated source in this increment
- Broader semantic binding work

## Notes
- Focus on realistic regression coverage for the existing supported subset
- Keep changes minimal and backend-facing
