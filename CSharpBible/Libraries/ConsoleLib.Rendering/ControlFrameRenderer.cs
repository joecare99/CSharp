using System;
using System.Drawing;
using ConsoleLib.Interfaces;
using ConsoleLib.Data;
using ConsoleLib.CommonControls;

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
        DrawBorder(control, cells, size, width, height);
        var text = GetDisplayText(control);
        var x = control.Position.X;
        var y = control.Position.Y + Math.Max(0, (height - 1) / 2);
        var border = GetBorder(control);
        var textStart = border ? 1 : 0;
        var textWidth = Math.Max(0, width - (border ? 2 : 0));
        var textValue = text.Length > textWidth && textWidth >= 1
            ? textWidth == 1 ? "…" : text[..(textWidth - 1)] + "…"
            : text;
        if (control is Button && textValue.Length < textWidth)
            textStart += (textWidth - textValue.Length) / 2;
        var maxLength = Math.Min(textValue.Length, textWidth);
        for (var index = 0; index < maxLength; index++)
        {
            var cellX = x + textStart + index;
            if (cellX < 0 || cellX >= size.Width || y < 0 || y >= size.Height)
                continue;
            cells[cellX, y] = new TerminalCell(textValue[index], control.GetActualForeColor(), control.GetActualBackColor());
        }

        foreach (var child in control.Children)
            DrawControl(child, cells, size);
    }

    private static string GetDisplayText(IControl control)
    {
        if (control is CheckBox checkBox)
            return (checkBox.IsChecked ? "[x] " : "[ ] ") + (control.Text ?? string.Empty);
        return control.Text ?? string.Empty;
    }

    private static bool GetBorder(IControl control) =>
        control is IHasBorder { BorderDefinition.Style: not BorderStyle.None };

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
