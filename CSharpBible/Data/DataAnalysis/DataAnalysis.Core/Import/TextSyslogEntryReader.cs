using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DataAnalysis.Core.Models;

namespace DataAnalysis.Core.Import;

/// <summary>
/// Liest Syslog-ähnliche Text- oder CSV-Dateien zeilenweise ein und parst Einträge heuristisch/strukturiert.
/// </summary>
public sealed class TextSyslogEntryReader : ISyslogEntryReader
{
    private static readonly Regex SeverityTokenRegex = new(
    pattern: @"(?<![A-Z])(?<sev>TRACE|DEBUG|INFO|WARN|WARNING|ERROR|ERR|FATAL|ALARM)(?![A-Z])",
    options: RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant
    );

    private static readonly Regex StructuredRegex = new(
    pattern: @"^(?<ts>[^\t\-\[]+?)\s+(?<sev>TRACE|DEBUG|INFO|WARN|WARNING|ERROR|ERR|FATAL|ALARM)\s+(?<src>[^:]+):\s*(?<msg>.*)$",
    options: RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant
    );

    private static readonly string[] TimestampCultures = new[]
    {
         CultureInfo.InvariantCulture.Name,
         new CultureInfo("de-DE").Name,
         new CultureInfo("en-US").Name,
    };

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
        string? line;
        while ((line = await reader.ReadLineAsync().ConfigureAwait(false)) is not null)
        {
            ct.ThrowIfCancellationRequested();
            total++;
            var entry = ParseLine(line);
            if (entry is not null)
            {
                entries.Add(entry);
            }
        }

        return new SyslogReadResult
        {
            TotalLines = total,
            Entries = entries
        };
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

    private static SyslogEntry? ParseLine(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
            return null;
        var span = line.AsSpan().Trim();

        // Try structured format: "<timestamp> <sev> <src>: <msg>"
        var m = StructuredRegex.Match(span.ToString());
        if (m.Success)
        {
            var ts = ParseTimestamp(m.Groups["ts"].Value);
            var sevText = m.Groups["sev"].Value;
            var sev = MapSeverity(sevText);
            var src = m.Groups["src"].Value.Trim();
            var msg = m.Groups["msg"].Value.Trim();
            return new SyslogEntry(ts, sev, sevText.ToUpperInvariant(), src, msg);
        }

        // Heuristic parsing
        DateTimeOffset? timestamp = null;
        string severityText = "";
        SyslogSeverity severity = SyslogSeverity.Unknown;
        string source = "";
        string message = span.ToString();

        var sevMatch = SeverityTokenRegex.Match(message);
        if (sevMatch.Success)
        {
            severityText = sevMatch.Groups["sev"].Value.ToUpperInvariant();
            severity = MapSeverity(severityText);
            var afterSev = message[(sevMatch.Index + sevMatch.Length)..].TrimStart();
            var idxSep = afterSev.IndexOf(':');
            if (idxSep < 0)
                idxSep = afterSev.IndexOf('-');
            if (idxSep > 0)
            {
                source = afterSev[..idxSep].Trim();
                message = afterSev[(idxSep + 1)..].Trim();
            }
            else
            {
                message = afterSev;
            }
            var beforeSev = message.Length == afterSev.Length ? span[..sevMatch.Index].ToString() : line[..sevMatch.Index].Trim();
            timestamp = ParseTimestamp(beforeSev);
        }
        else
        {
            var idx = line.IndexOf(' ');
            if (idx > 0)
            {
                var prefix = line[..idx];
                timestamp = ParseTimestamp(prefix);
                if (timestamp is not null)
                {
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
            }
        }

        if (string.IsNullOrWhiteSpace(message))
            return null;
        return new SyslogEntry(timestamp, severity, string.IsNullOrEmpty(severityText) ? nameof(SyslogSeverity.Unknown) : severityText, source, message);
    }

    private static DateTimeOffset? ParseTimestamp(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return null;
        raw = raw.Trim().Trim('[', ']');
        var formats = new[]
        {
 "yyyy-MM-dd HH:mm:ss",
 "yyyy-MM-dd HH:mm:ss,fff",
 "yyyy-MM-dd HH:mm:ss.fff",
 "dd.MM.yyyy HH:mm:ss",
 "dd.MM.yyyy HH:mm:ss,fff",
 "dd.MM.yyyy HH:mm:ss.fff",
 "yy-MM-dd HH:mm:ss",
 "yy-MM-dd HH:mm:ss,fff",
 "yy-MM-dd HH:mm:ss.fff",
 "yyyy-MM-ddTHH:mm:ssK",
 "yyyy-MM-ddTHH:mm:ss.fffK",
 "yyyy-MM-dd HH:mm:ssK",
 "yyyy-MM-dd HH:mm:ss.fffK",
 };
        foreach (var cultureName in new[] { CultureInfo.InvariantCulture.Name, new CultureInfo("de-DE").Name, new CultureInfo("en-US").Name })
        {
            var culture = CultureInfo.GetCultureInfo(cultureName);
            if (DateTimeOffset.TryParseExact(raw, formats, culture, DateTimeStyles.AssumeLocal, out var dto))
                return dto;
            if (DateTimeOffset.TryParse(raw, culture, DateTimeStyles.AssumeLocal, out dto))
                return dto;
        }
        return null;
    }

    private static SyslogSeverity MapSeverity(string sev)
    {
        sev = sev.ToUpperInvariant();
        return sev switch
        {
            "TRACE" => SyslogSeverity.Trace,
            "DEBUG" => SyslogSeverity.Debug,
            "INFO" => SyslogSeverity.Info,
            "WARN" or "WARNING" => SyslogSeverity.Warn,
            "ERROR" or "ERR" => SyslogSeverity.Error,
            "FATAL" or "ALARM" => SyslogSeverity.Fatal,
            _ => SyslogSeverity.Unknown
        };
    }
}
