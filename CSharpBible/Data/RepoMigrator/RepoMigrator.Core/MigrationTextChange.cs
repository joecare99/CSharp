namespace RepoMigrator.Core;

/// <summary>
/// Represents a normalized text-based file change with ordered hunks.
/// </summary>
public sealed class MigrationTextChange
{
    /// <summary>
    /// Gets the normalized line ending marker when one was preserved explicitly.
    /// </summary>
    public string? LineEnding { get; init; }

    /// <summary>
    /// Gets the ordered text hunks that describe the change.
    /// </summary>
    public IReadOnlyList<MigrationTextHunk> Hunks { get; init; } = Array.Empty<MigrationTextHunk>();
}
