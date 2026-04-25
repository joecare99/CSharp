# Backlog Item BI-RNZ-TRAUER-001: Refactor DataHandler Responsibilities

## Parent

- Feature `F-RNZ-TRAUER-001` - Refactor DataHandler and Separate Persistence

## Objective

Reduce the responsibility overload in `RnzTrauer.Core.Services.DataHandler` by separating persistence concerns, improving documentation, and making hard-coded operational texts explicit.

## Value

- Lowers maintenance cost for future importer changes
- Improves testability of persistence-related behavior
- Reduces accidental regressions caused by mixed extraction and database code
- Makes domain assumptions and technical constants more visible in the implementation

## Scope

- `RnzTrauer.Core.Services.DataHandler`
- New repository abstraction and implementation in `RnzTrauer.Core.Services`
- Targeted unit tests in `RnzTrauer.Tests`

## Acceptance Criteria

1. `DataHandler` delegates database access to a dedicated repository dependency.
2. A concrete repository encapsulates MySQL connection creation and SQL command execution.
3. Magic texts in the touched logic are replaced with named constants where repetition or ambiguity exists.
4. At least one focused unit test validates the new delegation or mapping behavior.
5. Existing caller usage in the RNZ console workflow remains compatible.

## Assumptions

- The current dictionary-based transport between extraction and persistence remains acceptable for this increment.
- Constructor-level repository composition is sufficient for the current application shape.
- No schema redesign is required for the refactoring step.

## Open Questions

- Should the next increment replace dictionary payloads with typed obituary models?
- Should SQL field-name based query methods be restricted or validated to reduce misuse risk?
- Should console status output be abstracted behind a progress or logging service?
