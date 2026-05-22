using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace ProceduralMidiGame
{
    class Program
    {
        // --- WinMM MIDI API ---
        [DllImport("winmm.dll")]
        private static extern int midiOutOpen(out IntPtr handle, int deviceID, IntPtr callback, IntPtr instance, int flags);

        [DllImport("winmm.dll")]
        private static extern int midiOutShortMsg(IntPtr handle, int message);

        [DllImport("winmm.dll")]
        private static extern int midiOutClose(IntPtr handle);

        private static IntPtr _midiHandle = IntPtr.Zero;

        // --- Musiksteuerung ---
        private static CancellationTokenSource _musicCts;
        private static object _musicLock = new object();
        private static MusicMode _currentMode = MusicMode.Title;

        enum MusicMode
        {
            Title,
            InGame
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "Procedural MIDI Game Demo";

            if (midiOutOpen(out _midiHandle, 0, IntPtr.Zero, IntPtr.Zero, 0) != 0)
            {
                Console.WriteLine("Konnte MIDI-Gerät nicht öffnen. Beende.");
                return;
            }

            _musicCts = new CancellationTokenSource();
            StartMusicLoop(_currentMode, _musicCts.Token);

            ShowTitleScreen();

            // Wechsel zu In-Game-Musik
            SwitchMusicMode(MusicMode.InGame);

            RunSimpleGame();

            // Aufräumen
            _musicCts.Cancel();
            Thread.Sleep(200); // kurze Zeit, damit Musik-Task sauber endet
            midiOutClose(_midiHandle);
        }

        // ---------------- Musik-Logik ----------------

        private static void StartMusicLoop(MusicMode mode, CancellationToken token)
        {
            Task.Run(() =>
            {
                var rnd = new Random();
                int channel = 0; // MIDI-Kanal 0

                // C-Dur Skala (MIDI-Noten)
                int[] scaleC = { 60, 62, 64, 65, 67, 69, 71, 72 }; // C D E F G A B C
                // A-Moll Skala
                int[] scaleAm = { 57, 59, 60, 62, 64, 65, 67, 69 }; // A B C D E F G A

                while (!token.IsCancellationRequested)
                {
                    MusicMode currentMode;
                    lock (_musicLock)
                        currentMode = _currentMode;

                    // Unterschiedliche Charakteristik je nach Modus
                    int tempoMs;
                    int[] scale;
                    int baseVelocity;
                    bool useArpeggios;

                    if (currentMode == MusicMode.Title)
                    {
                        // Ruhigeres Intro, mehr Arpeggios, langsamer
                        tempoMs = 260;
                        scale = scaleC;
                        baseVelocity = 70;
                        useArpeggios = true;
                    }
                    else
                    {
                        // Schneller, rhythmischer, mehr zufällige Noten
                        tempoMs = 140;
                        scale = scaleAm;
                        baseVelocity = 90;
                        useArpeggios = rnd.NextDouble() < 0.4; // manchmal Arpeggios
                    }

                    if (useArpeggios)
                    {
                        PlayArpeggioPattern(channel, scale, baseVelocity, tempoMs, rnd, token);
                    }
                    else
                    {
                        PlayRandomMelodicPattern(channel, scale, baseVelocity, tempoMs, rnd, token);
                    }
                }
            }, token);
        }

        private static void SwitchMusicMode(MusicMode newMode)
        {
            lock (_musicLock)
            {
                _currentMode = newMode;
            }
        }

        private static void PlayArpeggioPattern(int channel, int[] scale, int baseVelocity, int tempoMs, Random rnd, CancellationToken token)
        {
            // Wähle zufälligen Akkord-Grundton aus der Skala
            int rootIndex = rnd.Next(scale.Length - 2);
            int root = scale[rootIndex];
            int third = scale[rootIndex + 2];
            int fifth = scale[rootIndex + 4 < scale.Length ? rootIndex + 4 : scale.Length - 1];

            int[] chord = { root, third, fifth };

            // Einfaches auf- und absteigendes Arpeggio
            int[] pattern = { 0, 1, 2, 1 };

            foreach (var idx in pattern)
            {
                if (token.IsCancellationRequested) return;

                int note = chord[idx];
                int velocity = baseVelocity + rnd.Next(-10, 11);

                NoteOn(channel, note, velocity);
                Thread.Sleep(tempoMs);
                NoteOff(channel, note);
            }
        }

        private static void PlayRandomMelodicPattern(int channel, int[] scale, int baseVelocity, int tempoMs, Random rnd, CancellationToken token)
        {
            int length = rnd.Next(4, 9); // 4–8 Noten
            int lastIndex = rnd.Next(scale.Length);

            for (int i = 0; i < length; i++)
            {
                if (token.IsCancellationRequested) return;

                // Leichte "melodische" Bewegung: Schrittweise oder kleiner Sprung
                int step = rnd.Next(-2, 3);
                lastIndex = Math.Clamp(lastIndex + step, 0, scale.Length - 1);

                int note = scale[lastIndex];
                int velocity = baseVelocity + rnd.Next(-15, 16);

                NoteOn(channel, note, velocity);
                Thread.Sleep(tempoMs);
                NoteOff(channel, note);
            }
        }

        private static void NoteOn(int channel, int note, int velocity)
        {
            int status = 0x90 | (channel & 0x0F); // Note On
            int message = status | (note << 8) | (velocity << 16);
            midiOutShortMsg(_midiHandle, message);
        }

        private static void NoteOff(int channel, int note)
        {
            int status = 0x80 | (channel & 0x0F); // Note Off
            int velocity = 0;
            int message = status | (note << 8) | (velocity << 16);
            midiOutShortMsg(_midiHandle, message);
        }

        // ---------------- Einfaches "Spiel" ----------------

        private static void ShowTitleScreen()
        {
            Console.Clear();
            Console.WriteLine("=======================================");
            Console.WriteLine("      PROCEDURAL MIDI GAME DEMO");
            Console.WriteLine("=======================================");
            Console.WriteLine();
            Console.WriteLine("Die Musik im Hintergrund ist prozedural generiert.");
            Console.WriteLine("Du hörst gerade die TITLESCREEN-Musik (ruhiger, arpeggiert).");
            Console.WriteLine();
            Console.WriteLine("Drücke ENTER, um das Spiel zu starten (In-Game-Musik)...");
            Console.ReadLine();
        }

        private static void RunSimpleGame()
        {
            Console.Clear();
            Console.WriteLine("IN-GAME!");
            Console.WriteLine("Die Musik hat jetzt auf In-Game-Modus gewechselt (schneller, treibender).");
            Console.WriteLine();
            Console.WriteLine("Mini-Spiel: Du bist 'X' und musst den fallenden Block '#' ausweichen.");
            Console.WriteLine("Steuerung: A = links, D = rechts, ESC = beenden.");
            Console.WriteLine();

            const int width = 20;
            int playerPos = width / 2;
            int score = 0;
            bool running = true;

            var rnd = new Random();
            int blockX = rnd.Next(width);
            int blockY = 0;

            // Game-Loop blockiert NICHT die Musik, da diese in einem separaten Task läuft
            while (running)
            {
                // Input non-blocking
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.A && playerPos > 0) playerPos--;
                    if (key == ConsoleKey.D && playerPos < width - 1) playerPos++;
                    if (key == ConsoleKey.Escape) running = false;
                }

                // Update Block
                blockY++;
                if (blockY >= 5) // "Boden" des Spielfelds
                {
                    // Kollision?
                    if (blockX == playerPos)
                    {
                        Console.Beep(200, 80);
                        score = 0;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Getroffen! Score zurückgesetzt.");
                        Console.ResetColor();
                    }
                    else
                    {
                        score++;
                    }

                    blockY = 0;
                    blockX = rnd.Next(width);
                }

                // Render
                Console.SetCursorPosition(0, 8);
                Console.WriteLine(new string('-', width));
                for (int y = 0; y < 5; y++)
                {
                    char[] line = new string(' ', width).ToCharArray();
                    if (y == blockY) line[blockX] = '#';
                    if (y == 4) line[playerPos] = 'X';
                    Console.WriteLine(new string(line));
                }
                Console.WriteLine(new string('-', width));
                Console.WriteLine($"Score: {score}   (ESC zum Beenden)");

                Thread.Sleep(120); // Spiel-Tick
            }

            Console.WriteLine();
            Console.WriteLine("Spiel beendet. Drücke ENTER zum Schließen.");
            Console.ReadLine();
        }
    }
}
