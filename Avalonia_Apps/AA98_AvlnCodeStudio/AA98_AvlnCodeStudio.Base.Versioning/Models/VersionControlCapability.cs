namespace AA98_AvlnCodeStudio.Base.Versioning.Models;

/// <summary>
/// Describes a provider-neutral local repository capability.
/// </summary>
public enum VersionControlCapability
{
    /// <summary>
    /// The capability is not yet known.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Repository status inspection is available.
    /// </summary>
    InspectStatus,

    /// <summary>
    /// Branch or reference inspection is available.
    /// </summary>
    InspectReferences,

    /// <summary>
    /// Local change enumeration is available.
    /// </summary>
    EnumerateChanges,

    /// <summary>
    /// Staged versus unstaged distinction is available.
    /// </summary>
    DistinguishStagedChanges,

    /// <summary>
    /// Local ignore rules can be evaluated.
    /// </summary>
    EvaluateIgnoreRules
}
