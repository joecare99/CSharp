# Calc32WPFTests

Test projects for the WPF variant of the 32?bit calculator – covering view model, window initialization and converter / DI setup.

## Projects
- `Calc32WPFTests.csproj` (classic framework targets)
- `Calc32WPF_netTests.csproj` (modern .NET Windows targets: net6.0+)

## Test Focus
- `MainWindowViewModelTests.cs`  Validate state changes, command execution, error paths.
- `MainWindowTests.cs`  Basic initialization (DataContext, resources available, default values) – UI logic only, no rendering.
- `SettingsTests.cs`  Configuration / persistence.

## Methodology
- Construct view models with real core classes (avoid over-mocking) for higher confidence.
- Isolate pure UI artifacts (no actual window rendering in CI).

## Extension
- Snapshot tests of serialized view model states.
- Tests for future value converters / behaviors.
