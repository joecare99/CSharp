using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PlayMidiMusic2;

/// <summary>
/// Entry point for a small console game that demonstrates non-blocking,
/// procedurally generated MIDI playback via the native Windows multimedia API.
/// </summary>
internal static class Program
{
    private static async Task Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        if (!OperatingSystem.IsWindows())
        {
            Console.WriteLine("This sample requires Windows because it uses winmm.dll for MIDI output.");
            return;
        }

        using CancellationTokenSource cancellationTokenSource = new();

        try
        {
            Console.CursorVisible = false;

            using MidiOutDevice midiDevice = new();
            ProceduralMusicPlayer musicPlayer = new(midiDevice);

            Task musicTask = musicPlayer.PlayLoopAsync(cancellationTokenSource.Token);

            RunGameLoop(cancellationTokenSource);

            cancellationTokenSource.Cancel();
            await musicTask.ConfigureAwait(false);
        }
        catch (MidiException ex)
        {
            Console.Clear();
            Console.CursorVisible = true;
            Console.WriteLine($"MIDI output could not be initialized: {ex.Message}");
        }
        finally
        {
            Console.CursorVisible = true;
        }
    }

    /// <summary>
    /// Runs a minimal collectible game while the music thread continues independently.
    /// </summary>
    private static void RunGameLoop(CancellationTokenSource cancellationTokenSource)
    {
        const int boardWidth = 32;
        const int boardHeight = 16;
        const int frameTimeMilliseconds = 40;

        Random random = new();
        int playerX = boardWidth / 2;
        int playerY = boardHeight / 2;
        int targetX = random.Next(1, boardWidth - 1);
        int targetY = random.Next(1, boardHeight - 1);
        int score = 0;
        DateTime startTime = DateTime.UtcNow;

        while (!cancellationTokenSource.IsCancellationRequested)
        {
            while (Console.KeyAvailable)
            {
                ConsoleKey key = Console.ReadKey(intercept: true).Key;

                switch (key)
                {
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.A:
                        playerX = Math.Max(1, playerX - 1);
                        break;

                    case ConsoleKey.RightArrow:
                    case ConsoleKey.D:
                        playerX = Math.Min(boardWidth - 2, playerX + 1);
                        break;

                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                        playerY = Math.Max(1, playerY - 1);
                        break;

                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                        playerY = Math.Min(boardHeight - 2, playerY + 1);
                        break;

                    case ConsoleKey.Escape:
                        cancellationTokenSource.Cancel();
                        continue;
                }
            }

            if (playerX == targetX && playerY == targetY)
            {
                score++;
                do
                {
                    targetX = random.Next(1, boardWidth - 1);
                    targetY = random.Next(1, boardHeight - 1);
                }
                while (targetX == playerX && targetY == playerY);
            }

            DrawGame(boardWidth, boardHeight, playerX, playerY, targetX, targetY, score, DateTime.UtcNow - startTime);
            Thread.Sleep(frameTimeMilliseconds);
        }
    }

    /// <summary>
    /// Draws the simple playfield, player marker and collectible target.
    /// </summary>
    private static void DrawGame(int boardWidth, int boardHeight, int playerX, int playerY, int targetX, int targetY, int score, TimeSpan elapsed)
    {
        StringBuilder builder = new();
        builder.AppendLine("Procedural MIDI Game - Move with Arrow Keys or WASD, exit with ESC");
        builder.AppendLine($"Score: {score}    Time: {elapsed:mm\\:ss}    Music: procedural C major / A minor blend");

        for (int y = 0; y < boardHeight; y++)
        {
            for (int x = 0; x < boardWidth; x++)
            {
                char tile = ' ';

                if (y == 0 || y == boardHeight - 1 || x == 0 || x == boardWidth - 1)
                {
                    tile = '#';
                }
                else if (x == playerX && y == playerY)
                {
                    tile = '@';
                }
                else if (x == targetX && y == targetY)
                {
                    tile = '♪';
                }

                builder.Append(tile);
            }

            builder.AppendLine();
        }

        Console.SetCursorPosition(0, 0);
        Console.Write(builder.ToString());
    }
}

