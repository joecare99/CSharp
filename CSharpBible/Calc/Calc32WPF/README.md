# Calc32WPF

WPF front end for the 32?bit calculator. Provides both classic .NET Framework targets and modern .NET (separate in `Calc32WPF_net.csproj`). Uses the MVVM pattern with view models sourced (linked) from the WinForms core project.

## Included Projects
- `Calc32WPF.csproj`  Targets: net462; net472; net48; net481 – classic WPF executable.
- `Calc32WPF_net.csproj`  Targets: net6.0-windows; net7.0-windows; net8.0-windows – modern WPF executable.
- (Additional variants: `Calc32.WPF.csproj`, `Calc32_net.WPF.csproj` as alternative naming / packaging demos.)

## Code Sharing
Links source from `../Calc32/`:
- Model: `CalculatorClass`
- Interfaces: `ICalculatorClass`, `ICalculatorViewModel`
- ViewModel: `CalculatorViewModel`

Single implementation of the core logic reused across UIs.

## MVVM Infrastructure
- Uses `MVVM_BaseLib` for base classes (notification, commands, converters).
- DI via `Microsoft.Extensions.DependencyInjection` (modern targets) to resolve the view model in `App.xaml.cs`.

## UI Structure
- `Views/MainWindow.xaml` as the main window.
- `ViewModels/MainWindowViewModel.cs` encapsulates view-specific state and uses `CalculatorViewModel` for calculation logic.

## Resources & Localization
ResX files under `Properties` (auto-generated designer). Nullable enabled in modern projects.

## Tests
`Calc32WPFTests` / `Calc32WPF_netTests` provide UI-adjacent and view model tests. Shared strategy: construct via DI, mock services when needed.

## Extension Guidelines
- Add buttons / operations via commands in `CalculatorViewModel` + XAML bindings.
- Centralize styles / themes in `App.xaml`.
- For undo/redo add a command stack in the view model.
