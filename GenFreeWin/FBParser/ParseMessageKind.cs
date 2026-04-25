namespace FBParser;

/// <summary>
/// Represents the classification of a parser message.
/// </summary>
public enum ParseMessageKind
{
    /// <summary>
    /// A non-recoverable or validation error occurred while parsing.
    /// </summary>
    Error,

    /// <summary>
    /// A recoverable issue or suspicious input was detected while parsing.
    /// </summary>
    Warning,

    /// <summary>
    /// A diagnostic message emitted for tracing parser state.
    /// </summary>
    Debug,
}
