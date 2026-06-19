namespace Workbench.Builder.Core.Models.Diagnostics;

/// <summary>
/// Represents a structured diagnostic produced during project inspection or build execution.
/// </summary>
public sealed class BuildDiagnostic
{
    /// <summary>
    /// Initializes a new instance of <see cref="BuildDiagnostic"/>.
    /// </summary>
    /// <param name="severity">The severity level of the diagnostic.</param>
    /// <param name="code">The diagnostic code or category identifier.</param>
    /// <param name="message">The human-readable diagnostic message.</param>
    /// <param name="filePath">The optional file path associated with the diagnostic.</param>
    /// <param name="line">The optional one-based line number associated with the diagnostic.</param>
    /// <param name="column">The optional one-based column number associated with the diagnostic.</param>
    public BuildDiagnostic(
        BuildDiagnosticSeverity severity,
        string code,
        string message,
        string? filePath = null,
        int? line = null,
        int? column = null)
    {
        Severity = severity;
        Code = code;
        Message = message;
        FilePath = filePath;
        Line = line;
        Column = column;
    }

    /// <summary>
    /// Gets the severity level of the diagnostic.
    /// </summary>
    public BuildDiagnosticSeverity Severity { get; }

    /// <summary>
    /// Gets the diagnostic code or category identifier.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Gets the human-readable diagnostic message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets the optional file path associated with the diagnostic.
    /// </summary>
    public string? FilePath { get; }

    /// <summary>
    /// Gets the optional one-based line number associated with the diagnostic.
    /// </summary>
    public int? Line { get; }

    /// <summary>
    /// Gets the optional one-based column number associated with the diagnostic.
    /// </summary>
    public int? Column { get; }
}
