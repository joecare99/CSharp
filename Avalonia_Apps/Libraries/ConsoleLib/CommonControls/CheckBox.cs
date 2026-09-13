using System;
using ConsoleLib.Interfaces;

namespace ConsoleLib.CommonControls;

/// <summary>Keyboard-toggleable boolean input control.</summary>
public sealed class CheckBox : Control
{
    private bool _isChecked;

    public bool IsChecked
    {
        get => _isChecked;
        set
        {
            if (_isChecked == value)
                return;
            _isChecked = value;
            Invalidate();
        }
    }

    public override void Draw()
    {
        if (WidgetSet is IFormWidgetRenderer renderer)
            renderer.DrawCheckBox(this);
        else
            base.Draw();
    }

    public override void HandlePressKeyEvents(IKeyEvent e)
    {
        if (!Enabled || !Active)
        {
            base.HandlePressKeyEvents(e);
            return;
        }

        if (e.bKeyDown && (e.KeyChar == ' ' || e.usKeyCode == (ushort)ConsoleKey.Enter))
        {
            IsChecked = !IsChecked;
            Click();
            e.Handled = true;
            return;
        }
        base.HandlePressKeyEvents(e);
    }
}
