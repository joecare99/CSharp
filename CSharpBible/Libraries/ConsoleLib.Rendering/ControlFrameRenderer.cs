using System;
using System.Drawing;
using ConsoleLib.Interfaces;
using ConsoleLib.Data;
using ConsoleLib.CommonControls;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleLib.Rendering;

/// <summary>Composes a control tree into a canonical terminal-cell buffer.</summary>
public interface IControlFrameRenderer
{
    /// <summary>Renders the specified root into the supplied buffer.</summary>
    void Render(IControl root, TerminalCell[,] cells, Size size);
}

/// <summary>Minimal canonical renderer for text-bearing controls and their children.</summary>
public sealed class ControlFrameRenderer : IControlFrameRenderer
{
    public void Render(IControl root, TerminalCell[,] cells, Size size)
    {
        if (root is null)
            throw new ArgumentNullException(nameof(root));

        Fill(cells, size, new TerminalCell(' ', root.GetActualForeColor(), root.GetActualBackColor()));
        DrawControl(root, cells, size);
    }

    private static void DrawControl(IControl control, TerminalCell[,] cells, Size size)
    {
        if (!control.IsVisible)
            return;

        var width = Math.Max(1, control.size.Width);
        var height = Math.Max(1, control.size.Height);
        DrawShadow(control, cells, size, width, height);
        DrawBorder(control, cells, size, width, height);
        if (control is ListBox listBox)
        {
            DrawListBox(listBox, cells, size);
            DrawChildren(control, cells, size);
            return;
        }
        if (control is TabControl tabControl)
        {
            DrawTabControl(tabControl, cells, size);
            DrawChildren(control, cells, size);
            return;
        }
        if (control is TileView tileView)
        {
            DrawTileView(tileView, cells, size);
            DrawChildren(control, cells, size);
            return;
        }
        if (control is TreeView treeView)
        {
            DrawTreeView(treeView, cells, size);
            DrawChildren(control, cells, size);
            return;
        }
        if (control is ScrollBar scrollBar)
        {
            DrawScrollBar(scrollBar, cells, size);
            DrawChildren(control, cells, size);
            return;
        }
        if (control is ProgressBar progressBar)
        {
            DrawProgressBar(progressBar, cells, size);
            DrawChildren(control, cells, size);
            return;
        }
        if (control is MenuBar menuBar)
        {
            DrawMenuBar(menuBar, cells, size);
            return;
        }
        if (control is MenuPopup menuPopup)
        {
            DrawMenuPopup(menuPopup, cells, size);
            return;
        }
        if (control is MenuItem menuItem)
        {
            DrawMenuItem(menuItem, cells, size, menuItem.Position);
            DrawChildren(control, cells, size);
            return;
        }
        if (control is StatusBar statusBar)
        {
            DrawStatusBar(statusBar, cells, size);
            DrawChildren(control, cells, size);
            return;
        }

        var text = GetDisplayText(control);
        var x = control.Position.X;
        var y = control is TextBox { MultiLine: true }
            ? control.Position.Y
            : control.Position.Y + Math.Max(0, (height - 1) / 2);
        var border = GetBorder(control);
        var textStart = border ? 1 : 0;
        var textWidth = Math.Max(0, width - (border ? 2 : 0));
        var wraps = control is TextBox { MultiLine: true };
        var textValue = !wraps && text.Length > textWidth && textWidth >= 1
            ? textWidth == 1 ? "…" : text.Substring(0, textWidth - 1) + "…"
            : text;
        if (control is Button && textValue.Length < textWidth)
            textStart += (textWidth - textValue.Length) / 2;
        var lines = wraps && textWidth > 0
            ? WrapLines(textValue, textWidth)
            : new[] { textValue };
        var (foreground, background) = GetColors(control);
        for (var lineIndex = 0; lineIndex < lines.Count; lineIndex++)
        {
            if (y + lineIndex < 0 || y + lineIndex >= size.Height)
                continue;
            var line = lines[lineIndex];
            for (var index = 0; index < Math.Min(line.Length, textWidth); index++)
            {
                var cellX = x + textStart + index;
                if (cellX >= 0 && cellX < size.Width)
                    cells[cellX, y + lineIndex] = new TerminalCell(line[index], foreground, background);
            }
        }

        DrawChildren(control, cells, size);
    }

