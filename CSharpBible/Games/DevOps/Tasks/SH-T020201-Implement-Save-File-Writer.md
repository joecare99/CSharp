# SH-T020201 - Implement Save File Writer

## Work Item Type
Task

## Parent
`SH-BI0202 - Implement File Backed GamePersist`

## Goal
Write save DTOs to durable storage.

## Scope
- Choose initial serialization format according to decision.
- Write atomically where practical.
- Avoid user-specific hard-coded paths.

## Done
- `GamePersist` or related persistence service can write a save file.
- Save file contains version metadata.

## Status
Completed
