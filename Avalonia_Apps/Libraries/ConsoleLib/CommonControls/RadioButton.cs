using System;
using ConsoleLib.Interfaces;

namespace ConsoleLib.CommonControls;

/// <summary>Mutually exclusive boolean option within a parent container.</summary>
public sealed class RadioButton : Control
{
    private bool _isChecked;

    public bool IsChecked
    {
        get => _isChecked;
        private set
        {
            if (_isChecked == value)
                return;
            _isChecked = value;
            Invalidate();
        }
    }

    public void Select()
    {
        if (Parent is not null)
        {
            foreach (var sibling in Parent.Children)
                if (sibling is RadioButton radio && !ReferenceEquals(radio, this))
                    radio.IsChecked = false;
        }
        IsChecked = true;
        Click();
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
            Select();
            e.Handled = true;
            return;
        }
        base.HandlePressKeyEvents(e);
    }
}
