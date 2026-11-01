using System;

namespace AA98_AvlnCodeStudio.Base.Debugging.Models;

/// <summary>
/// Describes a state or output event reported by a debug adapter.
/// </summary>
public sealed class DebugSessionEventArgs : EventArgs
{
    /// <summary>
    /// Gets or sets the provider-specific session identifier.
    /// </summary>
    public string SessionId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the adapter event name.
    /// </summary>
    public string EventName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the resulting session state, when the event changes it.
    /// </summary>
    public DebugSessionState? State { get; set; }

    /// <summary>
    /// Gets or sets the adapter's event reason.
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// Gets or sets the adapter's human-readable event description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the affected thread identifier, when present.
    /// </summary>
    public int? ThreadId { get; set; }

    /// <summary>
    /// Gets or sets output text for an adapter output event.
    /// </summary>
    public string? Output { get; set; }
}
