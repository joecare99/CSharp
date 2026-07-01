namespace AA98_AvlnCodeStudio.Base.Versioning.Models;

/// <summary>
/// Represents a provider-neutral request for repository status information.
/// </summary>
public sealed class VersionControlStatusRequest
{
    /// <summary>
    /// Gets or sets the repository root path.
    /// </summary>
    public string RepositoryRootPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional repository context path used to discover the repository root.
    /// </summary>
    public string? RepositoryContextPath { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether detailed file changes should be included.
    /// </summary>
    public bool IncludeChanges { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether repository capabilities should be included.
    /// </summary>
    public bool IncludeCapabilities { get; set; } = true;
}
