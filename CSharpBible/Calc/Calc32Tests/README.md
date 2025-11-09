# Calc32Tests

Test collection for the Calc32 family (WinForms core, shared model / view model logic). Covers both classic framework targets and modern .NET Windows targets to ensure consistent functionality across runtimes.

## Projects
- `Calc32Tests.csproj` (classic: net462; net472; net48; net481)
- `Calc32_netTests.csproj` (modern targets: multiple – net6.0-windows through net9.0-windows variants)

## Test Types
- `NonVisual/CalculatorClassTests.cs`  Pure logic tests (arithmetic, memory functions, error cases).
- `Visual/FrmCalc32MainTests.cs`  UI-adjacent tests (form initialization, command bindings) – as far as feasible without a real UI thread (may be abstracted / minimized).
- `ProgramTests.cs`  Smoke tests for entry point / DI configuration.
- `Properties/SettingsTests.cs`  Persistence & configuration.

## Framework / Tooling
- MSTest plus (optionally) NSubstitute / other mocks (depending on package references in sibling test projects).
- Shared `CalculatorClass` & `CalculatorViewModel` referenced from the main project.

## Goals
- Ensure deterministic calculation results.
- Prevent regressions in command binding and resources.
- Cross-target parity (same tests in old and new frameworks).

## Extension
- Performance micro tests for critical operations (optional BenchmarkDotNet in a separate project).
- Property-based tests (random inputs) for robustness.
