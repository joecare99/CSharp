using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using DataAnalysis.Core.Export;
using DataAnalysis.Core.Export.Interfaces;
using DataAnalysis.Core.Import;

namespace DataAnalysis.Core.Models;

public sealed partial class AnalysisModel
{
    private readonly ISyslogAnalysisExporter _exporter;
    private readonly ISyslogEntryReader _slreader;
    private readonly AnalysisAggregateProfile _profile;

    public AnalysisModel(ISyslogAnalysisExporter exporter, ISyslogEntryReader slreader, AnalysisAggregateProfile? profile = null)
    {
        _exporter = exporter;
        _slreader = slreader;
        _profile = profile ?? AnalysisAggregateProfile.Default;
    }

    public async Task<AnalysisResult> AnalyzeAsync(string inputPath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(inputPath)) throw new ArgumentException("Pfad darf nicht leer sein", nameof(inputPath));
        if (!File.Exists(inputPath)) throw new FileNotFoundException("Eingabedatei nicht gefunden", inputPath);

        var read = await _slreader.ReadAsync(inputPath, cancellationToken).ConfigureAwait(false);
        var meta = AggregateMeta(read);
        var aggregations = BuildAggregations(read, _profile);
        return new AnalysisResult
        {
            TotalLines = meta.TotalLines,
            ParsedLines = meta.ParsedLines,
            FirstTimestamp = meta.First,
            LastTimestamp = meta.Last,
            GlobalFilterText = FilterTextFormatter.Describe(_profile.GlobalFilter),
            Aggregations = aggregations
        };
    }

    private static (int TotalLines, int ParsedLines, DateTimeOffset? First, DateTimeOffset? Last) AggregateMeta(SyslogReadResult read)
    {
        int total = read.TotalLines, parsed =0;
        DateTimeOffset? first = null, last = null;
        foreach (var entry in read.Entries)
        {
            parsed++;
            if (entry.Timestamp is DateTimeOffset ts)
            {
                first = first is null || ts < first ? ts : first;
                last = last is null || ts > last ? ts : last;
            }
        }
        return (total, parsed, first, last);
    }

    public static string NormalizeMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return string.Empty;
        var m = message.Trim();
        m = Regex.Replace(m, @"\s+", " ");
        m = Regex.Replace(m, @"\((id|ID)=[^)]+\)", "(id=*)");
        m = Regex.Replace(m, @"\[0x[0-9A-Fa-f]+\]", "[0x*]");
        if (m.Length >500) m = m[..500];
        return m;
    }

    private static IReadOnlyList<AggregationResult> BuildAggregations(SyslogReadResult read, AnalysisAggregateProfile profile)
    {
        var list = new List<AggregationResult>();
        if (profile.Queries.Count ==0) return list;

        var globalPredicate = FilterCompiler.Compile(profile.GlobalFilter);

        var items = profile.Queries
        .Select(q =>
        {
            var qb = new QueryBuilder(q) { FilterText = FilterTextFormatter.Describe(q.Filter) };
            return new { Builder = qb, Predicate = Combine(globalPredicate, FilterCompiler.Compile(q.Filter)) };
        })
        .ToArray();

        foreach (var e in read.Entries)
        {
            foreach (var it in items)
            {
                if (it.Predicate is null || it.Predicate(e))
                {
                    it.Builder.Observe(e);
                }
            }
        }

        foreach (var it in items) list.Add(it.Builder.Build());
        return list;
    }

    private static Func<SyslogEntry,bool>? Combine(Func<SyslogEntry,bool>? a, Func<SyslogEntry,bool>? b)
    {
        if (a is null) return b;
        if (b is null) return a;
        return e => a(e) && b(e);
    }
}
