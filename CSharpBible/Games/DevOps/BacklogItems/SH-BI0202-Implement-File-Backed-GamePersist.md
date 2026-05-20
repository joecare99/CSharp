# SH-BI0202 - Implement File Backed GamePersist

## Work Item Type
Backlog Item

## Parent
`SH-F02 - Durable Persistence`

## Description
Replace the placeholder durable persistence implementation with file-backed storage.

## Scope
- Implement `SharpHack.Persist.GamePersist`.
- Keep `InMemoryGamePersist` available for tests and transient sessions.
- Handle missing, corrupt, and unsupported save files explicitly.
- Store saves as compressed JSON.
- Provide recovery behavior for interrupted or partially corrupt save writes where practical.

## Product Decision
- Durable saves should use compressed JSON with recovery support.

## Acceptance Criteria
- A run can be saved to disk and loaded again.
- Saves are written in the selected compressed JSON format.
- Recovery behavior is documented and tested for interrupted or corrupt writes where practical.
- Corrupt save files return a clear failure result or exception type.
- Save tests do not rely on user-specific paths.

## Child Tasks
- `SH-T020201 - Implement Save File Writer`
- `SH-T020202 - Implement Save File Reader`
- `SH-T020203 - Add Save Error Handling`
- `SH-T020204 - Test File Backed Persistence`

## Status
Completed

## Status
In Progress
