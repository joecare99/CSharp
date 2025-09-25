using System;
using NAudio.Midi;
using AsteroidsModern.Engine.Abstractions;

namespace AsteroidsModern.UI;

// Simple MIDI-based sound implementation using NAudio's MidiOut.
// Maps game events to different MIDI notes and instruments.
public sealed class MidiSound : ISound, IDisposable
{
    private readonly MidiOut? _midi;
    private readonly int _channel;

    // Basic mappings
    private const int Instrument_Thrust = 90; // Pad 2 (warm)
    private const int Instrument_Shoot = 81;  // Lead 1 (square)
    private const int Instrument_Bang = 118;  // Synth Drum

    private const int VelocityLow = 80;
    private const int VelocityMid = 100;
    private const int VelocityHigh = 127;

    public MidiSound(int deviceIndex = 0, int channel = 0)
    {
        _channel = Math.Clamp(channel, 1, 16);
        try
        {
            if (MidiOut.NumberOfDevices > 0)
            {
                _midi = new MidiOut(Math.Clamp(deviceIndex, 0, MidiOut.NumberOfDevices - 1));
            }
        }
        catch
        {
            _midi = null; // Fallback to silent if MIDI not available
        }
    }

    public void PlayThrust()
    {
        if (_midi is null) return;
        ProgramChange(Instrument_Thrust);
        // Play a low, soft note to simulate a thruster pulse
        NoteOn(40, VelocityLow);
        NoteOffSoon(40);
    }

    public void PlayShoot()
    {
        if (_midi is null) return;
        ProgramChange(Instrument_Shoot);
        // Short high note
        NoteOn(76, VelocityHigh);
        NoteOffSoon(76);
    }

    public void PlayBang()
    {
        if (_midi is null) return;
        ProgramChange(Instrument_Bang);
        // Cluster hit effect via two quick notes
        NoteOn(38, VelocityMid);
        NoteOffSoon(38);
        NoteOn(50, VelocityMid);
        NoteOffSoon(50);
    }

    private void ProgramChange(int instrument)
    {
        _midi?.Send(MidiMessage.ChangePatch(instrument, _channel).RawData);
    }

    private void NoteOn(int note, int velocity)
    {
        _midi?.Send(MidiMessage.StartNote(note, velocity, _channel).RawData);
    }

    private void NoteOffSoon(int note)
    {
        // Use a quick Note Off via zero velocity Note On for simplicity
        _midi?.Send(MidiMessage.StartNote(note, 0, _channel).RawData);
    }

    public void Dispose()
    {
        _midi?.Dispose();
    }
}
