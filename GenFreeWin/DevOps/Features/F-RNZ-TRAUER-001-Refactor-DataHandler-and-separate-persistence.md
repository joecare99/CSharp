# Feature F-RNZ-TRAUER-001: Refactor DataHandler and Separate Persistence

## Parent

- Local repository planning item without assigned epic yet

## Feature Statement

As a maintainer of the RNZ obituary import,
I want extraction logic, persistence logic, and domain-specific constants to be separated clearly,
so that the code becomes easier to understand, test, and evolve without mixing JSON parsing and database access.

## Scope

- Improve the documentation quality of `DataHandler`
- Extract hard-coded technical and domain strings into named constants where practical
- Move direct MySQL access out of `DataHandler` into a dedicated repository class
- Preserve the current import behavior and current calling surface used by the console workflow
- Add targeted tests for the new repository interaction points

## Acceptance Criteria

1. `DataHandler` no longer owns direct MySQL connection and command execution.
2. Database reads and writes are encapsulated in a separate repository abstraction and implementation.
3. Repeated hard-coded texts in `DataHandler` are replaced by named constants where this improves clarity.
4. XML documentation reflects the revised responsibilities of the involved classes.
5. Automated tests cover at least the repository delegation or mapping behavior introduced by the refactoring.

## Notes

- The current implementation keeps the public `DataHandler` workflow methods available to avoid unnecessary ripple effects in the console host.
- The extraction result is still represented as dictionaries to minimize the scope of the refactoring increment.

## Risks

- Dictionary-based mappings still allow key drift and should be considered for later typed-model refactoring.
- Dynamic SQL fragments for field-based queries remain a follow-up hardening topic.
- Console progress output is still emitted directly from core workflow code and may deserve later abstraction.
