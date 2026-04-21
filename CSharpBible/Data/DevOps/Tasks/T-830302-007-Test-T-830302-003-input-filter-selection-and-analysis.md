# Task T-830302-007 - Test T-830302-003 input filter selection and analysis

## Status

Draft

## Parent

- Backlog Item `BI-830332` - `Create pluggable input filters for initial source formats`

## Goal

Validate that the deterministic input-filter selection and analysis behavior from `T-830302-003` works as specified.

## Scope

- Verify deterministic ranking order for matching filters
- Verify manual filter override behavior
- Verify unsupported-format outcome when no filter can handle input
- Verify analysis details are returned (`confidence`, extension match, decision lines)
- Verify stream handling is safe across repeated analyses and selection

## Test Focus

1. Manual override has highest priority when the chosen filter can handle input
2. Ranking order is deterministic:
   - confidence score
   - exact extension match
   - configured filter priority
   - stable tie-breaker by filter identifier
3. Non-seekable stream handling is stable (selection remains deterministic)
4. No matching filter returns a `null` selection with full analysis list
5. Decision lines are preserved in analysis outputs

## Implementation Notes

- Use `MSTest`
- Use `DataRow` for parameterized ranking cases where practical
- Use `NSubstitute` for filter test doubles

## Done Criteria

- Test cases cover all ranking dimensions
- Test cases cover manual override and no-match paths
- Test cases validate analysis payload shape
- Tests run successfully for the targeted project scope
