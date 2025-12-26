using System;
using System.Threading;
using System.Windows.Threading;
using TileSetAnimator.Models;

namespace TileSetAnimator.Services;

/// <inheritdoc />
public sealed class DispatcherAnimationPreviewService : IAnimationPreviewService
{
    private readonly Dispatcher dispatcher;
    private DispatcherTimer? timer;
    private int frameIndex;
    private TileAnimation? currentAnimation;
    private Action<TileDefinition>? frameCallback;

    /// <summary>
    /// Initializes a new instance of the <see cref="DispatcherAnimationPreviewService"/> class.
    /// </summary>
    public DispatcherAnimationPreviewService() => dispatcher = Dispatcher.CurrentDispatcher;

    /// <inheritdoc />
    public void Start(TileAnimation animation, Action<TileDefinition> onFrame, CancellationToken cancellationToken = default)
    {
        Stop();
        currentAnimation = animation;
        frameCallback = onFrame;
        frameIndex = 0;
        timer = new DispatcherTimer
        {
            Interval = animation.FrameDuration,
        };
        timer.Tick += OnTick;
        timer.Start();
    }

    /// <inheritdoc />
    public void Stop()
    {
        if (timer == null)
        {
            return;
        }

        timer.Stop();
        timer.Tick -= OnTick;
        timer = null;
        currentAnimation = null;
        frameCallback = null;
    }

    private void OnTick(object? sender, EventArgs e)
    {
        if (currentAnimation == null || frameCallback == null || currentAnimation.Frames.Count == 0)
        {
            Stop();
            return;
        }

        var frame = currentAnimation.Frames[frameIndex % currentAnimation.Frames.Count];
        frameIndex++;
        dispatcher.Invoke(() => frameCallback.Invoke(frame));
    }
}
