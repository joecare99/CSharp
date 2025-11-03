using System;
using System.Drawing;
using MathLibrary.TwoDim;
using static MathLibrary.TwoDim.Directions2D;

namespace Treppen.Base;

/// <summary>
/// Height-based labyrinth engine ported from Pascal THeightLaby.
/// </summary>
public sealed class HeightLabyrinth : IHeightLabyrinth
{
    private Rectangle _dimension;
    private int[,] _z;
    private readonly Random _rnd = new();
    public event Action<object, Point>? UpdateCell;

    public Rectangle Dimension
    {
        get => _dimension;
        set { _dimension = value; _z = new int[value.Width, value.Height]; }
    }

    public int this[int x, int y] => InBounds(x, y) ? _z[x, y] : 0;
    private bool InBounds(int x, int y) => x >= 0 && y >= 0 && x < _dimension.Width && y < _dimension.Height;

    public int BaseLevel(int x, int y) => (int)Math.Truncate(x / 1.3 + y / 1.3) + 1;

    public void Generate()
    {
        Array.Clear(_z, 0, _z.Length);
        // Seed pattern skipped for brevity; not required functionally
        var start = new Point(1, Math.Min(6, Math.Max(0, _dimension.Height - 1)));
        if (!InBounds(start.X, start.Y)) return;
        _z[start.X, start.Y] = BaseLevel(start.X, start.Y) - 1;
        var fifo = new Point[_dimension.Width * _dimension.Height];
        int push = 0, pop = 0;
        var act = start; var stored = act;
        int dirCount = 1;
        while (dirCount != 0 || push >= pop)
        {
            // swap
            (act, stored) = (stored, act);
            UpdateCell?.Invoke(this, act);
            dirCount = 0;
            var pos = new Point[4];
            var hh = new int[4];
            for (int i = 1; i < Dir4.Length; i++)
            {
                pos[dirCount] = new Point(Dir4[i].X, Dir4[i].Y);
                var next = new Point(act.X - pos[dirCount].X, act.Y - pos[dirCount].Y);
                if (InBounds(next.X, next.Y) && CalcStepHeight(act, next, out var h) && Math.Abs(h - BaseLevel(next.X, next.Y) - 1) < 3)
                { hh[dirCount] = h; dirCount++; }
            }
            if (dirCount > 0)
            {
                var pick = _rnd.Next(dirCount);
                if (dirCount > 1) { fifo[push++] = act; }
                var next = new Point(act.X - pos[pick].X, act.Y - pos[pick].Y);
                act = next; _z[next.X, next.Y] = hh[pick];
            }
            else if (push >= pop)
            { act = fifo[pop++]; }
        }
        // fill remaining zeros heuristically
        for (int x = (_dimension.Width - 1) | 1; x >= 0; x--)
            for (int y = (_dimension.Height - 1) | 1; y >= 0; y--)
            {
                var pp = new Point(x ^ 1, y ^ 1);
                if (!InBounds(pp.X, pp.Y) || _z[pp.X, pp.Y] != 0) continue;
                bool first = true; int zm = 0, cn = 0, zx = 0, cx = 0;
                for (int i = 1; i < Dir4.Length; i++)
                {
                    var n = new Point(pp.X + Dir4[i].X, pp.Y + Dir4[i].Y);
                    var zz = InBounds(n.X, n.Y) ? _z[n.X, n.Y] : 0;
                    if (zz > 0 && (first || zz <= zm)) { if (zz < zm) cn = 0; zm = zz; if (cn < 2) cn++; }
                    if (zz > 0 && (first || zz >= zx)) { if (zz > zx) cx = 0; zx = zz; if (cx < 2) cx++; first = false; }
                }
                if (zm > 0)
                    _z[pp.X, pp.Y] = (cx == 1 && zx - zm < 6) ? zx + cx : zm - cn;
                else _z[pp.X, pp.Y] = BaseLevel(pp.X, pp.Y);
            }
    }

    private bool CalcStepHeight(Point act, Point next, out int height)
    {
        height = 0;
        var ph = this[act.X, act.Y];
        if (ph == 0 || this[next.X, next.Y] != 0) return false;
        bool canm1 = true, canz = true, canp1 = true;
        IntPoint dd = default;
        for (int i = 1; i < Dir4.Length; i++)
        {
            var t = new Point(next.X + Dir4[i].X, next.Y + Dir4[i].Y);
            if (!(t == act))
            {
                var lb = this[t.X, t.Y];
                canm1 &= (lb == 0) || (lb < ph - 2) || (lb > ph);
                canz &= (lb == 0) || (lb < ph - 1) || (lb > ph + 1);
                canp1 &= (lb == 0) || (lb < ph) || (lb > ph + 2);
            }
            else dd = Dir4[i];
        }
        var dr = new IntPoint(-dd.Y, dd.X);
        var left = new Point(next.X - dr.X, next.Y - dr.Y);
        if (InBounds(left.X, left.Y) && this[left.X, left.Y] == 0)
        {
            var fl = new Point(left.X + dd.X, left.Y + dd.Y);
            var lb = InBounds(fl.X, fl.Y) ? this[fl.X, fl.Y] : 0;
            canm1 &= (lb == 0) || (lb != ph - 1);
            canz &= (lb == 0) || (lb != ph);
            canp1 &= (lb == 0) || (lb != ph + 1);
            var bl = new Point(left.X - dd.X, left.Y - dd.Y);
            lb = InBounds(bl.X, bl.Y) ? this[bl.X, bl.Y] : 0;
            canm1 &= (lb == 0) || (lb != ph - 1);
            canz &= (lb == 0) || (lb != ph);
            canp1 &= (lb == 0) || (lb != ph + 1);
        }
        var right = new Point(next.X + dr.X, next.Y + dr.Y);
        if (InBounds(right.X, right.Y) && this[right.X, right.Y] == 0)
        {
            var fr = new Point(right.X + dd.X, right.Y + dd.Y);
            var lb = InBounds(fr.X, fr.Y) ? this[fr.X, fr.Y] : 0;
            canm1 &= (lb == 0) || (lb != ph - 1);
            canz &= (lb == 0) || (lb != ph);
            canp1 &= (lb == 0) || (lb != ph + 1);
            var br = new Point(right.X - dd.X, right.Y - dd.Y);
            lb = InBounds(br.X, br.Y) ? this[br.X, br.Y] : 0;
            canm1 &= (lb == 0) || (lb != ph - 1);
            canz &= (lb == 0) || (lb != ph);
            canp1 &= (lb == 0) || (lb != ph + 1);
        }
        var blv = BaseLevel(next.X, next.Y);
        if (!(canm1 || canz || canp1)) return false;
        if (canp1 && (blv > ph || (!canz && (!canm1 || blv == ph)))) { height = ph + 1; return true; }
        if (canm1 && (blv < ph || (!canz && (!canp1 || blv == ph)))) { height = ph - 1; return true; }
        if (canz) { height = ph; return true; }
        return false;
    }
}
