namespace AA98_AvlnCodeStudio.Base.OS.Models;

/// <summary>
/// Represents the high-level state of a terminal session.
/// </summary>
public enum TerminalSessionState
{
    /// <summary>
    /// The session state is not known.
    /// </summary>
    Unknown,

    /// <summary>
    /// The session is actively running.
    /// </summary>
    Running,

    /// <summary>
    /// The session is stopped.
    /// </summary>
    Stopped,
}
