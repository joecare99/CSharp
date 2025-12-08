using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DataAnalysis.Core.Models;

namespace DataAnalysis.Core.Import;

/// <summary>
/// Reader, der anhand eines externen <see cref="SyslogParsingProfile"/> die Felder dynamisch zuordnet.
/// </summary>
public sealed class ConfigurableSyslogEntryReader : ISyslogEntryReader
{
    private readonly SyslogParsingProfile _profile;

    public ConfigurableSyslogEntryReader(SyslogParsingProfile profile)
    {
        _profile = profile ?? throw new ArgumentNullException(nameof(profile));
    }

    public async Task<SyslogReadResult> ReadAsync(string inputPath, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(inputPath))
            throw new ArgumentException("Pfad darf nicht leer sein", nameof(inputPath));
        if (!File.Exists(inputPath))
            throw new FileNotFoundException("Eingabedatei nicht gefunden", inputPath);

        int total = 0;
        var entries = new List<SyslogEntry>(capacity: 1024);
        using var fs = new FileStream(inputPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var reader = new StreamReader(fs, DetectEncoding(fs) ?? Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
        var regex = string.IsNullOrWhiteSpace(_profile.RegexPattern) ? null : new Regex(_profile.RegexPattern!, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        string? line;
        while ((line = await reader.ReadLineAsync().ConfigureAwait(false)) is not null)
        {
            ct.ThrowIfCancellationRequested();
            total++;
            var entry = ParseLine(line, regex);
            if (entry is not null)
            {
                entries.Add(entry);
            }
        }
        return new SyslogReadResult { TotalLines = total, Entries = entries };
    }

    private SyslogEntry? ParseLine(string line, Regex? regex)
    {
        if (string.IsNullOrWhiteSpace(line))
            return null;
        if (regex is not null)
        {
            var m = regex.Match(line);
            if (m.Success)
            {
                var fields = _profile.FieldGroupMap;
                var tsText = TryGet(m, fields, "Timestamp");
                var sevTextRaw = TryGet(m, fields, "Severity");
                var src = TryGet(m, fields, "Source") ?? string.Empty;
                var msg = TryGet(m, fields, "Message") ?? string.Empty;
                var ts = ParseTimestamp(tsText);
                var sev = MapSeverity(sevTextRaw, out var sevText);
                var attributes = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
                foreach (var gName in regex.GetGroupNames())
                {
                    if (int.TryParse(gName, out _))
                        continue; // skip numeric indices
                    if (!fields.Values.Contains(gName, StringComparer.OrdinalIgnoreCase))
                    {
                        var grp = m.Groups[gName];
                        attributes[gName] = grp.Success ? grp.Value : null;
                    }
                }

                // Apply extraction rules against chosen fields (e.g., Message)
                if (_profile.ExtractionRules.Count > 0)
                {
                    foreach (var rule in _profile.ExtractionRules)
                    {
                        string? sourceText = null;
                        if (!string.IsNullOrWhiteSpace(rule.SourceColumn))
                        {
                            var name = rule.SourceColumn;
                            if (name.Equals("Message", StringComparison.OrdinalIgnoreCase)) sourceText = msg;
                            else if (name.Equals("Source", StringComparison.OrdinalIgnoreCase)) sourceText = src;
                            else if (name.Equals("Severity", StringComparison.OrdinalIgnoreCase)) sourceText = sevText;
                        }
                        if (string.IsNullOrEmpty(sourceText)) continue;
                        var rx = new Regex(rule.RegexPattern, rule.Options);
                        var mm = rx.Match(sourceText);
                        if (!mm.Success) continue;
                        foreach (var gName in rx.GetGroupNames())
                        {
                            if (int.TryParse(gName, out _)) continue;
                            var attrName = rule.GroupMap is not null && rule.GroupMap.TryGetValue(gName, out var mapped) ? mapped : gName;
                            attributes[attrName] = mm.Groups[gName].Success ? mm.Groups[gName].Value : null;
                        }
                    }
                }

                return new SyslogEntry(ts, sev, sevText, src, msg) { Attributes = attributes };
            }
            if (!_profile.EnableHeuristics)
                return null;
        }
        // fallback to basic heuristic similar to TextSyslogEntryReader
        return HeuristicParse(line);
    }

    private static string? TryGet(Match m, IDictionary<string, string> map, string key)
    {
        return map.TryGetValue(key, out var grp) && !string.IsNullOrWhiteSpace(grp) && m.Groups[grp].Success
        ? m.Groups[grp].Value
        : null;
    }

    private static Encoding? DetectEncoding(FileStream fs)
    {
        var original = fs.Position;
        Span<byte> bom = stackalloc byte[4];
        fs.Position = 0;
        int read = fs.Read(bom);
        fs.Position = original;
        if (read >= 3 && bom[0] == 0xEF && bom[1] == 0xBB && bom[2] == 0xBF)
            return Encoding.UTF8;
        if (read >= 2 && bom[0] == 0xFF && bom[1] == 0xFE)
            return Encoding.Unicode;
        if (read >= 2 && bom[0] == 0xFE && bom[1] == 0xFF)
            return Encoding.BigEndianUnicode;
        if (read >= 4 && bom[0] == 0xFF && bom[1] == 0xFE && bom[2] == 0x00 && bom[3] == 0x00)
            return Encoding.UTF32;
        if (read >= 4 && bom[0] == 0x00 && bom[1] == 0x00 && bom[2] == 0xFE && bom[3] == 0xFF)
            return new UTF32Encoding(bigEndian: true, byteOrderMark: true);
        return null;
    }

    private SyslogEntry? HeuristicParse(string line)
    {
        // very small heuristic: split at first space for timestamp, then look for severity token
        var ts = (DateTimeOffset?)null;
        var sev = SyslogSeverity.Unknown;
        var sevText = nameof(SyslogSeverity.Unknown);
        var source = string.Empty;
        var message = line.Trim();

        var idx = line.IndexOf(' ');
        if (idx > 0)
        {
            var prefix = line[..idx];
            ts = ParseTimestamp(prefix);
            var rest = line[(idx + 1)..].TrimStart();
            var sep = rest.IndexOf(':');
            if (sep < 0)
                sep = rest.IndexOf('-');
            if (sep > 0)
            {
                source = rest[..sep].Trim();
                message = rest[(sep + 1)..].Trim();
            }
            else
            {
                message = rest;
            }
        }

        return new SyslogEntry(ts, sev, sevText, source, message);
    }

    private DateTimeOffset? ParseTimestamp(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return null;
        raw = raw.Trim().Trim('[', ']');
        foreach (var cultureName in _profile.Cultures)
        {
            var culture = string.IsNullOrEmpty(cultureName) ? CultureInfo.InvariantCulture : CultureInfo.GetCultureInfo(cultureName);
            if (_profile.TimestampFormats.Count > 0 && DateTimeOffset.TryParseExact(raw, _profile.TimestampFormats.ToArray(), culture, DateTimeStyles.AssumeLocal, out var dto))
                return dto;
            if (DateTimeOffset.TryParse(raw, culture, DateTimeStyles.AssumeLocal, out dto))
                return dto;
        }
        return null;
    }

    private SyslogSeverity MapSeverity(string? sevText, out string normalized)
    {
        if (string.IsNullOrWhiteSpace(sevText))
        {
            normalized = nameof(SyslogSeverity.Unknown);
            return SyslogSeverity.Unknown;
        }
        if (_profile.SeverityAliases.TryGetValue(sevText, out var sev))
        {
            normalized = sevText.ToUpperInvariant();
            return sev;
        }
        normalized = nameof(SyslogSeverity.Unknown);
        return SyslogSeverity.Unknown;
    }
}