/// <summary>
/// Continuously composes and plays a pleasant looping pattern using a fixed harmonic palette.
/// </summary>
internal sealed class ProceduralMusicPlayer
{
    private readonly MidiOutDevice _midiDevice;
    private readonly Random _random;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProceduralMusicPlayer"/> class.
    /// </summary>
    public ProceduralMusicPlayer(MidiOutDevice midiDevice)
    {
        _midiDevice = midiDevice;
        _random = new Random();
    }

    /// <summary>
    /// Starts an endless music loop on a background task until cancellation is requested.
    /// </summary>
    public async Task PlayLoopAsync(CancellationToken cancellationToken)
    {
        _midiDevice.SetInstrument(channel: 0, instrument: 0);
        _midiDevice.SetInstrument(channel: 1, instrument: 48);
        _midiDevice.SetInstrument(channel: 2, instrument: 32);

        _midiDevice.SetChannelVolume(channel: 0, volume: 90);
        _midiDevice.SetChannelVolume(channel: 1, volume: 70);
        _midiDevice.SetChannelVolume(channel: 2, volume: 85);

        ChordDefinition[] progression =
        [
            new ChordDefinition(60, new[] { 0, 4, 7 }, new[] { 0, 2, 4, 7 }),
            new ChordDefinition(67, new[] { 0, 4, 7 }, new[] { 0, 2, 4, 7 }),
            new ChordDefinition(69, new[] { 0, 3, 7 }, new[] { 0, 2, 3, 7 }),
            new ChordDefinition(65, new[] { 0, 4, 7 }, new[] { 0, 2, 4, 7 })
        ];

        int[] scale = [0, 2, 4, 5, 7, 9, 11];
        int stepMilliseconds = 160;

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                for (int chordIndex = 0; chordIndex < progression.Length; chordIndex++)
                {
                    ChordDefinition chord = progression[chordIndex];
                    await PlayMeasureAsync(chord, scale, stepMilliseconds, cancellationToken).ConfigureAwait(false);
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            _midiDevice.Reset();
        }
    }

    /// <summary>
    /// Plays one measure consisting of pad chord, bass pulse and arpeggiated melody.
    /// </summary>
    private async Task PlayMeasureAsync(ChordDefinition chord, int[] scale, int stepMilliseconds, CancellationToken cancellationToken)
    {
        List<int> sustainedChordNotes = new();

        foreach (int interval in chord.ChordIntervals)
        {
            int note = chord.RootNote + interval;
            sustainedChordNotes.Add(note);
            _midiDevice.NoteOn(channel: 1, note: note, velocity: 52);
        }

        try
        {
            for (int step = 0; step < 8; step++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                int bassNote = step % 2 == 0
                    ? chord.RootNote - 12
                    : chord.RootNote - 5;

                int melodyNote = BuildMelodyNote(chord, scale, step);
                int bassVelocity = step % 4 == 0 ? 96 : 78;
                int melodyVelocity = 72 + _random.Next(0, 20);

                _midiDevice.NoteOn(channel: 2, note: bassNote, velocity: bassVelocity);
                _midiDevice.NoteOn(channel: 0, note: melodyNote, velocity: melodyVelocity);

                await Task.Delay(stepMilliseconds - 20, cancellationToken).ConfigureAwait(false);

                _midiDevice.NoteOff(channel: 2, note: bassNote);
                _midiDevice.NoteOff(channel: 0, note: melodyNote);

                await Task.Delay(20, cancellationToken).ConfigureAwait(false);
            }
        }
        finally
        {
            foreach (int note in sustainedChordNotes)
            {
                _midiDevice.NoteOff(channel: 1, note: note);
            }
        }
    }

    /// <summary>
    /// Builds a melodic note by mixing arpeggio notes with scale passing tones.
    /// </summary>
    private int BuildMelodyNote(ChordDefinition chord, int[] scale, int step)
    {
        int octaveOffset = step < 4 ? 12 : 24;
        bool useArpeggio = step % 3 != 1;

        if (useArpeggio)
        {
            int interval = chord.ArpeggioPattern[step % chord.ArpeggioPattern.Length];
            return chord.RootNote + octaveOffset + interval;
        }

        int scaleDegree = scale[_random.Next(scale.Length)];
        return 60 + octaveOffset + scaleDegree;
    }
}

