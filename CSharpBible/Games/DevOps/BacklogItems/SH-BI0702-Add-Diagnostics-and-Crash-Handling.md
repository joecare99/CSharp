# SH-BI0702 - Add Diagnostics and Crash Handling

## Work Item Type
Backlog Item

## Parent
`SH-F07 - Test Quality and Release Readiness`

## Description
Add robust error handling and diagnostics for UI and server entry points.

## Scope
- Add structured error handling around UI entry points and server sessions.
- Add optional log sinks for session events and exceptions.
- Avoid leaking sensitive local paths in user-facing messages.

## Acceptance Criteria
- Unexpected exceptions are logged.
- UI surfaces show recoverable failure messages.
- Server client failures do not crash the listener.

## Child Tasks
- `SH-T070201 - Define Diagnostic Logging Abstraction`
- `SH-T070202 - Add UI Entry Point Error Handling`
- `SH-T070203 - Harden Server Client Handling`
- `SH-T070204 - Test Diagnostics And Failure Handling`
