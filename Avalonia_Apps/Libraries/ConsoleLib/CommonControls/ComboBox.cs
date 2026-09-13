using System;
using System.Collections.Generic;
using ConsoleLib.Interfaces;

namespace ConsoleLib.CommonControls;

/// <summary>Keyboard-navigable single-selection list.</summary>
public sealed class ComboBox : Control
{
    private readonly List<string> _items = new();
    private int _selectedIndex = -1;

    public IList<string> Items => _items;
    public int SelectedIndex => _selectedIndex;
    public string? SelectedItem => _selectedIndex >= 0 && _selectedIndex < _items.Count ? _items[_selectedIndex] : null;

    public override void Draw()
    {
        if (WidgetSet is IFormWidgetRenderer renderer)
            renderer.DrawComboBox(this);
        else
            base.Draw();
    }

    public bool SelectNext() => Select(_selectedIndex + 1);
    public bool SelectPrevious() => Select(_selectedIndex < 0 ? -1 : _selectedIndex - 1);

    public override void HandlePressKeyEvents(IKeyEvent e)
    {
        if (!Enabled || !Active)
        {
            base.HandlePressKeyEvents(e);
            return;
        }

        if (e.bKeyDown)
        {
            var handled = e.usKeyCode switch
            {
                (ushort)ConsoleKey.UpArrow => SelectPrevious(),
                (ushort)ConsoleKey.DownArrow => SelectNext(),
                _ => false
            };
            if (handled)
            {
                e.Handled = true;
                return;
            }
        }
        base.HandlePressKeyEvents(e);
    }

    private bool Select(int index)
    {
        if (index < 0 || index >= _items.Count)
            return false;
        _selectedIndex = index;
        Invalidate();
        return true;
    }
}
