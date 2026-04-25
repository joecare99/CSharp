using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TraceAnalysis.Base.Filters;
using TraceAnalysis.Base.Models;
using TraceAnalysis.Base.Models.Interfaces;
using TraceAnalysis.Filter.CSV.Model;

namespace TraceAnalysis.Filter.CSV.Filters;

/// <summary>
/// Input filter for flat CSV files with a header row.
/// Supports common separators (<c>;</c>, <c>\t</c>, <c>,</c>).
/// </summary>
public class FlatCsvInputFilter : IAnalyzableInputFilter
{
    /// <summary>Expected first line of a TraceCsv stream (used for format exclusion).</summary>
    private const string TraceCsvHeader = "[key]; [value]";

    /// <inheritdoc/>
    public string FilterId => "FlatCsv";

    /// <inheritdoc/>
    public int Priority => 90;

    /// <inheritdoc/>
    /// <remarks>
    /// Detects the format by file extension (<c>.csv</c>) first,
    /// then confirms the stream header is a flat CSV header row
    /// (i.e. not a TraceCsv stream).
    /// The stream position is restored after inspection.
    /// Returns <c>false</c> when the stream does not support seeking.
    /// </remarks>
    public bool CanHandle(Stream _stream, string _sourceId)
    {
        var analysis = Analyze(_stream, new FilterSourceDescriptor(_sourceId, System.IO.Path.GetExtension(_sourceId)));
        return analysis.CanHandle;
    }

    /// <inheritdoc/>
    public InputFilterAnalysisResult Analyze(Stream stream, FilterSourceDescriptor sourceDescriptor)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));
        if (sourceDescriptor == null)
            throw new ArgumentNullException(nameof(sourceDescriptor));

        var decisionLines = new List<string>();
        var ext = sourceDescriptor.SuggestedExtension ?? System.IO.Path.GetExtension(sourceDescriptor.SourceId);
        var isExactExtensionMatch = string.Equals(ext, ".csv", StringComparison.OrdinalIgnoreCase);

        if (!string.IsNullOrWhiteSpace(ext))
            decisionLines.Add($"Extension={ext}");

        if (!stream.CanSeek)
        {
            decisionLines.Add("Stream is not seekable.");
            return new InputFilterAnalysisResult(FilterId, false, 0, isExactExtensionMatch, decisionLines);
        }

        var startPos = stream.Position;
        try
        {
            using var reader = new StreamReader(stream, Encoding.UTF8, true, 1024, leaveOpen: true);
            var firstLine = reader.ReadLine();
            var secondLine = reader.ReadLine();
            var hasContent = !string.IsNullOrWhiteSpace(firstLine);
            var isTraceCsv = firstLine == TraceCsvHeader;
            var separator = DetectSeparator(firstLine);
            var headerColumns = !string.IsNullOrWhiteSpace(firstLine) && separator != null
                ? CsvModel.SplitCSVLine(firstLine, separator)
                : new List<string>();
            var sampleColumns = !string.IsNullOrWhiteSpace(secondLine) && separator != null
                ? CsvModel.SplitCSVLine(secondLine, separator)
                : new List<string>();
            var hasMatchingSampleWidth = headerColumns.Count > 0 && sampleColumns.Count == headerColumns.Count;

            decisionLines.Add(hasContent ? "Flat CSV header candidate detected." : "Header line missing.");
            decisionLines.Add(isTraceCsv ? "TraceCsv signature excluded." : "TraceCsv signature not present.");
            decisionLines.Add(separator != null ? $"Separator={separator}" : "No supported separator detected.");
            decisionLines.Add(hasMatchingSampleWidth ? "Sample row width matches header." : "Sample row width mismatch.");

            var canHandle = hasContent && !isTraceCsv && separator != null && hasMatchingSampleWidth;
            var confidenceScore = 0;
            if (hasContent && !isTraceCsv)
                confidenceScore += 80;
            if (separator != null)
                confidenceScore += 40;
            if (hasMatchingSampleWidth)
                confidenceScore += 40;
            if (isExactExtensionMatch)
                confidenceScore += 10;

            return new InputFilterAnalysisResult(FilterId, canHandle, confidenceScore, isExactExtensionMatch, decisionLines);
        }
        finally
        {
            stream.Position = startPos;
        }
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Reads the flat CSV stream into a <see cref="CsvModel"/> and converts it
    /// to the canonical <see cref="ITraceDataSet"/> structure.
    /// The first column is used as the mandatory timestamp / row key.
    /// All remaining columns are treated as optional value fields.
    /// Parse errors are collected in <see cref="ITraceDataSet.ParseErrors"/>.
    /// </remarks>
    public ITraceDataSet Read(Stream _stream, string _sourceId)
    {
        var errors = new List<string>();
        var model = new CsvModel();

        try
        {
            model.ReadCsv(_stream);
        }
        catch (Exception ex)
        {
            errors.Add($"Failed to read flat CSV from '{_sourceId}': {ex.Message}");
            return new TraceDataSet(
                new TraceMetadata(_sourceId, Array.Empty<ITraceFieldMetadata>()),
                Array.Empty<ITraceRecord>(),
                errors);
        }

        // Build field metadata from header, skipping the first column (= timestamp key).
        var fields = new List<ITraceFieldMetadata>();
        for (var i = 1; i < model.Header.Count; i++)
            fields.Add(new TraceFieldMetadata(model.Header[i].name, model.Header[i].type));

        // Convert each row to a canonical record.
        var records = new List<ITraceRecord>();
        for (var i = 0; i < model.Rows.Count; i++)
        {
            var row = model.Rows[i];
            var timestampKey = model.Header.Count > 0 ? model.Header[0].name : string.Empty;
            var timestamp = row.TryGetValue(timestampKey, out var ts) ? ts : (object)i;
            var values = new Dictionary<string, object?>();
            foreach (var kvp in row)
                if (kvp.Key != timestampKey)
                    values[kvp.Key] = kvp.Value;

            records.Add(new TraceRecord(timestamp, values));
        }

        return new TraceDataSet(new TraceMetadata(_sourceId, fields), records, errors);
    }

    /// <inheritdoc/>
    public ITraceDataSet Read(Stream stream, FilterSourceDescriptor sourceDescriptor)
    {
        if (sourceDescriptor == null)
            throw new ArgumentNullException(nameof(sourceDescriptor));

        return Read(stream, sourceDescriptor.SourceId);
    }

    private static string? DetectSeparator(string? line)
    {
        if (string.IsNullOrWhiteSpace(line))
            return null;

        if (line!.Contains(";"))
            return ";";
        if (line!.Contains("\t"))
            return "\t";
        if (line!.Contains(","))
            return ",";

        return null;
    }
}
