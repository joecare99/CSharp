# SH-T020301 - Add Save Load ViewModel Commands

## Work Item Type
Task

## Parent
`SH-BI0203 - Add Save Load UI Flows`

## Goal
Expose save/load actions through ViewModel commands.

## Scope
- Add save and load command surfaces.
- Keep file-picker behavior out of core ViewModels if possible.
- Report command status and errors for UI consumption.

## Done
- ViewModels can trigger save/load through injected services.
- Commands are testable without UI dialogs.

## Status
Completed
