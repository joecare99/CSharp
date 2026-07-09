namespace AA98_AvlnCodeStudio.Base.Building.Models;

/// <summary>
/// Represents a normalized build artifact entry.
/// </summary>
public sealed class BuilderCompilationArtifact
{
    /// <summary>
    /// Gets or sets the artifact kind.
    /// </summary>
    public string Kind { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the artifact path.
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional target framework.
    /// </summary>
    public string? TargetFramework { get; set; }
}
