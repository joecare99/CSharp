# Task T-GEN-DRIVERS-007: Port or Rewrite GEDCOM Reader and Writer

## Parent

- Backlog Item `BI-GEN-DRIVERS-003` - Create GEDCOM Driver Project

## Objective

Implement GEDCOM import and export behavior in the new driver project, using existing code as reference only after behavior and dependencies are reviewed.

## Prerequisites

- `T-GEN-DRIVERS-006` completed.
- Shared driver contracts defined.
- Baseline GEDCOM tests available.

## Scope

- GEDCOM line parser and record tree/object model.
- Mapping from GEDCOM records to the shared genealogy model.
- Mapping from the shared genealogy model to GEDCOM output.
- Diagnostics for unsupported tags, invalid levels, encoding issues, and partial data.

## Output

- GEDCOM reader implementation.
- GEDCOM writer implementation.
- Import/export fixture tests.
- Dialect/version support notes.

## Acceptance Criteria

1. Baseline GEDCOM resources can be imported into the central model.
2. Supported model data can be exported to GEDCOM.
3. Unsupported records are diagnosed in a deterministic way.
4. Reader/writer code remains independent from HEJ driver internals.
