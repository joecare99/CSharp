using System;

namespace ConsoleLib.Interfaces;

/// <summary>
/// Immutable terminal capability description.
/// </summary>
public sealed class TerminalCapabilities : ITerminalCapabilities
{
    public TerminalCapabilities(
        bool supportsAnsi,
        bool supportsColor,
        bool supportsCursor,
        bool supportsMouse,
        bool supportsClipboardCopy,
        bool supportsClipboardPaste,
        bool isInputRedirected,
        bool isOutputRedirected)
    {
        SupportsAnsi = supportsAnsi;
        SupportsColor = supportsColor;
        SupportsCursor = supportsCursor;
        SupportsMouse = supportsMouse;
        SupportsClipboardCopy = supportsClipboardCopy;
        SupportsClipboardPaste = supportsClipboardPaste;
        IsInputRedirected = isInputRedirected;
        IsOutputRedirected = isOutputRedirected;
    }

    public bool SupportsAnsi { get; }
    public bool SupportsColor { get; }
    public bool SupportsCursor { get; }
    public bool SupportsMouse { get; }
    public bool SupportsClipboardCopy { get; }
    public bool SupportsClipboardPaste { get; }
    public bool IsInputRedirected { get; }
    public bool IsOutputRedirected { get; }

    public static TerminalCapabilities PlainText(bool inputRedirected = false, bool outputRedirected = false)
    {
        return new TerminalCapabilities(
            supportsAnsi: false,
            supportsColor: false,
            supportsCursor: false,
            supportsMouse: false,
            supportsClipboardCopy: false,
            supportsClipboardPaste: false,
            isInputRedirected: inputRedirected,
            isOutputRedirected: outputRedirected);
    }
}
