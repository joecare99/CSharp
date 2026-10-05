using System;
using System.Collections.Generic;
using System.Linq;

namespace AhnWin52Backup.Core.Hej;

public sealed class HejDocument
{
    public HejDocument(
        IEnumerable<HejRecord> individuals,
        IEnumerable<HejRecord> marriages,
        IEnumerable<HejRecord> adoptions,
        IEnumerable<HejRecord> places,
        IEnumerable<HejRecord> sources,
        IEnumerable<string>? warnings = null)
    {
        Individuals = CopyAndValidate(individuals, HejSection.Individuals);
        Marriages = CopyAndValidate(marriages, HejSection.Marriages);
        Adoptions = CopyAndValidate(adoptions, HejSection.Adoptions);
        Places = CopyAndValidate(places, HejSection.Places);
        Sources = CopyAndValidate(sources, HejSection.Sources);
        Warnings = Array.AsReadOnly(warnings?.ToArray() ?? []);
    }

    public IReadOnlyList<HejRecord> Individuals { get; }

    public IReadOnlyList<HejRecord> Marriages { get; }

    public IReadOnlyList<HejRecord> Adoptions { get; }

    public IReadOnlyList<HejRecord> Places { get; }

    public IReadOnlyList<HejRecord> Sources { get; }

    public IReadOnlyList<string> Warnings { get; }

    public IReadOnlyList<HejRecord> GetRecords(HejSection section) => section switch
    {
        HejSection.Individuals => Individuals,
        HejSection.Marriages => Marriages,
        HejSection.Adoptions => Adoptions,
        HejSection.Places => Places,
        HejSection.Sources => Sources,
        _ => throw new ArgumentOutOfRangeException(nameof(section), section, "Unknown HEJ section.")
    };

    private static IReadOnlyList<HejRecord> CopyAndValidate(IEnumerable<HejRecord> records, HejSection section)
    {
        ArgumentNullException.ThrowIfNull(records);

        HejRecord[] copiedRecords = records.ToArray();
        int expectedFieldCount = HejSchema.GetFieldCount(section);
        for (int index = 0; index < copiedRecords.Length; index++)
        {
            HejRecord? record = copiedRecords[index];
            if (record is null)
            {
                throw new ArgumentException($"Record {index + 1} in {section} cannot be null.", nameof(records));
            }

            if (record.Fields.Count != expectedFieldCount)
            {
                throw new ArgumentException(
                    $"Record {index + 1} in {section} has {record.Fields.Count} fields; expected {expectedFieldCount}.",
                    nameof(records));
            }
        }

        return Array.AsReadOnly(copiedRecords);
    }
}
