using System;
using System.IO;
using TraceAnalysis.Base.Filters;
using TraceAnalysis.Base.Models.Interfaces;
using TraceAnalysis.Filter.JSON.Model;

namespace TraceAnalysis.Filter.JSON.Filters;

/// <summary>
/// Output filter that serializes the canonical trace data set to JSON.
/// </summary>
public sealed class JsonOutputFilter : IOutputFilter
{
    /// <inheritdoc/>
    public void Write(ITraceDataSet _dataSet, Stream _stream)
    {
        if (_dataSet == null)
            throw new ArgumentNullException(nameof(_dataSet));
        if (_stream == null)
            throw new ArgumentNullException(nameof(_stream));

        var model = JsonTraceModel.FromDataSet(_dataSet);
        model.Write(_stream);
    }
}
