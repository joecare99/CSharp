using System.Collections.Generic;

namespace TraceAnalysis.Base.Models.Interfaces;

/// <summary>
/// Describes source-level metadata for a complete trace data set.
/// </summary>
public interface ITraceMetadata
{
    /// <summary>Identifies the source of the trace data, e.g. a file path or stream label.</summary>
    string sSourceId { get; }

    /// <summary>Ordered list of field metadata definitions for the data set.</summary>
    IReadOnlyList<ITraceFieldMetadata> Fields { get; }
}
