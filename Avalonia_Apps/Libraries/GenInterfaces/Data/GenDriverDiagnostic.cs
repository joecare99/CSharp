using System;

namespace GenInterfaces.Data;

/// <summary>
/// Represents a diagnostic emitted by a genealogy import or export driver.
/// </summary>
public sealed class GenDriverDiagnostic
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GenDriverDiagnostic"/> class.
    /// </summary>
    /// <param name="severity">The diagnostic severity.</param>
    /// <param name="message">The human-readable diagnostic message.</param>
    /// <param name="fileContext">The logical file context, if known.</param>
    /// <param name="lineNumber">The source line number, if known.</param>
    /// <param name="code">An optional stable diagnostic code.</param>
    public GenDriverDiagnostic(
        GenDriverDiagnosticSeverity severity,
        string message,
        string? fileContext = null,
        int? lineNumber = null,
        string? code = null)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("A diagnostic message is required.", nameof(message));
        }

        if (lineNumber <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lineNumber), "Line numbers must be greater than zero when provided.");
        }

        Severity = severity;
        Message = message;
        FileContext = fileContext;
        LineNumber = lineNumber;
        Code = code;
    }

    /// <summary>
    /// Gets the severity of the diagnostic.
    /// </summary>
    public GenDriverDiagnosticSeverity Severity { get; }

    /// <summary>
    /// Gets the human-readable diagnostic message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets the logical file context for the diagnostic, if known.
    /// </summary>
    public string? FileContext { get; }

    /// <summary>
    /// Gets the source line number for the diagnostic, if known.
    /// </summary>
    public int? LineNumber { get; }

    /// <summary>
    /// Gets the optional stable diagnostic code.
    /// </summary>
    public string? Code { get; }
}
