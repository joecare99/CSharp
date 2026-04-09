namespace RepoMigrator.App.Logic.Models;

/// <summary>
/// Represents the updated resume values after a successful commit.
/// </summary>
public sealed class ResumeUpdateResult
{
    /// <summary>
    /// Gets or sets the exclusive lower boundary value.
    /// </summary>
    public string? FromExclusiveId { get; init; }

    /// <summary>
    /// Gets or sets the next selected SVN start revision.
    /// </summary>
    public string? SelectedSvnFromRevisionId { get; init; }
}
