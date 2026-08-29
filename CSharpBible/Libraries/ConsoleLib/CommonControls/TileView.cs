using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleLib.Interfaces;

namespace ConsoleLib.CommonControls;

/// <summary>Selectable, keyboard-navigable collection of tile items.</summary>
public sealed class TileView : Control
{
    private readonly List<TileItem> _items = new();
    private int _selectedIndex = -1;

    public IList<TileItem> Items => _items;
    public int SelectedIndex => _selectedIndex;
    public TileItem? SelectedItem => _selectedIndex >= 0 && _selectedIndex < _items.Count ? _items[_selectedIndex] : null;
    public int TileWidth { get; set; } = 1;
    public int TileHeight { get; set; } = 1;
    public int FirstVisibleIndex { get; private set; }

    public void SetItems(IEnumerable<TileItem> items)
    {
        if (items is null)
            throw new ArgumentNullException(nameof(items));
        _items.Clear();
        _items.AddRange(items);
        _selectedIndex = _items.Count == 0 ? -1 : 0;
        FirstVisibleIndex = 0;
        Invalidate();
    }

    public IReadOnlyList<TileItem> GetVisibleItems()
    {
        var columns = Math.Max(1, size.Width / Math.Max(1, TileWidth));
        var rows = Math.Max(1, size.Height / Math.Max(1, TileHeight));
        var count = columns * rows;
        return _items.Skip(FirstVisibleIndex).Take(count).ToArray();
    }

    public override void Draw()
    {
        if (WidgetSet is ITileViewRenderer renderer)
            renderer.DrawTileView(this);
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
                (ushort)ConsoleKey.LeftArrow or (ushort)ConsoleKey.UpArrow => SelectPrevious(),
                (ushort)ConsoleKey.RightArrow or (ushort)ConsoleKey.DownArrow => SelectNext(),
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
        var visibleCount = GetVisibleItems().Count;
        if (visibleCount > 0 && index >= FirstVisibleIndex + visibleCount)
            FirstVisibleIndex = index - visibleCount + 1;
        else if (index < FirstVisibleIndex)
            FirstVisibleIndex = index;
        Invalidate();
        return true;
    }
}