/// <summary>
/// Represents a reusable harmonic building block for procedural composition.
/// </summary>
internal sealed class ChordDefinition
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChordDefinition"/> class.
    /// </summary>
    public ChordDefinition(int rootNote, int[] chordIntervals, int[] arpeggioPattern)
    {
        RootNote = rootNote;
        ChordIntervals = chordIntervals;
        ArpeggioPattern = arpeggioPattern;
    }

    /// <summary>
    /// Gets the root MIDI note number.
    /// </summary>
    public int RootNote { get; }

    /// <summary>
    /// Gets the intervals that form the sustained chord.
    /// </summary>
    public int[] ChordIntervals { get; }

    /// <summary>
    /// Gets the intervals preferred for melodic arpeggiation.
    /// </summary>
    public int[] ArpeggioPattern { get; }
}

/// <summary>
/// Wraps the native Windows MIDI output device and exposes basic note/control operations.
/// </summary>
internal sealed class MidiOutDevice : IDisposable
{
    private const int MidiMapperDeviceId = -1;
    private readonly IntPtr _handle;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="MidiOutDevice"/> class.
    /// </summary>
    public MidiOutDevice()
    {
        int result = NativeMethods.midiOutOpen(out _handle, MidiMapperDeviceId, IntPtr.Zero, IntPtr.Zero, 0);
        if (result != 0)
        {
            throw new MidiException($"winmm midiOutOpen failed with error code {result}.");
        }
    }

    /// <summary>
    /// Sends a note-on event.
    /// </summary>
    public void NoteOn(int channel, int note, int velocity)
    {
        SendShortMessage(0x90 | (channel & 0x0F), note, velocity);
    }

    /// <summary>
    /// Sends a note-off event.
    /// </summary>
    public void NoteOff(int channel, int note)
    {
        SendShortMessage(0x80 | (channel & 0x0F), note, 0);
    }

    /// <summary>
    /// Selects the General MIDI instrument for the specified channel.
    /// </summary>
    public void SetInstrument(int channel, int instrument)
    {
        SendShortMessage(0xC0 | (channel & 0x0F), instrument, 0);
    }

    /// <summary>
    /// Sets the channel volume controller.
    /// </summary>
    public void SetChannelVolume(int channel, int volume)
    {
        SendShortMessage(0xB0 | (channel & 0x0F), 7, volume);
    }

    /// <summary>
    /// Silences all pending notes on the output device.
    /// </summary>
    public void Reset()
    {
        if (_disposed)
        {
            return;
        }

        NativeMethods.midiOutReset(_handle);
    }

    /// <summary>
    /// Releases the MIDI device handle.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        NativeMethods.midiOutReset(_handle);
        NativeMethods.midiOutClose(_handle);
        _disposed = true;
    }

    /// <summary>
    /// Packs a standard three-byte MIDI message into the format expected by winmm.
    /// </summary>
    private void SendShortMessage(int status, int data1, int data2)
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(MidiOutDevice));
        }

        int message = status | (data1 << 8) | (data2 << 16);
        int result = NativeMethods.midiOutShortMsg(_handle, message);

        if (result != 0)
        {
            throw new MidiException($"winmm midiOutShortMsg failed with error code {result}.");
        }
    }
}

/// <summary>
/// Represents an error reported by the native MIDI output layer.
/// </summary>
internal sealed class MidiException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MidiException"/> class.
    /// </summary>
    public MidiException(string message)
        : base(message)
    {
    }
}

/// <summary>
/// Provides the required native winmm MIDI function imports.
/// </summary>
internal static class NativeMethods
{
    [DllImport("winmm.dll")]
    public static extern int midiOutOpen(out IntPtr lphMidiOut, int uDeviceId, IntPtr dwCallback, IntPtr dwInstance, int dwFlags);

    [DllImport("winmm.dll")]
    public static extern int midiOutShortMsg(IntPtr hMidiOut, int dwMsg);

    [DllImport("winmm.dll")]
    public static extern int midiOutReset(IntPtr hMidiOut);

    [DllImport("winmm.dll")]
    public static extern int midiOutClose(IntPtr hMidiOut);
}
