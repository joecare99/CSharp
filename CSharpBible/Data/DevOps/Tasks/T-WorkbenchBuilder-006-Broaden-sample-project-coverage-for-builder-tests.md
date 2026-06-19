# Task T-WorkbenchBuilder-006 - Broaden sample project coverage for builder tests

## Status

Draft

## Parent

- Backlog Item `BI-WorkbenchBuilder-003` - `Expand builder regression coverage`

## Goal

Expand the builder test-data matrix so current and future behavior can be validated against a broader set of realistic project shapes.

## Scope

- Add or refine test-data projects for package-heavy, analyzer-using, or multi-target scenarios where valuable
- Keep sample inputs small and maintainable
- Ensure restore and test execution remain practical in the repository workflow

## Acceptance Criteria

- Additional sample projects cover at least one meaningful new risk area
- Test-data growth stays intentional and maintainable
- The new samples improve confidence for upcoming V1.2 work

## Dependencies

- `Workbench.Builder.Core.Tests/TestData`
- Existing restore helper and sample-path helper infrastructure
