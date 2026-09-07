using System;
using System.Drawing;
using System.Text;
using ConsoleLib.CommonControls;

namespace ConsoleLib.Showcase.Desktop.Controls;

/// <summary>
/// High-resolution text canvas using one Unicode Braille cell for each 2x4
/// pixel block.
/// </summary>
public sealed class BrailleCanvas : Label
{
    private static readonly Point[] DotOffsets =
    {
        new(0, 0), new(0, 1), new(0, 2), new(1, 0),
        new(1, 1), new(1, 2), new(0, 3), new(1, 3)
    };

    private bool[,] _pixels = new bool[1, 1];

    public int PixelWidth { get; private set; } = 2;
    public int PixelHeight { get; private set; } = 4;

    public BrailleCanvas()
    {
        ResizePixels(PixelWidth, PixelHeight);
    }

    public void ResizePixels(int width, int height)
    {
        PixelWidth = Math.Max(1, width);
        PixelHeight = Math.Max(1, height);
        _pixels = new bool[PixelWidth, PixelHeight];
        size = new Size((PixelWidth + 1) / 2, (PixelHeight + 3) / 4);
        UpdateText();
    }

    public void Clear()
    {
        Array.Clear(_pixels);
        UpdateText();
    }

    public void SetPixel(int x, int y, bool value = true)
    {
        if ((uint)x >= PixelWidth || (uint)y >= PixelHeight)
            return;

        _pixels[x, y] = value;
        UpdateText();
    }

    public string RenderText() => Render(_pixels, PixelWidth, PixelHeight);

    public static string Render(bool[,] pixels, int pixelWidth, int pixelHeight)
    {
        if (pixels is null)
            throw new ArgumentNullException(nameof(pixels));
        if (pixels.GetLength(0) < pixelWidth || pixels.GetLength(1) < pixelHeight)
            throw new ArgumentException("The pixel buffer is smaller than the requested dimensions.", nameof(pixels));

        var columns = (pixelWidth + 1) / 2;
        var rows = (pixelHeight + 3) / 4;
        var output = new StringBuilder();
        for (var row = 0; row < rows; row++)
        {
            if (row > 0)
                output.Append(Environment.NewLine);

            for (var column = 0; column < columns; column++)
            {
                var dots = 0;
                for (var dot = 0; dot < DotOffsets.Length; dot++)
                {
                    var x = column * 2 + DotOffsets[dot].X;
                    var y = row * 4 + DotOffsets[dot].Y;
                    if (x < pixelWidth && y < pixelHeight && pixels[x, y])
                        dots |= 1 << dot;
                }

                output.Append((char)(0x2800 + dots));
            }
        }

        return output.ToString();
    }

    private void UpdateText() => Text = Render(_pixels, PixelWidth, PixelHeight);
}
