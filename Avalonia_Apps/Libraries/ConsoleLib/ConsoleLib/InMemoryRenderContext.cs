using System;
using System.Drawing;
using ConsoleLib.Interfaces;

namespace ConsoleLib;

/// <summary>In-memory render context for previews, tests and deterministic frame inspection.</summary>
public sealed class InMemoryRenderContext : IRenderContext, IRenderSnapshot
{
    private readonly TerminalCell[,] _cells;
    private Rectangle _invalidated;

    public InMemoryRenderContext(Size size, TerminalCell? initialCell = null)
    {
        if (size.Width < 0 || size.Height < 0)
            throw new ArgumentOutOfRangeException(nameof(size));

        Size = size;
        _cells = new TerminalCell[size.Width, size.Height];
        var cell = initialCell ?? new TerminalCell(' ', ConsoleColor.Gray, ConsoleColor.Black);
        Fill(new Rectangle(Point.Empty, size), cell);
        _invalidated = Rectangle.Empty;
    }

    public Size Size { get; }

    public void SetCell(int x, int y, TerminalCell cell)
    {
        EnsureCellCoordinates(x, y);
        _cells[x, y] = cell;
    }

    public void Fill(Rectangle area, TerminalCell cell)
    {
        foreach (var point in Enumerate(area))
            _cells[point.X, point.Y] = cell;
    }

    public void Invalidate(Rectangle area) => _invalidated = Rectangle.Union(_invalidated, Clip(area));

    public TerminalCell GetCell(int x, int y)
    {
        EnsureCellCoordinates(x, y);
        return _cells[x, y];
    }

    public bool IsInvalidated(Rectangle area) => _invalidated.IntersectsWith(Clip(area));

    private Rectangle Clip(Rectangle area) => Rectangle.Intersect(area, new Rectangle(Point.Empty, Size));

    private void EnsureCellCoordinates(int x, int y)
    {
        if (x < 0 || x >= Size.Width || y < 0 || y >= Size.Height)
            throw new ArgumentOutOfRangeException();
    }

    private System.Collections.Generic.IEnumerable<Point> Enumerate(Rectangle area)
    {
        var clipped = Clip(area);
        for (var y = clipped.Top; y < clipped.Bottom; y++)
            for (var x = clipped.Left; x < clipped.Right; x++)
                yield return new Point(x, y);
    }
}
