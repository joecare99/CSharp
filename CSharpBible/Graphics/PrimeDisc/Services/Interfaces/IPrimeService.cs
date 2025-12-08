namespace PrimePlotter.Services.Interfaces;

/// <summary>
/// Stellt Dienste zur Ermittlung von Primzahlen bereit.
/// </summary>
/// <remarks>
/// Diese Schnittstelle abstrahiert die Generierung der ersten <c>n</c> Primzahlen und unterstützt:
/// <list type="bullet">
/// <item><description>Fortschrittsberichte über <see cref="IProgress{T}"/> (Prozentwert 0.0–100.0).</description></item>
/// <item><description>Kooperative Unterbrechung mittels <see cref="CancellationToken"/>.</description></item>
/// <item><description>Deterministische, geordnete Ausgabe (streng aufsteigende Primzahlen).</description></item>
/// </list>
/// Implementierungen sollten effizient sein (z. B. optimiertes Sieb oder inkrementelle Prüfung) und Ressourcen schonend arbeiten.
/// </remarks>
public interface IPrimeService
{
    /// <summary>
    /// Ermittelt die ersten <paramref name="n"/> Primzahlen in aufsteigender Reihenfolge.
    /// </summary>
    /// <param name="n">Anzahl der gewünschten Primzahlen. Muss größer oder gleich 0 sein. Bei <c>0</c> wird eine leere Liste zurückgegeben.</param>
    /// <param name="progress">
    /// Optionaler Fortschritts-Callback. Übergibt einen Prozentwert (0.0 bis 100.0), der den bereits berechneten Anteil widerspiegelt.
    /// Der letzte Aufruf sollte 100.0 melden, sofern nicht vorher abgebrochen wurde.
    /// </param>
    /// <param name="ct">
    /// Optionaler <see cref="CancellationToken"/> zur kooperativen Unterbrechung. Bei Abbruch sollte eine
    /// <see cref="OperationCanceledException"/> ausgelöst oder (alternativ) eine teilweise Ergebnisliste zurückgegeben werden –
    /// dies muss in der Implementierungsdokumentation eindeutig festgehalten werden.
    /// </param>
    /// <returns>
    /// Eine neue <see cref="List{T}"/> mit genau <paramref name="n"/> Primzahlen (beginnend bei 2), sortiert in streng aufsteigender Reihenfolge.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="n"/> ist negativ.</exception>
    /// <exception cref="OperationCanceledException">Der Vorgang wurde über <paramref name="ct"/> abgebrochen (falls Implementierung dies so handhabt).</exception>
    /// <remarks>
    /// Leistungsaspekte:
    /// <list type="bullet">
    /// <item><description>Für große <paramref name="n"/> ist ein segmentiertes Sieb typischerweise Speicher-effizienter als ein einfaches Sieb.</description></item>
    /// <item><description>Fortschritt kann auf Basis der Anzahl gefundener Primzahlen oder des abgearbeiteten Bereiches berechnet werden.</description></item>
    /// </list>
    /// Thread-Sicherheit:
    /// <list type="bullet">
    /// <item><description>Die Methode selbst sollte keine geteilten mutierbaren Zustände voraussetzen. Parallelisierung ist optional.</description></item>
    /// </list>
    /// Fehlerverhalten:
    /// <list type="bullet">
    /// <item><description>Bei ungültigen Parametern (z. B. <paramref name="n"/> &lt; 0) wird eine Ausnahme ausgelöst.</description></item>
    /// <item><description>Bei Abbruch über <paramref name="ct"/> erfolgt entweder <see cref="OperationCanceledException"/> oder Teilergebnis – Implementierungsspezifik.</description></item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code language="csharp">
    /// IPrimeService service = GetPrimeService();
    /// var primes = service.FirstNPrimes(10);
    /// // Ergebnis: [2, 3, 5, 7, 11, 13, 17, 19, 23, 29]
    /// </code>
    /// <code language="csharp">
    /// var progress = new Progress&lt;double&gt;(p =&gt; Console.WriteLine($"Fortschritt: {p:F2}%"));
    /// using var cts = new CancellationTokenSource();
    /// try
    /// {
    ///     var primes = service.FirstNPrimes(100_000, progress, cts.Token);
    /// }
    /// catch (OperationCanceledException)
    /// {
    ///     Console.WriteLine("Berechnung abgebrochen.");
    /// }
    /// </code>
    /// </example>
    List<int> FirstNPrimes(int n, IProgress<double>? progress = null, CancellationToken ct = default);
}