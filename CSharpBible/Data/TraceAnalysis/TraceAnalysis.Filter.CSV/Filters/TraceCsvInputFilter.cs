using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TraceAnalysis.Base.Filters;
using TraceAnalysis.Base.Models;
using TraceAnalysis.Base.Models.Interfaces;
using TraceAnalysis.Filter.CSV.Model;

namespace TraceAnalysis.Filter.CSV.Filters
{
    /// <summary>
    /// Input filter for the TraceCsv format produced by trace-export tools.
    /// The format is identified by the header line <c>[key]; [value]</c>.
    /// </summary>
    public class TraceCsvInputFilter : IAnalyzableInputFilter
    {
        /// <summary>Expected first line of a TraceCsv stream.</summary>
        private const string TraceCsvHeader = "[key]; [value]";

        /// <inheritdoc/>
        public string FilterId => "TraceCsv";

        /// <inheritdoc/>
        public int Priority => 100;

        /// <inheritdoc/>
        /// <remarks>
        /// Detects the format by file extension (<c>.csv</c>) first,
        /// then verifies the stream header line.
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
                var hasTraceHeader = firstLine == TraceCsvHeader;

                decisionLines.Add(hasTraceHeader ? "TraceCsv header detected." : "TraceCsv header missing.");

                var confidenceScore = hasTraceHeader ? 180 : 0;
                if (isExactExtensionMatch)
                    confidenceScore += 10;

                return new InputFilterAnalysisResult(FilterId, hasTraceHeader, confidenceScore, isExactExtensionMatch, decisionLines);
            }
            finally
            {
                stream.Position = startPos;
            }
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Reads the TraceCsv stream into a <see cref="CsvModel"/> and converts it
        /// to the canonical <see cref="ITraceDataSet"/> structure.
        /// The first column (<c>TimeBase</c>) is used as the mandatory timestamp.
        /// All other columns are treated as optional double fields.
        /// Parse errors are collected in <see cref="ITraceDataSet.ParseErrors"/>.
        /// </remarks>
        public ITraceDataSet Read(Stream _stream, string _sourceId)
        {
            var errors = new List<string>();
            var model = new CsvModel();

            try
            {
                model.ReadTraceCSV(_stream);
            }
            catch (Exception ex)
            {
                errors.Add($"Failed to read TraceCsv from '{_sourceId}': {ex.Message}");
                return new TraceDataSet(
                    new TraceMetadata(_sourceId, Array.Empty<ITraceFieldMetadata>()),
                    Array.Empty<ITraceRecord>(),
                    errors);
            }

            // Build field metadata from header, skipping the first column (TimeBase = timestamp key).
            var fields = new List<ITraceFieldMetadata>();
            for (var i = 1; i < model.Header.Count; i++)
                fields.Add(new TraceFieldMetadata(model.Header[i].name, model.Header[i].type));

            // Convert each row to a canonical record.
            var records = new List<ITraceRecord>();
            for (var i = 0; i < model.Rows.Count; i++)
            {
                var row = model.Rows[i];
                var timestamp = row.TryGetValue("TimeBase", out var ts) ? ts : (object)i;
                var values = new Dictionary<string, object?>();
                foreach (var kvp in row)
                    if (kvp.Key != "TimeBase")
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
    }
}
