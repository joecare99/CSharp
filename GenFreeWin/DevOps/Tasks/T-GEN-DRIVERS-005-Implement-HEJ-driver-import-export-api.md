# Task T-GEN-DRIVERS-005: Implement HEJ Driver Import/Export API

## Parent

- Backlog Item `BI-GEN-DRIVERS-002` - Extract HEJ Import/Export Driver Surface

## Objective

Implement the public `.hej` driver API in `WinAhnenCls` after shared contracts and model boundaries are established.

## Prerequisites

- `T-GEN-DRIVERS-003` completed.
- Required shared model/helper migrations completed or explicitly deferred.

## Scope

- HEJ import from stream.
- HEJ export to stream.
- Mapping to and from `BaseGenClasses`/`GenInterfaces` model objects.
- Driver diagnostics for unsupported sections or malformed data.

## Output

- HEJ import/export service class or classes.
- Tests for import, export, and representative semantic round trips.
- Documentation of supported HEJ sections.

## Acceptance Criteria

1. Consumers can import `.hej` data without directly using legacy internal classes.
2. Consumers can export supported genealogy data to `.hej`.
3. Tests cover existing resources and at least one generated export fixture.
4. Driver internals remain free of UI dependencies.
