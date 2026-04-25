namespace TraceAnalysis.Workbench.Core.Models;

/// <summary>
/// Represents one configured parameter within a processing step.
/// </summary>
public sealed class ProcessingParameterState
{
    /// <summary>
    /// Initializes a new instance of <see cref="ProcessingParameterState"/>.
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="valueText">The parameter value rendered as text.</param>
    public ProcessingParameterState(string name, string valueText)
    {
        Name = name;
        ValueText = valueText;
    }

    /// <summary>
    /// Gets the parameter name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the parameter value rendered as text.
    /// </summary>
    public string ValueText { get; }
}
