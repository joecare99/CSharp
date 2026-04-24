using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using TraceAnalysis.Base.Filters;
using TraceAnalysis.Base.Models;
using TraceAnalysis.Base.Models.Interfaces;

namespace TraceAnalysis.Filter.MovirunTrace.Filters
{
    /// <summary>
    /// Input filter for XML-based MOVIRUN <c>.trace</c> exports.
    /// </summary>
    public sealed class MovirunTraceXmlInputFilter : IAnalyzableInputFilter
    {
        /// <inheritdoc/>
        public string FilterId => "MovirunTraceXml";

        /// <inheritdoc/>
        public int Priority => 110;

        /// <inheritdoc/>
        public bool CanHandle(Stream _stream, string _sourceId)
        {
            if (_stream == null)
                throw new ArgumentNullException(nameof(_stream));

            if (!_stream.CanSeek)
                return false;

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
            var suggestedExtension = sourceDescriptor.SuggestedExtension ?? Path.GetExtension(sourceDescriptor.SourceId);
            var isExactExtensionMatch = string.Equals(suggestedExtension, ".trace", StringComparison.OrdinalIgnoreCase);

            if (!string.IsNullOrWhiteSpace(suggestedExtension))
                decisionLines.Add($"Extension={suggestedExtension}");

            if (!stream.CanSeek)
            {
                decisionLines.Add("Stream is not seekable.");
                return new InputFilterAnalysisResult(FilterId, false, 0, isExactExtensionMatch, decisionLines);
            }

            var startPosition = stream.Position;
            try
            {
                var document = XDocument.Load(stream, LoadOptions.None);
                var root = document.Root;
                var hasTraceRoot = string.Equals(root?.Name.LocalName, "Trace", StringComparison.Ordinal);
                var traceData = root?.Element("TraceData");
                var hasTraceRecord = traceData?.Elements("TraceRecord").Any() == true;
                var hasTraceVariable = traceData?.Descendants("TraceVariable").Any() == true;
                var hasSeriesElements = traceData?.Descendants("TraceVariable")
                    .Any(v => v.Element("Values") != null && v.Element("Timestamps") != null) == true;

                decisionLines.Add(hasTraceRoot ? "Trace root detected." : "Trace root missing.");
                decisionLines.Add(hasTraceRecord ? "TraceRecord element detected." : "TraceRecord element missing.");
                decisionLines.Add(hasTraceVariable ? "TraceVariable element detected." : "TraceVariable element missing.");
                decisionLines.Add(hasSeriesElements ? "Values and Timestamps series detected." : "Trace series elements missing.");

                var confidenceScore = 0;
                if (hasTraceRoot)
                    confidenceScore += 80;
                if (hasTraceRecord)
                    confidenceScore += 50;
                if (hasTraceVariable)
                    confidenceScore += 50;
                if (hasSeriesElements)
                    confidenceScore += 50;
                if (isExactExtensionMatch)
                    confidenceScore += 10;

                return new InputFilterAnalysisResult(
                    FilterId,
                    hasTraceRoot && hasTraceRecord && hasTraceVariable && hasSeriesElements,
                    confidenceScore,
                    isExactExtensionMatch,
                    decisionLines);
            }
            catch (Exception ex)
            {
                decisionLines.Add($"XML analysis failed: {ex.Message}");
                return new InputFilterAnalysisResult(FilterId, false, 0, isExactExtensionMatch, decisionLines);
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
            var recordsByTimestamp = new SortedDictionary<long, Dictionary<string, object?>>();
            var fieldTypesByName = new Dictionary<string, Type?>(StringComparer.Ordinal);
            var fieldNames = new SortedSet<string>(StringComparer.Ordinal);

            XDocument document;
            try
            {
                document = XDocument.Load(stream, LoadOptions.None);
            }
            catch (Exception ex)
            {
                errors.Add($"Failed to read MOVIRUN trace XML from '{sourceDescriptor.SourceId}': {ex.Message}");
                return new TraceDataSet(new TraceMetadata(sourceDescriptor.SourceId, Array.Empty<ITraceFieldMetadata>()), Array.Empty<ITraceRecord>(), errors);
            }

            var traceRecordElements = document.Root?
                .Element("TraceData")?
                .Elements("TraceRecord")
                .ToList();

            if (traceRecordElements == null || traceRecordElements.Count == 0)
            {
                errors.Add("TraceData/TraceRecord elements were not found.");
                return new TraceDataSet(new TraceMetadata(sourceDescriptor.SourceId, Array.Empty<ITraceFieldMetadata>()), Array.Empty<ITraceRecord>(), errors);
            }

            foreach (var traceRecordElement in traceRecordElements)
            {
                foreach (var traceVariableElement in traceRecordElement.Elements("TraceVariable"))
                {
                    ProcessTraceVariable(traceVariableElement, recordsByTimestamp, fieldTypesByName, fieldNames, errors);
                }
            }

            var fields = fieldNames
                .Select(fieldName => (ITraceFieldMetadata)new TraceFieldMetadata(fieldName, fieldTypesByName.TryGetValue(fieldName, out var fieldType) ? fieldType : null))
                .ToArray();

            var records = recordsByTimestamp
                .Select(pair => (ITraceRecord)new TraceRecord(pair.Key, pair.Value))
                .ToArray();

            return new TraceDataSet(new TraceMetadata(sourceDescriptor.SourceId, fields), records, errors);
        }

        private static void ProcessTraceVariable(
            XElement traceVariableElement,
            SortedDictionary<long, Dictionary<string, object?>> recordsByTimestamp,
            Dictionary<string, Type?> fieldTypesByName,
            SortedSet<string> fieldNames,
            ICollection<string> errors)
        {
            var fieldName = (string?)traceVariableElement.Attribute("VarName");
            if (string.IsNullOrWhiteSpace(fieldName))
            {
                errors.Add("TraceVariable without VarName was skipped.");
                return;
            }

            var valuesText = traceVariableElement.Element("Values")?.Value;
            var timestampsText = traceVariableElement.Element("Timestamps")?.Value;
            if (string.IsNullOrWhiteSpace(valuesText) || string.IsNullOrWhiteSpace(timestampsText))
            {
                errors.Add($"TraceVariable '{fieldName}' is missing Values or Timestamps.");
                return;
            }

            var rawValueTokens = SplitSeries(valuesText);
            var rawTimestampTokens = SplitSeries(timestampsText);
            if (rawValueTokens.Count != rawTimestampTokens.Count)
            {
                errors.Add($"TraceVariable '{fieldName}' has {rawValueTokens.Count} values but {rawTimestampTokens.Count} timestamps.");
            }

            var pairCount = Math.Min(rawValueTokens.Count, rawTimestampTokens.Count);
            if (pairCount == 0)
                return;

            fieldNames.Add(fieldName);
            for (var i = 0; i < pairCount; i++)
            {
                if (!long.TryParse(rawTimestampTokens[i], NumberStyles.Integer, CultureInfo.InvariantCulture, out var timestamp))
                {
                    errors.Add($"TraceVariable '{fieldName}' contains an invalid timestamp '{rawTimestampTokens[i]}' at index {i}.");
                    continue;
                }

                var parsedValue = ParseValue(rawValueTokens[i]);
                if (!recordsByTimestamp.TryGetValue(timestamp, out var rowValues))
                {
                    rowValues = new Dictionary<string, object?>(StringComparer.Ordinal);
                    recordsByTimestamp[timestamp] = rowValues;
                }

                rowValues[fieldName] = parsedValue;
                fieldTypesByName[fieldName] = MergeFieldType(fieldTypesByName.TryGetValue(fieldName, out var currentType) ? currentType : null, parsedValue);
            }
        }

        private static List<string> SplitSeries(string rawSeries)
        {
            return rawSeries
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(token => token.Trim())
                .Where(token => token.Length > 0)
                .ToList();
        }

        private static object ParseValue(string token)
        {
            if (long.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out var longValue))
                return longValue;

            if (double.TryParse(token, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var doubleValue))
                return doubleValue;

            if (bool.TryParse(token, out var boolValue))
                return boolValue;

            return token;
        }

        private static Type? MergeFieldType(Type? currentType, object value)
        {
            var valueType = value.GetType();
            if (currentType == null)
                return valueType;

            if (currentType == valueType)
                return currentType;

            if ((currentType == typeof(long) && valueType == typeof(double))
                || (currentType == typeof(double) && valueType == typeof(long)))
                return typeof(double);

            return typeof(string);
        }
    }
}
