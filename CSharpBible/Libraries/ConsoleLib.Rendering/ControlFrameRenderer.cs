using System;
using System.Drawing;
using ConsoleLib.Interfaces;

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
        var text = control.Text ?? string.Empty;
        var x = control.Position.X;
        var y = control.Position.Y + Math.Max(0, (height - 1) / 2);
        var maxLength = Math.Min(text.Length, Math.Max(0, width - 1));
        for (var index = 0; index < maxLength; index++)
        {
            var cellX = x + index;
            if (cellX < 0 || cellX >= size.Width || y < 0 || y >= size.Height)
                continue;
            cells[cellX, y] = new TerminalCell(text[index], control.GetActualForeColor(), control.GetActualBackColor());
        }

        foreach (var child in control.Children)
            DrawControl(child, cells, size);
    }

    private static void Fill(TerminalCell[,] cells, Size size, TerminalCell value)
    {
        for (var y = 0; y < size.Height; y++)
            for (var x = 0; x < size.Width; x++)
                cells[x, y] = value;
    }
}
