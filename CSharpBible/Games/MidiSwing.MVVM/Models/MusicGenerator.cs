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

    // Platzhalter für MIDI-Ausgabegerät aus der gewählten Bibliothek (z.B. NAudio)
    // private NAudio.Midi.MidiOut midiOut;

    public MusicGenerator()
    {
        // Initialisierung des MIDI-Ports (z.B. Port 0)
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

    /*
    PSEUDOCODE (Plan):
    - Ziel: Die bestehende Swing-Melodie um eine Begleitung (Akkord-Comping) und Schlagzeug (Hi-Hat, Kick, Snare) erweitern.
    - Kanäle:
      - Melodie: vorhandener Kanal _channel (z.B. 2), Instrument: Tenor Sax (GM 67).
      - Begleitung: eigener Kanal (z.B. 4), Instrument: Jazz-Gitarre (GM 27).
      - Drums: GM-Drum-Kanal 10 (Percussion; Patch egal).
    - Timing:
      - BPM definieren (z.B. 120).
      - beatMs = 60000 / BPM.
      - Swing-Achtel: long = 2/3 beat, short = 1/3 beat.
    - Melodie:
      - Vorhandene Notenliste weiterverwenden.
      - Nacheinander NoteOn -> Delay(duration) -> NoteOff -> kurze Pause.
    - Begleitung (Comping):
      - Einfache 4-taktige Akkordfolge passend zur Melodie (Cmaj7 | Dm7 | G7 | Cmaj7).
      - Pro Takt zwei kurze Stabs auf 2 und 4 (je ~140 ms), restliche Zeit Pause.
      - Akkordtöne im mittleren Register, Blockakkord anspielen, nach Länge NoteOff für alle.
    - Drums:
      - Hi-Hat (42) auf geswingten Achteln: On-Beat (stärker), Off-Beat (leiser).
      - Kick (36) auf 1 und 3, Snare (38) auf 2 und 4.
      - Jeder Schlag kurzer Impuls (NoteOff nach ~20–30 ms).
    - Nebenläufigkeit:
      - Melodie, Begleitung und Drums als drei Tasks parallel starten, am Ende await Task.WhenAll.
      - Thread-Safety: alle MIDI-Sends über Lock synchronisieren.
    - Fehlerbehandlung:
      - try/catch um Gesamtablauf; TaskCanceledException ignorieren.
      - Bei fehlendem MIDI (_midi == null) früh beenden.
    */

    public async Task PlaySwingMelody()
    {
        // Plan (Pseudocode):
        // - Ein einziger Timing-Task steuert alle Events deterministisch.
        // - Vorbereitungen:
        //   - Kanäle/Velocity/BPM/Swing berechnen.
        //   - Patches setzen.
        // - Timeline aufbauen:
        //   - Drums: Für 4 Takte pro Beat On-Beat (Hi-Hat, Kick/Snare), danach Swing-Off-Beat.
        //   - Begleitung: Pro Takt Stabs auf 2 und 4 (140 ms) mit Akkordtönen.
        //   - Melodie: Sequenz der Noten nacheinander mit gegebenen Dauer und kleiner Pause.
        // - Ein einziger Ablauf:
        //   - Ereignisse werden nacheinander mit Task.Delay ausgeführt, Reihenfolge durch Zeitstempel.
        //   - NoteOn/NoteOff/DrumHits mit Lock senden.
        // - Fehlerbehandlung: try/catch um Gesamtablauf, bei fehlendem MIDI früh beenden.

        if (_midi == null)
        {
            Console.WriteLine("Kein MIDI-Ausgabegerät verfügbar.");
            return;
        }

        const int drumsChannel = 10; // GM-Drums
        const int compChannel = 4;   // Begleitung
        const int melodyVelocity = 100;
        const int compVelocity = 80;
        const int hiHatVelocityOn = 70;
        const int hiHatVelocityOff = 55;
        const int kickVelocity = 90;
        const int snareVelocity = 95;

        int bpm = 120;
        int beatMs = 60000 / bpm;
        int swingLong = (int)Math.Round(beatMs * 2.0 / 3.0);
        int swingShort = beatMs - swingLong;

        try
        {
            // Instrumente setzen
            SetPatch(67, _channel); // Tenor Sax
            SetPatch(27, compChannel); // Jazz-Gitarre (GM 27)

            // Beispiel-Melodie mit Swing-Rhythmus (Triolen-basiert)
            var melody = new List<(int note, int duration)>
            {
                (60, 2), (62, 1), (64, 2), (62, 1), // C D E D
                (60, 6),                                   // C
                (65, 2), (64, 1), (62, 2), (60, 1), // F E D C
                (59, 6),                                    // H (B)
                (60, 2), (62, 1), (64, 2), (62, 1), // C D E D
                (60, 6),                                   // C
                (65, 2), (64, 1), (62, 2), (60, 1), // F E D C
                (59, 6),                                    // H (B)
          };

            // 4-taktige Akkordfolge (Cmaj7 | Dm7 | G7 | Cmaj7)
            var chords = new List<int[]>
            {
                new[] { 48, 52, 55, 59 }, // Cmaj7: C E G B (C3 E3 G3 B3)
                new[] { 50, 53, 57, 60 }, // Dm7:  D F A C
                new[] { 43, 47, 50, 53 }, // G7:   G B D F (tiefer für Klarheit)
                new[] { 48, 52, 55, 59 }  // Cmaj7
            };

            // Timeline-Event-Struktur
            var timeline = new List<(int atMs, Action action)>();

            // Hilfsfunktionen zum Hinzufügen
            void AddNoteOn(int atMs, int note, int vel, int ch) => timeline.Add((atMs, () => NoteOn(note, vel, ch)));
            void AddNoteOff(int atMs, int note, int ch) => timeline.Add((atMs, () => NoteOff(note, ch)));
            void AddDrumHit(int atMs, int midiNote, int vel, int ch, int lenMs)
            {
                timeline.Add((atMs, () => NoteOn(midiNote, vel, ch)));
                timeline.Add((atMs + lenMs, () => NoteOff(midiNote, ch)));
            }
            void AddChordStab(int atMs, IEnumerable<int> notes, int vel, int ch, int lenMs)
            {
                foreach (var n in notes)
                {
                    timeline.Add((atMs, () => NoteOn(n, vel, ch)));
                    timeline.Add((atMs + lenMs, () => NoteOff(n, ch)));
                }
            }

            // Gesamtdauer: 4 Takte à 4 Schläge
            int bars = 4;
            int barLenMs = beatMs * 4;
            int totalLenMs = bars * barLenMs;

            // Drums: On-Beat + Swing Off-Beat
            for (int bar = 0; bar < bars; bar++)
            {
                int barStart = bar * barLenMs;
                for (int beat = 0; beat < 4; beat++)
                {
                    int beatStart = barStart + beat * beatMs;

                    // On-Beat: Hi-Hat
                    AddDrumHit(beatStart, 42, hiHatVelocityOn, drumsChannel, 25);

                    // Kick auf 1 und 3, Snare auf 2 und 4
                    if (beat == 0 || beat == 2)
                    {
                        AddDrumHit(beatStart, 36, kickVelocity, drumsChannel, 25);
                    }
                    else
                    {
                        AddDrumHit(beatStart, 38, snareVelocity, drumsChannel, 25);
                    }

                    // Off-Beat (Swing)
                    int offBeat = beatStart + swingLong;
                    AddDrumHit(offBeat, 42, hiHatVelocityOff, drumsChannel, 25);
                }
            }

            // Begleitung: Stabs auf 2 und 4
            const int stabLen = 125;
            for (int bar = 0; bar < chords.Count; bar++)
            {
                int barStart = bar * barLenMs;
                var chord = chords[bar];

                int beat2 = barStart + beatMs;          // Schlag 2
                int beat4 = barStart + beatMs * 3;      // Schlag 4

                AddChordStab(beat2, chord, compVelocity, compChannel, stabLen);
                AddChordStab(beat4, chord, compVelocity, compChannel, stabLen);
            }

            // Melodie: nacheinander ohne weitere Parallelität
            // Start der Melodie am Beginn
            int t = 0;
            foreach (var (note, duration) in melody)
            {
                AddNoteOn(t , note, melodyVelocity, _channel);
                AddNoteOff(t + duration* beatMs / 3-25, note, _channel);
                t += duration* beatMs / 3; // kleine Pause zwischen den Noten
            }

            // Sortierte Ausführung nach Zeitstempel
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

            // Sicherstellen, dass Restzeit abgewartet wird, falls Melodie kürzer ist als Gesamt-Arrangement
            if (currentTime < totalLenMs)
            {
                await Task.Delay(totalLenMs - currentTime);
            }
        }
        catch (TaskCanceledException)
        {
            // Ignorieren, falls später Abbruch hinzugefügt wird
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler beim Abspielen: {ex.Message}");
        }
        finally
        {
            // Gerät schließen (falls gewünscht)
            // midiOut.Close();
            // midiOut.Dispose();
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

    private async Task PlayChordStabAsync(IEnumerable<int> notes, int velocity, int lengthMs, int channel)
    {
        foreach (var n in notes)
        {
            NoteOn(n, velocity, channel);
        }

        await Task.Delay(lengthMs);

        foreach (var n in notes)
        {
            NoteOff(n, channel);
        }
    }

    private async Task DrumHitAsync(int midiNote, int velocity, int channel)
    {
        NoteOn(midiNote, velocity, channel);
        await Task.Delay(25);
        NoteOff(midiNote, channel);
    }
}
