using BaseLib.Interfaces;
using ConsoleLib.Interfaces;
using ConsoleLib.Showcase.Desktop.Capabilities;
using ConsoleLib.Showcase.Services;

namespace ConsoleLib.Showcase.DesktopHost;

/// <summary>Capability registration for the native ExtendedConsole host.</summary>
public sealed class ExtConShowcaseHostCapabilities : IShowcaseHostCapabilities
{
    public ExtConShowcaseHostCapabilities(IConsole console, IShowcaseTerminalService terminal)
    {
        Alert = new ConsoleAlertService(console);
        Terminal = new ExtConTerminalCapability(terminal);
    }

    public IClipboardService? Clipboard => null;

    public bool SupportsMouse => true;

    public bool SupportsTerminal => true;

    public IShowcaseTerminalCapability Terminal { get; }

    public IShowcaseAlertService Alert { get; }
}
