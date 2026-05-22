using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

public static class Program
{
    // Native winmm.dll functions for MIDI output
    [DllImport("winmm.dll", EntryPoint = "midiOutOpen")]
    private static extern int MidiOutOpen(out IntPtr lphMidiOut, int uDeviceID, IntPtr dwCallback, IntPtr dwInstance, int dwFlags);

    [DllImport("winmm.dll", EntryPoint = "midiOutClose")]
    private static extern int MidiOutClose(IntPtr hMidiOut);

    [DllImport("winmm.dll", EntryPoint = "midiOutShortMsg")]
    private static extern int MidiOutShortMsg(IntPtr hMidiOut, int dwMsg);

    private static IntPtr _midiOut;
    private static bool _isPlaying = true;

    public static void Main()
    {
        Console.Title = "Midi Procedural Jam and Mini-Game";
        Console.CursorVisible = false;
        Console.Clear();

        // 1. Initialize MIDI Device
        if (MidiOutOpen(out _midiOut, -1, IntPtr.Zero, IntPtr.Zero, 0) != 0)
        {
            Console.WriteLine("MIDI-Gerät konnte nicht geöffnet werden.");
            return;
        }

        // Set instrument (Patch Change): Channel 0, Instrument Program 0 (Acoustic Grand Piano)
        // Set synth lead: Channel 1, Instrument Program 80 (Ocarina or Synth Lead)
        SendMidiMsg(0xC0, 80, 0); // Synth Lead on Ch 1
        SendMidiMsg(0xC0, 0, 1);  // Grand Piano on Ch 0
        SendMidiMsg(0xC0, 32, 3);  // Grand Piano on Ch 0

        // 2. Start procedural MIDI background task
        Task.Run(() => PlayProceduralMusic());

        // 3. Simple Non-blocking Action Game loop
        int playerX = 10;
        int playerY = 10;
        int targetX = 25;
        int targetY = 8;
        int score = 0;
        Random rnd = new();

        PrintInstructions();

        while (true)
        {
            // Draw game screen
            Console.SetCursorPosition(0, 3);
            for (int y = 4; y < 18; y++)
            {
                Console.SetCursorPosition(2, y);
                for (int x = 2; x < 40; x++)
                {
                    if (x == playerX && y == playerY)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("☺");
                    }
                    else if (x == targetX && y == targetY)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("★");
                    }
                    else if (x == 2 || x == 39 || y == 4 || y == 17)
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write("#");
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                }
                Console.WriteLine();
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(2, 19);
            Console.Write($"Score: {score}  | Drücke 'Q' zum Beenden.");

            // Non-blocking interaction check
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Q) break;

                int nextX = playerX;
                int nextY = playerY;

                if (key == ConsoleKey.LeftArrow) nextX--;
                if (key == ConsoleKey.RightArrow) nextX++;
                if (key == ConsoleKey.UpArrow) nextY--;
                if (key == ConsoleKey.DownArrow) nextY++;

                // Bound checking
                if (nextX > 2 && nextX < 39 && nextY > 4 && nextY < 17)
                {
                    playerX = nextX;
                    playerY = nextY;
                }

