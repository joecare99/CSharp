namespace RepoMigrator.Core;

/// <summary>
/// Defines how a binary payload is represented inside a structured migration change.
/// </summary>
public enum MigrationBinaryPayloadMode
{
    /// <summary>
    /// Represents a small payload carried directly in the model.
    /// </summary>
    Inline,

    /// <summary>
    /// Represents a payload stored outside the in-memory model and referenced indirectly.
    /// </summary>
    FileReference
}
