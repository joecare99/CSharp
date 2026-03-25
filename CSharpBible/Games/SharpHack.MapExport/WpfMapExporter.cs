using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SharpHack.ViewModel;
using SharpHack.Wpf.Services;

namespace SharpHack.MapExport;

public sealed class WpfMapExporter
{
    private readonly ITileService _tileService;

    public WpfMapExporter(ITileService tileService)
    {
        _tileService = tileService;
    }

    public void Export(IReadOnlyList<DisplayTile> tiles, int width, int height, int tileSize, string outputPath)
    {
        if (width <= 0 || height <= 0)
            throw new ArgumentOutOfRangeException(nameof(width));

        if (tiles.Count != width * height)
            throw new ArgumentException("tiles size mismatch", nameof(tiles));

        int pixelWidth = width * tileSize;
        int pixelHeight = height * tileSize;

        var visual = new DrawingVisual();
        using (var dc = visual.RenderOpen())
        {
            dc.DrawRectangle(Brushes.Black, null, new Rect(0, 0, pixelWidth, pixelHeight));

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var tile = tiles[y * width + x];
                    var img = _tileService.GetTile(tile);
                    dc.DrawImage(img, new Rect(x * tileSize, y * tileSize, tileSize, tileSize));
                }
            }
        }

        var rtb = new RenderTargetBitmap(pixelWidth, pixelHeight, 96, 96, PixelFormats.Pbgra32);
        rtb.Render(visual);

        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(outputPath))!);

        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(rtb));

        using var fs = File.Create(outputPath);
        encoder.Save(fs);
    }
}
