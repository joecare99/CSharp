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
    /// Gets or sets the current branch or equivalent version line name.
    /// </summary>
    public string? ActiveReferenceName { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the repository contains local changes.
    /// </summary>
    public bool HasLocalChanges { get; set; }

    /// <summary>
    /// Gets the tracked change summaries.
    /// </summary>
    public IList<VersionControlChangeSummary> Changes { get; } = new List<VersionControlChangeSummary>();
}
