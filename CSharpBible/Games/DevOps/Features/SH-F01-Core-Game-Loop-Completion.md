# SH-F01 - Core Game Loop Completion

## Work Item Type
Feature

## Parent
`SH-E01 - Complete SharpHack as a Playable Roguelike`

## Value
Players can start a run, progress through dungeon depth, die, win, and restart without undefined behavior.

## Scope
- Define explicit run lifecycle states.
- Define victory and dungeon-depth rules.
- Add restart and new-run flow.

## Child Backlog Items
- `SH-BI0101 - Define Run Lifecycle States`
- `SH-BI0102 - Define Victory and Dungeon Depth Rules`
- `SH-BI0103 - Add Restart and New Run Flow`

## Status
Completed

## Acceptance Criteria
- Run state is visible to engine consumers and ViewModels.
- Terminal states block unsafe gameplay actions.
- Death and victory both produce clear end-of-run behavior.
- A new run can start without restarting the application.

## Open Questions
- What is the final victory condition for the first complete game version?
- Should restart preserve previous run summary data?
