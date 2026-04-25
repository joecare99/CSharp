namespace TraceAnalysis.Workbench.Core.Models;

/// <summary>
/// Represents one configured input reference within a processing step.
/// </summary>
public sealed class ProcessingInputState
{
    /// <summary>
    /// Initializes a new instance of <see cref="ProcessingInputState"/>.
    /// </summary>
    /// <param name="sourceKind">The source kind, for example source or derived.</param>
    /// <param name="channelName">The referenced channel name.</param>
    /// <param name="sourceStepId">The producing step identifier when the input is derived.</param>
    public ProcessingInputState(string sourceKind, string channelName, string? sourceStepId)
    {
        SourceKind = sourceKind;
        ChannelName = channelName;
        SourceStepId = sourceStepId;
    }

    /// <summary>
    /// Gets the source kind.
    /// </summary>
    public string SourceKind { get; }

    /// <summary>
    /// Gets the referenced channel name.
    /// </summary>
    public string ChannelName { get; }

    /// <summary>
    /// Gets the producing step identifier for derived inputs.
    /// </summary>
    public string? SourceStepId { get; }
}
