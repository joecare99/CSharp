using System;
using System.IO;

namespace Code.Navigation;

/// <summary>
/// Identifies an optional position in a source document without depending on a
/// user interface, editor, or operating-system-specific host.
/// </summary>
/// <param name="Path">The normalized path of the source document.</param>
/// <param name="Line">The one-based line number, or <see langword="null"/> when unspecified.</param>
/// <param name="Column">The one-based column number, or <see langword="null"/> when unspecified.</param>
/// <param name="Offset">The zero-based character offset, or <see langword="null"/> when unspecified.</param>
public sealed record CodeLocation
{
    /// <summary>
    /// Initializes a source location and validates all supplied coordinates.
    /// </summary>
    public CodeLocation(string path, int? line = null, int? column = null, int? offset = null)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("A source path is required.", nameof(path));
        }

        if (line is <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(line), line, "The line number must be positive.");
        }

        if (column is <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(column), column, "The column number must be positive.");
        }

        if (offset is < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), offset, "The offset cannot be negative.");
        }

        Path = NormalizePath(path);
        Line = line;
        Column = column;
        Offset = offset;
    }

    /// <summary>Gets the normalized source-document path.</summary>
    public string Path { get; }

    /// <summary>Gets the optional one-based line number.</summary>
    public int? Line { get; }

    /// <summary>Gets the optional one-based column number.</summary>
    public int? Column { get; }

    /// <summary>Gets the optional zero-based character offset.</summary>
    public int? Offset { get; }

    private static string NormalizePath(string path)
    {
        var trimmedPath = path.Trim();
        return System.IO.Path.GetFullPath(trimmedPath);
    }
}
