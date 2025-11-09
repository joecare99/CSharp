# Calc64_Wpf

WPF front end for the `Calc64Base` logic with a modern MVVM orientation.

## Project
- `Calc64_Wpf.csproj` (modern multi-target: net6.0-windows+)

## Main Files
- `MainWindow.xaml` + code-behind – host for views.
- `Views/CalculatorView.xaml` – specific representation of the calculator.
- `ViewModels/CalculatorViewModel.cs` & `ViewModels/MainWindowViewModel.cs` – presentation logic orchestrating operations from `Calc64Base`.
- `App.xaml` / `App.xaml.cs` – startup, resources, DI bootstrap.

## Dependencies
- `Calc64Base`
- `MVVM_BaseLib` (base classes, commands, converters)

## Extension Ideas
- DataTemplates for additional number systems (hex, binary) in separate panels.
- Undo/Redo via command stack.
- Theme switch (dark/light) via merged ResourceDictionaries.