    private static void DrawChildren(IControl control, TerminalCell[,] cells, Size size)
    {
        for (var index = control.Children.Count - 1; index >= 0; index--)
            DrawControl(control.Children[index], cells, size);
    }

    private static (ConsoleColor Foreground, ConsoleColor Background) GetColors(IControl control)
    {
        if (control.Enabled)
            return (control.GetActualForeColor(), control.GetActualBackColor());
        if (control is Button button)
            return (button.DisabledFrontColor, button.DisabledBackColor);
        return (ConsoleColor.DarkGray, control.GetActualBackColor());
    }

    private static string GetDisplayText(IControl control)
    {
        if (control is CheckBox checkBox)
            return (checkBox.IsChecked ? "[x] " : "[ ] ") + (control.Text ?? string.Empty);
        if (control is ComboBox comboBox)
            return "[" + (comboBox.SelectedItem ?? string.Empty) + "]";
        return control.Text ?? string.Empty;
    }

    private static void DrawListBox(ListBox listBox, TerminalCell[,] cells, Size size)
    {
        var border = GetBorder(listBox);
        var left = listBox.Position.X + (border ? 1 : 0);
        var top = listBox.Position.Y + (border ? 1 : 0);
        var contentWidth = Math.Max(0, listBox.size.Width - (border ? 2 : 0));
        var contentHeight = Math.Max(0, listBox.size.Height - (border ? 2 : 0));
        var visibleRows = Math.Min(contentHeight, listBox.GetVisibleRows());
        for (var row = 0; row < visibleRows; row++)
        {
            var itemIndex = listBox.GetTopIndex() + row;
            var text = itemIndex >= 0 && itemIndex < listBox.GetItemCount()
                ? listBox.GetItemAt(itemIndex)?.ToString() ?? string.Empty
                : string.Empty;
            var (foreground, background) = itemIndex == listBox.GetSelectedIndex()
                ? (listBox.SelectedForeColor, listBox.SelectedBackColor)
                : GetColors(listBox);
            DrawLine(cells, size, left, top + row, contentWidth, text, foreground, background);
        }
    }

    private static void DrawTabControl(TabControl tabControl, TerminalCell[,] cells, Size size)
    {
        var text = string.Empty;
        foreach (var item in tabControl.Items)
        {
            text += ReferenceEquals(item, tabControl.SelectedItem)
                ? "[" + item.Header + "]"
                : " " + item.Header + " ";
        }

        var (foreground, background) = GetColors(tabControl);
        DrawLine(cells, size, tabControl.Position.X, tabControl.Position.Y, tabControl.size.Width, text, foreground, background);
    }

    private static void DrawTileView(TileView tileView, TerminalCell[,] cells, Size size)
    {
        var tileWidth = Math.Max(1, tileView.TileWidth);
        var tileHeight = Math.Max(1, tileView.TileHeight);
        var columns = Math.Max(1, tileView.size.Width / tileWidth);
        var visible = tileView.GetVisibleItems();
        var defaultColors = GetColors(tileView);
        for (var index = 0; index < visible.Count; index++)
        {
            var column = index % columns;
            var row = index / columns;
            var x = tileView.Position.X + column * tileWidth;
            var y = tileView.Position.Y + row * tileHeight;
            var selected = ReferenceEquals(visible[index], tileView.SelectedItem);
            var colors = selected
                ? (ConsoleColor.Yellow, defaultColors.Background)
                : defaultColors;
            for (var tileRow = 0; tileRow < tileHeight; tileRow++)
                DrawLine(cells, size, x, y + tileRow, tileWidth, tileRow == 0 ? visible[index].Text : string.Empty, colors.Item1, colors.Item2);
        }
    }

