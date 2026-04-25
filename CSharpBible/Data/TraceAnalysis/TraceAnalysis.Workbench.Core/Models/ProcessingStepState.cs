using System.Collections.Generic;

namespace TraceAnalysis.Workbench.Core.Models;

/// <summary>
/// Represents one processing step displayed by the workbench shell.
/// </summary>
public sealed class ProcessingStepState
{
    /// <summary>
    /// Initializes a new instance of <see cref="ProcessingStepState"/>.
    /// </summary>
    /// <param name="stepId">Stable step identifier.</param>
    /// <param name="operationName">Operation name.</param>
    /// <param name="isEnabled">Indicates whether the step is enabled.</param>
    /// <param name="inputs">Configured inputs for the step.</param>
    /// <param name="parameters">Configured parameters for the step.</param>
    /// <param name="outputs">Configured outputs for the step.</param>
    public ProcessingStepState(
        string stepId,
        string operationName,
        bool isEnabled,
        IReadOnlyList<ProcessingInputState> inputs,
        IReadOnlyList<ProcessingParameterState> parameters,
        IReadOnlyList<ProcessingOutputState> outputs)
    {
        StepId = stepId;
        OperationName = operationName;
        IsEnabled = isEnabled;
        Inputs = inputs;
        Parameters = parameters;
        Outputs = outputs;
    }

    /// <summary>
    /// Gets the stable step identifier.
    /// </summary>
    public string StepId { get; }

    /// <summary>
    /// Gets the operation name.
    /// </summary>
    public string OperationName { get; }

    /// <summary>
    /// Gets a value indicating whether the step is enabled.
    /// </summary>
    public bool IsEnabled { get; }

    /// <summary>
    /// Gets the configured inputs for the step.
    /// </summary>
    public IReadOnlyList<ProcessingInputState> Inputs { get; }

    /// <summary>
    /// Gets the configured parameters for the step.
    /// </summary>
    public IReadOnlyList<ProcessingParameterState> Parameters { get; }

    /// <summary>
    /// Gets the configured outputs for the step.
    /// </summary>
    public IReadOnlyList<ProcessingOutputState> Outputs { get; }
}
