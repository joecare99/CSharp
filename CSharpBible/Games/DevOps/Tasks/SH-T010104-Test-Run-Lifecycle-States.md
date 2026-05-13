# SH-T010104 - Test Run Lifecycle States

## Work Item Type
Task

## Parent
`SH-BI0101 - Define Run Lifecycle States`

## Goal
Add MSTest coverage for lifecycle transitions and terminal action guards.

## Scope
- Test `NotStarted` can be represented before active gameplay.
- Test player pending-death transition when `Player.HP <= 0`.
- Test recovery during the additional resolution round prevents `PlayerDead`.
- Test transition to `PlayerDead` after the recovery round if HP remains at or below zero.
- Test blocked movement after death.
- Test blocked wait/action behavior after terminal state.
- Test ViewModel state exposure where practical.

## Done
- Relevant lifecycle tests pass.
- Tests use MSTest and NSubstitute where substitutes are needed.