    private static void DrawTreeView(TreeView treeView, TerminalCell[,] cells, Size size)
    {
        var nodes = treeView.GetVisibleNodes();
        var defaultColors = GetColors(treeView);
        var row = treeView.Position.Y;
        foreach (var node in nodes)
        {
            if (row >= treeView.Position.Y + Math.Max(1, treeView.size.Height))
                break;

            var depth = GetTreeDepth(node);
            var marker = node.Children.Count == 0 ? ' ' : node.IsExpanded ? '-' : '+';
            var text = marker + " " + new string(' ', depth * 2) + node.Text;
            var colors = node.IsSelected
                ? (ConsoleColor.Yellow, defaultColors.Background)
                : defaultColors;
            DrawLine(cells, size, treeView.Position.X, row, treeView.size.Width, text, colors.Item1, colors.Item2);
            row++;
        }
    }

    private static int GetTreeDepth(TreeNode node)
    {
        var depth = 0;
        for (var parent = node.Parent; parent is not null; parent = parent.Parent)
            depth++;
        return depth;
    }

    private static void DrawScrollBar(ScrollBar scrollBar, TerminalCell[,] cells, Size size)
    {
        var width = Math.Max(1, scrollBar.size.Width);
        var height = Math.Max(1, scrollBar.size.Height);
        var disabled = !scrollBar.Enabled;
        var trackForeground = disabled ? scrollBar.DisabledColor : scrollBar.TrackColor;
        var trackBackground = disabled ? scrollBar.DisabledBackColor : scrollBar.BackColor;
        var thumbForeground = disabled ? scrollBar.DisabledColor : scrollBar.ThumbColor;
        var thumbBackground = disabled ? scrollBar.DisabledThumbBackColor : scrollBar.TrackColor;
        var arrowForeground = disabled ? scrollBar.DisabledColor : scrollBar.ArrowColor;
        var arrowBackground = disabled ? scrollBar.DisabledBackColor : scrollBar.BackColor;

        if (scrollBar.Vertical)
        {
            for (var row = 1; row < height - 1; row++)
                PutCell(cells, size, scrollBar.Position.X, scrollBar.Position.Y + row, '│', trackForeground, trackBackground);
            PutCell(cells, size, scrollBar.Position.X, scrollBar.Position.Y, '▲', arrowForeground, arrowBackground);
            PutCell(cells, size, scrollBar.Position.X, scrollBar.Position.Y + height - 1, '▼', arrowForeground, arrowBackground);
        }
        else
        {
            DrawLine(cells, size, scrollBar.Position.X + 1, scrollBar.Position.Y, Math.Max(0, width - 2), new string('─', Math.Max(0, width - 2)), trackForeground, trackBackground);
            PutCell(cells, size, scrollBar.Position.X, scrollBar.Position.Y, '◀', arrowForeground, arrowBackground);
            PutCell(cells, size, scrollBar.Position.X + width - 1, scrollBar.Position.Y, '▶', arrowForeground, arrowBackground);
        }

        var (thumbStart, thumbLength) = scrollBar.GetThumbData();
        for (var index = 0; index < thumbLength; index++)
        {
            var x = scrollBar.Position.X + (scrollBar.Vertical ? 0 : thumbStart + index);
            var y = scrollBar.Position.Y + (scrollBar.Vertical ? thumbStart + index : 0);
            PutCell(cells, size, x, y, '█', thumbForeground, thumbBackground);
        }
    }

    private static void DrawProgressBar(ProgressBar progressBar, TerminalCell[,] cells, Size size)
    {
        var border = GetBorder(progressBar);
        var width = Math.Max(0, progressBar.size.Width - (border ? 2 : 0));
        var fraction = Math.Max(0, Math.Min(1, progressBar.Fraction));
        var filled = (int)Math.Floor(width * fraction);
        var (foreground, background) = GetColors(progressBar);
        var x = progressBar.Position.X + (border ? 1 : 0);
        var y = progressBar.Position.Y + (border ? 1 : 0);
        DrawLine(cells, size, x, y, filled, new string('#', filled), foreground, background);
        DrawLine(cells, size, x + filled, y, width - filled, new string('-', width - filled), foreground, background);
    }

