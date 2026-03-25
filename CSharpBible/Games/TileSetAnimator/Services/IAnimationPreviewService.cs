using System;
using System.Threading;
using TileSetAnimator.Models;

namespace TileSetAnimator.Services;

/// <summary>
/// Provides animation preview orchestration.
/// </summary>
public interface IAnimationPreviewService
{
    /// <summary>
    /// Starts a dispatcher-bound preview.
    /// </summary>
    void Start(TileAnimation animation, Action<TileDefinition> onFrame, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops the currently running preview, if any.
    /// </summary>
    void Stop();
}
