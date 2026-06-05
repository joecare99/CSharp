# AA00-Bl001 Avalonia Test Console Library

## Summary
Provide a reusable Avalonia-based test-console component in `Libraries` that can replace the WinForms-oriented `TestConsole` for Avalonia scenarios while staying compatible with `BaseLib.Interfaces.IConsole` and existing content-based assertions.

## Motivation
The current reference implementation lives in a WinForms project outside the Avalonia library area. A native Avalonia library enables reuse in Avalonia-focused tests and applications without depending on WinForms.

## Acceptance Criteria
- A new standalone library exists under `Libraries`.
- The library implements `BaseLib.Interfaces.IConsole`.
- The library exposes encoded content export compatible with the current test-console format.
- A dedicated test project validates the primary behavior.

## Related Tasks
- `DevOps/Tasks/AA00-T001-Create-Avalonia-Test-Console-Library.md`

## Status
- In Progress
