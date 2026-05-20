using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using TraceAnalysis.Base.Filters;
using TraceAnalysis.Base.Models.Interfaces;
using TraceAnalysis.Filter.CSV.Filters;
using TraceAnalysis.Filter.JSON.Filters;
using TraceAnalysis.Filter.MovirunText.Filters;
using TraceAnalysis.Filter.MovirunTrace.Filters;
using TraceAnalysis.Workbench.Core.Models;

namespace TraceAnalysis.Workbench.Core.Services;

/// <summary>
/// Loads a trace file and derives a structural data basis from the selected shared input filter.
/// </summary>
public sealed class TraceSourceLoader : ITraceSourceLoader
{
    private readonly IInputFilterSelector _inputFilterSelector;
    private readonly IReadOnlyList<IAnalyzableInputFilter> _inputFilters;
    private readonly TraceSeriesProjector _traceSeriesProjector;

    /// <summary>
    /// Initializes a new instance of <see cref="TraceSourceLoader"/>.
    /// </summary>
    public TraceSourceLoader()
        : this(
            new InputFilterSelector(),
            [
                new TraceCsvInputFilter(),
                new FlatCsvInputFilter(),
                new JsonInputFilter(),
                new MovirunTextTraceInputFilter(),
                new MovirunTraceXmlInputFilter()
            ],
            new TraceSeriesProjector())
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="TraceSourceLoader"/>.
    /// </summary>
    /// <param name="inputFilterSelector">The filter selector.</param>
    /// <param name="inputFilters">The analyzable input filters.</param>
    /// <param name="traceSeriesProjector">The series projector used for chart visualization.</param>
    public TraceSourceLoader(IInputFilterSelector inputFilterSelector, IReadOnlyList<IAnalyzableInputFilter> inputFilters, TraceSeriesProjector traceSeriesProjector)
    {
        _inputFilterSelector = inputFilterSelector ?? throw new ArgumentNullException(nameof(inputFilterSelector));
        _inputFilters = inputFilters ?? throw new ArgumentNullException(nameof(inputFilters));
        _traceSeriesProjector = traceSeriesProjector ?? throw new ArgumentNullException(nameof(traceSeriesProjector));
    }

    /// <inheritdoc/>
    public TraceSourceState Load(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("A trace file path is required.", nameof(filePath));

        using var stream = File.OpenRead(filePath);
        var sourceDescriptor = new FilterSourceDescriptor(filePath, Path.GetExtension(filePath));
        var selection = _inputFilterSelector.Select(_inputFilters, stream, sourceDescriptor);
        if (selection.SelectedFilter == null)
            throw new InvalidDataException($"No supported input filter matched '{filePath}'.");

        if (stream.CanSeek)
            stream.Position = 0;

        var dataSet = selection.SelectedFilter.Read(stream, sourceDescriptor);
        return new TraceSourceState(filePath, dataSet.ParseErrors.Count, BuildDataBasis(dataSet), _traceSeriesProjector.Project(dataSet));
    }

    private static TraceDataBasisModel BuildDataBasis(ITraceDataSet dataSet)
    {
        var items = new List<TraceDataBasisItem>(dataSet.Metadata.Fields.Count);
        foreach (var field in dataSet.Metadata.Fields)
        {
            var typeName = field.FieldType?.Name ?? "Unknown";
            items.Add(new TraceDataBasisItem(field.sName, field.sGroup, field.sFormat, typeName));
        }

        var timeBaseText = InferTimeBaseText(dataSet);
        return new TraceDataBasisModel(timeBaseText, items);
    }

    private static string InferTimeBaseText(ITraceDataSet dataSet)
    {
        if (dataSet.Records.Count == 0)
            return "No records loaded";

        var firstTimestamp = dataSet.Records[0].Timestamp;
        var timestampTypeName = firstTimestamp?.GetType().Name ?? "Unknown";

        return string.Format(
            CultureInfo.InvariantCulture,
            "Timestamp type: {0}, record count: {1}",
            timestampTypeName,
            dataSet.Records.Count);
    }
}
