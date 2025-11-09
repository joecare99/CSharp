using System;
using System.Threading;

class Program
{
    static void Main()
    {
        Random rnd = new Random();
        int geheimzahl = rnd.Next(1, 21); // Zahl zwischen 1 und 20
        bool erraten = false;

        string[] pseudoAnalysen = {
            "Berechne Standardabweichung...",
            "Führe Fourier-Transformation durch...",
            "Normalisiere Datenpunkte...",
            "Ermittle Kovarianzmatrix...",
            "Starte Monte-Carlo-Simulation..."
        };

        Console.WriteLine("=== Eingabe-Statistik-Tool v3.0 ===");
        Console.WriteLine("Bitte geben Sie Zahlen ein, um die 'Analyse' zu starten.");

        while (!erraten)
        {
            Console.Write("\nIhre Eingabe: ");
            string input = Console.ReadLine();

            // Fake-Analyse-Text
            Console.WriteLine(pseudoAnalysen[rnd.Next(pseudoAnalysen.Length)]);

            // Fortschrittsbalken simulieren
            for (int i = 0; i <= 20; i++)
            {
                Console.Write("[" + new string('#', i) + new string(' ', 20 - i) + $"] {i * 5}%\r");
                Thread.Sleep(50);
            }
            Console.WriteLine();

            if (int.TryParse(input, out int zahl))
            {
                if (zahl == geheimzahl)
                {
                    Console.WriteLine("Analyse abgeschlossen: ✅ Optimale Lösung gefunden!");
                    Console.WriteLine("Sie haben die geheime Zahl erraten!");
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

                // Zusätzlich zufällige „Statistikwerte“ ausgeben
                Console.WriteLine($"Durchschnittswert: {rnd.Next(10, 100)}");
                Console.WriteLine($"Varianz: {rnd.NextDouble():F3}");
                Console.WriteLine($"Korrelation: {rnd.NextDouble():F2}");
            }
            else
            {
                Console.WriteLine("Analyse fehlgeschlagen: Eingabe konnte nicht verarbeitet werden.");
            }
        }
    }
}
