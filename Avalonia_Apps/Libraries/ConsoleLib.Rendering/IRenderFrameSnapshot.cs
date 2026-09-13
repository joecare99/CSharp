using System.Drawing;
using ConsoleLib.Interfaces;

namespace ConsoleLib.Rendering;

/// <summary>Immutable terminal frame published by the shared rendering service.</summary>
public interface IRenderFrameSnapshot : IRenderSnapshot
{
    /// <summary>Monotonically increasing frame revision.</summary>
    long Revision { get; }

    /// <summary>Cells that differ from the preceding frame known by the service.</summary>
    Rectangle DirtyRegion { get; }
}
