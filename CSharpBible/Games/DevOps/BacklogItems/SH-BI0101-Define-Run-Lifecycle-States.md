# SH-BI0101 - Define Run Lifecycle States

## Work Item Type
Backlog Item

## Parent
`SH-F01 - Core Game Loop Completion`

## Description
Introduce explicit run lifecycle states so gameplay actions, ViewModels, and UI surfaces can reason about running, terminal, and abandoned runs consistently.

## Scope
- Add run states such as `NotStarted`, `Running`, `PlayerDead`, `Victory`, and `Abandoned`.
- Prevent unsafe actions after terminal states.
- Surface lifecycle state through ViewModels.

## Clarified Decisions
- `NotStarted` is required for the finished game because the player should first see a start screen with story context, then choose a character/avatar, and only then enter active gameplay.
- The run state model should be named `GameRunState`.
- Player death should not become final immediately at `Player.HP <= 0`; the game should allow one additional resolution round for emergency recovery effects such as instant healing, magic, or spells.
- A terminal `PlayerDead` state is reached only if the player still has `HP <= 0` after that recovery window resolves.

## Acceptance Criteria
- New games can represent a pre-game `NotStarted` state before active gameplay begins.
- Player death changes the session to `PlayerDead` only after `Player.HP <= 0` and one recovery resolution round has completed without restoring HP above zero.
- Victory can change the session to `Victory`.
- Movement, wait, pickup, and door actions fail safely after terminal states.
- ViewModels expose lifecycle state changes.

## Child Tasks
- `SH-T010101 - Add Run State Model`
- `SH-T010102 - Block Actions In Terminal States`
- `SH-T010103 - Expose Run State In ViewModels`
- `SH-T010104 - Test Run Lifecycle States`
