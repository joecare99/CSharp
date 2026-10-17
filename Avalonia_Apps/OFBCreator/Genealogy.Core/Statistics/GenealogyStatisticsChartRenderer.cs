using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace Genealogy.Statistics;

/// <summary>
/// Renders normalized genealogy time series as monospace text or a portable PNG chart.
/// </summary>
public static class GenealogyStatisticsChartRenderer
{
    /// <summary>
    /// Renders the chart series as an ASCII/Unicode chart using error diffusion.
    /// </summary>
    /// <param name="model">The normalized chart model.</param>
    /// <param name="height">The number of chart rows.</param>
    /// <returns>A monospaced chart with the year axis.</returns>
    public static string RenderMonospace(GenealogyStatisticsChartModel model, int height = 12)
    {
        ArgumentNullException.ThrowIfNull(model);
        if (height < 2)
            throw new ArgumentOutOfRangeException(nameof(height));

        if (model.Points.Count == 0)
            return string.Empty;

        var output = new StringBuilder();
        for (var row = height - 1; row >= 0; row--)
        {
            var threshold = row / (double)(height - 1);
            var populationValue = model.PopulationAxisMaximum * row / (height - 1.0);
            var eventValue = model.EventAxisMaximum * row / (height - 1.0);
            output.Append(populationValue.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture).PadLeft(7));
            output.Append(" │ ");
            foreach (var point in model.Points)
            {
                var symbol = SelectSymbol(point, threshold, height);
                output.Append(symbol);
            }
            output.Append(" │ ");
            output.AppendLine(eventValue.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture));
        }

        output.Append("        └");
        output.Append('─', model.Points.Count + 1);
        output.AppendLine("→ Jahr");
        var labels = new char[model.Points.Count];
        Array.Fill(labels, ' ');
        foreach (var tick in model.TimeAxisTicks)
        {
            var index = (int)Math.Round(tick.Value - model.FirstYear);
            var label = tick.Label;
            if (index >= 0 && index < labels.Length)
            {
                for (var offset = 0; offset < label.Length && index + offset < labels.Length; offset++)
                    labels[index + offset] = label[offset];
            }
        }
        output.Append("          ");
        output.AppendLine(new string(labels));
        output.AppendLine($"Y-Achsen: Personen 0–{model.PopulationAxisMaximum:0.##} | Ereignisse 0–{model.EventAxisMaximum:0.##}; X-Achse: {model.FirstYear}–{model.LastYear}, Hauptteilung {model.TimeMajorStepYears} Jahre");
        output.AppendLine();
        output.AppendLine("█ lebende Personen | ▒ Geburten | ░ Todesfälle | ◆ Familiengründungen");
        return output.ToString();
    }

    /// <summary>
    /// Renders a self-contained RGB PNG chart from the shared normalized series.
    /// </summary>
    /// <param name="model">The normalized chart model.</param>
    /// <param name="width">The output width in pixels.</param>
    /// <param name="height">The output height in pixels.</param>
    /// <returns>The PNG bytes.</returns>
    public static byte[] RenderPng(GenealogyStatisticsChartModel model, int width = 1200, int height = 600)
    {
        ArgumentNullException.ThrowIfNull(model);
        if (width < 100)
            throw new ArgumentOutOfRangeException(nameof(width));
        if (height < 100)
            throw new ArgumentOutOfRangeException(nameof(height));

        var pixels = new byte[width * height * 3];
        Array.Fill(pixels, (byte)255);
        const int left = 72;
        const int right = 70;
        const int top = 30;
        const int bottom = 55;
        var plotRight = width - right;
        var plotBottom = height - bottom;
        DrawLine(pixels, width, height, left, plotBottom, plotRight, plotBottom, 70, 70, 70);
        DrawLine(pixels, width, height, left, top, left, plotBottom, 70, 70, 70);
        for (var subdivision = 0; subdivision <= 5; subdivision++)
        {
            var y = plotBottom - (int)Math.Round(subdivision / 5.0 * (plotBottom - top));
            DrawLine(pixels, width, height, left, y, plotRight, y, 225, 225, 225);
        }
        foreach (var tick in model.TimeAxisTicks)
        {
            var x = left + (int)Math.Round(
                (tick.Value - model.FirstYear) / Math.Max(1.0, model.LastYear - model.FirstYear) * (plotRight - left));
            var shade = tick.IsCentury ? (byte)160 : (byte)225;
            DrawLine(pixels, width, height, x, top, x, plotBottom, shade, shade, shade);
        }
        if (model.Points.Count > 0)
        {
            DrawSeries(pixels, width, height, model.Points, point => point.LivingPeople, left, plotBottom, plotRight, top, 30, 100, 210);
            DrawSeries(pixels, width, height, model.Points, point => point.Births, left, plotBottom, plotRight, top, 35, 155, 70);
            DrawSeries(pixels, width, height, model.Points, point => point.Deaths, left, plotBottom, plotRight, top, 210, 65, 55);
            DrawSeries(pixels, width, height, model.Points, point => point.FamilyFoundings, left, plotBottom, plotRight, top, 145, 75, 175);
        }
        return EncodePng(width, height, pixels);
    }

    private static char SelectSymbol(GenealogyStatisticsChartPoint point, double threshold, int height)
    {
        var living = point.LivingPeople;
        var births = point.Births;
        var deaths = point.Deaths;
        var familyFoundings = point.FamilyFoundings;
        var strongest = Math.Max(living, Math.Max(births, Math.Max(deaths, familyFoundings)));
        var quantized = QuantizeWithError(strongest, height);
        if (threshold > quantized)
            return ' ';
        if (living >= births && living >= deaths)
        {
            if (living >= familyFoundings)
                return '█';
        }
        if (births >= deaths && births >= familyFoundings)
            return '▒';
        if (deaths >= familyFoundings)
            return '░';
        return '◆';
    }

    private static double QuantizeWithError(double value, int levels)
    {
        var scaled = value * (levels - 1);
        var rounded = Math.Round(scaled, MidpointRounding.AwayFromZero);
        var error = scaled - rounded;
        var adjusted = Math.Clamp(rounded + error / 2, 0, levels - 1);
        return adjusted / (levels - 1);
    }

    private static void DrawSeries(
        byte[] pixels,
        int width,
        int height,
        IReadOnlyList<GenealogyStatisticsChartPoint> points,
        Func<GenealogyStatisticsChartPoint, double> selector,
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
            var y = bottom - (int)Math.Round(selector(points[0]) * (bottom - top));
            DrawLine(pixels, width, height, left, y, left + 4, y, red, green, blue);
            return;
        }
        for (var index = 1; index < points.Count; index++)
        {
            var previousX = left + (int)Math.Round((index - 1.0) / (points.Count - 1) * (right - left));
            var currentX = left + (int)Math.Round(index / (points.Count - 1.0) * (right - left));
            var previousY = bottom - (int)Math.Round(selector(points[index - 1]) * (bottom - top));
            var currentY = bottom - (int)Math.Round(selector(points[index]) * (bottom - top));
            DrawLine(pixels, width, height, previousX, previousY, currentX, currentY, red, green, blue);
        }
    }

    private static void DrawLine(byte[] pixels, int width, int height, int x0, int y0, int x1, int y1, byte red, byte green, byte blue)
    {
        var dx = Math.Abs(x1 - x0);
        var sx = x0 < x1 ? 1 : -1;
        var dy = -Math.Abs(y1 - y0);
        var sy = y0 < y1 ? 1 : -1;
        var error = dx + dy;
        while (true)
        {
            if (x0 >= 0 && x0 < width && y0 >= 0 && y0 < height)
            {
                var offset = (y0 * width + x0) * 3;
                pixels[offset] = red;
                pixels[offset + 1] = green;
                pixels[offset + 2] = blue;
            }
            if (x0 == x1 && y0 == y1)
                break;
            var twiceError = 2 * error;
            if (twiceError >= dy)
            {
                error += dy;
                x0 += sx;
            }
            if (twiceError <= dx)
            {
                error += dx;
                y0 += sy;
            }
        }
    }

    private static byte[] EncodePng(int width, int height, byte[] pixels)
    {
        using var output = new MemoryStream();
        output.Write(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 });
        var header = new byte[13];
        WriteBigEndian(header, 0, width);
        WriteBigEndian(header, 4, height);
        header[8] = 8;
        header[9] = 2;
        WriteChunk(output, "IHDR", header);

        using var raw = new MemoryStream();
        for (var row = 0; row < height; row++)
        {
            raw.WriteByte(0);
            raw.Write(pixels, row * width * 3, width * 3);
        }
        using var compressed = new MemoryStream();
        using (var compressor = new ZLibStream(compressed, CompressionLevel.Optimal, leaveOpen: true))
            compressor.Write(raw.ToArray());
        WriteChunk(output, "IDAT", compressed.ToArray());
        WriteChunk(output, "IEND", Array.Empty<byte>());
        return output.ToArray();
    }

    private static void WriteChunk(Stream output, string type, byte[] data)
    {
        Span<byte> length = stackalloc byte[4];
        WriteBigEndian(length, (uint)data.Length);
        output.Write(length);
        var typeBytes = Encoding.ASCII.GetBytes(type);
        output.Write(typeBytes);
        output.Write(data);
        var crcData = new byte[typeBytes.Length + data.Length];
        Buffer.BlockCopy(typeBytes, 0, crcData, 0, typeBytes.Length);
        Buffer.BlockCopy(data, 0, crcData, typeBytes.Length, data.Length);
        Span<byte> crc = stackalloc byte[4];
        WriteBigEndian(crc, CalculateCrc32(crcData));
        output.Write(crc);
    }

    private static uint CalculateCrc32(byte[] data)
    {
        var crc = 0xffffffffu;
        foreach (var value in data)
        {
            crc ^= value;
            for (var bit = 0; bit < 8; bit++)
                crc = (crc & 1) == 1 ? 0xedb88320u ^ (crc >> 1) : crc >> 1;
        }
        return ~crc;
    }

    private static void WriteBigEndian(byte[] destination, int offset, int value)
    {
        destination[offset] = (byte)(value >> 24);
        destination[offset + 1] = (byte)(value >> 16);
        destination[offset + 2] = (byte)(value >> 8);
        destination[offset + 3] = (byte)value;
    }

    private static void WriteBigEndian(Span<byte> destination, uint value)
    {
        destination[0] = (byte)(value >> 24);
        destination[1] = (byte)(value >> 16);
        destination[2] = (byte)(value >> 8);
        destination[3] = (byte)value;
    }
}
