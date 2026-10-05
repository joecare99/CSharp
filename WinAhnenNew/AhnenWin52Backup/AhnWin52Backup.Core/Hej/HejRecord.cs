using System;
using System.Collections.Generic;
using System.Linq;

namespace AhnWin52Backup.Core.Hej;

public sealed class HejRecord
{
    public HejRecord(IEnumerable<string> fields)
    {
        ArgumentNullException.ThrowIfNull(fields);

        string[] copiedFields = fields.ToArray();
        if (copiedFields.Any(static field => field is null))
        {
            throw new ArgumentException("HEJ fields cannot be null.", nameof(fields));
        }

        Fields = Array.AsReadOnly(copiedFields);
    }

    public IReadOnlyList<string> Fields { get; }
}
