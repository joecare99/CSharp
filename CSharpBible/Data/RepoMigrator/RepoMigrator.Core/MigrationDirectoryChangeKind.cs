namespace RepoMigrator.Core;

/// <summary>
/// Defines the normalized directory-level operation represented by a structured migration change.
/// </summary>
public enum MigrationDirectoryChangeKind
{
    /// <summary>
    /// Represents a directory creation.
    /// </summary>
    Add,

    /// <summary>
    /// Represents a directory removal.
    /// </summary>
    Delete,

    /// <summary>
    /// Represents a directory rename or move.
    /// </summary>
    Rename,

    /// <summary>
    /// Represents a directory copy.
    /// </summary>
    Copy
}
