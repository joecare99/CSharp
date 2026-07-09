namespace AA98_AvlnCodeStudio.Base.Versioning.Models;

/// <summary>
/// Represents a lightweight summary of a version-controlled item change.
/// </summary>
public sealed class VersionControlChangeSummary
{
    /// <summary>
    /// Gets or sets the repository-relative path of the changed item.
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional previous repository-relative path.
    /// </summary>
    public string? PreviousPath { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the change is staged.
    /// </summary>
    public bool IsStaged { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the change is ignored by local repository rules.
    /// </summary>
    public bool IsIgnored { get; set; }

    /// <summary>
    /// Gets or sets the high-level change kind.
    /// </summary>
    public VersionControlChangeKind ChangeKind { get; set; } = VersionControlChangeKind.Unknown;
}
