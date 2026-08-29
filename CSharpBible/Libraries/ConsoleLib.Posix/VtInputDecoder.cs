using System;
using System.Collections.Generic;
using ConsoleLib.Interfaces;

namespace ConsoleLib.Posix;

/// <summary>Decodes the common VT sequences emitted by POSIX terminals.</summary>
public sealed class VtInputDecoder : IVtInputDecoder
{
    private string _pending = string.Empty;

    public IReadOnlyList<KeyInput> Decode(string input)
    {
        if (input is null) throw new ArgumentNullException(nameof(input));
        _pending += input;
        var result = new List<KeyInput>();
        var index = 0;

        while (index < _pending.Length)
        {
            var current = _pending[index];
            if (current == '\u001b')
            {
                if (index + 1 >= _pending.Length)
                    break;
                if (index + 2 >= _pending.Length && _pending[index + 1] == '[')
                    break;

                if (index + 2 < _pending.Length && _pending[index + 1] == '[')
                {
                    var key = _pending[index + 2] switch
                    {
                        'A' => ConsoleKey.UpArrow,
                        'B' => ConsoleKey.DownArrow,
                        'C' => ConsoleKey.RightArrow,
                        'D' => ConsoleKey.LeftArrow,
                        _ => ConsoleKey.NoName
                    };
                    if (key != ConsoleKey.NoName)
                    {
                        result.Add(Key(key));
                        index += 3;
                        continue;
                    }
                }

                result.Add(Key(ConsoleKey.Escape));
                index++;
                continue;
            }

            result.Add(current switch
            {
                '\r' or '\n' => Key(ConsoleKey.Enter),
                '\t' => Key(ConsoleKey.Tab),
                '\b' or '\u007f' => Key(ConsoleKey.Backspace),
                _ => new KeyInput(MapPrintableKey(current), current, KeyModifiers.None, true)
            });
            index++;
        }

        _pending = _pending[index..];
        return result.ToArray();
    }

    public IReadOnlyList<KeyInput> Flush()
    {
        if (_pending.Length == 0) return Array.Empty<KeyInput>();
        var result = DecodePendingAsLiteral();
        _pending = string.Empty;
        return result;
    }

    private IReadOnlyList<KeyInput> DecodePendingAsLiteral()
    {
        var result = new List<KeyInput>(_pending.Length);
        foreach (var character in _pending)
            result.Add(character == '\u001b'
                ? Key(ConsoleKey.Escape)
                : new KeyInput(MapPrintableKey(character), character, KeyModifiers.None, true));
        return result.ToArray();
    }

    private static KeyInput Key(ConsoleKey key) => new(key, '\0', KeyModifiers.None, true);

    private static ConsoleKey MapPrintableKey(char character)
    {
        if (character >= 'a' && character <= 'z')
            return (ConsoleKey)((int)ConsoleKey.A + character - 'a');
        if (character >= 'A' && character <= 'Z')
            return (ConsoleKey)((int)ConsoleKey.A + character - 'A');
        return ConsoleKey.NoName;
    }
}
