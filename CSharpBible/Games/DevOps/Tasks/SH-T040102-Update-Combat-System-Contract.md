# SH-T040102 - Update Combat System Contract

## Work Item Type
Task

## Parent
`SH-BI0401 - Replace Simple Combat With Structured Combat Results`

## Goal
Update combat APIs to return structured results.

## Scope
- Change combat contract to return result data.
- Update `SimpleCombatSystem` and `GameSession` integration.
- Keep backward-compatible messaging only where needed temporarily.

## Done
- Combat callers use structured results.
- Existing behavior is preserved except for intentional API changes.
