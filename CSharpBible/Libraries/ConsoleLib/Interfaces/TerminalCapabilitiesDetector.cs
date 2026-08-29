using System;

namespace ConsoleLib.Interfaces;

/// <summary>Detects conservative ANSI capabilities from standard streams and environment.</summary>
public static class TerminalCapabilitiesDetector
{
    public static TerminalCapabilities Detect(
        bool inputRedirected,
        bool outputRedirected,
        Func<string, string?>? environment = null)
    {
        environment ??= Environment.GetEnvironmentVariable;
        var term = environment("TERM");
        var termProgram = environment("TERM_PROGRAM");
        var noColor = environment("NO_COLOR") is not null;
        var ansi = !outputRedirected
            && !noColor
            && (termProgram is not null || !string.IsNullOrWhiteSpace(term))
            && !string.Equals(term, "dumb", StringComparison.OrdinalIgnoreCase);
        var color = ansi && (term?.IndexOf("color", StringComparison.OrdinalIgnoreCase) >= 0
            || environment("COLORTERM") is not null
            || termProgram is not null);

        return new TerminalCapabilities(
            supportsAnsi: ansi,
            supportsColor: color,
            supportsCursor: ansi,
            supportsMouse: ansi && termProgram is not null,
            supportsClipboardCopy: false,
            supportsClipboardPaste: false,
            isInputRedirected: inputRedirected,
            isOutputRedirected: outputRedirected);
    }
}
