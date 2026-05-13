# SH-T060404 - Test 3D Mapping Contracts

## Work Item Type
Task

## Parent
`SH-BI0604 - Implement Initial 3D View`

## Goal
Add tests for renderer-independent 3D mapping contracts.

## Scope
- Test tile type to 3D descriptor mapping.
- Test room function and decoration mapping.
- Test actor and item marker mapping.
- Test that mapping does not mutate gameplay state.

## Done
- Mapping tests pass without requiring a GPU or visible UI.
- Tests prove the renderer can consume engine data without owning gameplay rules.
