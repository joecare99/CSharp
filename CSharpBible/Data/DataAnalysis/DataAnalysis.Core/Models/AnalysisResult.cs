namespace DataAnalysis.Core.Models;

public sealed class AnalysisResult
{
 public int TotalLines { get; init; }
 public int ParsedLines { get; init; }
 public int SkippedLines => TotalLines - ParsedLines;

 public DateTimeOffset? FirstTimestamp { get; init; }
 public DateTimeOffset? LastTimestamp { get; init; }

 // Alle Auswertungen als Aggregationen
 public IReadOnlyList<AggregationResult> Aggregations { get; init; } = Array.Empty<AggregationResult>();
}
