# Calc32Cons

Console-oriented (pseudo UI) visualization of the calculator based on `ConsoleLib` + `ExtendedConsole`. Demonstrates headless / terminal driven interaction using the same model and view model classes as WinForms & WPF.

## Projects
- `Calc32Cons.csproj`  Classic: net462–net481
- `Calc32Cons_net.csproj`  Modern: net6.0-windows; net7.0-windows

## Architecture
- Entry point: `Program.cs` initializes DI container, registers `IConsole` implementations, loads the view model.
- View: `Visual/ConsoleCalcView.cs` uses console controls (labels, text box simulation) from `ConsoleLib`.
- Shared logic linked from `../Calc32` (model & view model).

## Dependencies
- `ConsoleLib` (rendering / controls)
- `ExtendedConsole` (extended cursor & output services)
- `Microsoft.Extensions.DependencyInjection`

## Usage
Suitable for automated demos, remote shell tools or tests without a graphical desktop.

## Extension
- Additional keyboard commands by extending the event loop in `ConsoleCalcView`.
- Color / layout adjustments via `Application` / `Control` properties from `ConsoleLib`.
