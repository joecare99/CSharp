# SH-T050102 - Centralize Generator Selection

## Work Item Type
Task

## Parent
`SH-BI0501 - Standardize Active Map Generator`

## Goal
Move default generator selection into shared setup.

## Scope
- Remove divergent generator construction from UI projects.
- Add configuration or factory support for tests.
- Preserve deterministic test injection.

## Done
- Console and WPF2D use the same default generator path.
- Tests can still supply custom generators.
