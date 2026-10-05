using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AhnWin52Backup.Core.Abstractions;

namespace AhnWin52Backup.Core.Hej;

public sealed class HejCodec : IHejReader, IHejWriter
{
    private static readonly HejSection[] OrderedSections =
    [
        HejSection.Individuals,
        HejSection.Marriages,
        HejSection.Adoptions,
        HejSection.Places,
        HejSection.Sources
    ];

    public HejDocument Read(Stream source)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (!source.CanRead)
        {
            throw new ArgumentException("The HEJ source stream must be readable.", nameof(source));
        }

        using MemoryStream buffer = new();
        source.CopyTo(buffer);
        byte[] bytes = buffer.ToArray();
        List<byte[]> lines = SplitLines(bytes);
        Dictionary<HejSection, List<HejRecord>> records = OrderedSections.ToDictionary(
            static section => section,
            static _ => new List<HejRecord>());
        Dictionary<(HejSection Section, byte Value), int> ignoredControls = new();
        int sectionIndex = 0;
        int lineNumber = 0;

        foreach (byte[] line in lines)
        {
            lineNumber++;
            if (TryGetSectionHeader(line, out HejSection headerSection))
            {
                if (headerSection == HejSection.Individuals ||
                    sectionIndex >= OrderedSections.Length - 1 ||
                    OrderedSections[sectionIndex + 1] != headerSection)
                {
                    throw new HejFormatException(
                        $"Unexpected or out-of-order section header \"{HejSchema.GetHeader(headerSection)}\".",
                        lineNumber,
                        headerSection);
                }

                sectionIndex++;
                continue;
            }

            HejSection currentSection = OrderedSections[sectionIndex];
            HejRecord record = ParseRecord(line, currentSection, lineNumber, ignoredControls);
            records[currentSection].Add(record);
        }

        if (sectionIndex != OrderedSections.Length - 1)
        {
            HejSection missingSection = OrderedSections[sectionIndex + 1];
            throw new HejFormatException(
                $"Required section header \"{HejSchema.GetHeader(missingSection)}\" is missing.",
                lines.Count + 1,
                missingSection);
        }

        string[] warnings = ignoredControls
            .Select(static control =>
                $"{control.Key.Section}: control byte 0x{control.Key.Value:X2} occurs in the file " +
                $"(first seen on line {control.Value}) and is discarded by the legacy reader.")
            .ToArray();

