namespace RepoMigrator.Core;

/// <summary>
/// Represents one normalized logical change set that can be replayed independently from provider-specific formats.
/// </summary>
public sealed class MigrationChangeSet
{
    /// <summary>
    /// Gets the stable logical identifier of the change set.
    /// </summary>
    public string ChangeSetId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the logical message associated with the change set.
    /// </summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// Gets the author display name associated with the change set.
    /// </summary>
    public string AuthorName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the optional author e-mail address associated with the change set.
    /// </summary>
    public string? AuthorEmail { get; init; }

    /// <summary>
    /// Gets the timestamp associated with the change set.
    /// </summary>
    public DateTimeOffset Timestamp { get; init; }

    /// <summary>
    /// Gets the ordered normalized directory changes contained in the change set.
    /// </summary>
    public IReadOnlyList<MigrationDirectoryChange> DirectoryChanges { get; init; } = Array.Empty<MigrationDirectoryChange>();

    /// <summary>
    /// Gets the ordered normalized file changes contained in the change set.
    /// </summary>
    public IReadOnlyList<MigrationFileChange> FileChanges { get; init; } = Array.Empty<MigrationFileChange>();

    /// <summary>
    /// Gets optional provider-agnostic metadata preserved for diagnostics or later execution.
    /// </summary>
    public IReadOnlyDictionary<string, string> Metadata { get; init; } = new Dictionary<string, string>();
}
