using System;
using System.Threading;
using System.Threading.Tasks;
using NAudio.Midi; // Benötigt das NAudio NuGet-Paket

namespace ProceduralMusicGenerator
{
    /// <summary>
    /// Klasse zur prozeduralen Erzeugung von Musiknoten und ihrer Steuerung.
    /// </summary>
    public class MusicGenerator
    {
        // Die Tonleiter: C-Dur (C=0, D=2, E=4, F=5, G=7, A=9, B=11)
        private readonly int[] CMajorScale = { 0, 2, 4, 5, 7, 9, 11 };
        private readonly int Octave = 4; // Startton in der Oktave 4

        private Random random = new Random();

        /// <summary>
        /// Generiert eine zufällige Note aus der C-Dur Tonleiter.
        /// </summary>
        /// <returns>Die MIDI-Note (0-127).</returns>
        private int GenerateRandomNote()
        {
            // Wähle eine zufällige Stufe in der Tonleiter
            int scaleIndex = random.Next(CMajorScale.Length);

            // Füge die Oktave hinzu
            int note = scaleIndex + Octave;

            // Stellt sicher, dass die Note im gültigen MIDI-Bereich liegt
            return Math.Max(0, Math.Min(127, note));
        }

        /// <summary>
        /// Generiert eine Akkordfolge als Liste von MIDI-Noten.
        /// </summary>
        /// <param name="durationMs">Die Dauer, für die die Note gehalten wird (in Millisekunden).</param>
        /// <returns>Eine Liste von MIDI-Noten, die abgespielt werden sollen.</returns>
        public List<int> GenerateChordSequence(int durationMs)
        {
            var sequence = new List<int>();

            // Generiere eine Sequenz von 4 Noten für den Akkord
            for (int i = 0; i < 4; i++)
            {
                // Zufällige Note aus der Tonleiter
                int note = GenerateRandomNote();

                // Füge die Note und die Dauer hinzu
                sequence.Add(note);

                // Pause, um den Akkord rhythmisch zu gestalten
                Thread.Sleep(durationMs / 4);
            }

            return sequence;
        }
    }

    /// <summary>
    /// Klasse zur Wiedergabe der Musik über NAudio (simuliert das MIDI-Signal).
    /// </summary>
    public class MidiPlayer
    {
        private readonly MidiOut output;

        public MidiPlayer(MidiOut output)
        {
            this.output = output;
        }

        /// <summary>
        /// Spielt die generierte Akkordfolge ab.
        /// </summary>
        /// <param name="sequence">Die Liste der MIDI-Noten.</param>
        public void PlaySequence(List<int> sequence)
        {
            Console.WriteLine($"[Musik] Start der Sequenz...");

            foreach (int midiNote in sequence)
            {
                // Hier würde die eigentliche MIDI-Übertragung stattfinden.
                // Da wir hier NAudio verwenden, simulieren wir das Senden einer Frequenz.
                // In einer echten MIDI-Implementierung würde hier die MIDI-Nachrichten gesendet.

                // Simulierte Ausgabe für die Demonstration:
                Console.WriteLine($"[Musik] Note gespielt: {midiNote}");

                // Simuliere die Zeit, die für das Halten der Note benötigt wird
                Thread.Sleep(1000);
            }
            Console.WriteLine("[Musik] Sequenz beendet.");
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "Procedural MIDI Hintergrundmusik Generator";
            Console.WriteLine("Starte Musikgenerator... Drücke Enter, um die Musik abzuspielen.");

            // 1. Setup des Audio-Outputs (NAudio)
            // Wir verwenden einen AudioOutputStream, um die Musik im Hintergrund zu steuern.
            var midiOut = new MidiOut(0); // Vereinfachte NAudio-Ausgabe
            var midiPlayer = new MidiPlayer(midiOut);
            var generator = new MusicGenerator();

            // 2. Hintergrundmusik-Thread starten
            Console.WriteLine("Hintergrundmusik startet im Hintergrund (Multithreading)...");

            // Task, die die Musik unendlich generiert und spielt
            var musicTask = Task.Run(() =>
            {
                while (true) // Endlosschleife
                {
                    // Die Musik generiert und spielt einen Akkord
                    var sequence = generator.GenerateChordSequence(2000); // 2 Sekunden Dauer
                    midiPlayer.PlaySequence(sequence);

                    // Pause zwischen den Akkorden
                    Thread.Sleep(5000);
                }
            });

            // 3. Minimales Spiel (Haupt-Game-Schleife)
            Console.WriteLine("\n--- Spielmodus aktiv ---");
            Console.WriteLine("Spielmodus läuft. Die Musik läuft im Hintergrund. Tippe STRG+C, um das Programm zu beenden.");

            // Simulation des Spiels: Spieler reagiert, während Musik läuft.
            int gameTick = 0;
            while (true)
            {
                Console.Write($"Spieler-Tick {gameTick}: ");
                Console.Write(Console.ReadLine());
                gameTick++;

                // Überprüfen, ob der Benutzer das Programm beenden will (optional)
                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.C)
                {
                    Console.WriteLine("\nBeendigung angefordert. Musik wird gestoppt...");
                    // In einer echten App müsste hier ein Signal an musicTask gesendet werden,
                    // um den Thread sauber zu beenden.
                    break;
                }

                // Die Hauptlogik des Spiels läuft ungestört.
                await Task.Delay(100);
            }

            // Task beenden, wenn das Spiel beendet wird
            await musicTask;
            Console.WriteLine("Programm beendet.");
        }
    }
}
