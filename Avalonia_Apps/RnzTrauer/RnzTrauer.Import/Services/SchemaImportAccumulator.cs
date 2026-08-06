using System;
using System.Collections.Generic;

namespace RnzTrauer.Import.Services;

/// <summary>
/// UI-agnostic port of <c>TfraTrPortImport.ComputeFiltered</c>.
/// It preserves the legacy positional row and media callback behavior without a grid.
/// </summary>
public sealed class SchemaImportAccumulator : ISchemaImportAccumulator
{
    private readonly List<SchemaImportRow> _completedRows = [];
    private readonly List<string> _newFiles = [];
    private SchemaImportRow? _currentRow;
    private int _column;
    private char _computeMode;

    /// <inheritdoc />
    public IReadOnlyList<SchemaImportRow> CompletedRows => _completedRows;

    /// <inheritdoc />
    public SchemaImportRow? CurrentRow => _currentRow;

    /// <inheritdoc />
    public IReadOnlyList<string> NewFiles => _newFiles;

    /// <inheritdoc />
    public void Reset()
    {
        _completedRows.Clear();
        _newFiles.Clear();
        _currentRow = null;
        _column = 0;
        _computeMode = '\0';
    }

    /// <inheritdoc />
    public void Process(byte callbackType, string text)
    {
        text ??= string.Empty;
        switch (callbackType)
        {
            case 0:
                SetComputeMode(text);
                break;
            case 2:
                ProcessTagMarkup(text);
                break;
            case 3:
                ProcessStandardText(text);
                break;
        }
    }

    private void SetComputeMode(string text)
    {
        _computeMode = text.Length == 0 ? '\0' : char.ToUpperInvariant(text[0]);
        if (_computeMode == 'A')
        {
            _currentRow = new SchemaImportRow();
            _column = 1;
        }
        else if (_computeMode == 'D')
        {
            _currentRow ??= new SchemaImportRow();
            _column = 2;
        }
    }

    private void ProcessStandardText(string text)
    {
        if (_currentRow is null || (uint)_column >= SchemaImportRow.ColumnCount)
            return;

        _currentRow[_column] = text;
        if (_column == SchemaImportRow.ColumnCount - 1)
        {
            _completedRows.Add(_currentRow);
            _currentRow = null;
        }
    }

    private void ProcessTagMarkup(string text)
    {
        if (_computeMode == 'D' && text.StartsWith("<td ", StringComparison.Ordinal))
        {
            _column++;
            return;
        }

        var questionMark = text.IndexOf('?', StringComparison.Ordinal);
        if (_computeMode == 'A')
        {
            if (_currentRow is null)
                return;
            var path = ExtractAnchorPath(text, questionMark);
            _currentRow[2] = path;
        }
        else if (_computeMode == 'N' && questionMark >= 10)
        {
            var path = ExtractAnchorPath(text, questionMark);
            if (!string.IsNullOrWhiteSpace(path))
                _newFiles.Add(path);
        }
    }

    private static string ExtractAnchorPath(string text, int questionMark)
    {
        var equals = text.IndexOf('=', StringComparison.Ordinal);
        if (equals < 0)
            return string.Empty;

        var start = equals + 1;
        if (start < text.Length && (text[start] == '"' || text[start] == '\''))
            start++;
        var end = questionMark >= start ? questionMark : text.Length;
        if (end > start && (text[end - 1] == '"' || text[end - 1] == '\''))
            end--;
        return end > start ? text.Substring(start, end - start) : string.Empty;
    }
}
