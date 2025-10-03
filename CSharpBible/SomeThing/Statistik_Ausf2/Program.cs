using System;
using System.Collections.Generic;
using System.Threading;

class Program
{
    static void Main()
    {
        Console.Title = "Stat-Analyzer v5.3";
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("=== Stat-Analyzer v5.3 ===");
        Console.ResetColor();

        var rnd = new Random();
        int geheimzahl = rnd.Next(1, 21); // 1..20
        bool erraten = false;

        // "Analyse..." über 7-bit gepackte Distanzen (Bias 64), Start 'A' (65)
        // Diffs: [45, -13, 11, 13, -6, -14, -55, 0, 0]
        // Gepackte Konstante (korrekt berechnet):
        // 4_647_756_131_583_842_797UL
        string phraseAnalyse = DecodePackedDiffs(
            packed: 4_647_756_131_583_842_797UL,
            steps: 9,
            startChar: 65,
            bitWidth: 7,
            bias: 64
        );

        // Korrekte Diff-Listen für die anderen Phrasen (Startwert = erstes Zeichen)
        // "Varianz..."
        string phraseVarianz = DecodeDiffs(86, new int[] { 11, 17, -9, -8, 13, 12, -76, 0, 0 });
        // "Fourier..."
        string phraseFourier = DecodeDiffs(70, new int[] { 41, 6, -3, -9, -4, 13, -68, 0, 0 });
        // "Monte..."
        string phraseMonte = DecodeDiffs(77, new int[] { 34, -1, 6, -15, -55, 0, 0 });
        // "Kovarianz..."
        string phraseKov = DecodeDiffs(75, new int[] { 36, 7, -21, 17, -9, -8, 13, 12, -76, 0, 0 });

        var phrases = new List<string> { phraseAnalyse, phraseVarianz, phraseFourier, phraseMonte, phraseKov };

        Console.WriteLine("Bitte geben Sie Zahlen ein, um die 'Analyse' zu starten (1–20).");

        while (!erraten)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("\nEingabe: ");
            Console.ResetColor();
            string input = Console.ReadLine();

            // Dynamische Tarn-Phrase
            string status = phrases[rnd.Next(phrases.Count)];
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(status);
            Console.ResetColor();

            // Fortschrittsbalken
            RenderProgressBar(20, 28);

            // Pseudo-Kennzahlen
            Console.WriteLine($"Durchschnittswert: {rnd.Next(10, 100)}");
            Console.WriteLine($"Varianz: {rnd.NextDouble():F3}");
            Console.WriteLine($"Korrelation: {rnd.NextDouble():F2}");

            // ASCII-"Diagramm"
            RenderAsciiChart(rnd);

            // Spiel-Logik (in Tarnung)
            if (int.TryParse(input, out int zahl))
            {
                if (zahl == geheimzahl)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Analyse abgeschlossen: Optimale Lösung gefunden!");
                    Console.ResetColor();
                    erraten = true;
                }
                else if (zahl < geheimzahl)
                {
                    Console.WriteLine("Analyse abgeschlossen: Wert liegt UNTER dem Erwartungsbereich.");
                }
                else
                {
                    Console.WriteLine("Analyse abgeschlossen: Wert liegt ÜBER dem Erwartungsbereich.");
                }
            }
            else
            {
                Console.WriteLine("Analyse fehlgeschlagen: Eingabe konnte nicht verarbeitet werden.");
            }
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\nVerifikation: Prozesse erfolgreich abgeschlossen.");
        Console.ResetColor();
    }

    // Rekonstruiert einen String aus Distanzen (Startwert + Differenzen)
    static string DecodeDiffs(int startChar, int[] diffs)
    {
        var chars = new List<char> { (char)startChar };
        int current = startChar;
        foreach (int d in diffs)
        {
            current += d;
            chars.Add((char)current);
        }
        return new string(chars.ToArray());
    }

    // Rekonstruiert einen String aus einem 64-bit Wert mit festen Bitbreiten (z. B. 7 Bit) und Bias
    static string DecodePackedDiffs(ulong packed, int steps, int startChar, int bitWidth, int bias)
    {
        var chars = new List<char> { (char)startChar };
        int current = startChar;
        for (int i = 0; i < steps; i++)
        {
            int mask = (1 << bitWidth) - 1;
            int chunk = (int)((packed >> (i * bitWidth)) & (ulong)mask);
            int diff = chunk - bias;
            current += diff;
            chars.Add((char)current);
        }
        return new string(chars.ToArray());
    }

    static void RenderProgressBar(int width, int delayMs)
    {
        for (int i = 0; i <= width; i++)
        {
            Console.Write("[" + new string('#', i) + new string(' ', width - i) + $"] {(i * 100 / width),3}%\r");
            Thread.Sleep(delayMs);
        }
        Console.WriteLine();
    }

    static void RenderAsciiChart(Random rnd)
    {
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        int bars = rnd.Next(6, 10);
        for (int i = 0; i < bars; i++)
        {
            int h = rnd.Next(5, 20);
            Console.WriteLine(new string('|', h));
        }
        Console.ResetColor();
    }
}
