using System.IO;
using TraceAnalysis.Base.Models.Interfaces;

namespace TraceAnalysis.Base.Filters;

/// <summary>
/// Extended input-filter contract that supports deterministic source analysis
/// and ranking-based selection.
/// </summary>
public interface IAnalyzableInputFilter : IInputFilter
{
    /// <summary>
    /// Stable filter identifier used for deterministic tie-breaking and manual selection.
    /// </summary>
    string FilterId { get; }

    /// <summary>
    /// Configured filter priority used in deterministic ranking when confidence ties occur.
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// Analyses the source and returns deterministic filter-selection data.
    /// </summary>
    /// <param name="stream">Source stream to inspect.</param>
    /// <param name="sourceDescriptor">Logical source descriptor and selection hints.</param>
    /// <returns>Analysis result for deterministic ranking.</returns>
    InputFilterAnalysisResult Analyze(Stream stream, FilterSourceDescriptor sourceDescriptor);

    /// <summary>
    /// Reads the source stream into a canonical data set using a source descriptor.
    /// </summary>
    /// <param name="stream">Source stream.</param>
    /// <param name="sourceDescriptor">Logical source descriptor and selection hints.</param>
    /// <returns>Canonical trace data set with optional parse errors.</returns>
    ITraceDataSet Read(Stream stream, FilterSourceDescriptor sourceDescriptor);
}
