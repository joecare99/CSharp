namespace TraceAnalysis.Workbench.Core.Models;

/// <summary>
/// Describes the currently loaded trace source state for the workbench shell.
/// </summary>
public sealed class TraceSourceState
{
    /// <summary>
    /// Initializes a new instance of <see cref="TraceSourceState"/>.
    /// </summary>
    /// <param name="sourceId">Logical source identifier.</param>
    /// <param name="parseErrorCount">Number of parse errors observed for the source.</param>
    /// <param name="dataBasis">Derived structural data basis of the source.</param>
    public TraceSourceState(string? sourceId, int parseErrorCount, TraceDataBasisModel? dataBasis)
    {
        SourceId = sourceId;
        ParseErrorCount = parseErrorCount;
        DataBasis = dataBasis;
    }

    /// <summary>
    /// Gets the logical source identifier.
    /// </summary>
    public string? SourceId { get; }

    /// <summary>
    /// Gets the number of parse errors associated with the source.
    /// </summary>
    public int ParseErrorCount { get; }

    /// <summary>
    /// Gets the derived structural data basis for the source.
    /// </summary>
    public TraceDataBasisModel? DataBasis { get; }
}
