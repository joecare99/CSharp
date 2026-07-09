# Backlog Item BI-GEN-DRIVERS-001: Classify and Move Shared Genealogy Model

## Parent

- Feature `F-GEN-DRIVERS-001` - Refactor WinAhnenCls into HEJ Import/Export Driver

## Objective

Classify the current types in `WinAhnenCls` as HEJ-specific, GEDCOM-specific, or shared genealogy model behavior, then move shared behavior into `BaseGenClasses` in small, tested slices.

## Value

- Prevents generic model logic from remaining hidden in a file-format driver.
- Creates a stable model surface for both HEJ and GEDCOM import/export.
- Reduces duplicate model mapping work across drivers.

## Scope

- `..\WinAhnenNew\WinAhnenCls\Model\CHejGenealogy.cs`
- `..\WinAhnenNew\WinAhnenCls\Model\CHejBase.cs`
- `..\WinAhnenNew\WinAhnenCls\Model\GenBase\*.cs`
- `..\WinAhnenNew\WinAhnenCls\Utils\*.cs`
- Target types in `..\WinAhnenNew\BaseGenClasses\Model` and `..\WinAhnenNew\BaseGenClasses\Helper`

## Deliverables

- Classification table for current `WinAhnenCls` types.
- Migration order for shared types into `BaseGenClasses`.
- Compatibility notes for existing tests and consumers.
- Initial tests in `BaseGenClassesTests` before or alongside moved code.

## Acceptance Criteria

1. Each `WinAhnenCls` type has an assigned target ownership category.
2. The first shared-model migration slice is small enough to implement without broad driver rewrites.
3. Tests define behavior before moving shared logic.
4. Remaining HEJ-specific classes are explicitly documented as driver internals.

## Assumptions

- Some current classes with `CHej` names may still contain reusable genealogy behavior, but naming alone is not sufficient to decide ownership.
- `BaseGenClasses` should not receive HEJ delimiters, section markers, or file-format-specific string handling.

## Open Questions

- Should compatibility aliases remain temporarily in `WinAhnenCls` after shared classes move?
- Should model migration include namespace changes immediately, or should adapters reduce consumer churn first?
