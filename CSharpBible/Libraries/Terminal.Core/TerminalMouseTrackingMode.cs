namespace Terminal.Core;

/// <summary>
/// Defines the active mouse tracking mode reported by the terminal session.
/// </summary>
public enum TerminalMouseTrackingMode
{
    /// <summary>
    /// Mouse tracking is disabled.
    /// </summary>
    None,

    /// <summary>
    /// Mouse button press and release events are tracked.
    /// </summary>
    Button,

    /// <summary>
    /// Mouse drag events are tracked while a button is pressed.
    /// </summary>
    Drag,

    /// <summary>
    /// All mouse movement events are tracked.
    /// </summary>
    Move
}
