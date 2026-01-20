// ***********************************************************************
// Assembly         : ConsoleDisplay
// Author           : Mir
// Created          : 07-16-2022
//
// Last Modified By : Mir
// Last Modified On : 07-24-2022
// ***********************************************************************
// <copyright file="MyConsoleBase.cs" company="ConsoleDisplay">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;

namespace BaseLib.Interfaces;

/// <summary>
/// Defines an abstraction layer for console operations, enabling dependency injection
/// and testability for console-based applications.
/// </summary>
/// <remarks>
/// This interface provides a comprehensive set of console operations including:
/// <list type="bullet">
///     <item><description>Color management for foreground and background</description></item>
///     <item><description>Window sizing and positioning</description></item>
///     <item><description>Input/output operations</description></item>
///     <item><description>Cursor positioning</description></item>
///     <item><description>Audio feedback capabilities</description></item>
/// </list>
/// Implementations of this interface can wrap <see cref="System.Console"/> for production use
/// or provide mock implementations for unit testing purposes.
/// </remarks>
public interface IConsole
{
    /// <summary>
    /// Gets or sets the foreground color of the console output.
    /// </summary>
    /// <value>
    /// A <see cref="ConsoleColor"/> value representing the text color.
    /// </value>
    ConsoleColor ForegroundColor { get; set; }

    /// <summary>
    /// Gets or sets the background color of the console output.
    /// </summary>
    /// <value>
    /// A <see cref="ConsoleColor"/> value representing the background color behind the text.
    /// </value>
    ConsoleColor BackgroundColor { get; set; }

    /// <summary>
    /// Gets a value indicating whether the console output stream has been redirected.
    /// </summary>
    /// <value>
    /// <c>true</c> if the output is redirected to a file or another stream; otherwise, <c>false</c>.
    /// </value>
    bool IsOutputRedirected { get; }

    /// <summary>
    /// Gets a value indicating whether a key press is available in the input stream.
    /// </summary>
    /// <value>
    /// <c>true</c> if a key press is available to be read; otherwise, <c>false</c>.
    /// </value>
    bool KeyAvailable { get; }

    /// <summary>
    /// Gets the largest possible number of console window rows based on the current font and display resolution.
    /// </summary>
    /// <value>
    /// The maximum number of rows that can be displayed in the console window.
    /// </value>
    int LargestWindowHeight { get; }

    /// <summary>
    /// Gets or sets the title to display in the console window's title bar.
    /// </summary>
    /// <value>
    /// A string representing the console window title.
    /// </value>
    string Title { get; set; }

    /// <summary>
    /// Gets or sets the height of the console window area in rows.
    /// </summary>
    /// <value>
    /// The number of rows visible in the console window.
    /// </value>
    int WindowHeight { get; set; }

    /// <summary>
    /// Gets or sets the width of the console window area in columns.
    /// </summary>
    /// <value>
    /// The number of columns visible in the console window.
    /// </value>
    int WindowWidth { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the cursor is visible.
    /// </summary>
    bool CursorVisible { get; set; }

    /// <summary>
    /// Gets the width of the console buffer area.
    /// </summary>
    int BufferWidth { get; }

    /// <summary>
    /// Gets the height of the console buffer area.
    /// </summary>
    int BufferHeight { get; }

    /// <summary>
    /// Plays a beep sound through the console speaker.
    /// </summary>
    /// <param name="freq">The frequency of the beep sound in hertz (Hz). Valid range is typically 37 to 32767 Hz.</param>
    /// <param name="len">The duration of the beep sound in milliseconds.</param>
    void Beep(int freq, int len);

    /// <summary>
    /// Clears the console buffer and corresponding console window of all displayed information.
    /// </summary>
    void Clear();

    /// <summary>
    /// Gets the current position of the cursor within the console buffer.
    /// </summary>
    /// <returns>
    /// A tuple containing the <c>Left</c> (column) and <c>Top</c> (row) coordinates of the cursor position,
    /// where (0, 0) represents the top-left corner of the console buffer.
    /// </returns>
    (int Left, int Top) GetCursorPosition();

    /// <summary>
    /// Obtains the next character or function key pressed by the user.
    /// </summary>
    /// <returns>
    /// A <see cref="ConsoleKeyInfo"/> object describing the key pressed, including the character
    /// and any modifier keys (Alt, Shift, Control); or <c>null</c> if no key is available.
    /// </returns>
    ConsoleKeyInfo? ReadKey();

    /// <summary>
    /// Reads the next line of characters from the standard input stream.
    /// </summary>
    /// <returns>
    /// The next line of characters from the input stream, or an empty string if no input is available.
    /// </returns>
    string ReadLine();
    void ResetColor();

    /// <summary>
    /// Sets the position of the cursor within the console buffer.
    /// </summary>
    /// <param name="left">The column position of the cursor, where 0 is the leftmost column.</param>
    /// <param name="top">The row position of the cursor, where 0 is the topmost row.</param>
    void SetCursorPosition(int left, int top);

    /// <summary>
    /// Writes the specified Unicode character to the standard output stream.
    /// </summary>
    /// <param name="ch">The character to write to the console.</param>
    void Write(char ch);

    /// <summary>
    /// Writes the specified string value to the standard output stream.
    /// </summary>
    /// <param name="st">The string to write. Can be <c>null</c>, in which case nothing is written.</param>
    void Write(string? st);

    /// <summary>
    /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
    /// </summary>
    /// <param name="st">The string to write. Can be <c>null</c> or empty. Defaults to an empty string.</param>
    void WriteLine(string? st = "");
}