using System;
using System.Drawing;
using ConsoleLib.Interfaces;

namespace ConsoleLib.Rendering;

internal sealed class RenderFrameSnapshot : IRenderFrameSnapshot
{
    private readonly TerminalCell[,] _cells;

    public RenderFrameSnapshot(Size size, long revision, Rectangle dirtyRegion, TerminalCell[,] cells)
    {
        Size = size;
        Revision = revision;
        DirtyRegion = dirtyRegion;
        _cells = Copy(cells, size);
    }

    public Size Size { get; }
    public long Revision { get; }
    public Rectangle DirtyRegion { get; }

    public TerminalCell GetCell(int x, int y)
    {
        if (x < 0 || x >= Size.Width || y < 0 || y >= Size.Height)
            throw new ArgumentOutOfRangeException();
        return _cells[x, y];
    }

    public bool IsInvalidated(Rectangle area) => DirtyRegion.IntersectsWith(area);

    private static TerminalCell[,] Copy(TerminalCell[,] source, Size size)
    {
        var copy = new TerminalCell[size.Width, size.Height];
        for (var y = 0; y < size.Height; y++)
            for (var x = 0; x < size.Width; x++)
                copy[x, y] = source[x, y];
        return copy;
    }
}
