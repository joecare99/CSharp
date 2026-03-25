using System;
using System.Collections.Generic;
using System.Linq;
using TileSetAnimator.Models;

namespace TileSetAnimator.Services;

/// <inheritdoc />
public sealed class ContiguousAnimationDetectionService : IAnimationDetectionService
{
    /// <inheritdoc />
    public IReadOnlyList<TileAnimation> DetectAnimations(IEnumerable<TileDefinition> tiles, int minimumFrameCount, TimeSpan frameDuration)
    {
        var orderedTiles = tiles
            .OrderBy(static t => t.Row)
            .ThenBy(static t => t.Column)
            .ToArray();

        if (orderedTiles.Length == 0 || minimumFrameCount < 2)
        {
            return Array.Empty<TileAnimation>();
        }

        var result = new List<TileAnimation>();
        var current = new List<TileDefinition>();
        var animationIndex = 1;

        foreach (var tile in orderedTiles)
        {
            if (current.Count == 0)
            {
                current.Add(tile);
                continue;
            }

            var previous = current[^1];
            var isNeighbor = tile.Row == previous.Row && tile.Column == previous.Column + 1;
            if (isNeighbor)
            {
                current.Add(tile);
                continue;
            }

            FlushCurrent();
            current.Add(tile);
        }

        FlushCurrent();
        return result;

        void FlushCurrent()
        {
            if (current.Count >= minimumFrameCount)
            {
                var name = $"Auto #{animationIndex++}";
                result.Add(new TileAnimation(Guid.NewGuid(), name, current.ToArray(), frameDuration));
            }

            current.Clear();
        }
    }
}
