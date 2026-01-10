using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SharpHack.ViewModel;
using WPoint = System.Windows.Point;

namespace SharpHack.Wpf.Services;

public class TileService : ITileService
{
    private BitmapSource? _tileset;
    private int _tileSize;
    private readonly Dictionary<string, CroppedBitmap> _cache = new();

    private int TilesPerRow = 30;

    private ImageSource? _emptyTile;

    public void LoadTileset(string path, int tileSize)
    {
        _tileSize = tileSize;

        try
        {
            if (File.Exists(path))
            {
                _tileset = new BitmapImage(new Uri(path, UriKind.RelativeOrAbsolute));
                TilesPerRow = _tileset.PixelWidth / tileSize;
            }
            else
            {
                _tileset = CreateFallbackTileset(tileSize);
            }
        }
        catch
        {
            _tileset = CreateFallbackTileset(tileSize);
        }

        _cache.Clear();
        _emptyTile = null;
    }

    private BitmapSource CreateFallbackTileset(int size)
    {
        int width = size * TilesPerRow;
        int height = size * 30;

        var renderBitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
        var visual = new DrawingVisual();

        using (var dc = visual.RenderOpen())
        {
            dc.DrawRectangle(Brushes.Black, null, new Rect(0, 0, width, height));

            DrawChar(dc, size, (int)DisplayTile.Archaeologist, "@", Brushes.Yellow);
            DrawChar(dc, size, (int)DisplayTile.Wall_NS, "|", Brushes.Gray);
            DrawChar(dc, size, (int)DisplayTile.Wall_EW, "-", Brushes.Gray);
            DrawChar(dc, size, (int)DisplayTile.Wall_ENWS, "+", Brushes.Gray);
            DrawChar(dc, size, (int)DisplayTile.Wall_EN, "#", Brushes.Gray);
            DrawChar(dc, size, (int)DisplayTile.Wall_NW, "#", Brushes.Gray);
            DrawChar(dc, size, (int)DisplayTile.Wall_WS, "#", Brushes.Gray);
            DrawChar(dc, size, (int)DisplayTile.Wall_ES, "#", Brushes.Gray);
            DrawChar(dc, size, (int)DisplayTile.Wall_ENS, "#", Brushes.Gray);
            DrawChar(dc, size, (int)DisplayTile.Wall_ENW, "#", Brushes.Gray);
            DrawChar(dc, size, (int)DisplayTile.Wall_EWS, "#", Brushes.Gray);
            DrawChar(dc, size, (int)DisplayTile.Wall_NWS, "#", Brushes.Gray);
            DrawChar(dc, size, (int)DisplayTile.Floor_Lit, ".", Brushes.DarkGray);
            DrawChar(dc, size, (int)DisplayTile.Door_Closed, "+", Brushes.DarkKhaki);
            DrawChar(dc, size, (int)DisplayTile.Door_Open, "/", Brushes.DarkKhaki);
            DrawChar(dc, size, (int)DisplayTile.Stairs_Up, "<", Brushes.White);
            DrawChar(dc, size, (int)DisplayTile.Stairs_Down, ">", Brushes.White);
            DrawChar(dc, size, (int)DisplayTile.Goblin, "g", Brushes.Green);
            DrawChar(dc, size, (int)DisplayTile.Sword, "/", Brushes.Cyan);
            DrawChar(dc, size, (int)DisplayTile.Armor, "[", Brushes.Cyan);
        }

        renderBitmap.Render(visual);
        return renderBitmap;
    }

    private void DrawChar(DrawingContext dc, int size, int index, string text, Brush color)
    {
        int col = index % TilesPerRow;
        int row = index / TilesPerRow;

        var formattedText = new FormattedText(
            text,
            CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight,
            new Typeface("Consolas"),
            size,
            color,
            VisualTreeHelper.GetDpi(new ContainerVisual()).PixelsPerDip);

        double x = col * size + (size - formattedText.Width) / 2;
        double y = row * size + (size - formattedText.Height) / 2;

        dc.DrawText(formattedText, new WPoint(x, y));
    }

    public ImageSource GetTile(DisplayTile index)
    {
        EnsureLoaded();
        return GetSubImage((int)index);
    }

    private void EnsureLoaded()
    {
        if (_tileset is not null)
        {
            return;
        }

        LoadTileset(path: "tiles.png", tileSize: 32);
    }

    private ImageSource GetEmptyTile()
    {
        if (_emptyTile is not null)
        {
            return _emptyTile;
        }

        EnsureLoaded();
        var tileset = _tileset!;

        _emptyTile = new CroppedBitmap(tileset, new Int32Rect(0, 0, _tileSize, _tileSize));
        return _emptyTile;
    }

    private ImageSource GetSubImage(int index)
    {
        EnsureLoaded();
        var tileset = _tileset!;

        string key = index.ToString();
        if (_cache.TryGetValue(key, out var cached))
        {
            return cached;
        }

        int col = index % TilesPerRow;
        int row = index / TilesPerRow;
        int x = col * _tileSize;
        int y = row * _tileSize;

        if (x < 0 || y < 0 || x >= tileset.PixelWidth || y >= tileset.PixelHeight)
        {
            return GetEmptyTile();
        }

        var crop = new CroppedBitmap(tileset, new Int32Rect(x, y, _tileSize, _tileSize));
        _cache[key] = crop;
        return crop;
    }
}
