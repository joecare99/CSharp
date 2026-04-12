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
    /// Input filter for flat CSV files with a header row.
    /// Supports common separators (<c>;</c>, <c>\t</c>, <c>,</c>).
    /// </summary>
    public class FlatCsvInputFilter : IInputFilter
    {
        /// <summary>Expected first line of a TraceCsv stream (used for format exclusion).</summary>
        private const string TraceCsvHeader = "[key]; [value]";

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
            if (!_stream.CanSeek)
                return false;

            var ext = System.IO.Path.GetExtension(_sourceId);
            if (!string.IsNullOrEmpty(ext) && !ext.Equals(".csv", StringComparison.OrdinalIgnoreCase))
                return false;

            var startPos = _stream.Position;
            try
            {
                using var reader = new StreamReader(_stream, Encoding.UTF8, true, 1024, leaveOpen: true);
                var firstLine = reader.ReadLine();
                return !string.IsNullOrEmpty(firstLine) && firstLine != TraceCsvHeader;
            }
            finally
            {
                _stream.Position = startPos;
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
    }
}
