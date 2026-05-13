# SH-BI0301 - Introduce Data Driven Content Definitions

## Work Item Type
Backlog Item

## Parent
`SH-F03 - Content and Progression`

## Description
Move prototype creature and item creation from session logic into reusable content definitions.

## Scope
- Define creature archetypes, item archetypes, spawn tables, and depth scaling.
- Move hard-coded goblin, sword, and armor creation out of `GameSession`.
- Keep initial definitions in code or embedded data until format is decided.

## Acceptance Criteria
- Enemy and item spawning uses content definitions.
- Tests can inject deterministic content tables.
- Adding a new enemy does not require editing core turn logic.

## Child Tasks
- `SH-T030101 - Define Content Definition Models`
- `SH-T030102 - Implement Spawn Table Selection`
- `SH-T030103 - Refactor Session Spawning`
- `SH-T030104 - Test Data Driven Spawning`
