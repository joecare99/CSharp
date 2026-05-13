# SH-T060103 - Refactor Console Setup

## Work Item Type
Task

## Parent
`SH-BI0601 - Add Shared Application Orchestration`

## Goal
Move console gameplay setup to the shared orchestration service.

## Scope
- Replace duplicated dependency construction in console setup.
- Keep tile display and console rendering local to console project.
- Preserve current controls and rendering behavior.

## Done
- Console uses shared gameplay setup.
- Console-specific UI code remains local.
