using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AhnWin52Backup.Core.Abstractions;
using AhnWin52Backup.Core.Hej;
using AhnWin52Backup.Core.Paradox;

namespace AhnWin52Backup.Core.Workflows;

/// <summary>Maps the five AhnWin Paradox tables into their ordered HEJ sections.</summary>
public sealed class HejDatabaseExportService : IHejDatabaseExportService
{
    internal static readonly string[] IndividualFields =
    [
        "Nummer", "Vater", "Mutter", "Name", "Vornamen", "Geschlecht", "Religion", "Beruf",
        "Gebtag", "Gebmonat", "Gebjahr", "Gebort", "Tauftag", "Taufmonat", "Taufjahr", "Taufort",
        "Taufpat", "Lebensort", "Sttag", "Stmonat", "Stjahr", "Stort", "Todesurs", "Begtag",
        "Begmonat", "Begjahr", "Begort", "Quelleg", "Quellet", "Quelles", "Quelleb", "Kommentar",
        "Lebt", "Namex", "IDNR", "Kistat", "Hausname", "Adr1", "Adr2", "PLZ", "Ort", "Adrzus",
        "Indj", "Indm", "Indt", "Alter", "Tel", "Ema", "Ur", "Quelle", "Rufname"
    ];

    internal static readonly string[] MarriageFields =
    [
        "Nummer", "Epnum", "Htag", "Hmonat", "Hjahr", "Hort", "Trauz", "Satag", "Samonat", "Sajahr",
        "Saort", "Satrauz", "Verbind", "Schtag", "Schmonat", "Schjahr", "Schort", "Hqu", "Saqu", "Schqu",
        "Indj", "Indm"
    ];

    internal static readonly string[] AdoptionFields = ["Nummer", "Av", "Am"];
    internal static readonly string[] PlaceFields =
    [
        "Ort", "PLZ", "Land", "RegBez", "Gov", "Bland", "Gde", "Pfr", "Lkr", "Abk", "Lg", "Bg", "Maid"
    ];

    internal static readonly string[] SourceFields =
    [
        "Titel", "Abk", "Ereig", "Von", "Bis", "Standort", "Publ", "Rep", "Bem", "Bestand", "Med"
    ];

    private readonly IParadoxTableReader _tableReader;

    public HejDatabaseExportService(IParadoxTableReader tableReader)
    {
        ArgumentNullException.ThrowIfNull(tableReader);
        _tableReader = tableReader;
    }

    /// <inheritdoc />
    public HejDocument Export(string databaseDirectory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databaseDirectory);
        string fullDirectory = Path.GetFullPath(databaseDirectory);
        if (!Directory.Exists(fullDirectory))
        {
            throw new DirectoryNotFoundException($"Database directory does not exist: {fullDirectory}");
        }

        IReadOnlyList<HejRecord> individuals = ReadSection(
            fullDirectory,
            "AWD.DB",
            IndividualFields,
            HejSection.Individuals,
            "Bild");
        IReadOnlyList<HejRecord> marriages = ReadSection(
            fullDirectory,
            "MRG.DB",
            MarriageFields,
            HejSection.Marriages,
            "Numr");
        IReadOnlyList<HejRecord> adoptions = ReadSection(
            fullDirectory,
            "adp.DB",
            AdoptionFields,
            HejSection.Adoptions);
        IReadOnlyList<HejRecord> places = ReadSection(
            fullDirectory,
            "LOC.DB",
            PlaceFields,
            HejSection.Places);
        IReadOnlyList<HejRecord> sources = ReadSection(
            fullDirectory,
            "sour2.DB",
            SourceFields,
            HejSection.Sources,
            "N");

