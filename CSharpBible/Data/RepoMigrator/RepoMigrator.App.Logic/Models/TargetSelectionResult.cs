namespace RepoMigrator.App.Logic.Models;

/// <summary>
/// Represents the provider selection data required by the target side of the workflow.
/// </summary>
public sealed class TargetSelectionResult
{
    /// <summary>
    /// Gets or sets the available target branches.
    /// </summary>
    public IReadOnlyList<string> Branches { get; init; } = [];

    /// <summary>
    /// Gets or sets the default target branch.
    /// </summary>
    public string? DefaultBranch { get; init; }
}
