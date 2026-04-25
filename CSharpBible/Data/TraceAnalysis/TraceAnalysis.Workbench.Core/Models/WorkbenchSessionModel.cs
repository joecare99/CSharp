using System.Collections.Generic;

namespace TraceAnalysis.Workbench.Core.Models;

/// <summary>
/// Represents the shell-level state used to initialize the workbench view models.
/// </summary>
public sealed class WorkbenchSessionModel
{
    /// <summary>
    /// Initializes a new instance of <see cref="WorkbenchSessionModel"/>.
    /// </summary>
    /// <param name="configurationName">Current configuration name.</param>
    /// <param name="traceSource">Current trace source state.</param>
    /// <param name="channels">Discovered channels.</param>
    /// <param name="steps">Configured processing steps.</param>
    /// <param name="diagnostics">Source and processing diagnostics.</param>
    public WorkbenchSessionModel(
        string configurationName,
        TraceSourceState traceSource,
        IReadOnlyList<TraceChannelItem> channels,
        IReadOnlyList<ProcessingStepState> steps,
        WorkbenchDiagnosticsModel diagnostics)
    {
        ConfigurationName = configurationName;
        TraceSource = traceSource;
        Channels = channels;
        Steps = steps;
        Diagnostics = diagnostics;
    }

    /// <summary>
    /// Gets the current configuration name.
    /// </summary>
    public string ConfigurationName { get; }

    /// <summary>
    /// Gets the current trace source state.
    /// </summary>
    public TraceSourceState TraceSource { get; }

    /// <summary>
    /// Gets the discovered source or derived channels.
    /// </summary>
    public IReadOnlyList<TraceChannelItem> Channels { get; }

    /// <summary>
    /// Gets the configured processing steps.
    /// </summary>
    public IReadOnlyList<ProcessingStepState> Steps { get; }

    /// <summary>
    /// Gets the source and processing diagnostics.
    /// </summary>
    public WorkbenchDiagnosticsModel Diagnostics { get; }
}
