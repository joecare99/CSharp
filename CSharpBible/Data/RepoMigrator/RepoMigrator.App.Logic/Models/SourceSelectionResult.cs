using RepoMigrator.Core;

namespace RepoMigrator.App.Logic.Models;

/// <summary>
/// Represents the provider selection data required by the source side of the workflow.
/// </summary>
public sealed class SourceSelectionResult
{
    /// <summary>
    /// Gets or sets the available Git branches.
    /// </summary>
    public IReadOnlyList<RepositoryReferenceInfo> Branches { get; init; } = [];

    /// <summary>
    /// Gets or sets the available Git tags.
    /// </summary>
    public IReadOnlyList<RepositoryReferenceInfo> Tags { get; init; } = [];

    /// <summary>
    /// Gets or sets the default source branch.
    /// </summary>
    public string? DefaultBranch { get; init; }

    /// <summary>
    /// Gets or sets the available SVN revisions.
    /// </summary>
    public IReadOnlyList<RepositoryRevisionInfo> Revisions { get; init; } = [];

    /// <summary>
    /// Gets or sets the suggested inclusive SVN from revision.
    /// </summary>
    public string? SuggestedFromRevisionId { get; init; }

    /// <summary>
    /// Gets or sets the suggested inclusive SVN to revision.
    /// </summary>
    public string? SuggestedToRevisionId { get; init; }
}
