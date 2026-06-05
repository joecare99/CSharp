using ConsoleLib;
using ConsoleLib.Interfaces;
using System.Collections.Generic;
using WinFormsControl = System.Windows.Forms.Control;

namespace ConsoleLib.WinForms;

/// <summary>
/// Maintains the mapping between ConsoleLib controls and native WinForms controls.
/// </summary>
public sealed class NativeWidgetRegistry
{
    private readonly Dictionary<IControl, WinFormsControl> _widgets = new();

    /// <summary>
    /// Tries to get the registered native widget for a ConsoleLib control.
    /// </summary>
    public bool TryGetWidget(IControl control, out WinFormsControl widget) => _widgets.TryGetValue(control, out widget!);

    /// <summary>
    /// Registers or replaces the native widget for a ConsoleLib control.
    /// </summary>
    public void Register(IControl control, WinFormsControl widget)
    {
        _widgets[control] = widget;
    }

    /// <summary>
    /// Removes the native widget registration for a ConsoleLib control.
    /// </summary>
    public bool Remove(IControl control, out WinFormsControl? widget)
    {
        if (_widgets.TryGetValue(control, out WinFormsControl? existing))
        {
            _widgets.Remove(control);
            widget = existing;
            return true;
        }

        widget = null;
        return false;
    }
}
