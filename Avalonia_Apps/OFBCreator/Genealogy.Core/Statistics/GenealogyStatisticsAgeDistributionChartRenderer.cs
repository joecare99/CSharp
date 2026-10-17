using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace Genealogy.Statistics;

/// <summary>
/// Renders normalized yearly age series as monospace text or a portable PNG chart.
/// </summary>
public static class GenealogyStatisticsAgeDistributionChartRenderer
{
    /// <summary>
    /// Renders the distribution chart as ASCII/Unicode text.
    /// </summary>
    public static string RenderMonospace(GenealogyStatisticsAgeDistributionChartModel model, int height = 12)
    {
        ArgumentNullException.ThrowIfNull(model);
        if (height < 2)
        {
            throw new ArgumentOutOfRangeException(nameof(height));
        }

        if (model.Points.Count == 0)
        {
            return string.Empty;
        }

        var output = new StringBuilder();
        for (var row = height - 1; row >= 0; row--)
        {
            var threshold = row / (double)(height - 1);
            var value = model.MaximumValue * row / (height - 1.0);
            output.Append(value.ToString("0.##", CultureInfo.InvariantCulture).PadLeft(7));
            output.Append(" │ ");
            foreach (var point in model.Points)
            {
                output.Append(SelectSymbol(point, threshold));
            }
            output.Append(" │ ");
            output.AppendLine(value.ToString("0.##", CultureInfo.InvariantCulture));
        }

        output.Append("        └");
        output.Append('─', model.Points.Count + 1);
        output.AppendLine("→ Jahr");
        var labels = new char[model.Points.Count];
        Array.Fill(labels, ' ');
        foreach (var tick in model.TimeAxisTicks)
        {
            var index = (int)Math.Round(tick.Value - model.FirstYear);
            if (index >= 0 && index < labels.Length)
            {
                for (var offset = 0; offset < tick.Label.Length && index + offset < labels.Length; offset++)
                {
                    labels[index + offset] = tick.Label[offset];
                }
            }
        }

        output.Append("          ");
        output.AppendLine(new string(labels));
        output.AppendLine($"Y-Achse: Werte 0–{model.MaximumValue:0.##}; X-Achse: Jahr");
        output.AppendLine();
        output.AppendLine("█ Alter | ▒ Generationenabstand | ░ Todesalter");
        return output.ToString();
    }

    /// <summary>
    /// Renders the distribution chart as a self-contained PNG image.
    /// </summary>
    public static byte[] RenderPng(GenealogyStatisticsAgeDistributionChartModel model, int width = 1200, int height = 600)
    {
        ArgumentNullException.ThrowIfNull(model);
        if (width < 100)
        {
            throw new ArgumentOutOfRangeException(nameof(width));
        }
        if (height < 100)
        {
            throw new ArgumentOutOfRangeException(nameof(height));
        }

        var pixels = new byte[width * height * 3];
        Array.Fill(pixels, (byte)255);
        const int left = 60;
        const int right = 30;
        const int top = 25;
        const int bottom = 50;
        var plotRight = width - right;
        var plotBottom = height - bottom;
        DrawLine(pixels, width, height, left, plotBottom, plotRight, plotBottom, 70, 70, 70);
        DrawLine(pixels, width, height, left, top, left, plotBottom, 70, 70, 70);
        for (var subdivision = 0; subdivision <= 5; subdivision++)
        {
            var y = plotBottom - (int)Math.Round(subdivision / 5.0 * (plotBottom - top));
            DrawLine(pixels, width, height, left, y, plotRight, y, 230, 230, 230);
        }

        if (model.Points.Count > 0)
        {
            DrawSeries(pixels, width, height, model.Points, point => point.Age, left, plotBottom, plotRight, top, 30, 100, 210);
            DrawSeries(pixels, width, height, model.Points, point => point.GenerationGap, left, plotBottom, plotRight, top, 35, 155, 70);
            DrawSeries(pixels, width, height, model.Points, point => point.DeathAge, left, plotBottom, plotRight, top, 210, 65, 55);
        }

        foreach (var tick in model.TimeAxisTicks.Where(tick => tick.IsMajor))
        {
            var x = left + (int)Math.Round((tick.Value - model.FirstYear) / Math.Max(1.0, model.LastYear - model.FirstYear) * (plotRight - left));
            DrawLine(pixels, width, height, x, top, x, plotBottom, 225, 225, 225);
        }

        return EncodePng(width, height, pixels);
    }

    private static char SelectSymbol(GenealogyStatisticsAgeDistributionChartPoint point, double threshold)
    {
        var strongest = Math.Max(point.Age, Math.Max(point.GenerationGap, point.DeathAge));
        if (threshold > strongest)
        {
            return ' ';
        }

        if (point.Age >= point.GenerationGap && point.Age >= point.DeathAge)
        {
            return '█';
        }

        if (point.GenerationGap >= point.DeathAge)
        {
            return '▒';
        }

        return '░';
    }