                // Check target collision
                if (playerX == targetX && playerY == targetY)
                {
                    score++;
                    // Play a quick satisfying high chime note on MIDI channel 2
                    Task.Run(() => PlayChime());
                    targetX = rnd.Next(3, 38);
                    targetY = rnd.Next(5, 16);
                }
            }

            Thread.Sleep(50);
        }

        // Cleanup
        _isPlaying = false;
        Thread.Sleep(300); // Wait for notes to silence
        MidiOutClose(_midiOut);
        Console.CursorVisible = true;
        Console.ResetColor();
        Console.Clear();
        Console.WriteLine("Danke fürs Spielen!");
    }

    private static void PrintInstructions()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.SetCursorPosition(2, 0);
        Console.WriteLine("=== PROZEDURALE MIDI JAM ===");
        Console.SetCursorPosition(2, 1);
        Console.WriteLine("Bewege dich mit den Pfeiltasten und sammle Sterne.");
    }

    private static void PlayProceduralMusic()
    {
        // Pentatonic Scale (A Minor Pentatonic: A, C, D, E, G)
        int[] scale = { 57, 60, 62, 64, 67, 69, 72, 74, 76, 79, 81 };
        int[] chords = { 45, 48, 50, 52 }; // A2, C3, D3, E2 bass notes
        Random rnd = new();
        int bassNote = chords[0]; // Start with the first chord note
        int[] melodyNotes = {0,0,0,0,0}; // Placeholder for potential arpeggio notes
        int step = 0;
        while (_isPlaying)
        {
            // Mathematische Harmonisierung: Ein Bass-Akkord-Ton wechselt alle 8 Schritte
            if (step % 16 == 0)
            {
                bassNote = chords[rnd.Next(chords.Length)];
                // Play Bass Note on Channel 0
                SendMidiMsg(0x90, bassNote-12, 100, 0);
                // Cut the bass note shortly before next cycle
                Task.Delay(1900).ContinueWith(_ => SendMidiMsg(0x80, bassNote-12, 0, 0));
                for (int i = 0; i < melodyNotes.Length; i++)
                {
                    melodyNotes[i] = rnd.Next(scale.Length); 
                }

            }

            if (step % 4 == 0)
            {
                SendMidiMsg(0x90, scale[melodyNotes[0]] -12, 80, 3);
                SendMidiMsg(0x90, scale[melodyNotes[1]] - 12, 80, 3);
                SendMidiMsg(0x90, scale[melodyNotes[2]] - 12, 80, 3);
                // Cut the bass note shortly before next cycle
                Task.Delay(500).ContinueWith(_ => { 
                    SendMidiMsg(0x80, scale[melodyNotes[0]] - 12, 0, 3); 
                    SendMidiMsg(0x80, scale[melodyNotes[1]] - 12, 0, 3); 
                    SendMidiMsg(0x80, scale[melodyNotes[2]] - 12, 0, 3); }
                );

            }
            // Prozedurale Melodie-Generierung: Wähle harmonische Töne basierend auf Pentatonik
            int melodyNote1 = scale[melodyNotes[step % melodyNotes.Length]];

            // Arpeggio-Effekt: Manchmal eine zweite komplementäre Note spielen
            int melodyNote2 = bassNote + 4+ (step % 2)*3; // Terz / Intervall-Shift

            // Note On (Melodiestimme auf Kanal 1)
            SendMidiMsg(0x90, melodyNote1, 90, 1);
            if (rnd.NextDouble() > 0.3)
            {
                SendMidiMsg(0x90, melodyNote2, 75, 1);
            }

            // Gate-Dauer (Tandem-Loop Geschwindigkeit / Tempo bpm)
            Thread.Sleep(200-(step %2)*100);

            // Note Off (Melodiestimme stoppen)
            SendMidiMsg(0x80, melodyNote1, 0, 1);
            SendMidiMsg(0x80, melodyNote2, 0, 1);

            step++;
        }
    }

    private static void PlayChime()
    {
        // Quick high chime on target collection
        SendMidiMsg(0x90, 88, 110, 2); // E6
        Thread.Sleep(80);
        SendMidiMsg(0x80, 88, 0, 2);
        SendMidiMsg(0x90, 93, 120, 2); // A6
        Thread.Sleep(200);
        SendMidiMsg(0x80, 93, 0, 2);
    }

    private static void SendMidiMsg(int status, int data1, int data2, int channel = 0)
    {
        // Status Byte combining command code and channel 
        // e.g. 0x90 = Note On, 0x80 = Note Off
        int combinedStatus = (status & 0xF0) | (channel & 0x0F);
        int message = combinedStatus | (data1 << 8) | (data2 << 16);
        MidiOutShortMsg(_midiOut, message);
    }
}