    private static void DrawMenuBar(MenuBar menuBar, TerminalCell[,] cells, Size size)
    {
        DrawLine(cells, size, menuBar.Position.X, menuBar.Position.Y, menuBar.size.Width, string.Empty, menuBar.GetActualForeColor(), menuBar.GetActualBackColor());
        foreach (var item in menuBar.Children.OfType<MenuItem>())
            DrawMenuItem(item, cells, size, new Point(menuBar.Position.X + item.Position.X, menuBar.Position.Y + item.Position.Y));
    }

    private static void DrawMenuPopup(MenuPopup menuPopup, TerminalCell[,] cells, Size size)
    {
        var border = GetBorder(menuPopup);
        var contentWidth = Math.Max(0, menuPopup.size.Width - (border ? 2 : 0));
        var contentHeight = Math.Max(0, menuPopup.size.Height - (border ? 2 : 0));
        var contentX = menuPopup.Position.X + (border ? 1 : 0);
        var contentY = menuPopup.Position.Y + (border ? 1 : 0);
        for (var row = 0; row < contentHeight; row++)
            DrawLine(cells, size, contentX, contentY + row, contentWidth, string.Empty, menuPopup.GetActualForeColor(), menuPopup.GetActualBackColor());
        foreach (var item in menuPopup.Children.OfType<MenuItem>())
        {
            var origin = new Point(menuPopup.Position.X + item.Position.X, menuPopup.Position.Y + item.Position.Y);
            var width = item.IsSeparator ? contentWidth : item.size.Width;
            DrawMenuItem(item, cells, size, origin, width);
        }
    }

    private static void DrawMenuItem(MenuItem menuItem, TerminalCell[,] cells, Size size, Point origin, int? widthOverride = null)
    {
        var width = Math.Max(0, widthOverride ?? menuItem.size.Width);
        if (menuItem.IsSeparator)
        {
            DrawLine(cells, size, origin.X, origin.Y, width, new string('─', width), menuItem.ForeColor, menuItem.BackColor);
            return;
        }

        var text = NormalizeMenuText(menuItem.Text ?? string.Empty, out var acceleratorIndex);
        var selected = menuItem.IsHovered || menuItem.Active;
        var foreground = !menuItem.Enabled
            ? menuItem.DisabledForeColor
            : selected || (menuItem.Parent is MenuBar menuBar && menuBar.ShowAccelerators && acceleratorIndex >= 0)
                ? menuItem.HotColor
                : menuItem.GetActualForeColor();
        var background = selected ? menuItem.HotBackColor : menuItem.GetActualBackColor();
        DrawLine(cells, size, origin.X, origin.Y, width, text, foreground, background);

        if (menuItem.Enabled && menuItem.Parent is MenuBar { ShowAccelerators: true } && acceleratorIndex >= 0 && acceleratorIndex < width && origin.X + acceleratorIndex >= 0 && origin.X + acceleratorIndex < size.Width && origin.Y >= 0 && origin.Y < size.Height)
            cells[origin.X + acceleratorIndex, origin.Y] = new TerminalCell(text[acceleratorIndex], menuItem.HotColor, background);
    }

    private static string NormalizeMenuText(string text, out int acceleratorIndex)
    {
        var normalized = text.Replace("&&", "\0");
        acceleratorIndex = normalized.IndexOf('&');
        if (acceleratorIndex >= 0)
            normalized = normalized.Remove(acceleratorIndex, 1);
        return normalized.Replace('\0', '&');
    }

    private static void DrawStatusBar(StatusBar statusBar, TerminalCell[,] cells, Size size)
    {
        var (foreground, background) = GetColors(statusBar);
        var statusForeground = statusBar.Enabled ? statusBar.StatusColor : foreground;
        var border = GetBorder(statusBar);
        var x = statusBar.Position.X + (border ? 1 : 0);
        var y = statusBar.Position.Y + (border ? 1 : 0);
        var width = Math.Max(0, statusBar.size.Width - (border ? 2 : 0));
        DrawLine(cells, size, x, y, width, statusBar.Status ?? string.Empty, statusForeground, background);
    }

