using System;
using BaseLib.Interfaces;
using ConsoleLib.Showcase.Desktop.Capabilities;

namespace ConsoleLib.Showcase.DesktopHost;

/// <summary>Uses the host console beep for alarm feedback.</summary>
public sealed class ConsoleAlertService : IShowcaseAlertService
{
    private readonly IConsole _console;

    public ConsoleAlertService(IConsole console) =>
        _console = console ?? throw new ArgumentNullException(nameof(console));

    public bool IsAvailable => !_console.IsOutputRedirected;

    public void Alert()
    {
        if (IsAvailable)
            _console.Beep(880, 120);
    }
}
