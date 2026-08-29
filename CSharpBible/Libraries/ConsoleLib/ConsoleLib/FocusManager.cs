using System;
using System.Collections.Generic;
using ConsoleLib.Interfaces;

namespace ConsoleLib;

/// <summary>Default depth-first focus manager with Tab and Shift+Tab traversal.</summary>
public sealed class FocusManager : IFocusManager
{
    private readonly IControl _root;

    public FocusManager(IControl root)
    {
        _root = root ?? throw new ArgumentNullException(nameof(root));
    }

    public IControl? FocusedControl { get; private set; }

    public bool Focus(IControl control)
    {
        if (!IsFocusable(control))
            return false;

        if (FocusedControl is not null && !ReferenceEquals(FocusedControl, control))
            FocusedControl.Active = false;
        FocusedControl = control;
        control.Active = true;
        return true;
    }

    public bool MoveNext() => Move(1);

    public bool MovePrevious() => Move(-1);

    public void Clear()
    {
        if (FocusedControl is not null)
            FocusedControl.Active = false;
        FocusedControl = null;
    }

    public bool HandleKey(KeyInput input)
    {
        if (!input.IsKeyDown || input.Key != ConsoleKey.Tab)
            return false;
        return (input.Modifiers & KeyModifiers.Shift) != 0 ? MovePrevious() : MoveNext();
    }

    private bool Move(int direction)
    {
        var controls = new List<IControl>();
        Collect(_root, controls);
        if (controls.Count == 0)
            return false;

        var index = FocusedControl is null ? (direction > 0 ? -1 : controls.Count) : controls.IndexOf(FocusedControl);
        for (var i = 1; i <= controls.Count; i++)
        {
            var candidate = controls[(index + direction * i + controls.Count * 2) % controls.Count];
            if (Focus(candidate))
                return true;
        }
        return false;
    }

    private static void Collect(IControl parent, ICollection<IControl> controls)
    {
        foreach (var child in parent.Children)
        {
            if (IsFocusable(child))
                controls.Add(child);
            Collect(child, controls);
        }
    }

    private static bool IsFocusable(IControl control) => control.IsVisible && control.Visible && control.Enabled;
}
