# Calc64WF

WinForms UI for the 64?bit calculator logic (`Calc64Base`). Provides classic and modern Windows targets (separate *_net* projects present in this folder).

## Projects
- `Calc64WF.csproj` (classic framework targets)
- `Calc64WF_net.csproj` (modern targets net6.0-windows etc.)

## Components
- `Visual/FrmCalc64Main.*`  Main form + designer + resources.
- `Visual/CommandBindingAttribute.cs`  Reflection-based connection between UI controls and view model commands.
- `Visual/Converter/OperationModeToShortString.cs`  UI-specific converter for mode display.
- `ViewModels/FrmCalc64MainViewModel.cs`  Presentation logic / bindable properties.
- `Program.cs`  Application entry (DI, culture, optional styles).

## Interfaces
- `ViewModels/Interfaces/IFrmCalc64MainViewModel.cs` ensures testability and mocking.

## Dependencies
- `Calc64Base` logic library
- `BaseLib`, optionally further MVVM helpers

## Tests
See `Calc64WFTests` for view model and converter tests.

## Extensions
- Add history display: new control + collection in view model.
- Theme / skin support through centralized color / font constants.
