using System;
using System.Collections.Generic;
using System.Text;

namespace Cifar10.WPF.Model;


public static class PolarHS
{
    // Encode: RGB (0..1) -> (h, s)
    // Luminance (l) wird zurückgegeben, kann als Metadatum gespeichert werden.
    public static void EncodeRgbToHs(float r, float g, float b, 
        out float h, out float s, out float l)
    {
        // Clamp inputs
        r = Clamp01(r); g = Clamp01(g); b = Clamp01(b);

        float max = MathF.Max(r, MathF.Max(g, b));
        float min = MathF.Min(r, MathF.Min(g, b));
        l = (max + min) * 0.5f; // Lightness (HSL)

        if (max == min)
        {
            // achromatisch: kein Farbton, S = 0
            h = 0f;
            s = 0f;
            return;
        }

        float delta = max - min;

        // Saturation (HSL)
        if (l <= 0.5f)
            s = delta / (max + min);
        else
            s = delta / (2f - max - min);

        // Hue calculation (0..1)
        float hue;
        if (max == r)
            hue = (g - b) / delta + (g < b ? 6f : 0f);
        else if (max == g)
            hue = (b - r) / delta + 2f;
        else
            hue = (r - g) / delta + 4f;

        h = hue / 6f; // normalize to [0,1)
        h = h - MathF.Floor(h); // ensure in [0,1)
    }

    // Decode: (h, s, l) -> RGB (0..1)
    public static void DecodeHsToRgb(float h, float s, float l, 
        out float r, out float g, out float b)
    {
        h = Mod01(h);
        s = Clamp01(s);
        l = Clamp01(l);

        if (s == 0f)
        {
            // achromatisch
            r = g = b = l;
            return;
        }

        float q = l < 0.5f ? l * (1f + s) : l + s - l * s;
        float p = 2f * l - q;

        r = HueToRgb(p, q, h + 1f / 3f);
        g = HueToRgb(p, q, h);
        b = HueToRgb(p, q, h - 1f / 3f);
    }

    // Hilfsfunktionen
    private static float HueToRgb(float p, float q, float t)
    {
        t = Mod01(t);
        if (t < 1f / 6f) return p + (q - p) * 6f * t;
        if (t < 1f / 2f) return q;
        if (t < 2f / 3f) return p + (q - p) * (2f / 3f - t) * 6f;
        return p;
    }

    private static float Clamp01(float v) => v < 0f ? 0f : (v > 1f ? 1f : v);
    private static float Mod01(float v)
    {
        v = v % 1f;
        if (v < 0f) v += 1f;
        return v;
    }
}
