namespace AA98_AvlnCodeStudio.Base.Versioning.Models;

/// <summary>
/// Describes the kind of active local repository reference.
/// </summary>
public enum VersionControlReferenceKind
{
    /// <summary>
    /// The reference kind is not yet known.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// The repository is on a local branch.
    /// </summary>
    Branch,

    /// <summary>
    /// The repository is on a tag.
    /// </summary>
    Tag,

    /// <summary>
    /// The repository is in a detached commit state.
    /// </summary>
    Detached,

    /// <summary>
    /// The repository uses another local reference concept.
    /// </summary>
    Other
}
