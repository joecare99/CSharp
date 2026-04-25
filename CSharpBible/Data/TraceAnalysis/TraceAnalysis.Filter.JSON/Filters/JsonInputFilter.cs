using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TraceAnalysis.Base.Filters;
using TraceAnalysis.Base.Models;
using TraceAnalysis.Base.Models.Interfaces;
using TraceAnalysis.Filter.JSON.Model;

namespace TraceAnalysis.Filter.JSON.Filters;

/// <summary>
/// Input filter for canonical trace JSON cache payloads.
/// </summary>
public sealed class JsonInputFilter : IAnalyzableInputFilter
{
    /// <inheritdoc/>
    public string FilterId => "JsonTrace";

    /// <inheritdoc/>
    public int Priority => 95;

    /// <inheritdoc/>
    public bool CanHandle(Stream _stream, string _sourceId)
    {
        var analysis = Analyze(_stream, new FilterSourceDescriptor(_sourceId, Path.GetExtension(_sourceId)));
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
        var ext = sourceDescriptor.SuggestedExtension ?? Path.GetExtension(sourceDescriptor.SourceId);
        var isExactExtensionMatch = string.Equals(ext, ".json", StringComparison.OrdinalIgnoreCase);
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
            var content = reader.ReadToEnd();
            var hasJsonEnvelope = content.IndexOf("\"format\"", StringComparison.OrdinalIgnoreCase) >= 0;
            var hasTraceFormat = content.IndexOf(JsonTraceModel.PayloadFormat, StringComparison.Ordinal) >= 0;
            var hasRecords = content.IndexOf("\"records\"", StringComparison.OrdinalIgnoreCase) >= 0;

            decisionLines.Add(hasJsonEnvelope ? "JSON envelope detected." : "JSON envelope missing.");
            decisionLines.Add(hasTraceFormat ? "Trace JSON format marker detected." : "Trace JSON format marker missing.");
            decisionLines.Add(hasRecords ? "Records section detected." : "Records section missing.");

            var canHandle = hasJsonEnvelope && hasTraceFormat && hasRecords;
            var confidenceScore = 0;
            if (hasJsonEnvelope)
                confidenceScore += 40;
            if (hasTraceFormat)
                confidenceScore += 120;
            if (hasRecords)
                confidenceScore += 20;
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
    public ITraceDataSet Read(Stream _stream, string _sourceId)
    {
        try
        {
            var model = JsonTraceModel.Read(_stream);
            return model.ToDataSet();
        }
        catch (Exception ex)
        {
            return new TraceDataSet(
                new TraceMetadata(_sourceId, Array.Empty<ITraceFieldMetadata>()),
                Array.Empty<ITraceRecord>(),
                new[] { $"Failed to read trace JSON from '{_sourceId}': {ex.Message}" });
        }
    }

    /// <inheritdoc/>
    public ITraceDataSet Read(Stream stream, FilterSourceDescriptor sourceDescriptor)
    {
        if (sourceDescriptor == null)
            throw new ArgumentNullException(nameof(sourceDescriptor));

        return Read(stream, sourceDescriptor.SourceId);
    }
}
