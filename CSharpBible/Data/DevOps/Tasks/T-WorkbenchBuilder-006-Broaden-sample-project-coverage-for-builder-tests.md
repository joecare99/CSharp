# Task T-WorkbenchBuilder-006 - Broaden sample project coverage for builder tests

## Status

Completed

## Parent

- Backlog Item `BI-WorkbenchBuilder-003` - `Expand builder regression coverage`

## Goal

Expand the builder test-data matrix so current and future behavior can be validated against a broader set of realistic project shapes.

## Scope

- Add or refine test-data projects for package-heavy, analyzer-using, or multi-target scenarios where valuable
- Add a compact multi-target sample project as the next explicit coverage expansion for V1.1 hardening
- Keep sample inputs small and maintainable
- Ensure restore and test execution remain practical in the repository workflow

## Acceptance Criteria

- Additional sample projects cover at least one meaningful new risk area
- A dedicated multi-target sample project exists and is used by builder tests
- Test-data growth stays intentional and maintainable
- The new samples improve confidence for upcoming V1.2 work

## Dependencies

- `Workbench.Builder.Core.Tests/TestData`
- Existing restore helper and sample-path helper infrastructure

## Notes

The preferred next addition is a small SDK-style sample with `TargetFrameworks=net8.0;net10.0`. It should stay intentionally simple and primarily exercise target-framework recognition, current inspection behavior, and any visible best-effort limitations.

This slice is now implemented through `MultiTargetLibrary` under `Workbench.Builder.Core.Tests/TestData` together with focused tests that cover the current first-target fallback and explicit target-framework selection behavior.
