namespace AppKomponentBaseLib.Diagnostics;

/// <summary>
/// Represents a provider-neutral diagnostic entry.
/// </summary>
public sealed class Diagnostic
{
    /// <summary>
    /// Gets or sets the stable diagnostic code.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the human-readable diagnostic message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the normalized diagnostic severity.
    /// </summary>
    public DiagnosticSeverity Severity { get; set; } = DiagnosticSeverity.Unknown;

    /// <summary>
    /// Gets or sets the optional source file path associated with the diagnostic.
    /// </summary>
    public string? SourcePath { get; set; }

    /// <summary>
    /// Gets or sets the optional source line number associated with the diagnostic.
    /// </summary>
    public int? LineNumber { get; set; }
}
