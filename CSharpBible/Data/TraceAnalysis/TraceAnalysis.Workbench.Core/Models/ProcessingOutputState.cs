namespace TraceAnalysis.Workbench.Core.Models;

/// <summary>
/// Represents one derived output definition within a processing step.
/// </summary>
public sealed class ProcessingOutputState
{
    /// <summary>
    /// Initializes a new instance of <see cref="ProcessingOutputState"/>.
    /// </summary>
    /// <param name="outputRole">Semantic output role.</param>
    /// <param name="channelName">Derived channel name.</param>
    /// <param name="unitText">Optional unit text.</param>
    public ProcessingOutputState(string? outputRole, string channelName, string? unitText)
    {
        OutputRole = outputRole;
        ChannelName = channelName;
        UnitText = unitText;
    }

    /// <summary>
    /// Gets the semantic output role.
    /// </summary>
    public string? OutputRole { get; }

    /// <summary>
    /// Gets the derived channel name.
    /// </summary>
    public string ChannelName { get; }

    /// <summary>
    /// Gets the optional unit text.
    /// </summary>
    public string? UnitText { get; }
}
