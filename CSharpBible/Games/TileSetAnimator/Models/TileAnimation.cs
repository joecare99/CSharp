using System;
using System.Collections.Generic;
using System.Linq;

namespace TileSetAnimator.Models;

/// <summary>
/// Bundles multiple tiles into a looped animation.
/// </summary>
public sealed record TileAnimation(Guid Id, string Name, IReadOnlyList<TileDefinition> Frames, TimeSpan FrameDuration)
{
    /// <inheritdoc />
    public override string ToString() => $"{Name} ({Frames.Count})";

    /// <summary>
    /// Gets the first frame or null when no frame exists.
    /// </summary>
    public TileDefinition? FirstFrame => Frames.FirstOrDefault();
}
