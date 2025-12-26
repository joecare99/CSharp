using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TileSetAnimator.Models;

namespace TileSetAnimator.Services;

/// <inheritdoc />
public sealed class TileSetService : ITileSetService
{
    /// <inheritdoc />
    public async Task<BitmapSource> LoadTileSheetAsync(string filePath, CancellationToken cancellationToken = default)
    {
        await using var stream = File.OpenRead(filePath);
        var bitmap = new BitmapImage();
        bitmap.BeginInit();
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.StreamSource = stream;
        bitmap.EndInit();
        bitmap.Freeze();
        return bitmap;
    }

    /// <inheritdoc />
    public IReadOnlyList<TileDefinition> SliceTiles(BitmapSource sheet, TileGridSettings gridSettings)
    {
        if (!gridSettings.IsValid)
        {
            return Array.Empty<TileDefinition>();
        }

        var tiles = new List<TileDefinition>();
        var index = 0;
        var stepX = gridSettings.TileWidth + gridSettings.Spacing;
        var stepY = gridSettings.TileHeight + gridSettings.Spacing;
        var maxX = sheet.PixelWidth - gridSettings.Margin;
        var maxY = sheet.PixelHeight - gridSettings.Margin;

        for (var y = gridSettings.Margin; y + gridSettings.TileHeight <= maxY; y += stepY)
        {
            var row = (y - gridSettings.Margin) / stepY;
            for (var x = gridSettings.Margin; x + gridSettings.TileWidth <= maxX; x += stepX)
            {
                var column = (x - gridSettings.Margin) / stepX;
                var bounds = new Int32Rect(x, y, gridSettings.TileWidth, gridSettings.TileHeight);
                tiles.Add(new TileDefinition(index, row, column, bounds, $"Tile {index}", string.Empty, TileCategory.Unknown, string.Empty));
                index++;
            }
        }

        return tiles;
    }

    /// <inheritdoc />
    public async Task SaveTileAsync(BitmapSource sheet, TileDefinition tile, string destinationPath, double scale, CancellationToken cancellationToken = default)
    {
        var cropped = new CroppedBitmap(sheet, tile.Bounds);
        BitmapSource output = cropped;
        if (scale > 1)
        {
            output = new TransformedBitmap(cropped, new ScaleTransform(scale, scale));
        }

        output.Freeze();

        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(output));

        await using var stream = File.Open(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None);
        encoder.Save(stream);
    }
}
