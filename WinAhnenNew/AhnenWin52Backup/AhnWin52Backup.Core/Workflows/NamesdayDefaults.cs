using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using AhnWin52Backup.Core.Paradox;

namespace AhnWin52Backup.Core.Workflows;

/// <summary>Baseline namesday rows for chnt.DB, independent of the HEJ data sections.</summary>
public static class NamesdayDefaults
{
    private static readonly string[] FieldNames = ["Nr", "FELD001", "FELD002", "FELD003"];

    public static IReadOnlyList<IReadOnlyDictionary<string, string?>> Read()
    {
        using Stream resource = typeof(NamesdayDefaults).Assembly.GetManifestResourceStream(
            "AhnWin52Backup.Core.namesdays.json")
            ?? throw new InvalidDataException("The embedded namesday baseline is missing.");
        using JsonDocument document = JsonDocument.Parse(resource);
        JsonElement root = document.RootElement;
        if (root.ValueKind != JsonValueKind.Object ||
            root.GetProperty("table").GetString() != "chnt.DB" ||
            root.GetProperty("fields").EnumerateArray()
                .Select(static element => element.GetString())
                .Where(static value => value is not null)
                .SequenceEqual(FieldNames) == false)
        {
            throw new InvalidDataException("The embedded namesday schema is invalid.");
        }

        JsonElement rows = root.GetProperty("rows");
        if (rows.ValueKind != JsonValueKind.Array || rows.GetArrayLength() != 168)
        {
            throw new InvalidDataException("The embedded namesday baseline must contain 168 rows.");
        }

        List<IReadOnlyDictionary<string, string?>> result = new(168);
        int expectedNumber = 0;
        foreach (JsonElement row in rows.EnumerateArray())
        {
            if (row.ValueKind != JsonValueKind.Array || row.GetArrayLength() != FieldNames.Length ||
                row[0].GetInt32() != ++expectedNumber)
            {
                throw new InvalidDataException("The embedded namesday numbering is invalid.");
            }

            string name = row[1].GetString() ?? throw new InvalidDataException("A namesday name is missing.");
            string day = row[2].GetString() ?? throw new InvalidDataException("A namesday day is missing.");
            string month = row[3].GetString() ?? throw new InvalidDataException("A namesday month is missing.");
            if (string.IsNullOrWhiteSpace(name) || name.Length > 25 || day.Length != 2 || month.Length != 2 ||
                !int.TryParse(day, NumberStyles.None, CultureInfo.InvariantCulture, out int parsedDay) ||
                !int.TryParse(month, NumberStyles.None, CultureInfo.InvariantCulture, out int parsedMonth) ||
                parsedMonth is < 1 or > 12 || parsedDay is < 1 or > 31)
            {
                throw new InvalidDataException("An embedded namesday value is invalid.");
            }

            result.Add(new Dictionary<string, string?>
            {
                ["Nr"] = expectedNumber.ToString(CultureInfo.InvariantCulture),
                ["FELD001"] = name,
                ["FELD002"] = day,
                ["FELD003"] = month
            });
        }

        return result;
    }

    public static void ValidateSchema(ParadoxTableSchema schema)
    {
        ArgumentNullException.ThrowIfNull(schema);
        byte[] expectedTypes = [0x04, 0x01, 0x01, 0x01];
        int[] expectedLengths = [4, 25, 2, 2];
        if (schema.Fields.Count != FieldNames.Length ||
            Enumerable.Range(0, FieldNames.Length).Any(index =>
                !schema.Fields[index].Name.Equals(FieldNames[index], StringComparison.OrdinalIgnoreCase) ||
                schema.Fields[index].TypeCode != expectedTypes[index] ||
                schema.Fields[index].Length != expectedLengths[index]))
        {
            throw new InvalidDataException("The chnt.DB template does not match the namesday baseline schema.");
        }
    }
}
