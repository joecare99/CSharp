using System.Collections.Generic;

namespace TraceAnalysis.Workbench.Core.Models;

/// <summary>
/// Represents a persisted processing configuration used by the workbench and console runner.
/// </summary>
public sealed class ProcessingConfigurationModel
{
    /// <summary>
    /// Initializes a new instance of <see cref="ProcessingConfigurationModel"/>.
    /// </summary>
    /// <param name="configurationName">The configuration name.</param>
    /// <param name="steps">The configured processing steps.</param>
    public ProcessingConfigurationModel(string configurationName, IReadOnlyList<ProcessingStepState> steps)
    {
        ConfigurationName = configurationName;
        Steps = steps;
    }

    /// <summary>
    /// Gets the configuration name.
    /// </summary>
    public string ConfigurationName { get; }

    /// <summary>
    /// Gets the configured processing steps.
    /// </summary>
    public IReadOnlyList<ProcessingStepState> Steps { get; }
}
