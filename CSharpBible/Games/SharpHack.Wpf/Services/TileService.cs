using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Mime;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SharpHack.Base.Model;
using SharpHack.ViewModel;
using WPoint = System.Windows.Point; // Alias for WPF Point

namespace SharpHack.Wpf.Services;

public class TileService : ITileService
{
    private BitmapSource _tileset;
    private int _tileSize;
    private readonly Dictionary<string, CroppedBitmap> _cache = new();

    private int TilesPerRow = 30; // Standard NetHack tileset width in tiles

    public void LoadTileset(string path, int tileSize)
    {
        _tileSize = tileSize;
        // In a real app, load from file. For this demo, we might need a fallback or embedded resource.
        // Assuming the user provides a valid path or we use a placeholder.
        try
        {
            if (File.Exists(path))
            {
                _tileset = new BitmapImage(new Uri(path, UriKind.RelativeOrAbsolute));
                TilesPerRow = _tileset.PixelWidth / tileSize;
            }
            else
                _tileset = CreateFallbackTileset(tileSize);
        }
        catch
        {
            // Fallback: Create a generated bitmap if file not found
            _tileset = CreateFallbackTileset(tileSize);
        }
    }

    private BitmapSource CreateFallbackTileset(int size)
    {
        int width = size * TilesPerRow;
        int height = size * 30; // Enough rows for our indices
        
        var renderBitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
        var visual = new DrawingVisual();

        using (var dc = visual.RenderOpen())
        {
            // Fill background
            dc.DrawRectangle(Brushes.Black, null, new Rect(0, 0, width, height));

            // Draw characters for known types
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

        // Center text in tile
        double x = col * size + (size - formattedText.Width) / 2;
        double y = row * size + (size - formattedText.Height) / 2;

        dc.DrawText(formattedText, new WPoint(x, y));
    }

    public ImageSource GetTile(DisplayTile index)
    {
        return GetSubImage((int)index);
    }

    private ImageSource GetSubImage(int index)
    {
        string key = index.ToString();
        if (_cache.TryGetValue(key, out var cached)) return cached;

        int col = index % TilesPerRow;
        int row = index / TilesPerRow;
        int x = col * _tileSize;
        int y = row * _tileSize;

        // Check bounds
        if (x >= _tileset.PixelWidth || y >= _tileset.PixelHeight)
            return null; 

        var crop = new CroppedBitmap(_tileset, new Int32Rect(x, y, _tileSize, _tileSize));
        _cache[key] = crop;
        return crop;
    }
}
