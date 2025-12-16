using NAudio;
using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace MidiSwing.MVVM.Models;

public class MusicGenerator
{
    private int _channel;
    private MidiOut _midi;
    private readonly object _midiSync = new();

    public MusicGenerator()
    {
        _channel = Math.Clamp(2, 1, 16);
        try
        {
            if (MidiOut.NumberOfDevices > 0)
            {
                _midi = new MidiOut(Math.Clamp(0, 0, MidiOut.NumberOfDevices - 1));
            }
        }
        catch
        {
            _midi = null; // Fallback to silent if MIDI not available
        }
    }

    // Basic lengths in eighth-note units to keep rhythm handling simple
    private enum NoteLength
    {
        Eighth = 1,
        Quarter = 2,
        Half = 4,
        Whole = 8
    }

    private sealed class MelodyNote
    {
        public int Pitch { get; set; }
        public NoteLength Length { get; set; }
        public bool Accent { get; set; }
    }

    private List<MelodyNote> GenerateBaseMelody(int root = 60)
    {
        // Simple minor-blues flavored motif around root
        var scale = new[] { 0, 2, 4, 5, 7,9, 10, 12 };
        int Degree(int i) => root + scale[i];

        var motif = new List<MelodyNote>
        {
            new() { Pitch = Degree(0), Length = NoteLength.Quarter, Accent = true },
            new() { Pitch = Degree(1), Length = NoteLength.Eighth, Accent = false },
            new() { Pitch = Degree(2), Length = NoteLength.Eighth, Accent = false },
            new() { Pitch = Degree(3), Length = NoteLength.Eighth, Accent = false },
            new() { Pitch = Degree(4), Length = NoteLength.Eighth, Accent = false },
            new() { Pitch = Degree(5), Length = NoteLength.Quarter, Accent = false },
            new() { Pitch = Degree(6), Length = NoteLength.Eighth, Accent = false },
            new() { Pitch = Degree(7), Length = NoteLength.Eighth, Accent = false },
            new() { Pitch = Degree(5), Length = NoteLength.Quarter, Accent = false },
            new() { Pitch = Degree(7), Length = NoteLength.Eighth, Accent = false },
            new() { Pitch = Degree(2), Length = NoteLength.Eighth, Accent = false },
            new() { Pitch = Degree(0), Length = NoteLength.Quarter, Accent = false }
        };

        var phrase = new List<MelodyNote>();

        // Bar 1: original motif
        phrase.AddRange(motif);

        // Bar 2: up a second
        phrase.AddRange(motif.Select(n => new MelodyNote
        {
            Pitch = n.Pitch + 2,
            Length = n.Length,
            Accent = n.Accent
        }));

        // Bar 3: down a minor third
        phrase.AddRange(motif.Select(n => new MelodyNote
        {
            Pitch = n.Pitch - 3,
            Length = n.Length,
            Accent = n.Accent
        }));

        // Bar 4: original but with a held note at the end
        phrase.AddRange(motif.Take(motif.Count - 1));
        phrase.Add(new MelodyNote
        {
            Pitch = Degree(0),
            Length = NoteLength.Half,
            Accent = true
        });

        return phrase;
    }

    private List<MelodyNote> CreateVariation(List<MelodyNote> baseMelody, int semitoneShift, bool addSyncopation)
    {
        var rnd = new Random();
        var result = new List<MelodyNote>();

        foreach (var note in baseMelody)
        {
            var pitch = note.Pitch + semitoneShift;

            // Occasional neighbor note for variation
            if (rnd.NextDouble() < 0.2)
            {
                int neighbor = pitch + (rnd.Next(2) == 0 ? -1 : 1);
                result.Add(new MelodyNote
                {
                    Pitch = neighbor,
                    Length = NoteLength.Eighth,
                    Accent = false
                });

                result.Add(new MelodyNote
                {
                    Pitch = pitch,
                    Length = note.Length == NoteLength.Quarter ? NoteLength.Eighth : note.Length,
                    Accent = note.Accent
                });

                continue;
            }

            var len = note.Length;
            if (addSyncopation && len == NoteLength.Quarter && rnd.NextDouble() < 0.4)
            {
                // Split quarter into two eighths
                result.Add(new MelodyNote
                {
                    Pitch = pitch,
                    Length = NoteLength.Eighth,
                    Accent = note.Accent
                });
                result.Add(new MelodyNote
                {
                    Pitch = pitch,
                    Length = NoteLength.Eighth,
                    Accent = false
                });
            }
            else
            {
                result.Add(new MelodyNote
                {
                    Pitch = pitch,
                    Length = len,
                    Accent = note.Accent
                });
            }
        }

        return result;
    }

