# Task T-ConsoleProxy-Platform-Robustness

## Status

Done

## Goal

Make `BaseLib.Models.ConsoleProxy` behave robustly across Windows and non-Windows environments while keeping `BaseLib.Interfaces.IConsole` aligned more closely with `System.Console` for supported window-related members.

## Scope

- Replace reflection-based console access in `ConsoleProxy` with direct guarded `System.Console` calls
- Swallow platform-specific and terminal-specific console exceptions for unsupported operations
- Add window-related members to `IConsole` that improve drop-in compatibility with `System.Console`
- Provide ANSI fallback behavior for cursor visibility, cursor positioning, clear, reset-color, and bell operations when practical
- Update automated tests to verify no-throw behavior for the affected members

## Out of Scope

- A full terminal abstraction beyond `IConsole`
- Native Linux terminal bindings or provider-specific console backends
- Interactive test coverage for blocking input operations

## Implementation Notes

- Keep unsupported operations as silent no-ops instead of surfacing exceptions to callers
- Prefer cached fallback values when console getters fail on the current platform or terminal
- Limit ANSI fallback usage to non-redirected output scenarios
- Preserve compatibility with the repository multi-targeting setup, including older .NET Framework targets

## Test Strategy

- Update `ConsoleProxyTests` to execute the methods instead of asserting delegate existence
- Add coverage for `LargestWindowWidth`, `WindowLeft`, `WindowTop`, `SetWindowPosition`, and `SetWindowSize`
- Validate that platform-sensitive members do not throw under the current test environment

## Done Criteria

- `IConsole` exposes the planned additional window-related members
- `ConsoleProxy` no longer depends on reflection for its console access path
- Platform-sensitive operations no longer leak exceptions to callers
- Relevant `BaseLibTests` coverage passes
- Build validation completes for the modified solution scope

## Validation

- `dotnet test ..\Libraries\BaseLibTests\BaseLibTests.csproj -f net8.0 --filter "FullyQualifiedName~ConsoleProxyTests" --no-restore`
- Workspace build completed successfully in Visual Studio
