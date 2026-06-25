namespace Terminal.Core;

/// <summary>
/// Represents a single terminal character cell.
/// </summary>
public readonly record struct TerminalCell(char Character, TerminalColor Foreground, TerminalColor Background)
{
    /// <summary>
    /// Gets a blank cell using the default terminal colors.
    /// </summary>
    public static TerminalCell Blank { get; } = new(' ', TerminalColor.DefaultForeground, TerminalColor.DefaultBackground);
}
