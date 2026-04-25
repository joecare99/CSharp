using System.Collections.Generic;
using TraceAnalysis.Base.Models.Interfaces;

namespace TraceAnalysis.Base.Models;

/// <summary>
/// Default implementation of <see cref="ITraceMetadata"/>.
/// </summary>
public class TraceMetadata : ITraceMetadata
{
    /// <inheritdoc/>
    public string sSourceId { get; }

    /// <inheritdoc/>
    public IReadOnlyList<ITraceFieldMetadata> Fields { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="TraceMetadata"/>.
    /// </summary>
    /// <param name="_sourceId">Identifier for the trace source, e.g. a file path or stream label.</param>
    /// <param name="_fields">Ordered field metadata definitions.</param>
    public TraceMetadata(string _sourceId, IReadOnlyList<ITraceFieldMetadata> _fields)
    {
        sSourceId = _sourceId;
        Fields = _fields;
    }
}
