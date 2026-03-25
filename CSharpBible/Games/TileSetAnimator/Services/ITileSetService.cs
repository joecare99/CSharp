using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using TileSetAnimator.Models;

namespace TileSetAnimator.Services;

/// <summary>
/// Handles loading and slicing tile sheets.
/// </summary>
public interface ITileSetService
{
    /// <summary>
    /// Loads an image from disk and freezes it for UI usage.
    /// </summary>
    Task<BitmapSource> LoadTileSheetAsync(string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Produces tile definitions for the supplied layout.
    /// </summary>
    IReadOnlyList<TileDefinition> SliceTiles(BitmapSource sheet, TileGridSettings gridSettings);

    /// <summary>
    /// Exports the supplied tile to a dedicated image file.
    /// </summary>
    Task SaveTileAsync(BitmapSource sheet, TileDefinition tile, string destinationPath, double scale, CancellationToken cancellationToken = default);
}
