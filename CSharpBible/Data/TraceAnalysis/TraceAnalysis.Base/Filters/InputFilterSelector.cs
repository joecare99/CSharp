using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TraceAnalysis.Base.Filters;

/// <summary>
/// Default deterministic selector for <see cref="IAnalyzableInputFilter"/>.
/// </summary>
public sealed class InputFilterSelector : IInputFilterSelector
{
    /// <inheritdoc/>
    public InputFilterSelectionResult Select(
        IEnumerable<IAnalyzableInputFilter> filters,
        Stream stream,
        FilterSourceDescriptor sourceDescriptor)
    {
        if (filters == null)
            throw new ArgumentNullException(nameof(filters));
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));
        if (sourceDescriptor == null)
            throw new ArgumentNullException(nameof(sourceDescriptor));

        var filterList = filters.ToList();
        if (filterList.Count == 0)
            return new InputFilterSelectionResult(null, Array.Empty<InputFilterAnalysisResult>());

        var rewindableStream = PrepareRewindableStream(stream);
        var analyses = new List<InputFilterAnalysisResult>(filterList.Count);

        foreach (var filter in filterList)
        {
            ResetStream(rewindableStream);
            analyses.Add(filter.Analyze(rewindableStream, sourceDescriptor));
        }

        var manualSelection = SelectManualOverride(filterList, analyses, sourceDescriptor.ManualFilterId);
        if (manualSelection != null)
            return new InputFilterSelectionResult(manualSelection, analyses);

        var candidates = analyses
            .Where(a => a.CanHandle)
            .Join(filterList, a => a.FilterId, f => f.FilterId, (analysis, filter) => new Candidate(filter, analysis))
            .ToList();

        if (candidates.Count == 0)
            return new InputFilterSelectionResult(null, analyses);

        var selectedCandidate = candidates
            .OrderByDescending(c => c.Analysis.ConfidenceScore)
            .ThenByDescending(c => c.Analysis.IsExactExtensionMatch)
            .ThenByDescending(c => c.Filter.Priority)
            .ThenBy(c => c.Filter.FilterId, StringComparer.Ordinal)
            .First();

        return new InputFilterSelectionResult(selectedCandidate.Filter, analyses);
    }

    private static IAnalyzableInputFilter? SelectManualOverride(
        IReadOnlyList<IAnalyzableInputFilter> filters,
        IReadOnlyCollection<InputFilterAnalysisResult> analyses,
        string? manualFilterId)
    {
        if (string.IsNullOrWhiteSpace(manualFilterId))
            return null;

        var filter = filters.FirstOrDefault(f => string.Equals(f.FilterId, manualFilterId, StringComparison.Ordinal));
        if (filter == null)
            return null;

        var analysis = analyses.FirstOrDefault(a => string.Equals(a.FilterId, filter.FilterId, StringComparison.Ordinal));
        return analysis != null && analysis.CanHandle ? filter : null;
    }

    private static Stream PrepareRewindableStream(Stream sourceStream)
    {
        if (sourceStream.CanSeek)
            return sourceStream;

        var buffered = new MemoryStream();
        sourceStream.CopyTo(buffered);
        buffered.Position = 0;
        return buffered;
    }

    private static void ResetStream(Stream stream)
    {
        if (!stream.CanSeek)
            return;

        stream.Position = 0;
    }

    private sealed class Candidate
    {
        public Candidate(IAnalyzableInputFilter filter, InputFilterAnalysisResult analysis)
        {
            Filter = filter;
            Analysis = analysis;
        }

        public IAnalyzableInputFilter Filter { get; }

        public InputFilterAnalysisResult Analysis { get; }
    }
}
