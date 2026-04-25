using System.Collections.Generic;
using System.IO;
using TraceAnalysis.Base.Filters;
using TraceAnalysis.Base.Models.Interfaces;
using TraceAnalysis.Filter.CSV.Model;

namespace TraceAnalysis.Filter.CSV.Filters;

/// <summary>
/// Output filter that serializes the canonical <see cref="ITraceDataSet"/>
/// to a tab-separated CSV stream.
/// </summary>
public class CsvOutputFilter : IOutputFilter
{
    private readonly char _separator;

    /// <summary>
    /// Initializes a new instance of <see cref="CsvOutputFilter"/>.
    /// </summary>
    /// <param name="_separator">
    /// Column separator character. Defaults to <c>'\t'</c> (tab).
    /// </param>
    public CsvOutputFilter(char _separator = '\t')
    {
        this._separator = _separator;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// The first written column is the timestamp / row key.
    /// Subsequent columns follow the field order defined in
    /// <see cref="ITraceMetadata.Fields"/>.
    /// Missing optional field values are written as empty cells.
    /// </remarks>
    public void Write(ITraceDataSet _dataSet, Stream _stream)
    {
        var model = new CsvModel();

        // Build header: timestamp key column first, then one column per field.
        var header = new List<(string name, System.Type? type)>
        {
            ("TimeBase", typeof(object))
        };
        foreach (var field in _dataSet.Metadata.Fields)
            header.Add((field.sName, field.FieldType));

        model.SetHeader(header);

        // Append one row per canonical record.
        foreach (var record in _dataSet.Records)
        {
            var rowValues = new List<object?> { record.Timestamp };
            foreach (var field in _dataSet.Metadata.Fields)
                rowValues.Add(record.Values.TryGetValue(field.sName, out var v) ? v : null);

            model.AppendData(rowValues);
        }

        model.WriteCSV(_stream, _separator);
    }
}