    private List<int[]> GetMainChords()
    {
        // Rough C minor → F7 → G7 → Cmin pattern
        return new List<int[]>
        {
            new[] { 48, 51, 55, 58 }, // Cmin7
            new[] { 41, 45, 48, 52 }, // F7
            new[] { 43, 47, 50, 53 }, // G7
            new[] { 48, 51, 55, 58 }  // Cmin7
        };
    }

    public async Task PlaySwingMelody()
    {
        if (_midi == null)
        {
            Console.WriteLine("Kein MIDI-Ausgabegerät verfügbar.");
            return;
        }

        const int drumsChannel = 10;
        const int bassChannel = 3;
        const int compChannel = 4;
        const int leadChannel = 2;

        const int leadPatch = 67;   // Tenor Sax
        const int compPatch = 27;   // Jazz Guitar
        const int bassPatch = 33;   // Acoustic Bass

        int bpm = 120;
        int beatMs = 60000 / bpm;

        try
        {
            SetPatch(leadPatch, leadChannel);
            SetPatch(compPatch, compChannel);
            SetPatch(bassPatch, bassChannel);

            var timeline = new List<(int atMs, Action action)>();

            void AddNoteOn(int atMs, int note, int vel, int ch) =>
                timeline.Add((atMs, () => NoteOn(note, vel, ch)));

            void AddNoteOff(int atMs, int note, int ch) =>
                timeline.Add((atMs, () => NoteOff(note, ch)));

            void AddDrumHit(int atMs, int midiNote, int vel, int lenMs)
            {
                timeline.Add((atMs, () => NoteOn(midiNote, vel, drumsChannel)));
                timeline.Add((atMs + lenMs, () => NoteOff(midiNote, drumsChannel)));
            }

            void AddChordStab(int atMs, IEnumerable<int> notes, int vel, int ch, int lenMs)
            {
                foreach (var n in notes)
                {
                    timeline.Add((atMs, () => NoteOn(n, vel, ch)));
                    timeline.Add((atMs + lenMs, () => NoteOff(n, ch)));
                }
            }

            int TimeBeatsToMs(double beats) => (int)Math.Round(beats * beatMs);

            // Build musical material
            var baseMelody = GenerateBaseMelody(60);
            var variation1 = CreateVariation(baseMelody, 2, addSyncopation: true);
            var variation2 = CreateVariation(baseMelody, -3, addSyncopation: true);
            var chords = GetMainChords();

            int sectionStartMs = 0;

            // Intro: 4 bars, bass + light drums, short walk-in
            int introBars = 4;
            for (int bar = 0; bar < introBars; bar++)
            {
                int barStart = sectionStartMs + bar * beatMs * 4;

                int[] bassPattern = { 36, 43, 36, 43 }; // C2, G2
                for (int beat = 0; beat < 4; beat++)
                {
                    int t = barStart + beat * beatMs;
                    int note = bassPattern[beat % bassPattern.Length];
                    AddNoteOn(t, note, 85, bassChannel);
                    AddNoteOff(t + TimeBeatsToMs(0.9), note, bassChannel);

                    if (beat == 1 || beat == 3)
                    {
                        AddDrumHit(t, 42, 70, 30); // light hi-hat on 2 and 4
                    }
                }
            }

            sectionStartMs += introBars * beatMs * 4;

            // Helper to add one groove bar (drums + comp + bass)
            void AddGrooveBar(int barIndex, int startMs)
            {
                int chordIndex = barIndex % chords.Count;
                var chord = chords[chordIndex];

                for (int beat = 0; beat < 4; beat++)
                {
                    int beatStart = startMs + beat * beatMs;

                    // Hi-hat on each beat
                    AddDrumHit(beatStart, 42, 75, 25);

                    // Kick/snare
                    if (beat == 0 || beat == 2)
                        AddDrumHit(beatStart, 36, 90, 30); // kick
                    else
                        AddDrumHit(beatStart, 38, 95, 30); // snare

                    // Off-beat hi-hat (swing)
                    int off = beatStart + (int)Math.Round(beatMs * 2.0 / 3.0);
                    AddDrumHit(off, 42, 60, 25);
                }

                // Comp stabs on 2 and 4
                int beat2 = startMs + beatMs;
                int beat4 = startMs + 3 * beatMs;
                AddChordStab(beat2, chord, 75, compChannel, 140);
                AddChordStab(beat4, chord, 80, compChannel, 160);

                // Simple bass walking
                int[] bassLine = { chord[0], chord[2], chord[2] + 2, chord[3] };
                for (int beat = 0; beat < 4; beat++)
                {
                    int t = startMs + beat * beatMs;
                    int n = bassLine[beat % bassLine.Length] - 12;
                    AddNoteOn(t, n, 80, bassChannel);
                    AddNoteOff(t + TimeBeatsToMs(0.9), n, bassChannel);
                }
            }

            int barLenMs = beatMs * 4;

            // Main A section: 8 bars of groove
            int aSectionBars = 8;
            for (int bar = 0; bar < aSectionBars; bar++)
            {
                int barStart = sectionStartMs + bar * barLenMs;
                AddGrooveBar(bar, barStart);
            }

            // Helper to place a melody line on the lead channel
            void PlaceMelody(List<MelodyNote> melody, int startMs, int channel, int baseVelocity)
            {
                double beatPos = 0;
                foreach (var n in melody)
                {
                    int start = startMs + TimeBeatsToMs(beatPos);
                    double lenBeats = n.Length switch
                    {
                        NoteLength.Eighth => 0.5,
                        NoteLength.Quarter => 1.0,
                        NoteLength.Half => 2.0,
                        NoteLength.Whole => 4.0,
                        _ => 1.0
                    };

                    int vel = baseVelocity + (n.Accent ? 15 : 0);
                    AddNoteOn(start, n.Pitch, vel, channel);
                    AddNoteOff(start + TimeBeatsToMs(lenBeats * 0.9), n.Pitch, channel);

                    beatPos += lenBeats;
                }
            }

            int melodyStart = sectionStartMs;
            PlaceMelody(baseMelody, melodyStart, leadChannel, 100);

            int baseMelodyLenBeats = baseMelody.Sum(m => m.Length switch
            {
                NoteLength.Eighth => 1,
                NoteLength.Quarter => 2,
                NoteLength.Half => 4,
                NoteLength.Whole => 8,
                _ => 2
            });
            int baseMelodyLenMs = TimeBeatsToMs(baseMelodyLenBeats / 2.0); // convert eighth units back to beats

            PlaceMelody(baseMelody, melodyStart + baseMelodyLenMs, leadChannel, 105);

            sectionStartMs += aSectionBars * barLenMs;

            // A' section: variation1 over same groove
            int a2Bars = 8;
            for (int bar = 0; bar < a2Bars; bar++)
            {
                int barStart = sectionStartMs + bar * barLenMs;
                AddGrooveBar(bar, barStart);
            }

            PlaceMelody(variation1, sectionStartMs, leadChannel, 105);
            sectionStartMs += a2Bars * barLenMs;

            // B section: variation2, denser comping
            int bBars = 8;
            for (int bar = 0; bar < bBars; bar++)
            {
                int barStart = sectionStartMs + bar * barLenMs;
                AddGrooveBar(bar, barStart);

                int chordIndex = bar % chords.Count;
                var chord = chords[chordIndex];
                int off2 = barStart + beatMs + beatMs / 2;
                int off4 = barStart + 3 * beatMs + beatMs / 2;
                AddChordStab(off2, chord, 85, compChannel, 100);
                AddChordStab(off4, chord, 90, compChannel, 120);
            }

            PlaceMelody(variation2, sectionStartMs, leadChannel, 110);
            sectionStartMs += bBars * barLenMs;

            // Play timeline in order
            timeline.Sort((a, b) => a.atMs.CompareTo(b.atMs));

            int currentTime = 0;
            foreach (var (atMs, action) in timeline)
            {
                int wait = atMs - currentTime;
                if (wait > 0)
                {
                    await Task.Delay(wait);
                    currentTime = atMs;
                }

                action();
            }

            await Task.Delay(1000);
        }
        catch (TaskCanceledException)
        {
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler beim Abspielen: {ex.Message}");
        }
    }

    private void NoteOn(int noteNumber, int velocity = 100, int channel = -1)
    {
        if (_midi == null)
        {
            return;
        }

        var ch = channel > 0 ? channel : _channel;
        lock (_midiSync)
        {
            _midi.Send(MidiMessage.StartNote(noteNumber, velocity, ch).RawData);
        }
    }

    private void NoteOff(int noteNumber, int channel = -1)
    {
        if (_midi == null)
        {
            return;
        }

        var ch = channel > 0 ? channel : _channel;
        lock (_midiSync)
        {
            _midi.Send(MidiMessage.StartNote(noteNumber, 0, ch).RawData);
        }
    }

    private void SetPatch(int program, int channel)
    {
        if (_midi == null)
        {
            return;
        }

        lock (_midiSync)
        {
            _midi.Send(MidiMessage.ChangePatch(program, channel).RawData);
        }
    }
}
