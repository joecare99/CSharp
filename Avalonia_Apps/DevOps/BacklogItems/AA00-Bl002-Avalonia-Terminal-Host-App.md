# AA00-Bl002 Avalonia Terminal Host App

## Summary
Provide a small Avalonia desktop application that hosts the reusable `Avln_TestConsole` library and can run `%ComSpec%`/`cmd` with redirected output and line-based input for practical testing.

## Motivation
The reusable console library needs a practical manual host so its behavior can be exercised with a real child process instead of isolated unit tests only.

## Acceptance Criteria
- A separate Avalonia desktop app exists in the workspace.
- The app displays the `Avln_TestConsole` control.
- The app starts `%ComSpec%` or `cmd` and forwards `stdout`/`stderr` into the console view.
- The app forwards line-based user input to the child process via `stdin`.
- A dedicated automated test project validates the non-UI orchestration logic.

## Related Tasks
- `DevOps/Tasks/AA00-T002-Create-Avalonia-Terminal-Host-App.md`

## Status
- In Progress
