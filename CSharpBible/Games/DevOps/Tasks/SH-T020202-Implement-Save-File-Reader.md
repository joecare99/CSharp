# SH-T020202 - Implement Save File Reader

## Work Item Type
Task

## Parent
`SH-BI0202 - Implement File Backed GamePersist`

## Goal
Read save DTOs from durable storage and restore gameplay state.

## Scope
- Read selected serialization format.
- Validate version metadata.
- Restore domain state through mapping code.

## Done
- `GamePersist` or related persistence service can load a saved run.
- Missing files and unsupported versions are handled explicitly.

## Status
Completed
