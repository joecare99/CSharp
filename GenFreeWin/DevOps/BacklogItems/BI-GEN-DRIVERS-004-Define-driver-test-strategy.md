# Backlog Item BI-GEN-DRIVERS-004: Define Driver Test Strategy

## Parent

- Epic `E-GEN-DRIVERS-001` - Genealogy Import/Export Driver Separation

## Objective

Define the regression and characterization test strategy for moving model code, narrowing the HEJ driver, and introducing the GEDCOM driver.

## Value

- Reduces risk while moving code between projects.
- Makes legacy format compatibility measurable.
- Enables later implementation in small safe slices.

## Scope

- Characterization tests for current `WinAhnenCls` behavior.
- Shared model tests in `BaseGenClassesTests`.
- HEJ driver tests in `WinAhnenClsTests` or a renamed HEJ-specific test project.
- GEDCOM driver tests in a new dedicated test project.
- Fixture policy for `.hej`, `.ged`, and semantic JSON snapshots.

## Deliverables

- Test matrix by project and format.
- Fixture naming and expected-output conventions.
- Guidance for import, export, and semantic round-trip assertions.
- CI/build validation notes for multi-targeting.

## Acceptance Criteria

1. Each migration slice has a matching test expectation.
2. Existing resources are mapped to specific tests.
3. New fixture gaps are listed before implementation starts.
4. Test projects avoid UI dependencies and use MSTest.

## Assumptions

- Existing `WinAhnenClsTests\Resources` files are valuable characterization fixtures.
- Some expected outputs may need semantic comparison rather than byte-for-byte comparison.
- Driver tests should combine semantic JSON snapshots with explicit assertions.
- Round-trip checks should tolerate ordering differences for sources, places, and events unless a format contract explicitly requires a stable order.

## Open Questions

- Which canonicalization rules should test helpers apply before tolerant semantic comparisons are evaluated?
