using System;
using System.Collections.Generic;
using System.Threading;

namespace ScreenX.Base;

public delegate ExPoint DFunction(ExPoint p, ExPoint p0, ref bool shouldBreak);
public delegate uint CFunction(ExPoint p);

public readonly record struct RenderOptions(
 int Width,
 int Height,
 ExRect Source,
 IReadOnlyList<DFunction> Functions,
 CFunction Colorizer);

public readonly record struct RenderResult(
 int Width,
 int Height,
 uint[] Pixels);

public interface IRendererService
{
    RenderResult Render(RenderOptions options, CancellationToken ct = default);
}

public sealed class RendererService : IRendererService
{
    public RenderResult Render(RenderOptions options, CancellationToken ct = default)
    {
        if (options.Width <= 0 || options.Height <= 0)
            throw new ArgumentOutOfRangeException("Invalid dimensions");
        var w = options.Width;
        var h = options.Height;
        var pixels = new uint[w * h];
        var sx1 = options.Source.X1;
        var sy1 = options.Source.Y1;
        var dx = options.Source.X2 - options.Source.X1;
        var dy = options.Source.Y2 - options.Source.Y1;
        for (int x = 0; x < w; x++)
        {
            double vx = sx1 + dx * (x / (double)w);
            for (int y = 0; y < h; y++)
            {
                ct.ThrowIfCancellationRequested();
                double vy = sy1 + dy * (y / (double)h);
                var p0 = new ExPoint(vx, vy);
                var p = p0;
                bool brk = false;
                if (options.Functions != null)
                {
                    for (int i = 0; i < options.Functions.Count; i++)
                    {
                        p = options.Functions[i](p, p0, ref brk);
                        if (brk) break;
                    }
                }
                var col = options.Colorizer is null ? 0u : options.Colorizer(p);
                pixels[y * w + x] = col;
            }
        }
        return new RenderResult(w, h, pixels);
    }
}
