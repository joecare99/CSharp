namespace AA98_AvlnCodeStudio.Base.Building.Models;

/// <summary>
/// Represents a normalized builder diagnostic entry.
/// </summary>
public sealed class BuilderDiagnostic
{
    /// <summary>
    /// Gets or sets the diagnostic code.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the diagnostic message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the normalized diagnostic severity.
    /// </summary>
    public BuilderDiagnosticSeverity Severity { get; set; } = BuilderDiagnosticSeverity.Unknown;

    /// <summary>
    /// Gets or sets the optional source file path.
    /// </summary>
    public string? FilePath { get; set; }

    /// <summary>
    /// Gets or sets the optional source line number.
    /// </summary>
    public int? LineNumber { get; set; }

    /// <summary>
    /// Gets or sets the optional source column number.
    /// </summary>
    public int? ColumnNumber { get; set; }
}
