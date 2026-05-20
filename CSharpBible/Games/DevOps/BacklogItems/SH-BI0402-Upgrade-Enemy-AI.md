# SH-BI0402 - Upgrade Enemy AI

## Work Item Type
Backlog Item

## Parent
`SH-F04 - Combat AI and Magic Depth`

## Description
Improve enemy behavior with pathfinding, perception, and basic state transitions.

## Scope
- Use pathfinding for pursuit around walls and doors.
- Add perception based on field of view, distance, or sound.
- Add states such as idle, alerted, chasing, and fleeing.

## Acceptance Criteria
- Enemies can navigate around simple obstacles.
- Enemies do not always know the player position unless rules allow it.
- Tests cover blocked paths, visible player, unseen player, and adjacent attack behavior.

## Child Tasks
- `SH-T040201 - Add Enemy AI State Model`
- `SH-T040202 - Integrate Pathfinding Into AI`
- `SH-T040203 - Add Enemy Perception Rules`
- `SH-T040204 - Test Upgraded Enemy AI`
