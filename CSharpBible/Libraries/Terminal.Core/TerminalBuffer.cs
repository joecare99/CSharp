using System;
using System.Collections.Generic;

namespace Terminal.Core;

/// <summary>
/// Provides a mutable terminal viewport buffer with simple scrollback behavior.
/// </summary>
public sealed class TerminalBuffer : ITerminalBuffer
{
    private readonly int _scrollbackLimit;
    private readonly List<TerminalCell[]> _lines = [];
    private int _cursorColumn;
    private int _cursorViewportRow;
    private bool _cursorVisible = true;
    private TerminalSize _size;

    /// <summary>
    /// Initializes a new instance of the <see cref="TerminalBuffer"/> class.
    /// </summary>
    public TerminalBuffer(TerminalSize size, int scrollbackLimit = 1000)
    {
        _size = size.Normalize();
        _scrollbackLimit = scrollbackLimit < 0 ? 0 : scrollbackLimit;
        EnsureViewportLines();
    }

    /// <inheritdoc/>
    public TerminalSize Size => _size;

    /// <inheritdoc/>
    public TerminalCursorState Cursor => new(_cursorColumn, _cursorViewportRow, _cursorVisible);

    /// <inheritdoc/>
    public void Resize(TerminalSize size)
    {
        _size = size.Normalize();
        for (var index = 0; index < _lines.Count; index++)
        {
            _lines[index] = ResizeLine(_lines[index], _size.Columns);
        }

        EnsureViewportLines();
        _cursorColumn = Math.Clamp(_cursorColumn, 0, _size.Columns - 1);
        _cursorViewportRow = Math.Clamp(_cursorViewportRow, 0, _size.Rows - 1);
    }

    /// <inheritdoc/>
    public void ClearViewport()
    {
        for (var row = 0; row < _size.Rows; row++)
        {
            _lines[GetLineIndex(row)] = CreateBlankLine();
        }

        _cursorColumn = 0;
        _cursorViewportRow = 0;
    }

    /// <inheritdoc/>
    public void SetCursorPosition(int column, int row)
    {
        _cursorColumn = Math.Clamp(column, 0, _size.Columns - 1);
        _cursorViewportRow = Math.Clamp(row, 0, _size.Rows - 1);
    }

    /// <inheritdoc/>
    public void SetCursorVisibility(bool isVisible)
    {
        _cursorVisible = isVisible;
    }

    /// <inheritdoc/>
    public void Write(char character, TerminalColor foreground, TerminalColor background)
    {
        if (character == '\n')
        {
            LineFeed();
            return;
        }

        if (character == '\r')
        {
            CarriageReturn();
            return;
        }

        if (_cursorColumn >= _size.Columns)
        {
            CarriageReturn();
            LineFeed();
        }

        var lineIndex = GetLineIndex(_cursorViewportRow);
        _lines[lineIndex][_cursorColumn] = new TerminalCell(character, foreground, background);
        _cursorColumn++;
    }

    /// <inheritdoc/>
    public void CarriageReturn()
    {
        _cursorColumn = 0;
    }

    /// <inheritdoc/>
    public void LineFeed()
    {
        if (_cursorViewportRow < _size.Rows - 1)
        {
            _cursorViewportRow++;
            return;
        }

        _lines.Add(CreateBlankLine());
        TrimScrollback();
    }

    /// <inheritdoc/>
    public void Backspace()
    {
        if (_cursorColumn <= 0)
        {
            return;
        }

        _cursorColumn--;
        var lineIndex = GetLineIndex(_cursorViewportRow);
        _lines[lineIndex][_cursorColumn] = TerminalCell.Blank;
    }

    /// <inheritdoc/>
    public void MoveCursorForward(int columns)
    {
        if (columns <= 0)
        {
            return;
        }

        _cursorColumn = Math.Clamp(_cursorColumn + columns, 0, _size.Columns - 1);
    }

    /// <inheritdoc/>
    public void EraseCharacters(int characterCount)
    {
        if (characterCount <= 0)
        {
            return;
        }

        var lineIndex = GetLineIndex(_cursorViewportRow);
        var lastColumn = Math.Min(_size.Columns, _cursorColumn + characterCount);
        for (var column = _cursorColumn; column < lastColumn; column++)
        {
            _lines[lineIndex][column] = TerminalCell.Blank;
        }
    }

    /// <inheritdoc/>
    public void ClearToEndOfLine()
    {
        var lineIndex = GetLineIndex(_cursorViewportRow);
        for (var column = _cursorColumn; column < _size.Columns; column++)
        {
            _lines[lineIndex][column] = TerminalCell.Blank;
        }
    }

    /// <inheritdoc/>
    public void ClearCurrentLine()
    {
        _lines[GetLineIndex(_cursorViewportRow)] = CreateBlankLine();
        _cursorColumn = 0;
    }

    /// <inheritdoc/>
    public TerminalSnapshot CreateSnapshot()
    {
        var visibleLines = new List<IReadOnlyList<TerminalCell>>(_size.Rows);
        for (var row = 0; row < _size.Rows; row++)
        {
            var line = _lines[GetLineIndex(row)];
            var clone = new TerminalCell[_size.Columns];
            Array.Copy(line, clone, clone.Length);
            visibleLines.Add(clone);
        }

        return new TerminalSnapshot(_size, Cursor, visibleLines);
    }

    private void EnsureViewportLines()
    {
        while (_lines.Count < _size.Rows)
        {
            _lines.Add(CreateBlankLine());
        }
    }

    private int GetLineIndex(int viewportRow)
    {
        var firstVisibleLine = Math.Max(0, _lines.Count - _size.Rows);
        return firstVisibleLine + Math.Clamp(viewportRow, 0, _size.Rows - 1);
    }

    private TerminalCell[] CreateBlankLine()
    {
        var line = new TerminalCell[_size.Columns];
        Array.Fill(line, TerminalCell.Blank);
        return line;
    }

    private void TrimScrollback()
    {
        var maxLines = _size.Rows + _scrollbackLimit;
        while (_lines.Count > maxLines)
        {
            _lines.RemoveAt(0);
        }
    }

    private static TerminalCell[] ResizeLine(TerminalCell[] source, int width)
    {
        var resized = new TerminalCell[width];
        Array.Fill(resized, TerminalCell.Blank);
        Array.Copy(source, resized, Math.Min(source.Length, width));
        return resized;
    }
}
