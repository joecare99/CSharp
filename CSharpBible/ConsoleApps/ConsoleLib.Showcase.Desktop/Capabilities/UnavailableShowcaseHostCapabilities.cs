using ConsoleLib.Interfaces;

namespace ConsoleLib.Showcase.Desktop.Capabilities;

/// <summary>Default capability set for portable and designer hosts.</summary>
public sealed class UnavailableShowcaseHostCapabilities : IShowcaseHostCapabilities
{
    public IClipboardService? Clipboard => null;

    public bool SupportsMouse => false;

    public bool SupportsTerminal => false;

    public IShowcaseTerminalCapability? Terminal => null;

    public IShowcaseAlertService Alert { get; } = new UnavailableShowcaseAlertService();
}
