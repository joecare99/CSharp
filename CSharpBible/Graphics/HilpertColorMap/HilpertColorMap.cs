// Sie müssen das NuGet-Paket "System.Drawing.Common" zu Ihrem Projekt hinzufügen, um Bitmap und andere Typen aus System.Drawing zu verwenden.
// Beispiel (in Visual Studio): Rechtsklick auf das Projekt > NuGet-Pakete verwalten > System.Drawing.Common suchen und installieren.

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

// Stellen Sie sicher, dass Ihr Projekt einen Verweis auf das NuGet-Paket "System.Drawing.Common" hat.
// In Visual Studio: Rechtsklick auf das Projekt > NuGet-Pakete verwalten > System.Drawing.Common suchen und installieren.

class HilbertColorMap
{
    // Die Hilfsfunktion zur Rotation und Spiegelung, übersetzt aus Python
    private static void Rot(int n, ref int x, ref int y, int rx, int ry)
    {
        if (ry == 0)
        {
            if (rx == 1)
            {
                x = n - 1 - x;
                y = n - 1 - y;
            }
            // Tausche x und y
            int temp = x;
            x = y;
            y = temp;
        }
    }

    // Koordinate (x, y) zu Distanz (d) auf der Hilbert-Kurve abbilden
    private static int Xy2d(int n, int x, int y)
    {
        int d = 0;
        int s = n / 2;
        while (s > 0)
        {
            int rx = (x & s) > 0 ? 1 : 0;
            int ry = (y & s) > 0 ? 1 : 0;
            d += s * s * ((3 * rx) ^ ry);
            Rot(s, ref x, ref y, rx, ry);
            s /= 2;
        }
        return d;
    }



    // Hilfsfunktion zur Konvertierung von HSV zu RGB (C# hat keine direkte eingebaute HSV-Unterstützung in System.Drawing)
    private static Color ColorFromHSV(double hue, double saturation, double value)
    {
        int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
        double f = hue / 60 - Math.Floor(hue / 60);

        value = value * 255;
        int v = Convert.ToInt32(value);
        int p = Convert.ToInt32(value * (1 - saturation));
        int q = Convert.ToInt32(value * (1 - f * saturation));
        int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

        if (hi == 0) return Color.FromArgb(255, v, t, p);
        if (hi == 1) return Color.FromArgb(255, q, v, p);
        if (hi == 2) return Color.FromArgb(255, p, v, t);
        if (hi == 3) return Color.FromArgb(255, p, q, v);
        if (hi == 4) return Color.FromArgb(255, t, p, v);
        if (hi == 5) return Color.FromArgb(255, v, p, q);
        return Color.FromArgb(255, 0, 0, 0); // Fehlerfarbe
    }

    private static Bitmap Map3DTo2DColor(int sizeExponent, int i, int j)
    {
        int n = (int)Math.Pow(2, sizeExponent);
        int totalPixels = n * n;
        Bitmap img = new Bitmap(n, n, PixelFormat.Format24bppRgb);

        for (int x = 0; x < n; x++)
        {
            for (int y = 0; y < n; y++)
            {
                int distance = Xy2d(n, x, y);
                double normDist = (double)distance / (totalPixels - 1);
                var r = GetCubeCoordinateFromDistance(normDist, i);
                // HSV Werte zuweisen
                // Hue: 0 bis 360 Grad
                // Saturation: 1.0 (voll)
                // Value (Helligkeit): 1.0 (voll)
                var (hue, saturation, value) = j switch
                {
                    1 => (r.X, r.Z, r.Y),
                    2 => (r.Y, r.X, r.Z),
                    3 => (r.Z, r.Y, r.X),
                    4 => (r.X, r.Y, r.Z),
                    5 => (r.Y, r.Z, r.X),
                    _ => (r.Z, r.X, r.Y),
                };
                Color pixelColor = Color.FromArgb(
                    (int)(hue * 255),
                    (int)(saturation * 255),
                    (int)(value * 255)
                );
                //     Color pixelColor = ColorFromHSV(hue*360.0, saturation, value);
                img.SetPixel(x, y, pixelColor);
            }
        }
        return img;
    }

