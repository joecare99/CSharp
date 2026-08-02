using System;
using System.Collections.Generic;

namespace RnzTrauer.Core.Services;

/// <summary>One positional row produced by the legacy RNZ portal import grid.</summary>
public sealed class SchemaImportRow
{
    /// <summary>The legacy grid has sixteen positional columns.</summary>
    public const int ColumnCount = 16;

    private readonly string[] _columns = new string[ColumnCount];

    /// <summary>Creates a row with deterministic empty values in every legacy column.</summary>
    public SchemaImportRow()
    {
        Array.Fill(_columns, string.Empty);
    }

    /// <summary>Gets a column value by its legacy zero-based grid index.</summary>
    public string this[int index]
    {
        get
        {
            if ((uint)index >= ColumnCount)
                throw new ArgumentOutOfRangeException(nameof(index));
            return _columns[index];
        }
        set
        {
            if ((uint)index >= ColumnCount)
                throw new ArgumentOutOfRangeException(nameof(index));
            _columns[index] = value ?? string.Empty;
        }
    }

    /// <summary>Returns a snapshot suitable for persistence or fixture comparison.</summary>
    public IReadOnlyList<string> Columns => _columns;
}
