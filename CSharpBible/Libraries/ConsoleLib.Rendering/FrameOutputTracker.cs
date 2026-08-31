using System;
using System.Drawing;
using ConsoleLib.Interfaces;

namespace ConsoleLib.Rendering;

/// <summary>Tracks one host's last acquired canonical frame and its dirty region.</summary>
public sealed class FrameOutputTracker
{
    private IRenderFrameSnapshot? _last;

    public IRenderFrameSnapshot? LastSnapshot => _last;

    public FrameOutputDelta Acquire(IRenderFrameSnapshot current)
    {
        if (current is null)
            throw new ArgumentNullException(nameof(current));

        var dirty = _last is null || _last.Size != current.Size
            ? new Rectangle(Point.Empty, current.Size)
            : FindDifference(_last, current);
        _last = current;
        return new FrameOutputDelta(current, dirty);
    }

    public void Reset() => _last = null;

    private static Rectangle FindDifference(IRenderSnapshot previous, IRenderSnapshot current)
    {
        var bounds = Rectangle.Empty;
        for (var y = 0; y < current.Size.Height; y++)
            for (var x = 0; x < current.Size.Width; x++)
                if (!previous.GetCell(x, y).Equals(current.GetCell(x, y)))
                    bounds = bounds.IsEmpty ? new Rectangle(x, y, 1, 1) : Rectangle.Union(bounds, new Rectangle(x, y, 1, 1));
        return bounds;
    }
}

/// <summary>Snapshot and host-specific dirty region acquired together.</summary>
public readonly struct FrameOutputDelta
{
    public FrameOutputDelta(IRenderFrameSnapshot snapshot, Rectangle dirtyRegion)
    {
        Snapshot = snapshot;
        DirtyRegion = dirtyRegion;
    }

    public IRenderFrameSnapshot Snapshot { get; }
    public Rectangle DirtyRegion { get; }
    public bool IsDirty => !DirtyRegion.IsEmpty;
}
