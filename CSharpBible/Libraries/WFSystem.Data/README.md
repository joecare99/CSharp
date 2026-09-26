# WFSystem.Windows.Data

`WFSystem.Windows.Data` supplies lightweight, attribute-based bindings for
WinForms MVVM views. It is intended for views that keep their controls in the
`Views` layer and expose all presentation state and actions through a
ViewModel.

## Binding a ViewModel to a WinForms View

1. Place the form or user control in the application's `Views` namespace.
2. Add the appropriate binding attribute to each control field.
3. Inject the ViewModel into the View constructor.
4. Call `ViewBinding.Commit(this, viewModel)` once after `InitializeComponent`.

```csharp
using System.Windows.Forms;
using Views;

internal sealed class StatusPage : UserControl
{
    [TextBinding(nameof(StatusPageViewModel.StatusText))]
    private readonly Label _statusLabel = new();

    [CommandBinding(nameof(StatusPageViewModel.RefreshCommand))]
    private readonly Button _refreshButton = new();

    public StatusPage(StatusPageViewModel viewModel)
    {
        Controls.Add(_statusLabel);
        Controls.Add(_refreshButton);
        ViewBinding.Commit(this, viewModel);
    }
}
```

`ViewBinding.Commit` is the standard entry point. It activates every current
binding attribute in a deterministic order and preserves the individual
attribute `Commit` methods for existing views.

## Supported Attributes

| Attribute | Binds |
|---|---|
| `TextBindingAttribute` | `Control.Text` and writable string properties |
| `CheckedBindingAttribute` | `CheckBox` or `RadioButton` values |
| `ListBindingAttribute` | `ListBox` and `ComboBox` items and selection |
| `EnabledBindingAttribute` | `Control.Enabled` |
| `VisibilityBindingAttribute` | `Control.Visible` |
| `BackColorBindingAttribute` | `Control.BackColor` |
| `CommandBindingAttribute` | Click actions through `ICommand` |
| `DblClickBindingAttribute` | Double-click and Enter actions through `ICommand` |
| `KeyBindingAttribute` | A configured key through `ICommand` |

## Interfaces

- `IValueConverter` transforms values between a ViewModel and a control when a
  custom representation is needed.

## How It Helps

- Keeps binding declarations beside their corresponding controls.
- Removes repetitive binding setup from Forms and UserControls.
- Lets the ViewModel remain the owner of presentation state and commands.
- Supports thin WinForms Views without introducing a second binding framework.

## Limitations

- The library is a focused WinForms MVVM helper, not a replacement for every
  built-in `System.Windows.Forms.Binding` scenario.
- Bindings are activated once per ViewModel/View pair; do not call
  `ViewBinding.Commit` repeatedly for the same controls because the individual
  attributes subscribe to events.
- Views must not use these attributes to bypass ViewModels and bind directly to
  database, filesystem, or domain services.
