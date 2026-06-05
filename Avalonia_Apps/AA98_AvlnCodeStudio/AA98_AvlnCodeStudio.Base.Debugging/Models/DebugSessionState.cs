namespace AA98_AvlnCodeStudio.Base.Debugging.Models;

/// <summary>
/// Describes the high-level state of a debugging session.
/// </summary>
public enum DebugSessionState
{
    /// <summary>
    /// The state is not yet known.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// The session is starting.
    /// </summary>
    Starting,

    /// <summary>
    /// The session is active.
    /// </summary>
    Running,

    /// <summary>
    /// The session is paused.
    /// </summary>
    Paused,

    /// <summary>
    /// The session ended normally.
    /// </summary>
    Stopped,

    /// <summary>
    /// The session terminated unexpectedly.
    /// </summary>
    Faulted
}
