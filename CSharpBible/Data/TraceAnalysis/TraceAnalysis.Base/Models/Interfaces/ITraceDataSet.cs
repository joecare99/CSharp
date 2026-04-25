using System.Collections.Generic;

namespace TraceAnalysis.Base.Models.Interfaces;

/// <summary>
/// The canonical intermediate structure that carries imported trace data
/// between input and output filters.
/// </summary>
public interface ITraceDataSet
{
    /// <summary>Source-level metadata describing the origin and field layout of the data set.</summary>
    ITraceMetadata Metadata { get; }

    /// <summary>Ordered list of canonical trace records.</summary>
    IReadOnlyList<ITraceRecord> Records { get; }

    /// <summary>
    /// Errors collected during parsing.
    /// Processing continues with partial results when errors occur.
    /// </summary>
    IReadOnlyList<string> ParseErrors { get; }
}
