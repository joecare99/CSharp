// Sie müssen das NuGet-Paket "System.Drawing.Common" zu Ihrem Projekt hinzufügen, um Bitmap und andere Typen aus System.Drawing zu verwenden.
// Beispiel (in Visual Studio): Rechtsklick auf das Projekt > NuGet-Pakete verwalten > System.Drawing.Common suchen und installieren.

internal static class HilpertMath
{
    // Die Hilfsfunktion zur Rotation und Spiegelung, übersetzt aus Python
    private static (int, int) Rot(int n, int x, int y, bool rx, bool ry)
    {
        if (!ry)
        {
            if (rx)
            {
                (x, y) = (n - 1 - x, n - 1 - y);
            }
            // Tausche x und y
            (x, y) = (y, x);
        }
        return (x, y);
    }

    // Koordinate (x, y) zu Distanz (d) auf der Hilbert-Kurve abbilden
    public static int Xy2d(int n, int x, int y)
    {
        int d = 0;
        int s = n / 2;
        while (s > 0)
        {
            bool rx = (x & s) > 0;
            bool ry = (y & s) > 0;
            d += s * s * ((3 * (rx ? 1 : 0)) ^ (ry ? 1 : 0));
            (x, y) = Rot(s, x, y, rx, ry);
            s /= 2;
        }
        return d;
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

}