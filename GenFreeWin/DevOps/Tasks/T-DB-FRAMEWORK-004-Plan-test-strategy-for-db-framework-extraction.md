# Task T-DB-FRAMEWORK-004: Plan Test Strategy for DB Framework Extraction

## Parent

- Backlog Item `BI-DB-FRAMEWORK-001` - Assess and Plan Domain-Neutral DB Framework Extraction

## Objective

Define the required regression and provider-focused tests that should protect the extraction and later provider split.

## Test focus areas

- abstraction contract regression tests
- provider renderer tests
- provider connection factory tests
- `OleDb` integration tests where feasible
- compatibility adapter tests for current `GenFree*` consumers

## Notes

- Use `MSTest` and `NSubstitute` where suitable.
- Prioritize tests around any newly created neutral contracts before larger provider moves.
