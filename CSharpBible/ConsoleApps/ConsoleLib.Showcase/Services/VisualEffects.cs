using System;

namespace ConsoleLib.Showcase.Services;

/// <summary>Generates deterministic text frames used by the gallery animation.</summary>
public sealed class VisualEffects
{
    private static readonly char[] Ramp = { ' ', '░', '▒', '▓', '█' };

    public string CreateWaveFrame(int frame, int width)
    {
        if (width < 1)
            return string.Empty;

        var result = new char[width];
        for (var index = 0; index < width; index++)
        {
            var phase = Math.Sin((index + frame) * 0.35);
            var rampIndex = (int)Math.Round((phase + 1) * 2);
            result[index] = Ramp[Math.Clamp(rampIndex, 0, Ramp.Length - 1)];
        }

        return new string(result);
    }

    public string CreateProgressFrame(double fraction, int width)
    {
        if (width < 1)
            return string.Empty;

        var filled = (int)Math.Round(Math.Clamp(fraction, 0, 1) * width);
        return new string('█', filled) + new string('░', width - filled);
    }
}