    private static void DrawSeries(
        byte[] pixels,
        int width,
        int height,
        IReadOnlyList<GenealogyStatisticsAgeDistributionChartPoint> points,
        Func<GenealogyStatisticsAgeDistributionChartPoint, double> valueSelector,
        int left,
        int bottom,
        int right,
        int top,
        byte red,
        byte green,
        byte blue)
    {
        if (points.Count == 1)
        {
            var centerX = left + (right - left) / 2;
            var centerY = bottom - (int)Math.Round(valueSelector(points[0]) * (bottom - top));
            DrawLine(pixels, width, height, centerX - 1, centerY, centerX + 1, centerY, red, green, blue);
            return;
        }

        for (var index = 1; index < points.Count; index++)
        {
            var previous = points[index - 1];
            var current = points[index];
            var previousX = left + (previous.Year - points[0].Year) / Math.Max(1.0, points[^1].Year - points[0].Year) * (right - left);
            var currentX = left + (current.Year - points[0].Year) / Math.Max(1.0, points[^1].Year - points[0].Year) * (right - left);
            var previousY = bottom - Math.Clamp(valueSelector(previous), 0, 1) * (bottom - top);
            var currentY = bottom - Math.Clamp(valueSelector(current), 0, 1) * (bottom - top);
            DrawLine(pixels, width, height, (int)Math.Round(previousX), (int)Math.Round(previousY), (int)Math.Round(currentX), (int)Math.Round(currentY), red, green, blue);
        }
    }

    private static void DrawLine(byte[] pixels, int width, int height, int x1, int y1, int x2, int y2, byte red, byte green, byte blue)
    {
        var dx = Math.Abs(x2 - x1);
        var dy = -Math.Abs(y2 - y1);
        var sx = x1 < x2 ? 1 : -1;
        var sy = y1 < y2 ? 1 : -1;
        var error = dx + dy;
        while (true)
        {
            SetPixel(pixels, width, height, x1, y1, red, green, blue);
            if (x1 == x2 && y1 == y2)
            {
                break;
            }

            var doubleError = 2 * error;
            if (doubleError >= dy)
            {
                error += dy;
                x1 += sx;
            }
            if (doubleError <= dx)
            {
                error += dx;
                y1 += sy;
            }
        }
    }

    private static void SetPixel(byte[] pixels, int width, int height, int x, int y, byte red, byte green, byte blue)
    {
        if (x < 0 || y < 0 || x >= width || y >= height)
        {
            return;
        }

        var index = (y * width + x) * 3;
        pixels[index] = red;
        pixels[index + 1] = green;
        pixels[index + 2] = blue;
    }

    private static byte[] EncodePng(int width, int height, byte[] rgbPixels)
    {
        using var imageStream = new MemoryStream();
        imageStream.Write([137, 80, 78, 71, 13, 10, 26, 10]);
        WriteChunk(imageStream, "IHDR", BuildBytes(BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder(width)), BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder(height)), [8, 2, 0, 0, 0]));
        WriteChunk(imageStream, "IDAT", BuildZlibData(width, height, rgbPixels));
        WriteChunk(imageStream, "IEND", Array.Empty<byte>());
        return imageStream.ToArray();
    }

    private static byte[] BuildZlibData(int width, int height, byte[] rgbPixels)
    {
        using var rawStream = new MemoryStream();
        using (var deflateStream = new DeflateStream(rawStream, CompressionLevel.Optimal, leaveOpen: true))
        {
            var rowStride = width * 3;
            for (var row = 0; row < height; row++)
            {
                deflateStream.WriteByte(0);
                deflateStream.Write(rgbPixels, row * rowStride, rowStride);
            }
        }

        var deflated = rawStream.ToArray();
        var zlib = new List<byte> { 0x78, 0x9C };
        zlib.AddRange(deflated);
        return zlib.ToArray();
    }

    private static byte[] BuildBytes(params byte[][] parts) => parts.SelectMany(part => part).ToArray();

    private static void WriteChunk(Stream stream, string type, byte[] data)
    {
        var typeBytes = Encoding.ASCII.GetBytes(type);
        var lengthBytes = BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder(data.Length));
        stream.Write(lengthBytes, 0, lengthBytes.Length);
        stream.Write(typeBytes, 0, typeBytes.Length);
        stream.Write(data, 0, data.Length);
        var crc = BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder((int)Crc32.Compute(typeBytes.Concat(data).ToArray())));
        stream.Write(crc, 0, crc.Length);
    }

    private sealed class Crc32
    {
        private static readonly uint[] Table = BuildTable();

        public static uint Compute(byte[] bytes)
        {
            uint crc = 0xffffffff;
            foreach (var b in bytes)
            {
                crc = Table[(crc ^ b) & 0xff] ^ (crc >> 8);
            }

            return ~crc;
        }

        private static uint[] BuildTable()
        {
            var table = new uint[256];
            for (uint i = 0; i < table.Length; i++)
            {
                var crc = i;
                for (var j = 0; j < 8; j++)
                {
                    crc = (crc & 1) != 0 ? 0xedb88320 ^ (crc >> 1) : crc >> 1;
                }
                table[i] = crc;
            }
            return table;
        }
    }
}
