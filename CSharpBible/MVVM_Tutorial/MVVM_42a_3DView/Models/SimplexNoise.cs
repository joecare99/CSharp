// ***********************************************************************
// Assembly         : MVVM_42a_3DView
// Author           : Mir
// Created          : 03-09-2025
//
// Last Modified By : Mir
// Last Modified On : 03-09-2025
// ***********************************************************************
// <copyright file="SimplexNoise.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;

namespace MVVM_42a_3DView.Models;

internal static class SimplexNoise
{
    private static readonly double F2 = 0.5 * (Math.Sqrt(3.0) - 1.0);
    private static readonly double G2 = (3.0 - Math.Sqrt(3.0)) / 6.0;
    private static readonly int[][] Grad3 = new[]
    {
        new[] { 1, 1, 0 }, new[] { -1, 1, 0 }, new[] { 1, -1, 0 }, new[] { -1, -1, 0 },
        new[] { 1, 0, 1 }, new[] { -1, 0, 1 }, new[] { 1, 0, -1 }, new[] { -1, 0, -1 },
        new[] { 0, 1, 1 }, new[] { 0, -1, 1 }, new[] { 0, 1, -1 }, new[] { 0, -1, -1 }
    };

    private static readonly int[] Perm = new int[512];
    private static readonly int[] PermMod12 = new int[512];

    private static readonly int[] P =
    {
        151, 160, 137, 91, 90, 15,
        131, 13, 201, 95, 96, 53, 194, 233, 7, 225,
        140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23,
        190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117,
        35, 11, 32, 57, 177, 33, 88, 237, 149, 56, 87, 174, 20, 125, 136, 171,
        168, 68, 175, 74, 165, 71, 134, 139, 48, 27, 166, 77, 146, 158, 231, 83,
        111, 229, 122, 60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244,
        102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208,
        89, 18, 169, 200, 196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173,
        186, 3, 64, 52, 217, 226, 250, 124, 123, 5, 202, 38, 147, 118, 126, 255,
        82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42, 223,
        183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167,
        43, 172, 9, 129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178,
        185, 112, 104, 218, 246, 97, 228, 251, 34, 242, 193, 238, 210, 144, 12, 191,
        179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107, 49, 192, 214, 31, 181,
        199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254, 138,
        236, 205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215,
        61, 156, 180
    };

    static SimplexNoise()
    {
        for (var i = 0; i < 512; i++)
        {
            Perm[i] = P[i & 255];
            PermMod12[i] = Perm[i] % 12;
        }
    }

    public static double Noise(double xin, double yin)
    {
        var f2 = F2;
        var g2 = G2;

        var s = (xin + yin) * f2;
        var i = FastFloor(xin + s);
        var j = FastFloor(yin + s);

        var t = (i + j) * g2;
        var x0 = xin - (i - t);
        var y0 = yin - (j - t);

        int i1;
        int j1;
        if (x0 > y0)
        {
            i1 = 1;
            j1 = 0;
        }
        else
        {
            i1 = 0;
            j1 = 1;
        }

        var x1 = x0 - i1 + g2;
        var y1 = y0 - j1 + g2;
        var x2 = x0 - 1.0 + 2.0 * g2;
        var y2 = y0 - 1.0 + 2.0 * g2;

        var ii = i & 255;
        var jj = j & 255;
        var gi0 = PermMod12[ii + Perm[jj]];
        var gi1 = PermMod12[ii + i1 + Perm[jj + j1]];
        var gi2 = PermMod12[ii + 1 + Perm[jj + 1]];

        var n0 = CornerContribution(gi0, x0, y0);
        var n1 = CornerContribution(gi1, x1, y1);
        var n2 = CornerContribution(gi2, x2, y2);

        return 70.0 * (n0 + n1 + n2);
    }

    private static int FastFloor(double x)
    {
        return x > 0 ? (int)x : (int)x - 1;
    }

    private static double CornerContribution(int gi, double x, double y)
    {
        var t = 0.5 - x * x - y * y;
        if (t < 0)
        {
            return 0.0;
        }

        t *= t;
        return t * t * (Grad3[gi][0] * x + Grad3[gi][1] * y);
    }
}
