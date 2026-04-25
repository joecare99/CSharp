# Task T-RNZ-TRAUER-002: Move Database Access into Repository

## Parent

- Backlog Item `BI-RNZ-TRAUER-001` - Refactor DataHandler Responsibilities

## Objective

Extract the direct MySQL access code from `DataHandler` into a dedicated repository abstraction and implementation.

## Deliverables

- New `ITrauerDataRepository` abstraction
- New `TrauerDataRepository` implementation owning MySQL connection and SQL execution
- Updated `DataHandler` constructors and methods delegating persistence work to the repository

## Outcome Snapshot

- `DataHandler` now depends on `ITrauerDataRepository` for persistence operations.
- `TrauerDataRepository` encapsulates connection management, query execution, inserts, and update behavior.
- The existing console workflow can still instantiate `DataHandler` through the compatibility constructor using `DatabaseSettings`.

## Follow-up Considerations

- Register repository composition through dependency injection if the RNZ tools adopt a broader DI setup.
- Evaluate whether row dictionaries should be replaced by typed persistence models in a later increment.
