# SH-T010101 - Add Run State Model

## Work Item Type
Task

## Parent
`SH-BI0101 - Define Run Lifecycle States`

## Goal
Add the domain model needed to represent SharpHack run lifecycle state.

## Scope
- Add a `GameRunState` enum or equivalent model in the core gameplay layer.
- Include `NotStarted`, `Running`, `PlayerDead`, `Victory`, and `Abandoned` as required lifecycle states.
- Support `NotStarted` for the finished game flow where the player first sees story context and chooses a character/avatar before active gameplay starts.
- Initialize prototype sessions compatibly while preserving a model that can represent the future start-screen flow.
- Keep the model independent from UI-specific concerns.

## Done
- Run state is available from `GameSession` or an equivalent engine API.
- `GameRunState` can represent pre-game, active, victory, death, and abandoned states.
- Existing session creation behavior remains compatible.
- Relevant compile errors are resolved.
