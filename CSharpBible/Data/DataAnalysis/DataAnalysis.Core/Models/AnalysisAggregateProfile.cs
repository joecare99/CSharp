using System.Collections.Generic;

namespace DataAnalysis.Core.Models;

/// <summary>
/// Beschreibt eine konfigurierbare Liste von Auswertungen, die auf die gelesenen Einträge angewendet werden.
/// Kann später extern geladen werden (z. B. JSON).
/// </summary>
public sealed class AnalysisAggregateProfile
{
    // Optional: globaler Filter, der für alle Queries gilt
    public FilterDefinition? GlobalFilter { get; init; }

    public required IReadOnlyList<AnalysisQuery> Queries { get; init; }

    public static AnalysisAggregateProfile Default => new()
    {
        Queries = new List<AnalysisQuery>
        {
             new AnalysisQuery { Title = "Severity", Dimensions = new [] { DimensionKind.Severity } },
             new AnalysisQuery { Title = "Source", Filter = new ValueFilterDefinition  { Field = "Severity", Type = "Enum", Operator = FilterOperator.Le, Value = "Error" }, Dimensions = new [] { DimensionKind.Source }, TopN =300 },
             new AnalysisQuery { Title = "Hour x Source", Dimensions = new [] { DimensionKind.Hour, DimensionKind.Source },Filter = new ValueFilterDefinition  { Field = "Severity", Type = "Enum", Operator = FilterOperator.Le, Value = "Error" }, },
             new AnalysisQuery { Title = "Top-Events", Dimensions = new [] { DimensionKind.MessageNormalized }, TopN =100 },
             new AnalysisQuery { Title = "Top-Events2", Dimensions = new [] { DimensionKind.MessageNormalized }, TopN =30, Filter = new ValueFilterDefinition  { Field = "Severity", Type = "Enum", Operator = FilterOperator.Le, Value = "Error" }, },
             new AnalysisQuery { Title = "Severity x Source", Dimensions = new [] { DimensionKind.Source, DimensionKind.Severity }, Columns = Enum.GetValues<SyslogSeverity>().Select(s => s.ToString()).ToArray() },
             new AnalysisQuery { Title = "Events x Source (Top50)", Dimensions = new [] { DimensionKind.Source, DimensionKind.MessageNormalized }, TopN =300},
             new AnalysisQuery { Title = "Error Cluster", Dimensions = new [] { DimensionKind.X, DimensionKind.Y }, TopN = 30, IsDBScan = true, DbEps = 2.0, DbMinPts = 3, Filter = new ValueFilterDefinition  { Field = "Severity", Type = "Enum", Operator = FilterOperator.Le, Value = "Error" }, },
             new AnalysisQuery { Title = "Max-G Cluster (1.0)", Dimensions = new [] { DimensionKind.X, DimensionKind.Y }, TopN = 30, IsDBScan = true, DbEps = 1.0, DbMinPts = 3, Filter = new GroupFilterDefinition { Mode="And", Type = "group",
                        Filters=[ new ValueFilterDefinition  { Field = "Message", Type = "String", Operator = FilterOperator.Eq, Value = "Max. G-Force" },
                            new ValueFilterDefinition  { Field = "X", Type = "value", Operator = FilterOperator.Gt, Value = "10" }
                        ] }, },
             new AnalysisQuery { Title = "Max-G Cluster (0.5)", Dimensions = new [] { DimensionKind.X, DimensionKind.Y }, TopN = 50, IsDBScan = true, DbEps = 0.5, DbMinPts = 3, Filter = 
                 new GroupFilterDefinition { Mode="And", Type = "group",
                        Filters=[ 
                            new ValueFilterDefinition  { Field = "Message", Type = "String", Operator = FilterOperator.Eq, Value = "Max. G-Force" },
                            new ValueFilterDefinition  { Field = "X", Type = "value", Operator = FilterOperator.Gt, Value = "10" }
                        ] }, },
             new AnalysisQuery { Title = "SSCU Cluster (0.5)", Dimensions = new [] { DimensionKind.X, DimensionKind.Y }, TopN = 30, IsDBScan = true, DbEps = 0.5, DbMinPts = 3, Filter =
                 new GroupFilterDefinition { Mode="And", Type = "group", Filters=[
                     new ValueFilterDefinition  { Field = "Message", Type = "String", Operator = FilterOperator.StartsWith, Value = "SSCU" },
                     new ValueFilterDefinition  { Field = "X", Type = "value", Operator = FilterOperator.Gt, Value = "10" },
                     ] 
                 },  },
             new AnalysisQuery { Title = "SSCU Cluster (0.25)", Dimensions = new [] { DimensionKind.X, DimensionKind.Y }, TopN = 50, IsDBScan = true, DbEps = 0.25, DbMinPts = 3, Filter =
                new GroupFilterDefinition { Mode="And", Type = "group", Filters=[ 
                    new ValueFilterDefinition  { Field = "Message", Type = "String", Operator = FilterOperator.StartsWith, Value = "SSCU" },
                     new ValueFilterDefinition  { Field = "X", Type = "value", Operator = FilterOperator.Gt, Value = "10" },
                    ] },
             }
        }
    };
}
