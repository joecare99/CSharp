using System;
using System.Collections.Generic;
using ConsoleLib.Interfaces;

namespace ConsoleLib.CommonControls;

/// <summary>Keyboard-navigable tab collection.</summary>
public sealed class TabControl : Control
{
    private readonly List<TabItem> _items = new();
    private int _selectedIndex = -1;

    public IList<TabItem> Items => _items;
    public int SelectedIndex => _selectedIndex;
    public TabItem? SelectedItem => _selectedIndex >= 0 && _selectedIndex < _items.Count ? _items[_selectedIndex] : null;

    public override void Draw()
    {
        if (WidgetSet is IFormWidgetRenderer renderer)
            renderer.DrawTabControl(this);
        else
            base.Draw();
    }

    public bool SelectNext() => Select(_selectedIndex + 1);
    public bool SelectPrevious() => Select(_selectedIndex < 0 ? -1 : _selectedIndex - 1);

    private bool Select(int index)
    {
        if (index < 0 || index >= _items.Count)
            return false;
        _selectedIndex = index;
        Invalidate();
        return true;
    }

    public override void HandlePressKeyEvents(IKeyEvent e)
    {
        if (Enabled && Active && e.bKeyDown)
        {
            var handled = e.usKeyCode switch
            {
                (ushort)ConsoleKey.LeftArrow => SelectPrevious(),
                (ushort)ConsoleKey.RightArrow => SelectNext(),
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
}
