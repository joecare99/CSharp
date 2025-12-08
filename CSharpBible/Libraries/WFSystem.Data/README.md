# WFSystem.Windows.Data

Lightweight binding / attribute library supporting WinForms MVVM patterns. Provides declarative attributes evaluated by a binding framework (reflection / runtime adapters).

## Key Attributes
- `TextBindingAttribute`
- `CheckedBindingAttribute`
- `EnabledBindingAttribute`
- `VisibilityBindingAttribute`
- `BackColorBindingAttribute`
- `ListBindingAttribute`
- `CommandBindingAttribute`
- `DblClickBindingAttribute`
- `KeyBindingAttribute`

These attributes decorate control classes or properties and allow automatic connection to view model properties / commands.

## Interfaces
- `IValueConverter` (similar to the WPF concept) for transforming values between model and UI.

## Usage
Used in WinForms parts (`Calc32`, `Calc64WF`) to achieve a WPF-like binding experience without heavy frameworks.

## Extension
- Add new attributes following the naming convention (suffix `BindingAttribute`).
- Central binding engine (not included here) should treat attributes generically ? minimal changes required.

## Limitations
- Focus on common standard cases (text, lists, enabled). Not a full DataBinding replacement, but a targeted simplification.
