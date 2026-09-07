using ConsoleLib.Interfaces;

namespace ConsoleLib.Showcase.Desktop.Capabilities;

/// <summary>
/// Host capabilities selected by the widget-set composition root.
/// Portable desktop code consumes this contract instead of detecting an OS.
/// </summary>
public interface IShowcaseHostCapabilities
{
    /// <summary>Optional clipboard implementation supplied by the host.</summary>
    IClipboardService? Clipboard { get; }

    /// <summary>Whether the host can deliver pointer input with the desktop.</summary>
    bool SupportsMouse { get; }

    /// <summary>Whether a terminal session provider is available.</summary>
    bool SupportsTerminal { get; }

    /// <summary>Host alert output, which may be unavailable.</summary>
    IShowcaseAlertService Alert { get; }
}
