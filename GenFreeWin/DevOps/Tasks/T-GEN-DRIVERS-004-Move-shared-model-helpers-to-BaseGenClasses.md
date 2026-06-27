# Task T-GEN-DRIVERS-004: Move Shared Model Helpers to BaseGenClasses

## Parent

- Backlog Item `BI-GEN-DRIVERS-001` - Classify and Move Shared Genealogy Model

## Objective

Move the first approved slice of reusable genealogy helper/model code from `WinAhnenCls` into `BaseGenClasses` after tests describe the expected behavior.

## Prerequisites

- `T-GEN-DRIVERS-001` completed.
- Characterization tests exist for the selected helper/model behavior.

## Scope

- Only shared, format-neutral helpers or model concepts.
- No HEJ delimiters, HEJ section markers, GEDCOM tags, or UI text.
- Update references in `WinAhnenCls` to consume moved code.

## Output

- Moved source files or new equivalent `BaseGenClasses` types.
- Updated namespaces and project references.
- Compatibility adapter or alias plan if required.
- Passing relevant tests.

## Acceptance Criteria

1. `BaseGenClasses` owns the selected shared behavior.
2. `WinAhnenCls` no longer duplicates that behavior.
3. Existing behavior remains covered by tests.
4. Build remains valid for all supported target frameworks practical in the current environment.
