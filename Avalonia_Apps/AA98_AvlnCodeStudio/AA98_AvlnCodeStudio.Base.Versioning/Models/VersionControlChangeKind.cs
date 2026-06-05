namespace AA98_AvlnCodeStudio.Base.Versioning.Models;

/// <summary>
/// Describes the high-level kind of a version-controlled file change.
/// </summary>
public enum VersionControlChangeKind
{
    /// <summary>
    /// The change kind is not yet known.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// A new item was added.
    /// </summary>
    Added,

    /// <summary>
    /// An existing item was modified.
    /// </summary>
    Modified,

    /// <summary>
    /// An existing item was deleted.
    /// </summary>
    Deleted,

    /// <summary>
    /// An existing item was renamed.
    /// </summary>
    Renamed,

    /// <summary>
    /// An existing item was copied.
    /// </summary>
    Copied,

    /// <summary>
    /// An existing item was moved.
    /// </summary>
    Moved
}
