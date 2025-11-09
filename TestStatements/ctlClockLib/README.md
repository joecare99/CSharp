# ctlClockLib

Windows Forms custom control library providing reusable clock and alarm controls.

## Purpose
Supply UI components (ctlClock, ctlAlarmClock) reused by dependent WinForms applications (e.g., TestClockApp).

## Target Frameworks
- net481
- net9.0-windows (for forward compatibility & showcasing multi-era Windows desktop targeting)

## Features
- UserControl-based time display components
- Potential design-time support (SubType=UserControl metadata)
- Separation of UI logic from host application

## Dependencies
- System.Data.DataSetExtensions (legacy convenience APIs)
- Microsoft.CSharp (dynamic invocation or binder use)

## Build
`dotnet build ctlClockLib/ctlClockLib.csproj`

## Consumption
Add a project reference or compiled DLL; controls should appear in the WinForms designer when built.
