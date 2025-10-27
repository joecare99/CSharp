namespace DataAnalysis.Core.Models;

public sealed class SyslogAnalysisResult
{
 public int TotalLines { get; init; }
 public int ParsedLines { get; init; }
 public int SkippedLines => TotalLines - ParsedLines;

 public DateTimeOffset? FirstTimestamp { get; init; }
 public DateTimeOffset? LastTimestamp { get; init; }

 public IReadOnlyDictionary<SyslogSeverity, int> BySeverity { get; init; } = new Dictionary<SyslogSeverity, int>();
 public IReadOnlyDictionary<string, int> BySource { get; init; } = new Dictionary<string, int>();
 public IReadOnlyDictionary<DateTimeOffset, int> ByHour { get; init; } = new Dictionary<DateTimeOffset, int>();
 public IReadOnlyList<(string Message, int Count)> TopMessages { get; init; } = Array.Empty<(string, int)>();
}
