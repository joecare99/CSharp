using System.Collections.Generic;

namespace DataAnalysis.Core.Models;

/// <summary>
/// Beschreibt eine konfigurierbare Liste von Auswertungen, die auf die gelesenen Einträge angewendet werden.
/// Kann später extern geladen werden (z. B. JSON).
/// </summary>
public sealed class AnalysisAggregateProfile
{
    public required IReadOnlyList<AnalysisQuery> Queries { get; init; }

    public static AnalysisAggregateProfile Default => new()
    {
        Queries = new List<AnalysisQuery>
        {
             new AnalysisQuery { Title = "Severity", Dimensions = new [] { DimensionKind.Severity } },
             new AnalysisQuery { Title = "Source", Dimensions = new [] { DimensionKind.Source } },
             new AnalysisQuery { Title = "Hour x Source", Dimensions = new [] { DimensionKind.Hour, DimensionKind.Source } },
             new AnalysisQuery { Title = "Top-Events", Dimensions = new [] { DimensionKind.MessageNormalized }, TopN =100 },
             new AnalysisQuery { Title = "Top-Events2", Dimensions = new [] { DimensionKind.MessageNormalized }, TopN =10 },
             new AnalysisQuery { Title = "Severity x Source", Dimensions = new [] { DimensionKind.Source, DimensionKind.Severity }, Columns = Enum.GetValues<SyslogSeverity>().Select(s => s.ToString()).ToArray() },
             new AnalysisQuery { Title = "Events x Source (Top50)", Dimensions = new [] { DimensionKind.Source, DimensionKind.MessageNormalized }},
        }
    };
}
