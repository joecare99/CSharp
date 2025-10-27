using System.Collections.Generic;

namespace DataAnalysis.Core.Models;

public sealed class AggregationResult
{
 public required string Title { get; init; }
 public required IReadOnlyList<DimensionKind> Dimensions { get; init; }

 // For1D aggregations
 public IReadOnlyDictionary<string, int>? Series { get; init; }

 // For2D aggregations
 public IReadOnlyList<string>? Columns { get; init; }
 public IReadOnlyDictionary<string, IReadOnlyDictionary<string, int>>? Matrix { get; init; }
}
