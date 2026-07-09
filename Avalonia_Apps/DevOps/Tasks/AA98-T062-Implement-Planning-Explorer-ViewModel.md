# AA98-T062 Implement Planning Explorer ViewModel

## Parent
- Backlog Item: `../BacklogItems/AA98-Bl044-DevOps-Planning-UI-Baseline.md`

## Goal
Implement a view model that exposes the local planning hierarchy for UI browsing.

## Scope
- Present epics, features, backlog items, and tasks as a navigable hierarchy.
- Provide selected item metadata and diagnostics.
- Keep editing out of the first UI slice unless already trivial.

## Execution Notes
1. Build on the local planning model reader.
2. Keep UI text localizable where user-facing.
3. Avoid direct provider dependencies.

## Acceptance Criteria
- Planning hierarchy can be browsed through a testable view model.
- Selection exposes source path, ID, title, parent, and status where available.

## Delivered
- Added `PlanningExplorerViewModel` in `AA98_AvlnCodeStudio.UI` to load local planning items through `IPlanningReader`, project them into a deterministic hierarchy, and expose root nodes plus diagnostics for UI browsing.
- Added `PlanningExplorerItemViewModel` as a provider-neutral node model for tree navigation with child links and selection metadata fields.
- Integrated the planning explorer into `MainWindowViewModel` and startup composition so the explorer is available through the main shell view model.
- Added deterministic tests in `PlanningExplorerViewModelTests` that validate hierarchy construction, selection metadata exposure, and diagnostics propagation.

## Validation
- Run view model tests.
- `run_tests` for project `AA98_AvlnCodeStudio.Tests` including planning explorer and startup/DI composition checks.

## Status
- Completed
