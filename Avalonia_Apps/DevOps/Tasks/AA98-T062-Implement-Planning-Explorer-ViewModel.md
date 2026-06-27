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

## Validation
- Run view model tests.

## Status
- Proposed
