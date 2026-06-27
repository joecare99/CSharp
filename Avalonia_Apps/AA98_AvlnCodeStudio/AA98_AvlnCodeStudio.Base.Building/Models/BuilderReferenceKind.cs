namespace AA98_AvlnCodeStudio.Base.Building.Models;

/// <summary>
/// Identifies the normalized kind of a builder reference entry.
/// </summary>
public enum BuilderReferenceKind
{
    /// <summary>
    /// The reference kind is not known.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// The reference points to another project.
    /// </summary>
    Project = 1,

    /// <summary>
    /// The reference points to a package dependency.
    /// </summary>
    Package = 2,

    /// <summary>
    /// The reference points to a framework dependency.
    /// </summary>
    Framework = 3,

    /// <summary>
    /// The reference points to a resolved metadata asset.
    /// </summary>
    Metadata = 4,
}
