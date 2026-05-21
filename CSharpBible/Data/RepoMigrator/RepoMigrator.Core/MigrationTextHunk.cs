namespace RepoMigrator.Core;

/// <summary>
/// Represents one normalized text hunk inside a text change.
/// </summary>
public sealed class MigrationTextHunk
{
    /// <summary>
    /// Gets the starting line number of the original text range.
    /// </summary>
    public int OriginalStartLine { get; init; }

    /// <summary>
    /// Gets the starting line number of the resulting text range.
    /// </summary>
    public int ModifiedStartLine { get; init; }

    /// <summary>
    /// Gets the removed text block for the hunk.
    /// </summary>
    public string RemovedText { get; init; } = string.Empty;

    /// <summary>
    /// Gets the added text block for the hunk.
    /// </summary>
    public string AddedText { get; init; } = string.Empty;
}
