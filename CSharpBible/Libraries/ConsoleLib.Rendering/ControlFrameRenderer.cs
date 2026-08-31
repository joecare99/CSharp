using System;
using System.Drawing;
using ConsoleLib.Interfaces;
using ConsoleLib.Data;
using ConsoleLib.CommonControls;
using System.Collections.Generic;

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
            ? textWidth == 1 ? "…" : text[..(textWidth - 1)] + "…"
            : text;
        if (control is Button && textValue.Length < textWidth)
            textStart += (textWidth - textValue.Length) / 2;
        var lines = wraps
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

    private static IReadOnlyList<string> WrapLines(string text, int width)
    {
        if (width <= 0)
            return Array.Empty<string>();
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
