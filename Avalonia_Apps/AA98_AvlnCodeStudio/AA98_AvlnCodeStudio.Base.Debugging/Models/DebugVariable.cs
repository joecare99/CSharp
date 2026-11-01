namespace AA98_AvlnCodeStudio.Base.Debugging.Models;

/// <summary>
/// Represents a variable value in a paused debug session.
/// </summary>
public sealed class DebugVariable
{
    /// <summary>
    /// Gets or sets the containing scope name.
    /// </summary>
    public string Scope { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the variable name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display value.
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional type name.
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Gets or sets the optional expression that can be used to evaluate the variable.
    /// </summary>
    public string? EvaluateName { get; set; }

    /// <summary>
    /// Gets or sets the adapter reference for expandable child variables.
    /// </summary>
    public int VariablesReference { get; set; }
}
