# SH-T030102 - Implement Spawn Table Selection

## Work Item Type
Task

## Parent
`SH-BI0301 - Introduce Data Driven Content Definitions`

## Goal
Select content entries from spawn tables deterministically under injected randomness.

## Scope
- Add weighted or depth-filtered selection logic.
- Use existing random abstraction where possible.
- Support test-controlled selection.

## Done
- Spawn table selection can choose enemies and items by depth.
- Selection is covered by deterministic tests.
