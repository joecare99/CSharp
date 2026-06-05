# AA00-T001 Create Avalonia Test Console Library

## Parent
- Backlog Item: `DevOps/BacklogItems/AA00-Bl001-Avalonia-Test-Console-Library.md`

## Scope
Create a standalone Avalonia-based library under `Libraries` that provides a reusable test-console surface compatible with `BaseLib.Interfaces.IConsole` and exports its textual content in the same encoded style as the existing WinForms `TestConsole`.

## Goals
- Add a reusable Avalonia library component for test-console scenarios.
- Implement `BaseLib.Interfaces.IConsole` without referencing the existing WinForms `TestConsole` implementation.
- Preserve the encoded `Content` export behavior needed by tests.
- Add a dedicated automated test project for the new component.
- Keep the implementation self-contained while allowing references to `BaseLib` and `Avln_BaseLib` where useful.

## Assumptions
- A library-only delivery is sufficient for this increment.
- Headless-friendly tests should focus on buffer and export behavior first.
- The initial key handling only needs to support the compatibility level already used by the WinForms test console.

## Tasks
- [ ] Add the new Avalonia library project under `Libraries`.
- [ ] Implement the console buffer, export, rendering control, and `IConsole` wrapper.
- [ ] Add a dedicated MSTest project.
- [ ] Cover content export and basic console behavior with tests.
- [ ] Validate build and relevant tests.

## Validation
- Pending.

## Status
- In Progress
