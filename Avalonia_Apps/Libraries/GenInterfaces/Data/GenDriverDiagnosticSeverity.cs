namespace GenInterfaces.Data;

/// <summary>
/// Defines the severity levels a genealogy driver diagnostic can report.
/// </summary>
public enum GenDriverDiagnosticSeverity
{
    /// <summary>
    /// Provides low-level tracing information.
    /// </summary>
    Trace = 0,

    /// <summary>
    /// Provides general informational context.
    /// </summary>
    Info = 1,

    /// <summary>
    /// Indicates a recoverable issue that should be visible to the caller.
    /// </summary>
    Warning = 2,

    /// <summary>
    /// Indicates a non-recoverable issue for the current operation.
    /// </summary>
    Error = 3,
}
