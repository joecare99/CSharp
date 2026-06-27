# Task T-GEN-DRIVERS-008: Document Driver Migration and Compatibility

## Parent

- Epic `E-GEN-DRIVERS-001` - Genealogy Import/Export Driver Separation

## Objective

Document how existing consumers should migrate from current `WinAhnenCls` model/parser usage to the new HEJ and GEDCOM driver APIs.

## Scope

- Consumer migration notes.
- Temporary compatibility adapters, if needed.
- Supported format feature matrix.
- Known limitations and diagnostics behavior.

## Output

- Markdown migration guide in `DevOps` or project documentation.
- Compatibility decision list for renamed/moved types.
- Follow-up backlog items for unsupported format features.

## Acceptance Criteria

1. Existing consumers have a clear migration path.
2. Breaking changes are listed explicitly.
3. Compatibility adapters have removal criteria if introduced.
4. Format support limitations are visible before release work starts.
