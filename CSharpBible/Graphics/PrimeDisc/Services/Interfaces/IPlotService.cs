namespace PrimePlotter.Services.Interfaces;

/// <summary>
/// Stellt Plot-Funktionalität für Primzahl-Punktwolken bereit, inklusive optionaler
/// Downsampling-Strategien zur Reduzierung der Anzahl zu rendernder Punkte bei großen Datenmengen.
/// </summary>
/// <remarks>
/// Diese Schnittstelle kapselt die Berechnung von Bildschirm-/Canvas-Koordinaten aus einer
/// übergebenen Folge von Primzahlen. Implementierungen können verschiedene Projektions- oder
/// Layout-Verfahren (z. B. Polar-, Spiral-, Gitter- oder benutzerdefinierte Anordnungen) nutzen.
/// <para>
/// Der Fokus der Methode <see cref="PlotPointsDownsampled"/> liegt auf Performance bei sehr großen
/// Primlisten: Durch einen <c>scaleFactor</c> kann eine grobe Rasterung / Downsampling durchgeführt
/// werden, um die resultierende Punktmenge zu reduzieren (z. B. für Vorschau- oder Echtzeitdarstellung).
/// </para>
/// <para>
/// Rückgabewert: Ein flaches eindimensionales Array von Ganzzahlen, das die berechneten Punkte
/// als interleavte X-/Y-Koordinaten enthält: Index 0 = X0, Index 1 = Y0, Index 2 = X1, Index 3 = Y1, usw.
/// Dieses Format vermeidet Objekt-Overhead (z. B. von Tupeln oder Strukturen) und ist GC-freundlich.
/// </para>
/// <para>
/// Thread-Sicherheit: Instanzen müssen nicht zwingend thread-sicher sein; parallele Nutzung sollte
/// nur erfolgen, wenn die konkrete Implementierung dies explizit dokumentiert.
/// </para>
/// </remarks>
public interface IPlotService
{
    /// <summary>
    /// Berechnet Bildschirm-/Raster-Koordinaten für eine Menge von Primzahlen und führt dabei
    /// ein Downsampling anhand eines Skalierungsfaktors durch, um die resultierende Punktmenge
    /// zu verringern oder zu verdichten.
    /// </summary>
    /// <param name="primes">
    /// Sortierte oder unsortierte Liste von Primzahlen, die geplottet werden sollen. Die Interpretation
    /// (z. B. Reihenfolge für eine Spiral-/Sequenzprojektion) liegt bei der Implementierung. Muss
    /// nicht leer sein; bei leerer Liste wird ein leeres Array zurückgegeben.
    /// </param>
    /// <param name="width">
    /// Zielbreite der Zeichenfläche (in Pixeln). Muss &gt; 0 sein. Wird zur Begrenzung / Normalisierung
    /// berechneter X-Koordinaten genutzt.
    /// </param>
    /// <param name="height">
    /// Zielhöhe der Zeichenfläche (in Pixeln). Muss &gt; 0 sein. Wird zur Begrenzung / Normalisierung
    /// berechneter Y-Koordinaten genutzt.
    /// </param>
    /// <param name="scaleFactor">
    /// Downsampling- bzw. Verdichtungsfaktor. Größere Werte reduzieren typischerweise die Anzahl
    /// der zurückgegebenen Punkte, indem z. B. nur jede n-te Primzahl oder aggregierte Cluster
    /// berücksichtigt werden. Muss &gt; 0 sein; Werte von 1 bedeuten kein Downsampling.
    /// </param>
    /// <param name="progress">
    /// Optionaler Fortschritts-Callback (0.0–100.0 oder 0.0–1.0 je nach Implementierungskonvention),
    /// über den Zwischenschritte gemeldet werden können (z. B. bei sehr großen Eingaben). Kann null sein.
    /// </param>
    /// <param name="ct">
    /// <see cref="CancellationToken"/> zur vorzeitigen Unterbrechung der Berechnung. Wird regelmäßig
    /// geprüft; bei Auslösung sollte eine <see cref="OperationCanceledException"/> geworfen werden.
    /// </param>
    /// <returns>
    /// Eindimensionales int-Array mit interleavten X-/Y-Koordinaten der resultierenden (ggf. reduzierten)
    /// Punktmenge. Die Länge ist immer gerade (2 * Anzahl Punkte). Kann leer sein, aber niemals null.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Wird ausgelöst, wenn <paramref name="width"/>, <paramref name="height"/> oder
    /// <paramref name="scaleFactor"/> ungültige (nicht positive) Werte besitzen.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// Wenn die Operation über <paramref name="ct"/> abgebrochen wurde.
    /// </exception>
    /// <remarks>
    /// Performance-Hinweise:
    /// <list type="bullet">
    /// <item>Implementierungen sollten soweit möglich auf Allocation-Minimierung achten.</item>
    /// <item>Zwischenpuffer können aus Array-Pools bezogen und nach Nutzung zurückgegeben werden.</item>
    /// <item>Bei sehr großen Eingaben empfiehlt sich eine Vektorisierung oder Parallelisierung,
    /// solange deterministische Reihenfolgen nicht erforderlich sind.</item>
    /// </list>
    /// </remarks>
    int[] PlotPointsDownsampled(
        IReadOnlyList<int> primes,
        int width,
        int height,
        int scaleFactor,
        IProgress<double>? progress = null,
        CancellationToken ct = default);
}