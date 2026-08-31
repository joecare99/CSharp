using System;
using System.Drawing;
using System.Linq;
using System.Text;
using ConsoleLib.Interfaces;

namespace ConsoleLib.Cxaml.Designer.Preview;

/// <summary>Creates a deterministic, monospace representation of a ConsoleLib control tree.</summary>
public sealed class ConsolePreviewRenderer
{
    public string Render(IControl root)
    {
        if (root is null)
            throw new ArgumentNullException(nameof(root));

        var width = root.size.Width > 0 ? root.size.Width : 80;
        var height = root.size.Height > 0 ? root.size.Height : 25;
        var cells = new TerminalCell[width, height];
        for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
                cells[x, y] = new TerminalCell(' ', root.GetActualForeColor(), root.GetActualBackColor());

        DrawControl(root, cells, width, height);
        var result = new StringBuilder(height * (width + Environment.NewLine.Length));
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
                result.Append(cells[x, y].Character);
            if (y < height - 1)
                result.AppendLine();
        }

        return result.ToString();
    }

    private static void DrawControl(IControl control, TerminalCell[,] cells, int width, int height)
    {
        var text = string.IsNullOrEmpty(control.Text) ? control.GetType().Name : control.Text;
        var startX = Math.Max(0, control.Position.X);
        var startY = Math.Max(0, control.Position.Y);
        for (var i = 0; i < text.Length; i++)
        {
            var x = startX + i;
            if (x >= width || startY >= height)
                break;
            cells[x, startY] = new TerminalCell(text[i], control.GetActualForeColor(), control.GetActualBackColor());
        }

        foreach (var child in control.Children)
            DrawControl(child, cells, width, height);
    }
}
