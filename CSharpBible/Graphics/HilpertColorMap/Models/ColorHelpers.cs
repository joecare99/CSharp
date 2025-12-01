// Sie müssen das NuGet-Paket "System.Drawing.Common" zu Ihrem Projekt hinzufügen, um Bitmap und andere Typen aus System.Drawing zu verwenden.
// Beispiel (in Visual Studio): Rechtsklick auf das Projekt > NuGet-Pakete verwalten > System.Drawing.Common suchen und installieren.

public static class ColorHelpers
{
    
    // Hilfsfunktion zur Konvertierung von HSV zu RGB (C# hat keine direkte eingebaute HSV-Unterstützung in System.Drawing)
    /// <summary>
    /// Converts a color from HSV (hue, saturation, value) color space to an equivalent RGB color represented as a
    /// System.Drawing.Color object.
    /// </summary>
    /// <remarks>If any parameter is outside its valid range, the resulting color may not be meaningful. The
    /// alpha channel of the returned color is always set to 255 (fully opaque).</remarks>
    /// <param name="hue">The hue component of the color, in degrees. Valid values are from 0 to 360.</param>
    /// <param name="saturation">The saturation component of the color, as a value between 0 and 1. A value of 0 produces a shade of gray, and 1
    /// produces the most vivid color.</param>
    /// <param name="value">The value (brightness) component of the color, as a value between 0 and 1. A value of 0 results in black, and 1
    /// results in the brightest color.</param>
    /// <returns>A Color structure representing the equivalent RGB color with full opacity (alpha = 255).</returns>
    public static Color ColorFromHSV(double hue, double saturation, double value)
    {
        int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
        double f = hue / 60 - Math.Floor(hue / 60);

        value = value * 255;
        int v = Convert.ToInt32(value);
        int p = Convert.ToInt32(value * (1 - saturation));
        int q = Convert.ToInt32(value * (1 - f * saturation));
        int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

        if (hi == 0)
            return Color.FromArgb(255, v, t, p);
        if (hi == 1)
            return Color.FromArgb(255, q, v, p);
        if (hi == 2)
            return Color.FromArgb(255, p, v, t);
        if (hi == 3)
            return Color.FromArgb(255, p, q, v);
        if (hi == 4)
            return Color.FromArgb(255, t, p, v);
        if (hi == 5)
            return Color.FromArgb(255, v, p, q);
        return Color.FromArgb(255, 0, 0, 0); // Fehlerfarbe
    }
}