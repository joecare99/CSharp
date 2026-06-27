namespace AA98_AvlnCodeStudio.Base.Building.Models;

/// <summary>
/// Represents a normalized builder reference entry.
/// </summary>
public sealed class BuilderReferenceDescriptor
{
    /// <summary>
    /// Gets or sets the reference kind.
    /// </summary>
    public BuilderReferenceKind Kind { get; set; } = BuilderReferenceKind.Unknown;

    /// <summary>
    /// Gets or sets the reference display name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional identity or include string.
    /// </summary>
    public string? Identity { get; set; }

    /// <summary>
    /// Gets or sets the optional version.
    /// </summary>
    public string? Version { get; set; }

    /// <summary>
    /// Gets or sets the optional resolved path.
    /// </summary>
    public string? Path { get; set; }
}
