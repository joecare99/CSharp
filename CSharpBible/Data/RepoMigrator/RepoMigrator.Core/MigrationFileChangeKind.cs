namespace RepoMigrator.Core;

/// <summary>
/// Defines the normalized file-level operation represented by a structured migration change.
/// </summary>
public enum MigrationFileChangeKind
{
    /// <summary>
    /// Represents a file creation.
    /// </summary>
    Add,

    /// <summary>
    /// Represents an in-place file content or metadata modification.
    /// </summary>
    Modify,

    /// <summary>
    /// Represents a file removal.
    /// </summary>
    Delete,

    /// <summary>
    /// Represents a file rename or move.
    /// </summary>
    Rename,

    /// <summary>
    /// Represents a file copy.
    /// </summary>
    Copy,

    /// <summary>
    /// Represents a file mode or attribute change without required content replacement.
    /// </summary>
    ModeChange
}
