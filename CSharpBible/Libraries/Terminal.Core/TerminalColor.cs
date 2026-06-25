namespace Terminal.Core;

/// <summary>
/// Represents an RGB terminal color.
/// </summary>
public readonly record struct TerminalColor(byte Red, byte Green, byte Blue)
{
    /// <summary>
    /// Gets the default foreground color.
    /// </summary>
    public static TerminalColor DefaultForeground { get; } = new(220, 220, 220);

    /// <summary>
    /// Gets the default background color.
    /// </summary>
    public static TerminalColor DefaultBackground { get; } = new(12, 12, 12);
}
