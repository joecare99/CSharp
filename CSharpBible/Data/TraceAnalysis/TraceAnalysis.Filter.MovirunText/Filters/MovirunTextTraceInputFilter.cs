using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using TraceAnalysis.Base.Filters;
using TraceAnalysis.Base.Models;
using TraceAnalysis.Base.Models.Interfaces;

namespace TraceAnalysis.Filter.MovirunText.Filters
{
    /// <summary>
    /// Input filter for whitespace-separated MOVIRUN trace text exports.
    /// </summary>
    public sealed class MovirunTextTraceInputFilter : IAnalyzableInputFilter
    {
        private const string SignaturePrefix = "MOVIRUN open Editor";
        private const string SignatureMarker = "Trace: Trace";
        private const string TimestampHeaderPrefix = "Zeitstempel(";

        /// <inheritdoc/>
        public string FilterId => "MovirunTextTrace";

        /// <inheritdoc/>
        public int Priority => 100;

        /// <inheritdoc/>
        public bool CanHandle(Stream _stream, string _sourceId)
        {
            if (_stream == null)
                throw new ArgumentNullException(nameof(_stream));

            if (!_stream.CanSeek)
                return false;

            var result = Analyze(
                _stream,
                new FilterSourceDescriptor(_sourceId, Path.GetExtension(_sourceId)));

            return result.CanHandle;
        }

        /// <inheritdoc/>
        public InputFilterAnalysisResult Analyze(Stream stream, FilterSourceDescriptor sourceDescriptor)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (sourceDescriptor == null)
                throw new ArgumentNullException(nameof(sourceDescriptor));

            var decisionLines = new List<string>();
            var suggestedExtension = sourceDescriptor.SuggestedExtension ?? Path.GetExtension(sourceDescriptor.SourceId);
            var xIsExactExtensionMatch = string.Equals(suggestedExtension, ".txt", StringComparison.OrdinalIgnoreCase);

            if (!string.IsNullOrWhiteSpace(suggestedExtension))
                decisionLines.Add($"Extension={suggestedExtension}");

            if (!stream.CanSeek)
            {
                decisionLines.Add("Stream is not seekable.");
                return new InputFilterAnalysisResult(FilterId, false, 0, xIsExactExtensionMatch, decisionLines);
            }

            var startPosition = stream.Position;
            try
            {
                using var reader = new StreamReader(stream, Encoding.UTF8, true, 1024, leaveOpen: true);
                var probeLines = ReadNextNonEmptyLines(reader, 4);
                if (probeLines.Count < 4)
                {
                    decisionLines.Add("Insufficient non-empty lines for MOVIRUN trace detection.");
                    return new InputFilterAnalysisResult(FilterId, false, 0, xIsExactExtensionMatch, decisionLines);
                }

                var xHasSignature = IsSignatureLine(probeLines[0]);
                var xHasHeader = TryParseHeader(probeLines[2], out var headerColumns);
                var xHasSampleRow = xHasHeader && TrySplitDataLine(probeLines[3], headerColumns.Count, out _);

                decisionLines.Add(xHasSignature ? "MOVIRUN signature detected." : "MOVIRUN signature missing.");
                decisionLines.Add(xHasHeader ? "Zeitstempel header detected." : "Zeitstempel header missing.");
                decisionLines.Add(xHasSampleRow ? "Whitespace-separated sample row detected." : "Sample row does not match header width.");

                var confidenceScore = 0;
                if (xHasSignature)
                    confidenceScore += 80;
                if (xHasHeader)
                    confidenceScore += 80;
                if (xHasSampleRow)
                    confidenceScore += 40;
                if (xIsExactExtensionMatch)
                    confidenceScore += 10;

                return new InputFilterAnalysisResult(
                    FilterId,
                    xHasSignature && xHasHeader && xHasSampleRow,
                    confidenceScore,
                    xIsExactExtensionMatch,
                    decisionLines);
            }
            finally
            {
                stream.Position = startPosition;
            }
        }

        /// <inheritdoc/>
        public ITraceDataSet Read(Stream _stream, string _sourceId)
        {
            if (_stream == null)
                throw new ArgumentNullException(nameof(_stream));

            return Read(_stream, new FilterSourceDescriptor(_sourceId, Path.GetExtension(_sourceId)));
        }

        /// <inheritdoc/>
        public ITraceDataSet Read(Stream stream, FilterSourceDescriptor sourceDescriptor)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (sourceDescriptor == null)
                throw new ArgumentNullException(nameof(sourceDescriptor));

            var errors = new List<string>();
            var records = new List<ITraceRecord>();

            using var reader = new StreamReader(stream, Encoding.UTF8, true, 1024, leaveOpen: true);

            var iLineNumber = 0;
            var signatureLine = ReadNextNonEmptyLine(reader, ref iLineNumber);
            var sourceLine = ReadNextNonEmptyLine(reader, ref iLineNumber);
            var headerLine = ReadNextNonEmptyLine(reader, ref iLineNumber);

