using System;
using System.Collections.Generic;
using System.Linq;

namespace AhnWin52Backup.Core.Paradox;

/// <summary>Contains a Paradox table schema and its active records.</summary>
public sealed class ParadoxTable
{
    public ParadoxTable(
        string name,
        IReadOnlyList<ParadoxField> fields,
        IReadOnlyList<IReadOnlyList<string?>> records)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(fields);
        ArgumentNullException.ThrowIfNull(records);

        ParadoxField[] copiedFields = fields.ToArray();
        if (copiedFields.Length == 0 ||
            copiedFields.Any(static field => field is null || string.IsNullOrWhiteSpace(field.Name) || field.Length <= 0))
        {
            throw new ArgumentException("A Paradox table must have named fields with positive lengths.", nameof(fields));
        }

        HashSet<string> fieldNames = new(StringComparer.OrdinalIgnoreCase);
        if (copiedFields.Any(field => !fieldNames.Add(field.Name)))
        {
            throw new ArgumentException("Paradox field names must be unique.", nameof(fields));
        }

        IReadOnlyList<string?>[] copiedRecords = records
            .Select(record =>
            {
                ArgumentNullException.ThrowIfNull(record);
                string?[] values = record.ToArray();
                if (values.Length != copiedFields.Length)
                {
                    throw new ArgumentException("Each Paradox record must contain exactly one value per field.", nameof(records));
                }

                return (IReadOnlyList<string?>)Array.AsReadOnly(values);
            })
            .ToArray();

        Name = name;
        Fields = Array.AsReadOnly(copiedFields);
        Records = Array.AsReadOnly(copiedRecords);
    }

    /// <summary>Gets the table name stored in its Paradox header.</summary>
    public string Name { get; }

    /// <summary>Gets the physical fields in their declared order.</summary>
    public IReadOnlyList<ParadoxField> Fields { get; }

    /// <summary>Gets active records in physical data-block order.</summary>
    public IReadOnlyList<IReadOnlyList<string?>> Records { get; }
}
