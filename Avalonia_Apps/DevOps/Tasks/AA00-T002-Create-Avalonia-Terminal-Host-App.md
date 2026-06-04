# AA00-T002 Create Avalonia Terminal Host App

## Parent
- Backlog Item: `DevOps/BacklogItems/AA00-Bl002-Avalonia-Terminal-Host-App.md`
- Related Task: `DevOps/Tasks/AA00-T001-Create-Avalonia-Test-Console-Library.md`

## Scope
Create a separate Avalonia desktop application that hosts `Libraries/Avln_TestConsole` as a terminal-like window, starts `%ComSpec%`/`cmd`, redirects `stdout` and `stderr` into the Avalonia console, and forwards line-based user input to the child process through `stdin`.

## Goals
- Keep `Avln_TestConsole` reusable and library-only.
- Add a small practical host application for manual validation.
- Support redirected output and line-based input for the hosted shell process.
- Keep process orchestration testable through abstractions and view-model logic.
- Add a dedicated automated test project for the host orchestration slice.

## Assumptions
- The first increment targets `%ComSpec%`/`cmd` only.
- A simple input box plus send action is sufficient for initial `stdin` support.
- Full VT terminal emulation is explicitly out of scope for this slice.

## Tasks
- [x] Extend the reusable console library only where hosting requires it.
- [x] Add the Avalonia desktop host app and dedicated tests.
- [x] Implement process launching, output forwarding, and input submission.
- [x] Validate scoped builds and tests.

## Validation
- Build: `dotnet build C:\Projekte\CSharp\Avalonia_Apps\Libraries\Avln_TestConsole\Avln_TestConsole.csproj -nologo`
- Build: `dotnet build C:\Projekte\CSharp\Avalonia_Apps\Avln_TerminalHost\Avln_TerminalHost\Avln_TerminalHost.csproj -nologo`
- Tests: `dotnet test C:\Projekte\CSharp\Avalonia_Apps\Libraries\Avln_TestConsoleTests\Avln_TestConsoleTests.csproj -nologo --no-restore`
- Tests: `dotnet test C:\Projekte\CSharp\Avalonia_Apps\Avln_TerminalHost\Avln_TerminalHostTests\Avln_TerminalHostTests.csproj -nologo --no-restore`
- Note: IDE-backed solution-wide build/test remained blocked by unrelated existing workspace errors outside this scope.

## Status
- Done
