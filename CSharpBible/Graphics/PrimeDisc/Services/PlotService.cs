// Services/PlotService.cs
using System;
using System.Collections.Generic;
using System.Threading;

namespace PrimePlotter.Services
{
    public static class PlotService
    {
        // Liefert ARGB-Int-Puffer (schwarz=0xFF000000, weiß=0xFFFFFFFF)
        public static int[] PlotPoints(
            IReadOnlyList<int> primes,
            int width, int height,
            IProgress<double>? progress = null,
            CancellationToken ct = default)
        {
            int w = width, h = height;
            var pixels = new int[w * h];
            int argbBlack = unchecked((int)0xFF000000);
            int argbWhite = unchecked((int)0xFFFFFFFF);
            for (int i = 0; i < pixels.Length; i++) pixels[i] = argbBlack;

            if (primes.Count == 0) return pixels;

            // Skalierung so, dass r_max (sqrt letzter p) knapp ins Bild passt
            double rMax = Math.Sqrt(primes[^1]);
            double margin = 0.96; // 4% Rand
            double scale = margin * 0.5 * Math.Min(w, h) / Math.Max(1e-9, rMax);
            double cx = (w - 1) / 2.0;
            double cy = (h - 1) / 2.0;

            int lastPct = 50;
            for (int i = 0; i < primes.Count; i++)
            {
                ct.ThrowIfCancellationRequested();
                double p = primes[i];

                // r = sqrt(p), theta = (pi)*sqrt(p)
                double r = Math.Sqrt(p);
                double theta = Math.PI * r;

                double x = cx + scale * r * Math.Cos(theta);
                double y = cy - scale * r * Math.Sin(theta);

                int ix = (int)Math.Round(x);
                int iy = (int)Math.Round(y);

                if ((uint)ix < (uint)w && (uint)iy < (uint)h)
                {
                    int idx = iy * w + ix;
                    pixels[idx] = argbWhite; // einfache Punktdarstellung
                }

                // Fortschritt von 50%..100%
                if ((i & 0x3FFF) == 0 || i == primes.Count - 1) // alle ~16384 Punkte
                {
                    int pct = 50 + (int)Math.Round(50.0 * (i + 1) / primes.Count);
                    if (pct != lastPct) { progress?.Report(pct); lastPct = pct; }
                }
            }

            progress?.Report(100.0);
            return pixels;
        }

        public static int[] PlotPointsDownsampled(
    IReadOnlyList<int> primes,
    int width, int height,
    int scaleFactor,
    IProgress<double>? progress = null,
    CancellationToken ct = default)
        {
            int bigW = width * scaleFactor;
            int bigH = height * scaleFactor;
            var bigBuffer = new float[bigW * bigH]; // Zählpuffer statt Farbe

            if (primes.Count == 0) return new int[width * height];

            double rMax = Math.Sqrt(primes[^1]);
            double margin = 0.96;
            double scale = margin * 0.5 * Math.Min(bigW, bigH) / Math.Max(1e-9, rMax);
            double cx = (bigW - 1) / 2.0;
            double cy = (bigH - 1) / 2.0;

            for (int i = 0; i < primes.Count; i++)
            {
                ct.ThrowIfCancellationRequested();
                double p = primes[i];
                double r = Math.Sqrt(p);
                double theta = Math.PI * r;


                double fx = cx + scale * r * Math.Cos(theta);
                double fy = cy - scale * r * Math.Sin(theta);

                // Bilineare Interpolation der Position (fx,fy) im bigBuffer
                int x0 = (int)Math.Floor(fx);
                int y0 = (int)Math.Floor(fy);
                int x1 = x0 + 1;
                int y1 = y0 + 1;
                double dx = fx - x0;
                double dy = fy - y0;
                if ((uint)x0 < (uint)bigW && (uint)y0 < (uint)bigH)
                    bigBuffer[y0 * bigW + x0] += (float)((1 - dx) * (1 - dy));
                if ((uint)x1 < (uint)bigW && (uint)y0 < (uint)bigH)
                    bigBuffer[y0 * bigW + x1] += (float)(dx * (1 - dy));
                if ((uint)x0 < (uint)bigW && (uint)y1 < (uint)bigH)
                    bigBuffer[y1 * bigW + x0] += (float)((1 - dx) * dy);
                if ((uint)x1 < (uint)bigW && (uint)y1 < (uint)bigH)
                    bigBuffer[y1 * bigW + x1] += (float)(dx * dy);

                if ((i & 0x3FFF) == 0)
                    progress?.Report(50.0 + 50.0 * (i + 1) / primes.Count);
            }

            // Downsampling: pro Pixel der Zielgröße Mittelwert bilden
            var smallBuffer = new int[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float sum = 0;
                    for (int dy = 0; dy < scaleFactor; dy++)
                    {
                        for (int dx = 0; dx < scaleFactor; dx++)
                        {
                            sum += bigBuffer[(y * scaleFactor + dy) * bigW + (x * scaleFactor + dx)];
                        }
                    }
                    // Maximalwert finden oder mit Faktor skalieren
                    int intensity = Math.Min(255, (int)Math.Round(sum*255)); // hier einfach clamp
                    smallBuffer[y * width + x] = unchecked((int)(0xFF000000 | (intensity << 16) | (intensity << 8) | intensity));
                }
            }

            return smallBuffer;
        }

    }
}
