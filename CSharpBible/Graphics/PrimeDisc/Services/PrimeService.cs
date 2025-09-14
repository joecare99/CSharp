// Services/PrimeService.cs
using System;
using System.Collections.Generic;

namespace PrimePlotter.Services;

public static class PrimeService
{
    public static List<int> FirstNPrimes(int n, IProgress<double>? progress = null, System.Threading.CancellationToken ct = default)
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
