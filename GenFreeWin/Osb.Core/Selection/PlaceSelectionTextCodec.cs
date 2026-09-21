using System;
using System.Collections.Generic;
using System.Linq;

namespace Osb.Core.Selection;

/// <summary>
/// Converts the logical entries of a saved place selection to and from the line-oriented format
/// historically used by OSB <c>.OSP</c> files.
/// </summary>
public static class PlaceSelectionTextCodec
{
    private const string LineEnding = "\r\n";

    /// <summary>
    /// Serializes selection entries as deterministic CRLF-separated text.
    /// </summary>
    /// <param name="entries">The ordered non-empty entries selected by the user.</param>
    /// <returns>The serialized selection text without a trailing line ending.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="entries"/> or an entry is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when an entry is empty or contains a line break.
    /// </exception>
    public static string Serialize(IEnumerable<string> entries)
    {
        if (entries == null)
        {
            throw new ArgumentNullException(nameof(entries));
        }

        var serializedEntries = entries.Select(ValidateEntry);
        return string.Join(LineEnding, serializedEntries);
    }

    /// <summary>
    /// Deserializes selection entries from CRLF, LF, or CR line-oriented text.
    /// Empty trailing lines are ignored because they do not represent selectable places.
    /// </summary>
    /// <param name="text">The persisted place-selection text.</param>
    /// <returns>The ordered selection entries.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="text"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when a non-trailing line is empty.
    /// </exception>
    public static IReadOnlyList<string> Deserialize(string text)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        if (text.Length == 0)
        {
            return Array.Empty<string>();
        }

        var entries = text
            .Replace("\r\n", "\n")
            .Replace('\r', '\n')
            .Split('\n');

        var entryCount = entries.Length;
        while (entryCount > 0 && entries[entryCount - 1].Length == 0)
        {
            entryCount--;
        }

        var result = new string[entryCount];
        for (var index = 0; index < entryCount; index++)
        {
            result[index] = ValidateEntry(entries[index]);
        }

        return result;
    }

    private static string ValidateEntry(string entry)
    {
        if (entry == null)
        {
            throw new ArgumentNullException(nameof(entry));
        }

        if (entry.Length == 0)
        {
            throw new ArgumentException("A place selection entry cannot be empty.", nameof(entry));
        }

        if (entry.Contains('\r') || entry.Contains('\n'))
        {
            throw new ArgumentException("A place selection entry cannot contain a line break.", nameof(entry));
        }

        return entry;
    }
}
