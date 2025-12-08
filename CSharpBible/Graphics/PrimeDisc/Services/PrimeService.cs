// Services/PrimeService.cs
using PrimePlotter.Services.Interfaces;
using System;
using System.Collections.Generic;

namespace PrimePlotter.Services;

public class PrimeService : IPrimeService
{
    /// <summary>
    /// Ermittelt die ersten <paramref name="n"/> Primzahlen in aufsteigender Reihenfolge.
    /// </summary>
    /// <param name="n">Anzahl der gewünschten Primzahlen (falls kleiner oder gleich 0: leere Liste).</param>
    /// <param name="progress">
    /// Optionaler Fortschritts-Reporter (0.0–50.0). Die Methode meldet:
    ///  - Groben Fortschritt (0–40%) während der Eliminierung von Vielfachen im Sieb (äußere <c>p</c>-Schleife).
    ///  - Verfeinerung (40–50%) während des Sammelns der Primzahlen aus dem fertigen Sieb.
    /// Ein Endwert von 50.0 signalisiert Abschluss. Werte >50% sind hier absichtlich nicht genutzt, um ggf.
    /// nachgelagerte Verarbeitung (Visualisierung etc.) separat abzubilden.
    /// </param>
    /// <param name="ct">
    /// Optionales Cancellation Token zur vorzeitigen Unterbrechung. Bei Auslösung wird
    /// <see cref="OperationCanceledException"/> geworfen, sobald der nächste Abbruchpunkt erreicht ist.
    /// </param>
    /// <returns>Liste der ersten <paramref name="n"/> Primzahlen (leer, wenn <paramref name="n"/> &lt;= 0).</returns>
    /// <remarks>
    /// <para>
    /// Algorithmus:
    ///  1. Behandlung kleiner Grenzfälle (n ≤ 6) durch direkte Rückgabe einer vordefinierten Liste.
    ///  2. Abschätzung einer oberen Schranke für die n-te Primzahl nach Dusart zur Bestimmung der Siebgröße.
    ///  3. Initialisierung eines booleschen Arrays (<c>true</c> = potentiell prim).
    ///  4. Klassisches Sieb des Eratosthenes: Streichen der Vielfachen jedes gefundenen Primkandidaten p ab p².
    ///  5. Sammeln der ersten n Primzahlen durch lineares Durchlaufen des Siebs.
    /// </para>
    /// <para>
    /// Komplexität:
    ///  - Zeit: O(L log log L) für das Sieben, wobei L die berechnete obere Schranke ist.
    ///  - Speicher: O(L) für das boolesche Sieb.
    /// Die Dusart-Schätzung führt zu einer leichten Überdimensionierung, reduziert aber die Gefahr,
    /// das Sieb zu klein zu wählen und neu allokieren zu müssen.
    /// </para>
    /// <para>
    /// Thread-Sicherheit: Die Methode erzeugt ausschließlich lokale Datenstrukturen und ist daher
    /// reentrant und nebenläufig sicher (sofern <paramref name="progress"/> selbst thread-sicher
    /// implementiert ist).
    /// </para>
    /// <para>
    /// Fortschritt: Der Fortschritt endet bewusst bei 50%, um weiteren nachgelagerten Verarbeitungsschritten
    /// (z.B. grafische Darstellung) eigenen Fortschrittsspielraum zu lassen.
    /// </para>
    /// </remarks>
    /// <exception cref="OperationCanceledException">
    /// Falls das übergebene <paramref name="ct"/> während der Berechnung abgebrochen wird.
    /// </exception>
    /// <example>
    /// <code>
    /// // Einfache Nutzung:
    /// var primes = PrimeService.FirstNPrimes(10);
    /// // Ergebnis: [2,3,5,7,11,13,17,19,23,29]
    ///
    /// // Mit Fortschritts-Reporting und Abbruch:
    /// var cts = new System.Threading.CancellationTokenSource();
    /// var progress = new Progress&lt;double&gt;(p =&gt; Console.WriteLine($"Fortschritt: {p:F1}%"));
    /// try
    /// {
    ///     var firstThousand = PrimeService.FirstNPrimes(1000, progress, cts.Token);
    /// }
    /// catch (OperationCanceledException)
    /// {
    ///     Console.WriteLine("Berechnung abgebrochen.");
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="System.IProgress{T}"/>
    public List<int> FirstNPrimes(int n, IProgress<double>? progress = null, System.Threading.CancellationToken ct = default)
    {
        if (n <= 0) return new List<int>();
        if (n <= 6) // kleine Grenzfälle direkt
        {
            var small = new List<int> { 2, 3, 5, 7, 11, 13 };
            return small.GetRange(0, n);
        }

        // obere Schranke für n-te Primzahl (Dusart): n(ln n + ln ln n) + 3
        double nd = n;
        int limit = (int)Math.Ceiling(nd * (Math.Log(nd) + Math.Log(Math.Log(nd))) + 3);
        var sieve = new bool[limit + 1];
        var primes = new List<int>(n);

        int max = (int)Math.Sqrt(limit);
        for (int i = 2; i <= limit; i++) sieve[i] = true;

        int reported = 0;
        for (int p = 2; p <= max; p++)
        {
            ct.ThrowIfCancellationRequested();
            if (!sieve[p]) continue;
            for (int m = p * p; m <= limit; m += p) sieve[m] = false;

            // grobe Fortschrittsmeldung über die p-Schleife (0..40%)
            int pct = (int)(40.0 * (p - 2) / Math.Max(1, max - 2));
            if (pct != reported) { progress?.Report(pct); reported = pct; }
        }

        for (int i = 2; i <= limit && primes.Count < n; i++)
        {
            ct.ThrowIfCancellationRequested();
            if (sieve[i]) primes.Add(i);

            // Rest-Fortschritt bis 50%
            if (i % 1000 == 0)
            {
                double frac = Math.Min(1.0, primes.Count / (double)n);
                progress?.Report(40.0 + 10.0 * frac);
            }
        }
        progress?.Report(50.0);
        return primes;
    }
}
