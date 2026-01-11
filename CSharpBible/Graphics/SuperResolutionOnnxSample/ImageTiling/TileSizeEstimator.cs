using System;
using System.Collections.Generic;

namespace SuperResolutionOnnxSample.ImageTiling;

public static class TileSizeEstimator
{
    public static int EstimateSquareTileSize(int imageWidth, int imageHeight, int maxTiles = 2000)
    {
        if (imageWidth <= 0 || imageHeight <= 0) return 0;

        int g = Gcd(imageWidth, imageHeight);
        if (g <= 1) return 0;

        foreach (var t in GetDivisorsDescending(g))
        {
            int cols = imageWidth / t;
            int rows = imageHeight / t;
            if (cols <= 0 || rows <= 0) continue;
            long tiles = (long)cols * rows;
            if (tiles <= maxTiles) return t;
        }

        return 0;
    }

    private static int Gcd(int a, int b)
    {
        a = Math.Abs(a);
        b = Math.Abs(b);
        while (b != 0)
        {
            int t = a % b;
            a = b;
            b = t;
        }
        return a;
    }

    private static List<int> GetDivisorsDescending(int n)
    {
        var small = new List<int>();
        var large = new List<int>();
        int limit = (int)Math.Sqrt(n);

        for (int i = 1; i <= limit; i++)
        {
            if (n % i != 0) continue;
            small.Add(i);
            int other = n / i;
            if (other != i) large.Add(other);
        }

        large.Sort((a, b) => b.CompareTo(a));
        small.Sort((a, b) => b.CompareTo(a));
        large.AddRange(small);
        return large;
    }
}
