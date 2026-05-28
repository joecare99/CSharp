# AA98-F36 Test Data and Mocking Standards

## Parent
- Epic: `DevOps/Epics/AA98-E09-Quality-Tests-and-Engineering-Baseline.md`
- Vision: `DevOps/Vision.md`

## Goal
Define the first test data and mocking standards so tests across the solution stay readable, maintainable, and consistent as the framework grows.

## Scope
- Define baseline conventions for test data arrangement.
- Clarify how mocks, substitutes, and test fixtures should be used.
- Keep standards aligned with the repository's testing approach.
- Prepare the path for later test readability and maintainability improvements.

## Included
- Test data conventions
- Mocking and substitute usage guidance
- Fixture and arrangement standards
- Extensibility path for later test style refinements

## Excluded for Now
- Organization-wide test framework policy enforcement
- Advanced generated test data libraries
- Snapshot-testing governance
- Complex shared test infrastructure catalogs

## Success Indicators
- Tests remain easy to read and update.
- Mocking usage is consistent across new and updated tests.
- The standards help incremental development without adding friction.

## Candidate Backlog Items
- Define baseline conventions for test data
- Clarify mock and substitute usage patterns
- Standardize fixture and arrangement structure
- Keep the standards aligned with future test growth

## Assumptions
- Readability and maintainability are more important than clever test setup.
- The standards should remain lightweight and practical for day-to-day development.

## Open Questions
- Which test data patterns should be preferred for new tests in this solution?
- Should shared test helpers be introduced early or only when duplication becomes painful?

## Status
- Proposed
