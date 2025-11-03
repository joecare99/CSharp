// Copyright (c) JC-Soft
// Direction helpers for grid-based movement
using System;
using System.Collections.Generic;

namespace MathLibrary.TwoDim;

/// <summary>
/// Predefined direction sets (4/8/12) and helpers akin to Pascal dir4/dir8/dir12.
/// </summary>
public static class Directions2D
{
    public static readonly IntPoint[] Dir4 =
    [new(0, 0), new(1, 0), new(0, 1), new(-1, 0), new(0, -1)];
    public static readonly IntPoint[] Dir8 =
    [Dir4[0], Dir4[1], new(1, 1), Dir4[2], new(-1, 1), Dir4[3], new(-1, -1), Dir4[4], new(1, -1)];
    public static readonly IntPoint[] Dir12 =
    [Dir4[0], new(2, 0), new(2, 1), new(1, 2), new(0, 2), new(-1, 2), new(-2, 1), new(-2, 0), new(-2, -1), new(-1, -2), new(0, -2), new(1, -2), new(2, -1)];

    public static int GetDirNo(IntPoint v)
    {
        if (v.X == 0 && v.Y == 0) return0;
        var m = v.MLen();
        if (m == 1) { for (int i = 1; i < Dir8.Length; i++) if (Dir8[i] == v) return i; }
        else if (m == 2) { for (int i = 1; i < Dir12.Length; i++) if (Dir12[i] == v) return i; }
        return -1;
    }

    public static int GetInvDir(int dir, int radius)
    {
        if (dir < 1) return dir;
        return radius switch
        {
            10 => ((dir + 2) % 6) + 1,
            15 => GetDirNo(Dir8[dir].Scale(-1)),
            22 => GetDirNo(Dir12[dir].Scale(-1)),
            _ => dir
        };
    }
}
