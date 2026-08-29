namespace ConsoleLib.Interfaces;

/// <summary>
/// Describes optional capabilities exposed by a terminal host.
/// </summary>
public interface ITerminalCapabilities
{
    bool SupportsAnsi { get; }
    bool SupportsColor { get; }
    bool SupportsCursor { get; }
    bool SupportsMouse { get; }
    bool SupportsClipboardCopy { get; }
    bool SupportsClipboardPaste { get; }
    bool IsInputRedirected { get; }
    bool IsOutputRedirected { get; }
}
