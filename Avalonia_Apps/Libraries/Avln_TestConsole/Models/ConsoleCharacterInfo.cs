using System;

namespace Avln_TestConsole.Models;

/// <summary>
/// Represents a single console cell including its character and colors.
/// </summary>
public struct ConsoleCharacterInfo
{
    /// <summary>
    /// Gets or sets the character stored in the console cell.
    /// </summary>
    public char Character { get; set; }

    /// <summary>
    /// Gets or sets the foreground color for the console cell.
    /// </summary>
    public ConsoleColor ForegroundColor { get; set; }

    /// <summary>
    /// Gets or sets the background color for the console cell.
    /// </summary>
    public ConsoleColor BackgroundColor { get; set; }

    /// <summary>
    /// Creates a default console cell.
    /// </summary>
    /// <returns>A new console cell with default colors and an empty character.</returns>
    public static ConsoleCharacterInfo CreateDefault()
        => new()
        {
            Character = '\0',
            ForegroundColor = ConsoleColor.Gray,
            BackgroundColor = ConsoleColor.Black,
        };
}
