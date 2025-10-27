using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using DataAnalysis.Core.Export;
using DataAnalysis.Core.Import;

namespace DataAnalysis.Core.Models;

/// <summary>
/// Kern-Model: Liest eine SEW Syslog-Textdatei, führt Auswertungen durch und gibt Ergebnisse an einen Exporter.
/// </summary>
public sealed class SyslogAnalysisModel
{
    private readonly ISyslogAnalysisExporter _exporter;
    private readonly ISyslogEntryReader _slreader;

    public SyslogAnalysisModel(ISyslogAnalysisExporter exporter, ISyslogEntryReader slreader)
    {
        _exporter = exporter;
        _slreader = slreader;
    }

    public async Task<string> AnalyzeAsync(string inputPath, string? outputPath = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(inputPath))
            throw new ArgumentException("Pfad darf nicht leer sein", nameof(inputPath));
        if (!File.Exists(inputPath))
            throw new FileNotFoundException("Eingabedatei nicht gefunden", inputPath);

        var read = await _slreader.ReadAsync(inputPath, cancellationToken).ConfigureAwait(false);
        var result = Aggregate(read);
        return await _exporter.ExportAsync(result, inputPath, outputPath, cancellationToken).ConfigureAwait(false);
    }

    private static SyslogAnalysisResult Aggregate(SyslogReadResult read)
    {
        int total = read.TotalLines, parsed = 0;
        var bySeverity = new Dictionary<SyslogSeverity, int>();
        var bySource = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var byHour = new Dictionary<DateTimeOffset, int>();
        var messageCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        DateTimeOffset? first = null, last = null;

        foreach (var entry in read.Entries)
        {
            parsed++;
            if (entry.Timestamp is DateTimeOffset ts)
            {
                first = first is null || ts < first ? ts : first;
                last = last is null || ts > last ? ts : last;
                var hour = new DateTimeOffset(ts.Year, ts.Month, ts.Day, ts.Hour, 0, 0, ts.Offset);
                byHour[hour] = byHour.TryGetValue(hour, out var cH) ? cH + 1 : 1;
            }
            bySeverity[entry.Severity] = bySeverity.TryGetValue(entry.Severity, out var cS) ? cS + 1 : 1;
            var source = string.IsNullOrWhiteSpace(entry.Source) ? "(unbekannt)" : entry.Source.Trim();
            bySource[source] = bySource.TryGetValue(source, out var cSrc) ? cSrc + 1 : 1;
            var normMsg = NormalizeMessage(entry.Message);
            if (!string.IsNullOrEmpty(normMsg))
                messageCounts[normMsg] = messageCounts.TryGetValue(normMsg, out var cM) ? cM + 1 : 1;
        }

        var topMessages = messageCounts.OrderByDescending(kv => kv.Value)
        .ThenBy(kv => kv.Key)
        .Take(100)
        .Select(kv => (kv.Key, kv.Value))
        .ToList();

        return new SyslogAnalysisResult
        {
            TotalLines = total,
            ParsedLines = parsed,
            FirstTimestamp = first,
            LastTimestamp = last,
            BySeverity = bySeverity,
            BySource = bySource,
            ByHour = byHour,
            TopMessages = topMessages
        };
    }

    private static string NormalizeMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return string.Empty;
        var m = message.Trim();
        // Collapse multiple spaces
        m = Regex.Replace(m, @"\s+", " ");
        // Remove volatile IDs in brackets like (id=123), [0x1A2B], etc.
        m = Regex.Replace(m, @"\((id|ID)=[^)]+\)", "(id=*)");
        m = Regex.Replace(m, @"\[0x[0-9A-Fa-f]+\]", "[0x*]");
        if (m.Length > 500) m = m[..500];
        return m;
    }
}
