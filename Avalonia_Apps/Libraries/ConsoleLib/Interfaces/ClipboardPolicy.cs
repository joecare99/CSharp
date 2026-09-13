using System;

namespace ConsoleLib.Interfaces;

/// <summary>
/// Defines the safety policy for terminal clipboard integration.
/// </summary>
public sealed class ClipboardPolicy
{
    public ClipboardPolicy(
        bool allowOsc52Copy = false,
        bool allowOsc52Paste = false,
        int maximumPayloadLength = 4096)
    {
        if (maximumPayloadLength <= 0)
            throw new ArgumentOutOfRangeException(nameof(maximumPayloadLength));

        AllowOsc52Copy = allowOsc52Copy;
        AllowOsc52Paste = allowOsc52Paste;
        MaximumPayloadLength = maximumPayloadLength;
    }

    public bool AllowOsc52Copy { get; }
    public bool AllowOsc52Paste { get; }
    public int MaximumPayloadLength { get; }

    public bool CanCopy(ITerminalCapabilities? capabilities, string? text)
    {
        var payload = text;
        if (!AllowOsc52Copy || capabilities is null || !capabilities.SupportsClipboardCopy || payload is null || payload.Length == 0)
            return false;

        return payload.Length <= MaximumPayloadLength;
    }

    public bool CanPaste(ITerminalCapabilities? capabilities)
    {
        return AllowOsc52Paste && capabilities is not null && capabilities.SupportsClipboardPaste;
    }
}
