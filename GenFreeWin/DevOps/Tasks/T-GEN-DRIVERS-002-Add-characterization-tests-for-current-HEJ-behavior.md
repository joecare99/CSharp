# Task T-GEN-DRIVERS-002: Add Characterization Tests for Current HEJ Behavior

## Parent

- Backlog Item `BI-GEN-DRIVERS-004` - Define Driver Test Strategy

## Objective

Add or refine tests that document current `.hej` import behavior before moving model or parser code.

## Scope

- Existing resources in `..\WinAhnenNew\WinAhnenClsTests\Resources`
- Current reader behavior in `CHejGenealogy` and related HEJ data classes
- Counts, relationships, places, sources, adoptions, and representative date/name handling

## Output

- MSTest coverage for current HEJ import behavior.
- Fixture map describing which resource covers which HEJ sections.
- List of missing HEJ fixture cases.

## Acceptance Criteria

1. Existing `.hej` sample files are tied to explicit tests.
2. Tests assert semantic behavior, not only that parsing completes.
3. Tests are stable across supported target frameworks.
4. Any intentionally unsupported behavior is documented in the test notes.
