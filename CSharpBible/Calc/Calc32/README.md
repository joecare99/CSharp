# Calc32

Multi-target WinForms application (classic .NET Framework targets) PLUS modern .NET (separate *_net* variants) implementing a 32?bit calculator. Contains model and view model source code that is reused (linked / project reference) by other projects (WPF, Console, Tests).

## Projects in this folder
- `Calc32.csproj`  Targets: net462; net472; net48; net481 – classic WinForms EXE.
- `Calc32_net.csproj`  Targets: net6.0-windows through net9.0-windows – modern WinForms EXE (single SDK style; DI / nullable enabled via shared props file).
- `Calc32.Model.csproj`  (if present) – Pure model / logic (no UI) to separate presentation concerns.
- `Calc32.WForms.csproj` / `Calc32_net.WForms.csproj`  Alternative UI packaging / naming demonstrations.

## Architecture
- Model: `Models/CalculatorClass.cs`, interface `ICalculatorClass`.
- ViewModel: `ViewModels/CalculatorViewModel.cs` implements binding logic (CommunityToolkit.Mvvm + DI via Microsoft.Extensions.DependencyInjection in *_net* projects).
- View (WinForms): `Visual/FrmCalc32Main*` form + designer.
- Command bindings: `Visual/CommandBindingAttribute.cs` (reflection to connect UI events and view model commands).

## Shared Usage
Projects `Calc32WPF`, `Calc32Cons` and their test projects link source files or reference the project to avoid duplication.

## Dependencies
- `BaseLib` (helpers, IoC, utilities)
- `WFSystem.Windows.Data` (attributes / binding support for WinForms)
- `CommunityToolkit.Mvvm` (modern *_net* only)
- `Microsoft.Extensions.DependencyInjection` (modern *_net* only)

## Build / Multi-Target
Shared settings live in `Calc.props` (classic) and `Calc_net.props` (modern). Resource (*.resx) files provide localization (de/en).

## Tests
Functional / UI-adjacent tests reside in `Calc32Tests` (classic + modern targets) and reuse common model / view model classes.

## Extension Guidelines
- Add new calculator operations by extending `CalculatorClass` or introducing strategy objects.
- Additional UI technologies (e.g. MAUI) can directly consume existing model / view model classes.
