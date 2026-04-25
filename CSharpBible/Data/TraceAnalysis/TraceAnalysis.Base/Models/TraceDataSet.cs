using System.Collections.Generic;
using TraceAnalysis.Base.Models.Interfaces;

namespace TraceAnalysis.Base.Models;

/// <summary>
/// Default implementation of <see cref="ITraceDataSet"/>.
/// </summary>
public class TraceDataSet : ITraceDataSet
{
    private readonly List<string> _parseErrors = new();

    /// <inheritdoc/>
    public ITraceMetadata Metadata { get; }

    /// <inheritdoc/>
    public IReadOnlyList<ITraceRecord> Records { get; }

    /// <inheritdoc/>
    public IReadOnlyList<string> ParseErrors => _parseErrors;

    /// <summary>
    /// Initializes a new instance of <see cref="TraceDataSet"/>.
    /// </summary>
    /// <param name="_metadata">Source-level metadata for this data set.</param>
    /// <param name="_records">Ordered list of canonical trace records.</param>
    /// <param name="_errors">Errors collected during parsing, if any.</param>
    public TraceDataSet(
        ITraceMetadata _metadata,
        IReadOnlyList<ITraceRecord> _records,
        IEnumerable<string>? _errors = null)
    {
        Metadata = _metadata;
        Records = _records;
        if (_errors != null)
            _parseErrors.AddRange(_errors);
    }
}
