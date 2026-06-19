namespace Workbench.Builder.Core.Models.Diagnostics;

/// <summary>
/// Defines the severity level for a build or inspection diagnostic.
/// </summary>
public enum BuildDiagnosticSeverity
{
    /// <summary>
    /// The diagnostic is informational and does not indicate a problem.
    /// </summary>
    Information,

    /// <summary>
    /// The diagnostic indicates a warning that should be reviewed.
    /// </summary>
    Warning,

    /// <summary>
    /// The diagnostic indicates an error that prevents the intended operation.
    /// </summary>
    Error,
}
