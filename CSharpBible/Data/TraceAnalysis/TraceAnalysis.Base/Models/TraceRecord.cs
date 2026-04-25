using System.Collections.Generic;
using TraceAnalysis.Base.Models.Interfaces;

namespace TraceAnalysis.Base.Models;

/// <summary>
/// Default implementation of <see cref="ITraceRecord"/>.
/// </summary>
public class TraceRecord : ITraceRecord
{
    /// <inheritdoc/>
    public object Timestamp { get; }

    /// <inheritdoc/>
    public IReadOnlyDictionary<string, object?> Values { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="TraceRecord"/>.
    /// </summary>
    /// <param name="_timestamp">Mandatory timestamp or row key for this record.</param>
    /// <param name="_values">Field values keyed by field name.</param>
    public TraceRecord(object _timestamp, IReadOnlyDictionary<string, object?> _values)
    {
        Timestamp = _timestamp;
        Values = _values;
    }
}