    private static void DrawLine(
        TerminalCell[,] cells,
        Size size,
        int x,
        int y,
        int width,
        string text,
        ConsoleColor foreground,
        ConsoleColor background)
    {
        for (var index = 0; index < width; index++)
        {
            var cellX = x + index;
            if (cellX < 0 || cellX >= size.Width || y < 0 || y >= size.Height)
                continue;
            var character = index < text.Length ? text[index] : ' ';
            cells[cellX, y] = new TerminalCell(character, foreground, background);
        }
    }

    private static void PutCell(
        TerminalCell[,] cells,
        Size size,
        int x,
        int y,
        char character,
        ConsoleColor foreground,
        ConsoleColor background)
    {
        if (x >= 0 && x < size.Width && y >= 0 && y < size.Height)
            cells[x, y] = new TerminalCell(character, foreground, background);
    }

    private static IReadOnlyList<string> WrapLines(string text, int width)
    {
        var lines = new List<string>();
        foreach (var sourceLine in text.Replace("\r", string.Empty).Split('\n'))
        {
            if (sourceLine.Length == 0)
            {
                lines.Add(string.Empty);
                continue;
            }
            for (var offset = 0; offset < sourceLine.Length;)
            {
                var length = Math.Min(width, sourceLine.Length - offset);
                if (offset + length < sourceLine.Length)
                {
                    var split = sourceLine.LastIndexOf(' ', offset + length - 1, length);
                    var endsAtWordBoundary = offset + length >= sourceLine.Length
                        || char.IsWhiteSpace(sourceLine[offset + length]);
                    if (split >= offset && !endsAtWordBoundary)
                        length = split - offset;
                }
                lines.Add(sourceLine.Substring(offset, length).TrimEnd());
                offset += length;
                while (offset < sourceLine.Length && sourceLine[offset] == ' ')
                    offset++;
            }
        }
        return lines;
    }

    private static bool GetBorder(IControl control) =>
        control is IHasBorder { BorderDefinition.Style: not BorderStyle.None };

    private static void DrawShadow(IControl control, TerminalCell[,] cells, Size size, int width, int height)
    {
        if (!control.Shadow)
            return;
        for (var row = 0; row < height; row++)
            for (var column = 0; column < width; column++)
            {
                var x = control.Position.X + column + 1;
                var y = control.Position.Y + row + 1;
                if (x >= 0 && x < size.Width && y >= 0 && y < size.Height)
                    cells[x, y] = new TerminalCell('░', ConsoleColor.Gray, ConsoleColor.Black);
            }
    }

    private static void DrawBorder(IControl control, TerminalCell[,] cells, Size size, int width, int height)
    {
        if (control is not IHasBorder { BorderDefinition: { } definition } || definition.Style == BorderStyle.None)
            return;
        var x = control.Position.X;
        var y = control.Position.Y;
        var glyphs = definition.Style == BorderStyle.Double
            ? new[] { '═', '║', '╔', '╗', '╚', '╝' }
            : new[] { '─', '│', '┌', '┐', '└', '┘' };
        for (var column = 0; column < width; column++)
        {
            Put(cells, size, x + column, y, column == 0 ? glyphs[2] : column == width - 1 ? glyphs[3] : glyphs[0], definition, control);
            if (height > 1)
                Put(cells, size, x + column, y + height - 1, column == 0 ? glyphs[4] : column == width - 1 ? glyphs[5] : glyphs[0], definition, control);
        }
        for (var row = 1; row < height - 1; row++)
        {
            Put(cells, size, x, y + row, glyphs[1], definition, control);
            if (width > 1)
                Put(cells, size, x + width - 1, y + row, glyphs[1], definition, control);
        }
    }

    private static void Put(TerminalCell[,] cells, Size size, int x, int y, char character, IBorderDefinition definition, IControl control)
    {
        if (x >= 0 && x < size.Width && y >= 0 && y < size.Height)
            cells[x, y] = new TerminalCell(character, definition.BorderColor, control.GetActualBackColor());
    }

    private static void Fill(TerminalCell[,] cells, Size size, TerminalCell value)
    {
        for (var y = 0; y < size.Height; y++)
            for (var x = 0; x < size.Width; x++)
                cells[x, y] = value;
    }
}
