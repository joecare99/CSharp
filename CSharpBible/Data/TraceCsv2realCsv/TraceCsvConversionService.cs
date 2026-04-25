using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TraceAnalysis.Base.Filters;
using TraceAnalysis.Filter.CSV.Filters;

namespace TraceCsv2realCsv;

/// <summary>
/// Converts supported trace source formats to selected output formats through the shared
/// TraceAnalysis intake and export pipeline.
/// </summary>
public sealed class TraceCsvConversionService
{
    private readonly IInputFilterSelector _inputFilterSelector;
    private readonly IReadOnlyList<IAnalyzableInputFilter> _inputFilters;
    private readonly IReadOnlyDictionary<string, IOutputFilter> _outputFilters;

    /// <summary>
    /// Initializes a new instance of <see cref="TraceCsvConversionService"/>.
    /// </summary>
    /// <param name="_inputFilterSelector">Selector used to choose the best input filter.</param>
    /// <param name="_inputFilters">Registered analyzable input filters.</param>
    /// <param name="_outputFilters">Output filters keyed by supported extension.</param>
    public TraceCsvConversionService(
        IInputFilterSelector _inputFilterSelector,
        IEnumerable<IAnalyzableInputFilter> _inputFilters,
        IEnumerable<IOutputFilter> _outputFilters)
    {
        this._inputFilterSelector = _inputFilterSelector ?? throw new ArgumentNullException(nameof(_inputFilterSelector));
        if (_inputFilters == null)
            throw new ArgumentNullException(nameof(_inputFilters));
        if (_outputFilters == null)
            throw new ArgumentNullException(nameof(_outputFilters));

        this._inputFilters = new List<IAnalyzableInputFilter>(_inputFilters);
        if (this._inputFilters.Count == 0)
            throw new ArgumentException("At least one input filter is required.", nameof(_inputFilters));

        this._outputFilters = _outputFilters
            .GroupBy(GetExtension)
            .ToDictionary(group => group.Key, group => group.Last(), StringComparer.OrdinalIgnoreCase);
        if (this._outputFilters.Count == 0)
            throw new ArgumentException("At least one output filter is required.", nameof(_outputFilters));
    }

    /// <summary>
    /// Gets the stable identifiers of the loaded input filters.
    /// </summary>
    public IReadOnlyList<string> LoadedInputFilterIds => _inputFilters.Select(filter => filter.FilterId).ToArray();

    /// <summary>
    /// Gets the display names of the loaded output filters.
    /// </summary>
    public IReadOnlyList<string> LoadedOutputFilterNames => _outputFilters.Values.Select(filter => filter.GetType().Name).OrderBy(name => name, StringComparer.Ordinal).ToArray();

    /// <summary>
    /// Converts the source file to the output format selected by target extension.
    /// </summary>
    /// <param name="_inputPath">Input file path.</param>
    /// <param name="_outputPath">Output file path.</param>
    public void ConvertFile(string _inputPath, string _outputPath)
    {
        using var inputStream = File.OpenRead(_inputPath);
        using var outputStream = new FileStream(_outputPath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
        Convert(inputStream, _inputPath, outputStream);
    }

    /// <summary>
    /// Builds the default flat CSV output path for a source file.
    /// </summary>
    /// <param name="_inputPath">Input file path.</param>
    /// <returns>Derived output file path.</returns>
    public string GetDefaultOutputPath(string _inputPath)
    {
        if (string.IsNullOrWhiteSpace(_inputPath))
            throw new ArgumentException("An input path is required.", nameof(_inputPath));

        if (_inputPath.EndsWith(".trace.csv", StringComparison.OrdinalIgnoreCase))
        {
            var sDirectory = Path.GetDirectoryName(_inputPath);
            var sFileNameWithoutExtension = Path.GetFileNameWithoutExtension(_inputPath);
            var sBaseFileName = sFileNameWithoutExtension.EndsWith(".trace", StringComparison.OrdinalIgnoreCase)
                ? sFileNameWithoutExtension.Substring(0, sFileNameWithoutExtension.Length - ".trace".Length)
                : sFileNameWithoutExtension;
            var sOutputFileName = $"{sBaseFileName}.csv";
            return string.IsNullOrEmpty(sDirectory)
                ? sOutputFileName
                : Path.Combine(sDirectory, sOutputFileName);
        }

        return Path.ChangeExtension(_inputPath, ".csv");
    }

    /// <summary>
    /// Converts the source stream to the output format selected by target extension.
    /// </summary>
    /// <param name="_inputStream">Input stream.</param>
    /// <param name="_sourceId">Logical input identifier.</param>
    /// <param name="_outputStream">Output stream.</param>
    public void Convert(Stream _inputStream, string _sourceId, Stream _outputStream)
    {
        Convert(_inputStream, _sourceId, _outputStream, GetDefaultOutputPath(_sourceId));
    }

    /// <summary>
    /// Converts the source stream to the output format selected by the provided output identifier.
    /// </summary>
    /// <param name="_inputStream">Input stream.</param>
    /// <param name="_sourceId">Logical input identifier.</param>
    /// <param name="_outputStream">Output stream.</param>
    /// <param name="_outputId">Logical output identifier used for format selection.</param>
    public void Convert(Stream _inputStream, string _sourceId, Stream _outputStream, string _outputId)
    {
        if (_inputStream == null)
            throw new ArgumentNullException(nameof(_inputStream));
        if (string.IsNullOrWhiteSpace(_sourceId))
            throw new ArgumentException("A source identifier is required.", nameof(_sourceId));
        if (_outputStream == null)
            throw new ArgumentNullException(nameof(_outputStream));
        if (string.IsNullOrWhiteSpace(_outputId))
            throw new ArgumentException("An output identifier is required.", nameof(_outputId));

        var sourceDescriptor = new FilterSourceDescriptor(_sourceId, Path.GetExtension(_sourceId));
        var selectionResult = _inputFilterSelector.Select(_inputFilters, _inputStream, sourceDescriptor);
        if (selectionResult.SelectedFilter == null)
            throw new InvalidDataException($"No supported input filter matched '{_sourceId}'.");

        var outputFilter = GetOutputFilter(_outputId);

        if (_inputStream.CanSeek)
            _inputStream.Position = 0;

        var dataSet = selectionResult.SelectedFilter.Read(_inputStream, sourceDescriptor);
        outputFilter.Write(dataSet, _outputStream);
    }

    private IOutputFilter GetOutputFilter(string outputId)
    {
        var extension = Path.GetExtension(outputId);
        if (string.IsNullOrWhiteSpace(extension))
            extension = ".csv";

        if (_outputFilters.TryGetValue(extension, out var outputFilter))
            return outputFilter;

        throw new InvalidDataException($"No supported output filter matched '{outputId}'.");
    }

    private static string GetExtension(IOutputFilter outputFilter)
    {
        return outputFilter switch
        {
            CsvOutputFilter => ".csv",
            _ when string.Equals(outputFilter.GetType().Name, "JsonOutputFilter", StringComparison.Ordinal) => ".json",
            _ => throw new ArgumentException($"Unsupported output filter type '{outputFilter.GetType().FullName}'.", nameof(outputFilter))
        };
    }
}
