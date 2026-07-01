namespace AppKomponentBaseLib.Diagnostics;

/// <summary>
/// Defines normalized severities for diagnostics.
/// </summary>
public enum DiagnosticSeverity
{
    /// <summary>
    /// The diagnostic severity is unknown.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// The diagnostic is informational.
    /// </summary>
    Info = 1,

    /// <summary>
    /// The diagnostic represents a warning.
    /// </summary>
    Warning = 2,

    /// <summary>
    /// The diagnostic represents an error.
    /// </summary>
    Error = 3,
}