        return new HejDocument(
            records[HejSection.Individuals],
            records[HejSection.Marriages],
            records[HejSection.Adoptions],
            records[HejSection.Places],
            records[HejSection.Sources],
            warnings);
    }

    public void Write(HejDocument document, Stream destination)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentNullException.ThrowIfNull(destination);
        if (!destination.CanWrite)
        {
            throw new ArgumentException("The HEJ destination stream must be writable.", nameof(destination));
        }

        foreach (HejSection section in OrderedSections)
        {
            string? header = HejSchema.GetHeader(section);
            if (header is not null)
            {
                WriteAscii(header, destination);
                WriteCrLf(destination);
            }

            foreach (HejRecord record in document.GetRecords(section))
            {
                WriteRecord(record, destination);
                WriteCrLf(destination);
            }
        }
    }

    private static List<byte[]> SplitLines(byte[] bytes)
    {
        List<byte[]> lines = new();
        int lineStart = 0;
        int index = 0;

        while (index < bytes.Length)
        {
            if (bytes[index] == 0x0A)
            {
                throw new HejFormatException("A bare line-feed is not valid; expected CRLF.", lines.Count + 1);
            }

            if (bytes[index] != 0x0D)
            {
                index++;
                continue;
            }

            if (index + 1 >= bytes.Length || bytes[index + 1] != 0x0A)
            {
                throw new HejFormatException("A bare carriage return is not valid; expected CRLF.", lines.Count + 1);
            }

            if (index == lineStart)
            {
                throw new HejFormatException("Blank records are not valid.", lines.Count + 1);
            }

            lines.Add(bytes.AsSpan(lineStart, index - lineStart).ToArray());
            index += 2;
            lineStart = index;
        }

        if (lineStart < bytes.Length)
        {
            lines.Add(bytes.AsSpan(lineStart).ToArray());
        }

        if (lines.Count == 0)
        {
            throw new HejFormatException("The HEJ file is empty.", 1);
        }

        return lines;
    }

    private static bool TryGetSectionHeader(byte[] line, out HejSection section)
    {
        foreach (HejSection candidate in OrderedSections.Skip(1))
        {
            string header = HejSchema.GetHeader(candidate)!;
            if (line.Length != header.Length)
            {
                continue;
            }

            bool matches = true;
            for (int index = 0; index < header.Length; index++)
            {
                if (line[index] != (byte)header[index])
                {
                    matches = false;
                    break;
                }
            }

            if (matches)
            {
                section = candidate;
                return true;
            }
        }

        section = default;
        return false;
    }

    private static HejRecord ParseRecord(
        byte[] line,
        HejSection section,
        int lineNumber,
        IDictionary<(HejSection Section, byte Value), int> ignoredControls)
    {
        List<string> fields = new();
        List<byte> fieldBytes = new();

        foreach (byte value in line)
        {
            if (value == 0x0F)
            {
                fields.Add(DecodeField(fieldBytes));
                fieldBytes.Clear();
            }
            else if (value is < 0x20 and not 0x10)
            {
                ignoredControls.TryAdd((section, value), lineNumber);
            }
            else
            {
                fieldBytes.Add(value);
            }
        }

        fields.Add(DecodeField(fieldBytes));
        int expectedFieldCount = HejSchema.GetFieldCount(section);
        int minimumFieldCount = HejSchema.GetMinimumLegacyFieldCount(section);
        if (fields.Count < minimumFieldCount || fields.Count > expectedFieldCount)
        {
            throw new HejFormatException(
                $"Record has {fields.Count} fields; expected between {minimumFieldCount} and {expectedFieldCount}.",
                lineNumber,
                section);
        }

        while (fields.Count < expectedFieldCount)
        {
            fields.Add(string.Empty);
        }

        return new HejRecord(fields);
    }

    private static string DecodeField(List<byte> fieldBytes)
    {
        List<byte> textBytes = new(fieldBytes.Count);
        StringBuilder result = new(fieldBytes.Count);
        foreach (byte value in fieldBytes)
        {
            if (value == 0x10)
            {
                result.Append(Windows1252.Decode(textBytes.ToArray()));
                textBytes.Clear();
                result.Append('\n');
            }
            else
            {
                textBytes.Add(value);
            }
        }

        result.Append(Windows1252.Decode(textBytes.ToArray()));

        return result.ToString();
    }

    private static void WriteRecord(HejRecord record, Stream destination)
    {
        for (int fieldIndex = 0; fieldIndex < record.Fields.Count; fieldIndex++)
        {
            if (fieldIndex > 0)
            {
                destination.WriteByte(0x0F);
            }

            WriteField(record.Fields[fieldIndex], destination);
        }
    }

    private static void WriteField(string field, Stream destination)
    {
        for (int index = 0; index < field.Length; index++)
        {
            char character = field[index];
            if (character == '\r')
            {
                if (index + 1 < field.Length && field[index + 1] == '\n')
                {
                    index++;
                }

                destination.WriteByte(0x10);
            }
            else if (character == '\n')
            {
                destination.WriteByte(0x10);
            }
            else if (character is '\u000F' or '\u0010')
            {
                destination.WriteByte((byte)' ');
            }
            else if (character < '\u0020')
            {
                throw new ArgumentException(
                    $"HEJ fields cannot contain control character U+{(int)character:X4}.");
            }
            else
            {
                Windows1252.Encode(field.AsSpan(index, 1), destination);
            }
        }
    }

    private static void WriteAscii(string value, Stream destination)
    {
        foreach (char character in value)
        {
            destination.WriteByte((byte)character);
        }
    }

    private static void WriteCrLf(Stream destination)
    {
        destination.WriteByte(0x0D);
        destination.WriteByte(0x0A);
    }
}
