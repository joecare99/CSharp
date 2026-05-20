# SH-T070202 - Add UI Entry Point Error Handling

## Work Item Type
Task

## Parent
`SH-BI0702 - Add Diagnostics and Crash Handling`

## Goal
Handle unexpected UI entry point errors gracefully.

## Scope
- Add top-level exception handling for console and selected WPF UI.
- Log diagnostic details.
- Show recoverable user-facing messages without sensitive paths.

## Done
- UI entry point failures are logged and presented safely.
