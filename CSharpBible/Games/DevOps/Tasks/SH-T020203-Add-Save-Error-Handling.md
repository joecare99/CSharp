# SH-T020203 - Add Save Error Handling

## Work Item Type
Task

## Parent
`SH-BI0202 - Implement File Backed GamePersist`

## Goal
Handle persistence failures clearly and safely.

## Scope
- Define errors for missing, corrupt, incompatible, and inaccessible save files.
- Keep user-facing formatting outside low-level persistence.
- Avoid leaking sensitive local paths in UI messages.

## Done
- Persistence errors are distinguishable and testable.
- Callers can present meaningful messages.

## Status
Completed
