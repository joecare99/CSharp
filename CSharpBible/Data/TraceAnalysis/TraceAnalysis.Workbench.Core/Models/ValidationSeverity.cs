namespace TraceAnalysis.Workbench.Core.Models;

/// <summary>
/// Defines the severity of a validation or diagnostic issue.
/// </summary>
public enum ValidationSeverity
{
    /// <summary>
    /// Informational item.
    /// </summary>
    Info,

    /// <summary>
    /// Warning item.
    /// </summary>
    Warning,

    /// <summary>
    /// Error item.
    /// </summary>
    Error
}
