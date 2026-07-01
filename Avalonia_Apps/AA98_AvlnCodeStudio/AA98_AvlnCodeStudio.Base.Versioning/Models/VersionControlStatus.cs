using System.Collections.Generic;

namespace AA98_AvlnCodeStudio.Base.Versioning.Models;

/// <summary>
/// Represents a provider-neutral snapshot of repository state information.
/// </summary>
public sealed class VersionControlStatus
{
    /// <summary>
    /// Gets or sets the repository root path.
    /// </summary>
    public string RepositoryRootPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the provider-neutral repository display name.
    /// </summary>
    public string? RepositoryName { get; set; }

    /// <summary>
    /// Gets or sets the current branch or equivalent version line name.
    /// </summary>
    public string? ActiveReferenceName { get; set; }

    /// <summary>
    /// Gets or sets the kind of the active local repository reference.
    /// </summary>
    public VersionControlReferenceKind ActiveReferenceKind { get; set; } = VersionControlReferenceKind.Unknown;

    /// <summary>
    /// Gets or sets a value indicating whether the repository root was discovered from context.
    /// </summary>
    public bool IsRepositoryRootDiscovered { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the repository contains local changes.
    /// </summary>
    public bool HasLocalChanges { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the working tree is detached from a named branch.
    /// </summary>
    public bool IsDetached { get; set; }

    /// <summary>
    /// Gets the provider-neutral repository capabilities.
    /// </summary>
    public IList<VersionControlCapability> Capabilities { get; } = new List<VersionControlCapability>();

    /// <summary>
    /// Gets the tracked change summaries.
    /// </summary>
    public IList<VersionControlChangeSummary> Changes { get; } = new List<VersionControlChangeSummary>();
}
