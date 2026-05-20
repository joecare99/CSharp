# SH-T030103 - Refactor Session Spawning

## Work Item Type
Task

## Parent
`SH-BI0301 - Introduce Data Driven Content Definitions`

## Goal
Move hard-coded enemy and item spawning out of `GameSession`.

## Scope
- Replace inline goblin creation with content-driven enemy creation.
- Replace inline sword/armor creation with content-driven item creation.
- Keep `GameSession` responsible for placement and game flow only where appropriate.

## Done
- Adding a new spawnable enemy or item does not require editing core turn logic.
- Existing tests are updated for injected content behavior.
