namespace AA98_AvlnCodeStudio.Base.Debugging.Models;

/// <summary>
/// Represents provider-neutral summary information about a debugging session.
/// </summary>
public sealed class DebugSessionInfo
{
    /// <summary>
    /// Gets or sets the provider-specific session identifier.
    /// </summary>
    public string SessionId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional display name of the session target.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Gets or sets the current session state.
    /// </summary>
    public DebugSessionState State { get; set; } = DebugSessionState.Unknown;
}
