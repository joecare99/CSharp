namespace AA98_AvlnCodeStudio.Base.Building.Models;

/// <summary>
/// Represents the normalized severity of a builder diagnostic.
/// </summary>
public enum BuilderDiagnosticSeverity
{
    /// <summary>
    /// No severity has been assigned.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// The diagnostic is informational.
    /// </summary>
    Info = 1,

    /// <summary>
    /// The diagnostic is a warning.
    /// </summary>
    Warning = 2,

    /// <summary>
    /// The diagnostic is an error.
    /// </summary>
    Error = 3,
}
