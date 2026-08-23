# Task: Ollama Project Structure 01

## Goal
Reorganize all Ollama production, host, sample, UI, and test projects into scope-oriented folders. Folder paths and namespaces must match, while preserving public behavior and project boundaries.

## Target dependency direction

```text
Interfaces -> Models/Services -> Application -> UI/Host
```

- `Interfaces` contains contracts and provider-neutral abstractions.
- `Models` contains options, requests, responses, value objects, and result types.
- `Services` contains implementations and orchestration services.
- `Application` contains use-case coordination and application-facing state.
- `ViewModels` contains presentation state and commands.
- `Views` contains XAML/Avalonia/WPF views and code-behind.
- `Host` contains executable composition roots and host-specific adapters.
- `Tests` is excluded from the production namespace path.

## Namespace rules

1. A production namespace follows the project-relative folder path.
2. Test classes use `<production namespace>.Tests` for the corresponding production scope.
3. Test helper classes use `<production namespace>.Tests.TestDoubles` or another explicit test-only child namespace.
4. A `Tests` project/folder does not become part of the production namespace path.
5. UI converters belong under `Views.Converters` when they are view-specific.
6. Small samples may use `Host` without artificial model/service layers.

## Migration waves

1. Inventory and rules (complete).
2. Core libraries: `Ollama.Protocol`, `Ollama.Client`, and their tests.
3. Shared agent runtime and application layers (in progress).
4. Console, Desktop, Git, and HostCheck projects (Desktop completed).
5. Tools and tool tests.
6. Samples, service examples, and WPF UI.
7. Global namespace/XAML/reference synchronization and full validation.

## Acceptance criteria

- No production C# file remains directly in a project root unless it is an intentionally documented project entry point.
- Namespaces correspond to folder paths.
- Test namespaces follow the production namespace plus `.Tests`.
- Dependency direction is documented and respected for each migrated project.
- All affected projects build successfully.
- All affected test projects pass.
- Each completed wave is recorded here before the next wave starts.

## Completed checkpoint: Desktop wave

The `Ollama.CodingAgent.Desktop` project was reorganized into the following scope folders:

- `Host`: Avalonia application startup, composition, and command-line options.
- `Models`: desktop configuration, configuration state, and desktop options.
- `Services`: configuration persistence, dynamic sessions, and endpoint model discovery.
- `ViewModels`: `DesktopSessionViewModel`.
- `Views`: `MainWindow` and code-behind.
- `Widgets`: existing Avalonia widget controls remain grouped as view components.

The project-root entry-point exception is intentional: `Program.cs`, `App.axaml`, and `App.axaml.cs` remain in the project root. `DesktopComposition` and command-line option parsing remain scoped under `Host`.

Namespaces and Avalonia `x:Class` declarations now follow the folder paths. The desktop production project builds successfully, and `Ollama.CodingAgent.Desktop.Tests` passes 12/12 tests. SVN-aware moves were used for the tracked files; previously unversioned desktop files were added to the working copy before being moved into their target scopes.
