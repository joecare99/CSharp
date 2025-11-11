using PrimePlotter.Services.Interfaces;

namespace PrimePlotter.Services;

/// <summary>
/// Statische Helferklasse zum Rendern einer Primzahlen-Spirale auf einen ARGB-Pixelpuffer.
/// </summary>
/// <remarks>
/// Darstellung:
/// Die Primzahlen werden auf einer modifizierten Spirale (ähnlich einer Fermat-/Archimedes-Spirale)
/// positioniert, wobei für jede Primzahl p gilt:
/// <code>
/// r     = sqrt(p)
/// theta = PI * sqrt(p)
/// x     = cx + scale * r * cos(theta)
/// y     = cy - scale * r * sin(theta)  (negatives Vorzeichen für Bildschirm-Koordinaten nach unten)
/// </code>
/// Der Radius wächst also proportional zur Quadratwurzel der Primzahl. Dadurch entsteht eine relativ
/// gleichmäßige, aber dennoch strukturierte Verteilung der Punkte. Die Farb-Codierung ist aktuell
/// monochrom (Schwarz/Weiß bzw. Graustufen beim Downsampling).
///
/// Thread-Sicherheit:
/// Die Klasse ist rein funktional (keine Felder / kein Zustand) und damit threadsicher.
///
/// Performance-Hinweise:
/// Die Routinen sind O(n) bezüglich der Anzahl der Primzahlen. Für sehr große Mengen empfiehlt sich,
/// das Downsampling-Verfahren zu benutzen, da es visuelle Dichte besser wiedergibt.
/// </remarks>
/// <example>
/// Einfache Verwendung:
/// <code>
/// var primes = new List&lt;int&gt; {2,3,5,7,11,13,17,19};
/// int[] argb = PlotService.PlotPoints(primes, 800, 800);
/// // argb enthält jetzt 800*800 Pixel in ARGB-Form
/// </code>
/// Mit Fortschritt und Abbruch:
/// <code>
/// var cts = new CancellationTokenSource();
/// var progress = new Progress&lt;double&gt;(p =&gt; Console.WriteLine($"{p:F1}%"));
/// int[] argb = PlotService.PlotPoints(primes, 1920, 1080, progress, cts.Token);
/// </code>
/// </example>
public class PlotService : IPlotService
{
    /// <summary>
    /// Rendert eine Punktdarstellung der Primzahlen auf einen ARGB-Puffer (Schwarz = Hintergrund, Weiß = Primzahlpunkt).
    /// </summary>
    /// <param name="primes">Aufsteigende Liste von Primzahlen (die letzte bestimmt den maximalen Radius).</param>
    /// <param name="width">Breite des Zielbildes in Pixeln (&gt; 0).</param>
    /// <param name="height">Höhe des Zielbildes in Pixeln (&gt; 0).</param>
    /// <param name="progress">
    /// Optionaler Fortschritts-Reporter (Wertebereich: zuerst 50..100%). Die Initialisierung der Skalen (0..50%) kann extern erfolgen.
    /// </param>
    /// <param name="ct">Abbruch-Token zur frühzeitigen Unterbrechung der Berechnung.</param>
    /// <returns>
    /// Ein eindimensionales int-Array der Länge <c>width * height</c> im ARGB-Format (0xAARRGGBB), zeilenweise von oben nach unten.
    /// </returns>
    /// <exception cref="OperationCanceledException">Geworfen, wenn der Abbruch via <paramref name="ct"/> ausgelöst wurde.</exception>
    /// <remarks>
    /// Skalierungslogik:
    /// Der größte Radius wird aus der Quadratwurzel der letzten Primzahl bestimmt und so normiert,
    /// dass die Darstellung nahezu (96%) das kleinere Seitenmaß ausfüllt.
    ///
    /// Fortschritt:
    /// Alle ~16384 Punkte (Bitmaske 0x3FFF) wird ein neuer Prozentsatz berechnet. Startwert ist 50%, Endwert 100%.
    /// </remarks>
    /// <example>
    /// <code>
    /// var primes = PrimeGenerator.UpTo(1_000_000);
    /// int[] pixels = PlotService.PlotPoints(primes, 1024, 1024);
    /// </code>
    /// </example>
    public int[] PlotPoints(
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

    /// <summary>
    /// Rendert eine glättete (Downsampling) Primzahl-Spirale mit bilinear verteilter Punktgewichtung zur Erzeugung von Graustufen.
    /// </summary>
    /// <param name="primes">Aufsteigende Liste von Primzahlen (leer => vollständig schwarzes Bild).</param>
    /// <param name="width">Zielbreite des resultierenden Puffers.</param>
    /// <param name="height">Zielhöhe des resultierenden Puffers.</param>
    /// <param name="scaleFactor">
    /// Downsampling-Faktor (&gt;= 1). Intern wird ein <c>width * scaleFactor</c> x <c>height * scaleFactor</c> großer Float-Puffer aufgebaut.
    /// </param>
    /// <param name="progress">Optionaler Fortschritts-Reporter (Werte 50..100%).</param>
    /// <param name="ct">Abbruch-Token zur Unterbrechung.</param>
    /// <returns>
    /// ARGB-Pixelpuffer in der Zielgröße. Jeder Kanal (R,G,B) enthält die identische Intensität (Graustufen),
    /// Alpha ist stets 0xFF.
    /// </returns>
    /// <exception cref="OperationCanceledException">Abbruch während der Berechnung.</exception>
    /// <remarks>
    /// Algorithmus:
    /// 1. Erster Pass: Für jede Primzahl wird ihre Position im vergrößerten Puffer berechnet und
    ///    bilinear auf bis zu vier Nachbarzellen verteilt (weiche Verteilung).
    /// 2. Zweiter Pass: Aggregation der <paramref name="scaleFactor"/>^2 Subpixel in einen Zielpixel
    ///    mittels einfacher Summation und Clamping auf 0..255 nach Multiplikation mit 255.
    ///
    /// Intensitätsberechnung:
    /// Die Summe der Gewichte wird auf den Bereich [0..255] skaliert. Für hohe Punktdichten kann eine
    /// alternative Normalisierung oder logarithmische Skalierung sinnvoll sein (Erweiterungspotenzial).
    /// </remarks>
    /// <example>
    /// <code>
    /// var primes = PrimeGenerator.UpTo(500_000);
    /// int[] smooth = PlotService.PlotPointsDownsampled(primes, 800, 800, scaleFactor: 4);
    /// </code>
    /// </example>
    public int[] PlotPointsDownsampled(
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

        // Downsampling: pro Pixel der Zielgröße Mittelwert / Summe bilden
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
                int intensity = Math.Min(255, (int)Math.Round(sum * 255)); // hier einfach clamp
                smallBuffer[y * width + x] = unchecked((int)(0xFF000000 | (intensity << 16) | (intensity << 8) | intensity));
            }
        }

        return smallBuffer;
    }
}
