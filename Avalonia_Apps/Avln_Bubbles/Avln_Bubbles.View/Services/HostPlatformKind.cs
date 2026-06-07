namespace Avln_Bubbles.View.Services;

/// <summary>
/// Describes the current hosting platform for the shared Avalonia application.
/// </summary>
public enum HostPlatformKind
{
    /// <summary>
    /// Classic desktop host.
    /// </summary>
    Desktop,

    /// <summary>
    /// Browser host using the Avalonia WebAssembly stack.
    /// </summary>
    Browser,

    /// <summary>
    /// Reserved for future remote-hosted scenarios.
    /// </summary>
    Remote
}
