namespace Terminal.Core;

/// <summary>
/// Defines a mutable terminal text buffer.
/// </summary>
public interface ITerminalBuffer
{
    /// <summary>
    /// Gets the viewport size.
    /// </summary>
    TerminalSize Size { get; }

    /// <summary>
    /// Gets the current cursor state.
    /// </summary>
    TerminalCursorState Cursor { get; }

    /// <summary>
    /// Resizes the viewport.
    /// </summary>
    void Resize(TerminalSize size);

    /// <summary>
    /// Clears the visible viewport.
    /// </summary>
    void ClearViewport();

    /// <summary>
    /// Moves the cursor to the specified position.
    /// </summary>
    void SetCursorPosition(int column, int row);

    /// <summary>
    /// Sets the cursor visibility.
    /// </summary>
    void SetCursorVisibility(bool isVisible);

    /// <summary>
    /// Writes a character to the buffer.
    /// </summary>
    void Write(char character, TerminalColor foreground, TerminalColor background);

    /// <summary>
    /// Performs a carriage return.
    /// </summary>
    void CarriageReturn();

    /// <summary>
    /// Performs a line feed.
    /// </summary>
    void LineFeed();

    /// <summary>
    /// Performs a backspace.
    /// </summary>
    void Backspace();

    /// <summary>
    /// Moves the cursor forward by the specified number of columns.
    /// </summary>
    void MoveCursorForward(int columns);

    /// <summary>
    /// Erases the specified number of characters starting at the cursor position.
    /// </summary>
    void EraseCharacters(int characterCount);

    /// <summary>
    /// Clears from the cursor to the end of the line.
    /// </summary>
    void ClearToEndOfLine();

    /// <summary>
    /// Clears the current line.
    /// </summary>
    void ClearCurrentLine();

    /// <summary>
    /// Creates an immutable snapshot of the visible viewport.
    /// </summary>
    TerminalSnapshot CreateSnapshot();
}