        return new HejDocument(individuals, marriages, adoptions, places, sources);
    }

    private IReadOnlyList<HejRecord> ReadSection(
        string databaseDirectory,
        string fileName,
        IReadOnlyList<string> expectedFields,
        HejSection section,
        string? omittedField = null)
    {
        string databasePath = FindTablePath(databaseDirectory, fileName);
        ParadoxTable table = _tableReader.Read(databasePath);
        ValidateSchema(table, fileName, expectedFields, omittedField);

        int[] fieldIndexes = expectedFields
            .Where(fieldName => !string.Equals(fieldName, omittedField, StringComparison.OrdinalIgnoreCase))
            .Select(fieldName => FindFieldIndex(table.Fields, fieldName))
            .ToArray();
        int expectedHejFieldCount = HejSchema.GetFieldCount(section);
        if (fieldIndexes.Length != expectedHejFieldCount)
        {
            throw new InvalidDataException(
                $"The {fileName} mapping provides {fieldIndexes.Length} fields for {section}; expected {expectedHejFieldCount}.");
        }

        return table.Records
            .Select(record => new HejRecord(fieldIndexes.Select(index => record[index] ?? string.Empty)))
            .ToArray();
    }

    private static void ValidateSchema(
        ParadoxTable table,
        string fileName,
        IReadOnlyList<string> expectedFields,
        string? omittedField)
    {
        int expectedPhysicalFieldCount = expectedFields.Count + (omittedField is null ? 0 : 1);
        if (table.Fields.Count != expectedPhysicalFieldCount)
        {
            throw new InvalidDataException(
                $"The {fileName} table has {table.Fields.Count} fields; expected {expectedPhysicalFieldCount}.");
        }

        string[] actualNames = table.Fields.Select(static field => field.Name).ToArray();
        string[] expectedNames = expectedFields.ToArray();
        if (omittedField is not null)
        {
            int omittedIndex = fileName.Equals("AWD.DB", StringComparison.OrdinalIgnoreCase) ? 33 : 0;
            expectedNames = expectedNames
                .Take(omittedIndex)
                .Append(omittedField)
                .Concat(expectedNames.Skip(omittedIndex))
                .ToArray();
        }

        if (!actualNames.SequenceEqual(expectedNames, StringComparer.OrdinalIgnoreCase))
        {
            throw new InvalidDataException(
                $"The field order in {fileName} does not match the verified AhnWin-to-HEJ mapping.");
        }

        for (int index = 0; index < table.Fields.Count; index++)
        {
            byte expectedType = GetExpectedFieldType(fileName, actualNames[index]);
            if (table.Fields[index].TypeCode != expectedType)
            {
                throw new InvalidDataException(
                    $"Field \"{actualNames[index]}\" in {fileName} has Paradox type 0x{table.Fields[index].TypeCode:X2}; expected 0x{expectedType:X2}.");
            }
        }
    }

    private static byte GetExpectedFieldType(string fileName, string fieldName)
    {
        if (fileName.Equals("AWD.DB", StringComparison.OrdinalIgnoreCase))
        {
            return fieldName switch
            {
                "Nummer" or "Vater" or "Mutter" => 0x04,
                "Kommentar" => 0x0C,
                "Bild" => 0x10,
                _ => 0x01
            };
        }

        if (fileName.Equals("MRG.DB", StringComparison.OrdinalIgnoreCase))
        {
            return fieldName switch
            {
                "Numr" => 0x16,
                "Nummer" or "Epnum" => 0x04,
                _ => 0x01
            };
        }

        if (fileName.Equals("adp.DB", StringComparison.OrdinalIgnoreCase))
        {
            return 0x04;
        }

        if (fileName.Equals("LOC.DB", StringComparison.OrdinalIgnoreCase))
        {
            return 0x01;
        }

        return fieldName switch
        {
            "N" => 0x16,
            "Publ" or "Bem" => 0x0C,
            _ => 0x01
        };
    }

    private static int FindFieldIndex(IReadOnlyList<ParadoxField> fields, string fieldName)
    {
        for (int index = 0; index < fields.Count; index++)
        {
            if (string.Equals(fields[index].Name, fieldName, StringComparison.OrdinalIgnoreCase))
            {
                return index;
            }
        }

        throw new InvalidDataException($"Required Paradox field \"{fieldName}\" is missing.");
    }

    private static string FindTablePath(string databaseDirectory, string fileName)
    {
        string? path = Directory
            .EnumerateFiles(databaseDirectory, "*", SearchOption.TopDirectoryOnly)
            .FirstOrDefault(candidate =>
                string.Equals(Path.GetFileName(candidate), fileName, StringComparison.OrdinalIgnoreCase));
        return path ?? throw new FileNotFoundException($"Required Paradox table {fileName} was not found.", fileName);
    }
}
