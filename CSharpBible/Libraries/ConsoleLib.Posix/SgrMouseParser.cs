using System;
using System.Collections.Generic;
using System.Drawing;
using ConsoleLib.Interfaces;

namespace ConsoleLib.Posix;

/// <summary>Decodes SGR (1006) mouse reports into zero-based pointer events.</summary>
public sealed class SgrMouseParser
{
    private const string Prefix = "\u001b[<";
    private string _pending = string.Empty;

    public IReadOnlyList<PointerInput> Decode(string input)
    {
        if (input is null) throw new ArgumentNullException(nameof(input));
        _pending += input;
        var result = new List<PointerInput>();
        var index = 0;
        while (index < _pending.Length)
        {
            var start = _pending.IndexOf(Prefix, index, StringComparison.Ordinal);
            if (start < 0)
            {
                index = Math.Max(0, _pending.Length - Prefix.Length + 1);
                break;
            }

            var end = FindTerminator(start + Prefix.Length);
            if (end < 0)
            {
                index = start;
                break;
            }

            if (TryParse(_pending.Substring(start + Prefix.Length, end - start - Prefix.Length), _pending[end], out var pointer))
                result.Add(pointer);
            index = end + 1;
        }

        _pending = index < _pending.Length ? _pending[index..] : string.Empty;
        return result.ToArray();
    }

    public IReadOnlyList<PointerInput> Flush()
    {
        _pending = string.Empty;
        return Array.Empty<PointerInput>();
    }

    private int FindTerminator(int start)
    {
        for (var index = start; index < _pending.Length; index++)
            if (_pending[index] is 'M' or 'm')
                return index;
        return -1;
    }

    private static bool TryParse(string payload, char terminator, out PointerInput pointer)
    {
        pointer = default;
        var first = payload.IndexOf(';');
        var second = first < 0 ? -1 : payload.IndexOf(';', first + 1);
        if (first <= 0 || second <= first + 1 || !int.TryParse(payload[..first], out var button)
            || !int.TryParse(payload[(first + 1)..second], out var column)
            || !int.TryParse(payload[(second + 1)..], out var row)
            || column < 1 || row < 1)
            return false;

        var modifiers = KeyModifiers.None;
        if ((button & 4) != 0) modifiers |= KeyModifiers.Shift;
        if ((button & 8) != 0) modifiers |= KeyModifiers.Alt;
        if ((button & 16) != 0) modifiers |= KeyModifiers.Control;
        var baseButton = button & 3;
        var buttons = baseButton switch
        {
            0 => PointerButtons.Left,
            1 => PointerButtons.Middle,
            2 => PointerButtons.Right,
            _ => PointerButtons.None
        };

        if ((button & 64) != 0)
        {
            pointer = new PointerInput(new Point(column - 1, row - 1), PointerInputKind.Wheel, buttons, button == 64 ? 120 : -120, modifiers);
        }
        else if ((button & 32) != 0)
        {
            pointer = new PointerInput(new Point(column - 1, row - 1), PointerInputKind.Move, buttons, 0, modifiers);
        }
        else
        {
            pointer = new PointerInput(new Point(column - 1, row - 1),
                terminator == 'M' ? PointerInputKind.Press : PointerInputKind.Release, buttons, 0, modifiers);
        }

        return true;
    }
}
