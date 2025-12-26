using System.Collections.Generic;
using System;
using TileSetAnimator.Models;

namespace TileSetAnimator.Services;

/// <summary>
/// Provides algorithms for finding repeating tile sequences.
/// </summary>
public interface IAnimationDetectionService
{
    /// <summary>
    /// Groups tiles into simple contiguous animations.
    /// </summary>
    IReadOnlyList<TileAnimation> DetectAnimations(IEnumerable<TileDefinition> tiles, int minimumFrameCount, TimeSpan frameDuration);
}
