# SH-T010102 - Block Actions In Terminal States

## Work Item Type
Task

## Parent
`SH-BI0101 - Define Run Lifecycle States`

## Goal
Prevent gameplay mutations after a run reaches a terminal state.

## Scope
- Guard movement, waiting, pickup, combat, door, and primary actions.
- Return safe no-op behavior or explicit failure according to existing API style.
- Add terminal-state messages only in UI-facing or message-capable layers.
- Treat `Player.HP <= 0` as a pending-death condition first, not an immediate terminal state.
- Allow one additional recovery resolution round for emergency effects such as instant healing, magic, or spells before transitioning to `PlayerDead`.

## Done
- Terminal runs no longer mutate map, player, enemy, or inventory state through normal actions.
- Player death becomes terminal only after the recovery window completes and HP remains at or below zero.
- Behavior is covered by tests in the related test task.
