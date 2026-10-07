using System;
using System.Collections.Generic;
using System.Linq;
using AhnWin52Backup.Core.Hej;
using AhnWin52Backup.Core.Paradox;

namespace AhnWin52Backup.Core.Workflows;

internal sealed class HejParadoxRecordMapper
{
    private static readonly string[] PhysicalIndividualFields = InsertAt(
        HejDatabaseExportService.IndividualFields,
        33,
        "Bild");
    private static readonly string[] PhysicalMarriageFields = InsertAt(
        HejDatabaseExportService.MarriageFields,
        0,
        "Numr");
    private static readonly string[] PhysicalSourceFields = InsertAt(
        HejDatabaseExportService.SourceFields,
        0,
        "N");

    public IReadOnlyDictionary<string, IReadOnlyList<IReadOnlyDictionary<string, string?>>> Map(
        HejDocument document,
        IReadOnlyDictionary<string, IReadOnlyList<ParadoxField>> tableFields)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentNullException.ThrowIfNull(tableFields);

        return new Dictionary<string, IReadOnlyList<IReadOnlyDictionary<string, string?>>>(
            StringComparer.OrdinalIgnoreCase)
        {
            ["AWD.DB"] = MapSection(
                "AWD.DB",
                HejSection.Individuals,
                document.Individuals,
                tableFields,
                PhysicalIndividualFields),
            ["MRG.DB"] = MapSection(
                "MRG.DB",
                HejSection.Marriages,
                document.Marriages,
                tableFields,
                PhysicalMarriageFields),
            ["adp.DB"] = MapSection(
                "adp.DB",
                HejSection.Adoptions,
                document.Adoptions,
                tableFields,
                HejDatabaseExportService.AdoptionFields),
            ["LOC.DB"] = MapSection(
                "LOC.DB",
                HejSection.Places,
                document.Places,
                tableFields,
                HejDatabaseExportService.PlaceFields),
            ["sour2.DB"] = MapSection(
                "sour2.DB",
                HejSection.Sources,
                document.Sources,
                tableFields,
                PhysicalSourceFields)
        };
    }

    private static IReadOnlyList<IReadOnlyDictionary<string, string?>> MapSection(
        string tableName,
        HejSection section,
        IReadOnlyList<HejRecord> records,
        IReadOnlyDictionary<string, IReadOnlyList<ParadoxField>> tableFields,
        IReadOnlyList<string> physicalFieldNames)
    {
        if (!tableFields.TryGetValue(tableName, out IReadOnlyList<ParadoxField>? fields))
        {
            throw new InvalidOperationException($"The restore template is missing {tableName}.");
        }

        if (!fields.Select(static field => field.Name)
                .SequenceEqual(physicalFieldNames, StringComparer.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"The {tableName} field order does not match the verified HEJ restore mapping.");
        }

        string[] hejFieldNames = GetHejFieldNames(section);
        List<IReadOnlyDictionary<string, string?>> mapped = new(records.Count);
        for (int row = 0; row < records.Count; row++)
        {
            HejRecord record = records[row];
            if (record.Fields.Count != hejFieldNames.Length)
            {
                throw new InvalidOperationException(
                    $"{section} record {row + 1} has {record.Fields.Count} fields; expected {hejFieldNames.Length}.");
            }

            Dictionary<string, string?> values = new(StringComparer.OrdinalIgnoreCase);
            int hejFieldIndex = 0;
            foreach (string physicalField in physicalFieldNames)
            {
                string? value;
                if (IsOmittedField(physicalField, section))
                {
                    value = null;
                }
                else
                {
                    if (hejFieldIndex >= hejFieldNames.Length ||
                        !physicalField.Equals(hejFieldNames[hejFieldIndex], StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException(
                            $"The {tableName} field {physicalField} cannot be mapped from HEJ {section}.");
                    }

                    value = record.Fields[hejFieldIndex++];
                }

                values.Add(physicalField, value);
            }

            if (hejFieldIndex != hejFieldNames.Length)
            {
                throw new InvalidOperationException(
                    $"The {section} mapping consumed {hejFieldIndex} of {hejFieldNames.Length} fields.");
            }

            mapped.Add(values);
        }

        return mapped;
    }

    private static bool IsOmittedField(string fieldName, HejSection section) =>
        (section == HejSection.Individuals && fieldName.Equals("Bild", StringComparison.OrdinalIgnoreCase)) ||
        (section == HejSection.Marriages && fieldName.Equals("Numr", StringComparison.OrdinalIgnoreCase)) ||
        (section == HejSection.Sources && fieldName.Equals("N", StringComparison.OrdinalIgnoreCase));

    private static string[] GetHejFieldNames(HejSection section) => section switch
    {
        HejSection.Individuals => HejDatabaseExportService.IndividualFields,
        HejSection.Marriages => HejDatabaseExportService.MarriageFields,
        HejSection.Adoptions => HejDatabaseExportService.AdoptionFields,
        HejSection.Places => HejDatabaseExportService.PlaceFields,
        HejSection.Sources => HejDatabaseExportService.SourceFields,
        _ => throw new ArgumentOutOfRangeException(nameof(section), section, "Unknown HEJ section.")
    };

    private static string[] InsertAt(IReadOnlyList<string> fields, int index, string fieldName)
    {
        string[] result = new string[fields.Count + 1];
        for (int source = 0, destination = 0; destination < result.Length; destination++)
        {
            result[destination] = destination == index ? fieldName : fields[source++];
        }

        return result;
    }
}
