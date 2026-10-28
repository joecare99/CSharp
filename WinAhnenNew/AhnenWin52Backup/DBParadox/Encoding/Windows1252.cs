using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AhnWin52Backup.Core.Hej;

internal static class Windows1252
{
    private static readonly char[] ExtendedCharacters =
    [
        '\u20AC', '\u0081', '\u201A', '\u0192', '\u201E', '\u2026', '\u2020', '\u2021',
        '\u02C6', '\u2030', '\u0160', '\u2039', '\u0152', '\u008D', '\u017D', '\u008F',
        '\u0090', '\u2018', '\u2019', '\u201C', '\u201D', '\u2022', '\u2013', '\u2014',
        '\u02DC', '\u2122', '\u0161', '\u203A', '\u0153', '\u009D', '\u017E', '\u0178'
    ];

    private static readonly IReadOnlyDictionary<char, byte> ExtendedByteValues = CreateExtendedByteValues();

    public static string Decode(ReadOnlySpan<byte> bytes)
    {
        StringBuilder text = new(bytes.Length);
        foreach (byte value in bytes)
        {
            if (value is >= 0x80 and <= 0x9F)
            {
                text.Append(ExtendedCharacters[value - 0x80]);
            }
            else
            {
                text.Append((char)value);
            }
        }

        return text.ToString();
    }

    public static void Encode(ReadOnlySpan<char> text, Stream destination)
    {
        foreach (char character in text)
        {
            if (character <= '\u007F' || character is >= '\u00A0' and <= '\u00FF')
            {
                destination.WriteByte((byte)character);
            }
            else if (ExtendedByteValues.TryGetValue(character, out byte value))
            {
                destination.WriteByte(value);
            }
            else
            {
                destination.WriteByte((byte)'?');
            }
        }
    }

    private static IReadOnlyDictionary<char, byte> CreateExtendedByteValues()
    {
        Dictionary<char, byte> values = new();
        for (int index = 0; index < ExtendedCharacters.Length; index++)
        {
            values.TryAdd(ExtendedCharacters[index], (byte)(index + 0x80));
        }

        return values;
    }
}
