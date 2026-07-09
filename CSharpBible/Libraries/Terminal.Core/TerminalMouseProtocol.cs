namespace Terminal.Core;

/// <summary>
/// Defines the active mouse reporting protocol negotiated with the terminal session.
/// </summary>
public enum TerminalMouseProtocol
{
    /// <summary>
    /// No mouse reporting protocol is active.
    /// </summary>
    None,

    /// <summary>
    /// SGR extended mouse reporting protocol is active.
    /// </summary>
    Sgr
}
