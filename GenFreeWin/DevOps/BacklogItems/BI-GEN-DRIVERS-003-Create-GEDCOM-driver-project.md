# Backlog Item BI-GEN-DRIVERS-003: Create GEDCOM Driver Project

## Parent

- Feature `F-GEN-DRIVERS-002` - Create Separate GEDCOM Import/Export Driver

## Objective

Create a separate GEDCOM driver project and move GEDCOM parsing/writing responsibility out of `WinAhnenCls`.

## Value

- Separates GEDCOM evolution from HEJ compatibility work.
- Makes GEDCOM support independently packageable, testable, and replaceable.
- Reduces risk that format-specific assumptions leak into shared model code.

## Scope

- New production project for GEDCOM import/export.
- New MSTest project for GEDCOM driver behavior.
- Initial parser and object model boundaries.
- Mapping to and from `BaseGenClasses`/`GenInterfaces`.
- Review of existing `WinAhnenCls\Model\GenBase` and legacy `GedLes` code as references, with `GedLes` treated mainly as a negative example.

## Deliverables

- Project structure proposal and initial `.csproj` references.
- Public GEDCOM import/export API proposal.
- GEDCOM reader/writer implementation slices.
- Tests for baseline GEDCOM samples.

## Acceptance Criteria

1. GEDCOM-specific code no longer needs to live in the HEJ driver project.
2. The new project has no UI dependencies.
3. Import and export behavior are covered by resource-based tests.
4. Unsupported GEDCOM structures produce diagnostics rather than silent data loss where practical.

## Assumptions

- Initial project target frameworks should align with `BaseGenClasses` where practical.
- Existing GEDCOM reader code can be adapted only after behavior is covered by tests.
- The project should live in a neutral solution area under the long-term name `Genealogy.GedCom`.
- The first implementation increment should read GEDCOM `5.5`, `5.5.1`, and `7`, with a slight preference for `5.5.1`.

## Open Questions

- Which extension-data representation is appropriate when unknown GEDCOM tags are preserved in configurable compatibility modes?
