namespace Terminal.Core;

/// <summary>
/// Provides a small set of terminal input encodings for common keys.
/// </summary>
public static class TerminalInputEncoder
{
    /// <summary>
    /// Encodes plain text for a terminal session.
    /// </summary>
    public static string EncodeText(string? text) => text ?? string.Empty;

    /// <summary>
    /// Encodes the Enter key.
    /// </summary>
    public static string EncodeEnter() => "\r";

    /// <summary>
    /// Encodes the Backspace key.
    /// </summary>
    public static string EncodeBackspace() => "\b";

    /// <summary>
    /// Encodes the Up Arrow key.
    /// </summary>
    public static string EncodeArrowUp() => "\u001b[A";

    /// <summary>
    /// Encodes the Down Arrow key.
    /// </summary>
    public static string EncodeArrowDown() => "\u001b[B";

    /// <summary>
    /// Encodes the Right Arrow key.
    /// </summary>
    public static string EncodeArrowRight() => "\u001b[C";

    /// <summary>
    /// Encodes the Left Arrow key.
    /// </summary>
    public static string EncodeArrowLeft() => "\u001b[D";

    /// <summary>
    /// Encodes an SGR mouse report for the active terminal session.
    /// </summary>
    public static string EncodeMouseSgr(int buttonCode, int column, int row, bool isRelease)
    {
        return $"\u001b[<{buttonCode};{column};{row}{(isRelease ? 'm' : 'M')}";
    }
}
