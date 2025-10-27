using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using DataAnalysis.Core.Import;

namespace DataAnalysis.Core.Models;

/// <summary>
/// Adapter, der eine DataTable aus einem ITableReader in eine SyslogReadResult-Struktur überführt.
/// Trägt die Abhängigkeit SyslogEntry ausschließlich in diese Schicht.
/// </summary>
public sealed class TableToEntryAdapter : ISyslogEntryReader
{
    private readonly ITableReader _tableReader;
    private readonly Func<string, string, SyslogSeverity> _mapSeverity;

    public TableToEntryAdapter(ITableReader tableReader, Func<string, string, SyslogSeverity> mapSeverity)
    {
        _tableReader = tableReader ?? throw new ArgumentNullException(nameof(tableReader));
        _mapSeverity = mapSeverity ?? throw new ArgumentNullException(nameof(mapSeverity));
    }

    public async Task<SyslogReadResult> ReadAsync(string inputPath, CancellationToken cancellationToken = default)
    {
        var table = await _tableReader.ReadTableAsync(inputPath, cancellationToken).ConfigureAwait(false);
        var entries = new List<SyslogEntry>(table.Rows.Count);
        foreach (DataRow r in table.Rows)
        {
            DateTimeOffset? ts = null;
            if (table.Columns.Contains("Timestamp") && DateTimeOffset.TryParse(Convert.ToString(r["Timestamp"]), out var dto))
                ts = dto;
            var sevText = table.Columns.Contains("Severity") ? Convert.ToString(r["Severity"]) ?? string.Empty : string.Empty;
            var sev = _mapSeverity(sevText, sevText);
            var src = table.Columns.Contains("Source") ? Convert.ToString(r["Source"]) ?? string.Empty : string.Empty;
            var msg = table.Columns.Contains("Message") ? Convert.ToString(r["Message"]) ?? string.Empty : string.Empty;
            var attrs = new Dictionary<string, string?>();
            foreach (DataColumn c in table.Columns)
            {
                var name = c.ColumnName;
                if (name is "Timestamp" or "Severity" or "Source" or "Message")
                    continue;
                attrs[name] = Convert.ToString(r[c])!;
            }
            entries.Add(new SyslogEntry(ts, sev, string.IsNullOrEmpty(sevText) ? nameof(SyslogSeverity.Unknown) : sevText, src, msg) { Attributes = attrs });
        }
        return new SyslogReadResult { TotalLines = table.Rows.Count, Entries = entries };
    }

    private static Dictionary<string, SyslogSeverity> BuildDefaultAliases()
    {
        /*
         Pseudocode:
         - Create case-insensitive dictionary for aliases -> enum value.
         - Define alias groups data-driven: each entry lists alias keys and candidate enum names to parse.
         - For each group, parse the first matching enum name; if successful, add all aliases.
         - Add canonical enum names (all names of TSeverity) to the dictionary.
         - Determine whether the enum underlying type is signed.
         - For each enum value, compute its numeric string (signed or unsigned, depending on underlying type) and add it to the dictionary.
         - Return dictionary.
        */

        var dict = new Dictionary<string, SyslogSeverity>(StringComparer.OrdinalIgnoreCase);

        var aliasGroups = new string[][]
        {
            ["Trace"],
            ["Debug"],
            ["Info", "Information"],
            ["Warn", "Warning"],
            ["Error", "Err"],
            ["Fatal", "Alarm"],
        };

        foreach (var group in aliasGroups)
        {
            if (TryParseAny(out var sev, group))
            {
                foreach (var alias in group)
                    dict[alias.ToUpper()] = sev;
            }
        }

        // Add numeric representations
        var isSigned = _IsSignedType(typeof(SyslogSeverity));

        foreach (var sev in Enum.GetValues<SyslogSeverity>())
        {
            var numeric = isSigned
                ? Convert.ToInt64(sev).ToString(System.Globalization.CultureInfo.InvariantCulture)
                : Convert.ToUInt64(sev).ToString(System.Globalization.CultureInfo.InvariantCulture);

            dict[numeric] = sev;
        }

        return dict;

        static bool _IsSignedType(Type enumType)
        {
            var underlying = Enum.GetUnderlyingType(enumType);
            return underlying == typeof(sbyte) || underlying == typeof(short) || underlying == typeof(int) || underlying == typeof(long);
        }
    }

    private static bool TryParseAny(out SyslogSeverity value, params string[] names)
    {
        foreach (var name in names)
        {
            if (Enum.TryParse(name, true, out value))
                return true;
        }
        value = default;
        return false;
    }
}
