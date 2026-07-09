# Task T-Terminal-006 - Add terminal core tests and validate slice

## Status

Done

## Goal

Add automated coverage for the first terminal slice and validate build and tests.

## Scope

- Add MSTest coverage for core buffer, parser, and backend selection logic
- Validate the new projects through build and targeted tests
- Update the terminal tasks to their final delivery status

## Out of Scope

- Interactive end-to-end UI automation
- Runtime verification of Posix behavior on Windows hosts

## Done Criteria

- `Terminal.Core.Tests` covers the first core slice
- Relevant project builds succeed
- Targeted tests pass
- Terminal task statuses are updated to reflect delivery state

## Validation

- `dotnet build ..\Libraries\Terminal.Avalonia\Terminal.Avalonia.csproj`
- `dotnet test ..\Libraries\Terminal.Core.Tests\Terminal.Core.Tests.csproj -f net8.0`
