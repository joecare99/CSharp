# Task T-GEN-DRIVERS-001: Inventory WinAhnenCls Format and Model Types

## Parent

- Backlog Item `BI-GEN-DRIVERS-001` - Classify and Move Shared Genealogy Model

## Objective

Create a detailed inventory of all production types in `WinAhnenCls` and classify them as HEJ-specific, GEDCOM-specific, shared model/helper, compatibility adapter, or obsolete candidate.

## Scope

- `..\WinAhnenNew\WinAhnenCls\*.cs`
- `..\WinAhnenNew\WinAhnenCls\Helper\*.cs`
- `..\WinAhnenNew\WinAhnenCls\Model\**\*.cs`
- `..\WinAhnenNew\WinAhnenCls\Utils\*.cs`

## Output

- Classification table with current file, type, category, target project, dependencies, and migration risk.
- List of tests that currently cover each type.
- List of uncovered behavior requiring characterization tests.

## Acceptance Criteria

1. Every `WinAhnenCls` production file has one target category.
2. Shared candidates for `BaseGenClasses` are listed separately from HEJ driver internals.
3. GEDCOM-specific candidates are linked to `BI-GEN-DRIVERS-003`.
4. Uncovered high-risk behavior has follow-up test tasks.
