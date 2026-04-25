namespace TraceAnalysis.Workbench.Core.Models;

/// <summary>
/// Represents a validation or diagnostic issue shown by the workbench shell.
/// </summary>
public sealed class ValidationIssue
{
    /// <summary>
    /// Initializes a new instance of <see cref="ValidationIssue"/>.
    /// </summary>
    /// <param name="severity">Issue severity.</param>
    /// <param name="message">Human-readable issue message.</param>
    public ValidationIssue(ValidationSeverity severity, string message)
    {
        Severity = severity;
        Message = message;
    }

    /// <summary>
    /// Gets the issue severity.
    /// </summary>
    public ValidationSeverity Severity { get; }

    /// <summary>
    /// Gets the human-readable issue message.
    /// </summary>
    public string Message { get; }
}
