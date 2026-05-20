# SH-BI0501 - Standardize Active Map Generator

## Work Item Type
Backlog Item

## Parent
`SH-F05 - Map Generation and World Structure`

## Description
Select and document the primary default dungeon generator for the complete game.

## Scope
- Decide whether `BSPMapGenerator` or `BSPRoomMazeMapGenerator` is primary.
- Move generator selection into shared setup.
- Document generator rules and constraints.

## Acceptance Criteria
- Console and WPF2D use the same default generator.
- Generator choice is configurable for tests or debug builds.
- Documentation identifies the default generator.

## Child Tasks
- `SH-T050101 - Decide Default Map Generator`
- `SH-T050102 - Centralize Generator Selection`
- `SH-T050103 - Document Generator Rules`
- `SH-T050104 - Test Generator Selection`
