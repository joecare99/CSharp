namespace BaseGenClasses.Helper;

/// <summary>
/// Represents the helper message classification used by the genealogical parser integration.
/// </summary>
public enum EventType
{
    /// <summary>
    /// Indicates a recoverable warning.
    /// </summary>
    Warning,

    /// <summary>
    /// Indicates a non-recoverable error.
    /// </summary>
    Error,
}
