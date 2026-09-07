using System;
using Terminal.Core;

namespace ConsoleLib.Showcase.Terminal.Core;

/// <summary>Negotiates whether parsed terminal mouse reporting can be used by the showcase.</summary>
public sealed class TerminalMouseNegotiator
{
    public bool IsEnabled(TerminalDocument document) =>
        document is not null &&
        document.MouseTrackingMode != TerminalMouseTrackingMode.None &&
        document.MouseProtocol == TerminalMouseProtocol.Sgr;

    public string? Encode(TerminalDocument document, int buttonCode, int column, int row, bool isRelease)
    {
        if (!IsEnabled(document))
            return null;
        return TerminalInputEncoder.EncodeMouseSgr(buttonCode, column, row, isRelease);
    }
}