            if (!IsSignatureLine(signatureLine))
            {
                errors.Add($"Line {iLineNumber}: MOVIRUN trace signature was not found.");
                return new TraceDataSet(new TraceMetadata(sourceDescriptor.SourceId, Array.Empty<ITraceFieldMetadata>()), Array.Empty<ITraceRecord>(), errors);
            }

            if (string.IsNullOrWhiteSpace(sourceLine))
            {
                errors.Add($"Line {iLineNumber}: MOVIRUN source description line is missing.");
                return new TraceDataSet(new TraceMetadata(sourceDescriptor.SourceId, Array.Empty<ITraceFieldMetadata>()), Array.Empty<ITraceRecord>(), errors);
            }

            if (!TryParseHeader(headerLine, out var headerColumns))
            {
                errors.Add($"Line {iLineNumber}: MOVIRUN header row is missing or invalid.");
                return new TraceDataSet(new TraceMetadata(sourceDescriptor.SourceId, Array.Empty<ITraceFieldMetadata>()), Array.Empty<ITraceRecord>(), errors);
            }

            var fieldTypes = new Type?[headerColumns.Count - 1];
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                iLineNumber++;
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                if (!TrySplitDataLine(line, headerColumns.Count, out var values))
                {
                    errors.Add($"Line {iLineNumber}: Expected {headerColumns.Count} columns but found a different count.");
                    continue;
                }

                var timestampValue = ParseToken(values[0]);
                var recordValues = new Dictionary<string, object?>(StringComparer.Ordinal);
                for (var i = 1; i < headerColumns.Count; i++)
                {
                    var parsedValue = ParseToken(values[i]);
                    recordValues[headerColumns[i]] = parsedValue;
                    fieldTypes[i - 1] = MergeFieldType(fieldTypes[i - 1], parsedValue);
                }

                records.Add(new TraceRecord(timestampValue, recordValues));
            }

            var fields = new List<ITraceFieldMetadata>(headerColumns.Count - 1);
            for (var i = 1; i < headerColumns.Count; i++)
                fields.Add(new TraceFieldMetadata(headerColumns[i], fieldTypes[i - 1]));

            return new TraceDataSet(new TraceMetadata(sourceDescriptor.SourceId, fields), records, errors);
        }

        private static List<string> ReadNextNonEmptyLines(StreamReader _reader, int _count)
        {
            var result = new List<string>(_count);
            string? line;
            while (result.Count < _count && (line = _reader.ReadLine()) != null)
            {
                if (!string.IsNullOrWhiteSpace(line))
                    result.Add(line);
            }

            return result;
        }

        private static string? ReadNextNonEmptyLine(StreamReader _reader, ref int _lineNumber)
        {
            string? line;
            while ((line = _reader.ReadLine()) != null)
            {
                _lineNumber++;
                if (!string.IsNullOrWhiteSpace(line))
                    return line;
            }

            return null;
        }

        private static bool IsSignatureLine(string? _line)
        {
            if (string.IsNullOrWhiteSpace(_line))
                return false;

            return _line.StartsWith(SignaturePrefix, StringComparison.OrdinalIgnoreCase)
                   && _line.IndexOf(SignatureMarker, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool TryParseHeader(string? _line, out List<string> _headerColumns)
        {
            _headerColumns = new List<string>();
            if (string.IsNullOrWhiteSpace(_line))
                return false;

            var tokens = SplitColumns(_line);
            if (tokens.Count < 2)
                return false;

            if (!tokens[0].StartsWith(TimestampHeaderPrefix, StringComparison.OrdinalIgnoreCase) || !tokens[0].EndsWith(")", StringComparison.Ordinal))
                return false;

            _headerColumns.AddRange(tokens);
            return true;
        }

        private static bool TrySplitDataLine(string _line, int _expectedColumnCount, out List<string> _values)
        {
            _values = SplitColumns(_line);
            return _values.Count == _expectedColumnCount;
        }

        private static List<string> SplitColumns(string _line)
        {
            return _line
                .Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();
        }

        private static object ParseToken(string _token)
        {
            if (long.TryParse(_token, NumberStyles.Integer, CultureInfo.InvariantCulture, out var iLongValue))
                return iLongValue;

            if (double.TryParse(_token, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var fDoubleValue))
                return fDoubleValue;

            return _token;
        }

        private static Type? MergeFieldType(Type? _currentType, object _value)
        {
            var valueType = _value.GetType();
            if (_currentType == null)
                return valueType;

            if (_currentType == valueType)
                return _currentType;

            if ((_currentType == typeof(long) && valueType == typeof(double))
                || (_currentType == typeof(double) && valueType == typeof(long)))
                return typeof(double);

            return typeof(string);
        }
    }
}
