using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TileSetAnimator.Models;

namespace TileSetAnimator.Services;

public sealed class TileCutoutService : ITileCutoutService
{
    public TileCutoutResult CreateCutout(
        BitmapSource sheet,
        TileDefinition foregroundTile,
        IReadOnlyList<TileDefinition> candidateBackgroundTiles,
        TileCutoutOptions options)
    {
        ArgumentNullException.ThrowIfNull(sheet);
        ArgumentNullException.ThrowIfNull(foregroundTile);
        ArgumentNullException.ThrowIfNull(candidateBackgroundTiles);

        if (candidateBackgroundTiles.Count == 0)
            throw new ArgumentException("No background candidates provided.", nameof(candidateBackgroundTiles));

        var fg = new CroppedBitmap(sheet, foregroundTile.Bounds);
        fg.Freeze();

        var bestBgTile = FindBestBackgroundTile(sheet, fg, candidateBackgroundTiles, options);
        var bg = new CroppedBitmap(sheet, bestBgTile.Bounds);
        bg.Freeze();

        var result = CreateAlphaMaskedBitmap(fg, bg, options);
        result.Freeze();

        return new TileCutoutResult(bestBgTile, result);
    }

    public async Task SaveCutoutAsync(BitmapSource cutoutBitmap, string destinationPath, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(cutoutBitmap);
        if (string.IsNullOrWhiteSpace(destinationPath))
            throw new ArgumentException("Destination path is required.", nameof(destinationPath));

        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(cutoutBitmap));

        await using var stream = File.Open(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None);
        encoder.Save(stream);
    }

    private static TileDefinition FindBestBackgroundTile(
        BitmapSource sheet,
        BitmapSource fg,
        IReadOnlyList<TileDefinition> candidates,
        TileCutoutOptions options)
    {
        // Score by comparing a border of pixels. Lower score => better match.
        var fgPixels = GetPixelsBgra32(fg);
        int w = fg.PixelWidth;
        int h = fg.PixelHeight;

        var bt = Math.Max(1, options.BorderThickness);

        double bestScore = double.MaxValue;
        TileDefinition best = candidates[0];

        foreach (var c in candidates)
        {
            var bg = new CroppedBitmap(sheet, c.Bounds);
            bg.Freeze();

            if (bg.PixelWidth != w || bg.PixelHeight != h)
                continue;

            var bgPixels = GetPixelsBgra32(bg);

            long sum = 0;
            int count = 0;

            void Acc(int x, int y)
            {
                int i = (y * w + x) * 4;
                int db = fgPixels[i + 0] - bgPixels[i + 0];
                int dg = fgPixels[i + 1] - bgPixels[i + 1];
                int dr = fgPixels[i + 2] - bgPixels[i + 2];
                sum += Math.Abs(db) + Math.Abs(dg) + Math.Abs(dr);
                count++;
            }

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    bool inBorder = x < bt || y < bt || x >= w - bt || y >= h - bt;
                    if (!inBorder) continue;
                    Acc(x, y);
                }
            }

            if (count == 0) continue;
            double score = sum / (double)count;
            if (score < bestScore)
            {
                bestScore = score;
                best = c;
            }
        }

        return best;
    }

    private static BitmapSource CreateAlphaMaskedBitmap(BitmapSource fg, BitmapSource bg, TileCutoutOptions options)
    {
        // both expected BGRA32
        var fgPixels = GetPixelsBgra32(fg);
        var bgPixels = GetPixelsBgra32(bg);

        int w = fg.PixelWidth;
        int h = fg.PixelHeight;

        var outPixels = new byte[w * h * 4];

        int t = Math.Max(0, options.BackgroundThreshold);
        int softMin = Math.Max(0, options.AlphaSoftThresholdMin);
        int softMax = Math.Max(softMin + 1, options.AlphaSoftThresholdMax);

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                int i = (y * w + x) * 4;

                byte fb = fgPixels[i + 0];
                byte fgG = fgPixels[i + 1];
                byte fr = fgPixels[i + 2];

                byte bb = bgPixels[i + 0];
                byte bgG = bgPixels[i + 1];
                byte br = bgPixels[i + 2];

                int dist = Math.Abs(fb - bb) + Math.Abs(fgG - bgG) + Math.Abs(fr - br);

                byte a;
                if (!options.UseSoftAlpha)
                {
                    a = dist <= t ? (byte)0 : (byte)255;
                }
                else
                {
                    if (dist <= softMin) a = 0;
                    else if (dist >= softMax) a = 255;
                    else
                    {
                        double v = (dist - softMin) / (double)(softMax - softMin);
                        a = (byte)Math.Clamp((int)Math.Round(v * 255), 0, 255);
                    }
                }

                if (a == 0 && !options.PreserveSourceRgbOnTransparent)
                {
                    // set rgb to background to reduce fringes
                    outPixels[i + 0] = bb;
                    outPixels[i + 1] = bgG;
                    outPixels[i + 2] = br;
                    outPixels[i + 3] = 0;
                }
                else
                {
                    outPixels[i + 0] = fb;
                    outPixels[i + 1] = fgG;
                    outPixels[i + 2] = fr;
                    outPixels[i + 3] = a;
                }
            }
        }

        var wb = new WriteableBitmap(w, h, fg.DpiX, fg.DpiY, PixelFormats.Bgra32, null);
        wb.WritePixels(new Int32Rect(0, 0, w, h), outPixels, w * 4, 0);
        wb.Freeze();
        return wb;
    }

    private static byte[] GetPixelsBgra32(BitmapSource source)
    {
        if (source.Format != PixelFormats.Bgra32)
        {
            var conv = new FormatConvertedBitmap(source, PixelFormats.Bgra32, null, 0);
            conv.Freeze();
            source = conv;
        }

        int w = source.PixelWidth;
        int h = source.PixelHeight;
        var pixels = new byte[w * h * 4];
        source.CopyPixels(pixels, w * 4, 0);
        return pixels;
    }
}