    /// <summary>
    /// Wandelt eine gegebene Distanz (0.0 - 1.0) in eine normalisierte 3D-Koordinate (x,y,z) im Einheitswürfel um.
    /// Verwendet dazu eine Morton-/Z-Order-Abbildung (Bit-Interleaving) für gleichmäßige Raumabdeckung.
    /// </summary>
    /// <param name="distance">Distanz entlang der linearen Sequenz (0.0 bis 1.0).</param>
    /// <param name="order">Auflösungsexponent (n = 2^order Punkte pro Achse).</param>
    /// <returns>Tuple (x,y,z) mit Werten im Bereich 0.0 - 1.0.</returns>
    public static (double X, double Y, double Z) GetCubeCoordinateFromDistance(double distance, int order)
    {
        if (order < 1 || order > 20)
        {
            throw new ArgumentOutOfRangeException(nameof(order), "order muss zwischen 1 und 20 liegen.");
        }

        distance = Math.Clamp(distance, 0d, 1d);

        int n = 1 << order; // Punkte pro Achse
        long totalPoints = 1L << (3 * order); // Gesamtanzahl entlang der 3D-Hilbert-Kurve
        double scaledIndex = distance * (totalPoints - 1);
        long index = (long)Math.Floor(scaledIndex);
        double frac = scaledIndex - index;

        var (xi, yi, zi) = HilbertIndexToCoord3D(order, index);
        var (xi1, yi1, zi1) = HilbertIndexToCoord3D(order, index + 1L);

        double denom = n - 1d;
        return ((xi * (1 - frac) + xi1 * frac) / denom,
            (yi * (1 - frac) + yi1 * frac) / denom,
            (zi * (1 - frac) + zi1 * frac) / denom);
    }

    // 3D-Hilbert: Index -> (x,y,z) nach Skilling.
    // Diese Version stellt sicher, dass aufeinanderfolgende Indizes nur in genau einer Dimension um 1 differieren.
    // Pseudocode:
    // 1. Transponiere den Hilbert-Index h in D=3 Bitspalten: x[0..2] (je 'order' Bits).
    //    Für Bit-Ebene b (0..order-1): hole 3 Bits aus h und schreibe sie in das b-te Bit jeder Achse.
    // 2. Gray-Decode der transponierten Form:
    //    t = x[D-1] >> 1
    //    Für i von D-1 bis 1: x[i] ^= x[i-1]
    //    x[0] ^= t
    // 3. Entwirre Rotationen/Reflexionen gemäß Skilling:
    //    Für q = 2; q < 1<<order; q <<= 1:
    //       p = q - 1
    //       Für i = D-1 .. 0:
    //          Wenn (x[i] & q) != 0: x[0] ^= p
    //          Sonst:
    //             t = (x[0] ^ x[i]) & p
    //             x[0] ^= t
    //             x[i] ^= t
    // 4. Ergebnis sind die kartesischen Koordinaten im Bereich [0, 2^order - 1].
    private static (int X, int Y, int Z) HilbertIndexToCoord3D(int order, long index)
    {
        const int nDim = 3;
        int[] x = new int[nDim];

        // Schritt 1: Transponiere Index -> Bitspalten
        // Wir lesen die Bits in Gruppen zu je nDim (höherwertige Gruppen zuerst).
        for (int bit = 0; bit < order; bit++)
        {
            int group = 0;
            // Hole die nDim Bits für diese Ebene (von oben nach unten).
            for (int d = 0; d < nDim; d++)
            {
                long shift = (long)bit * nDim + (nDim - 1 - d);
                group |= (int)(((index >> (byte)shift) & 1) << d);
            }

            // Schreibe Bits in die jeweiligen Achsen (bit-Position 'bit').
            for (int d = 0; d < nDim; d++)
            {
                int b = (group >> d) & 1;
                if (b == 1)
                {
                    x[d] |= (1 << bit);
                }
            }
        }

        // Schritt 2: Gray-Decode
        int tGray = x[nDim - 1] >> 1;
        for (int i = nDim - 1; i > 0; i--)
        {
            x[i] ^= x[i - 1];
        }
        x[0] ^= tGray;

        // Schritt 3: Rotationen/Reflexionen rückgängig machen
        for (int q = 2; q < (1 << order); q <<= 1)
        {
            int p = q - 1;
            for (int i = nDim - 1; i >= 0; i--)
            {
                if ((x[i] & q) != 0)
                {
                    x[0] ^= p; // reflektiere
                }
                else
                {
                    int t = (x[0] ^ x[i]) & p; // rotiere
                    x[0] ^= t;
                    x[i] ^= t;
                }
            }
        }

        return (x[0], x[1], x[2]);
    }

    static void Main(string[] args)
    {
        // Kantenlänge des Bildes wählen (2 hoch 10 = 1024 Pixel)
        int exponent = 11;
        var PictureDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "HilbertColorMaps");
        Directory.CreateDirectory(PictureDir);
        for (int i = 1; i < 7; i++)
        {
            for (int j = 1; j < 7; j++)
            {
                Bitmap resultImage = Map3DTo2DColor(exponent, i, j);
                int size = (int)Math.Pow(2, exponent);
                string filename = $"Hilbert_Color_Map_RGB_{size}_{j}{" abcdefg"[i]}.png";

                // Bild speichern
                try
                {
                    resultImage.Save(Path.Combine(PictureDir, filename), ImageFormat.Png);
                    Console.WriteLine($"Bild wurde als '{filename}' gespeichert.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fehler beim Speichern des Bildes: {ex.Message}");
                }
            }
        }
    }
}
