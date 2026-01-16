using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using TileSetAnimator.Models;

namespace TileSetAnimator.Services;

public sealed record TileCutoutOptions(
    int BorderThickness,
    int BackgroundBorderThickness,
    int BackgroundThreshold,
    int AlphaSoftThresholdMin,
    int AlphaSoftThresholdMax,
    bool UseSoftAlpha,
    bool PreserveSourceRgbOnTransparent);

public sealed record TileCutoutResult(
    TileDefinition BackgroundTile,
    BitmapSource ResultBitmap);

public interface ITileCutoutService
{
    TileCutoutResult CreateCutout(BitmapSource sheet, TileDefinition foregroundTile, IReadOnlyList<TileDefinition> candidateBackgroundTiles, TileCutoutOptions options);

    Task SaveCutoutAsync(BitmapSource cutoutBitmap, string destinationPath, CancellationToken cancellationToken = default);
}
