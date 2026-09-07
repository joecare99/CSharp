using System;
using System.Drawing;
using ConsoleLib.CommonControls;
using ConsoleLib.Interfaces;

namespace ConsoleLib.Showcase.Desktop.Controls;

/// <summary>
/// A colored half-block pixel canvas. Two vertical pixels are packed into one
/// console cell, matching the rendering technique used by ConsoleDisplay.
/// </summary>
public sealed class ClockPixelCanvas : Label, ICellColorSource
{
    private readonly ConsoleColor[,] _pixels = new ConsoleColor[29, 28];

    public ClockPixelCanvas()
    {
        size = new Size(29, 14);
        Clear();
    }

    public void SetPixel(int x, int y, ConsoleColor color)
    {
        if ((uint)x < 28 && (uint)y < 28)
            _pixels[x, y] = color;
    }

    public void Clear()
    {
        for (var y = 0; y < 28; y++)
            for (var x = 0; x < 28; x++)
                _pixels[x, y] = ConsoleColor.Black;
        Text = RenderText();
    }

    public override void SetText(string value)
    {
        var rows = (value ?? string.Empty).Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        for (var y = 0; y < 28; y++)
            for (var x = 0; x < 28; x++)
                _pixels[x, y] = ConsoleColor.Black;
        for (var y = 0; y < Math.Min(28, rows.Length); y++)
            {
                for (var x = 0; x < Math.Min(28, rows[y].Length); x++)
                {
                    var encoded = rows[y][x];
                    if (encoded >= '0' && encoded <= '9')
                        _pixels[x, y] = (ConsoleColor)(encoded - '0');
                    else if (encoded >= 'A' && encoded <= 'F')
                        _pixels[x, y] = (ConsoleColor)(encoded - 'A' + 10);
                }
            }
            base.SetText(RenderText());
    }

    public string RenderText()
    {
        var rows = new string[14];
        for (var y = 0; y < 28; y += 2)
        {
            var chars = new char[28];
            for (var x = 0; x < 28; x++)
                chars[x] = _pixels[x, y] == ConsoleColor.Black && _pixels[x, y + 1] == ConsoleColor.Black
                    ? ' '
                    : '▄';
            rows[y / 2] = new string(chars);
        }

        return string.Join(Environment.NewLine, rows);
    }

    public ConsoleColor GetCellForeground(int x, int y) => _pixels[x, y * 2 + 1];

    public ConsoleColor GetCellBackground(int x, int y) => _pixels[x, y * 2];

    public void Refresh() => Text = RenderText();
}
