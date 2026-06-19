namespace Workbench.Builder.Core.Models.Projects;

/// <summary>
/// Represents a package dependency declared in the inspected project file.
/// </summary>
public sealed class PackageReferenceInfo
{
    /// <summary>
    /// Initializes a new instance of <see cref="PackageReferenceInfo"/>.
    /// </summary>
    /// <param name="packageId">The package identifier.</param>
    /// <param name="version">The declared or evaluated package version.</param>
    /// <param name="privateAssets">The optional private assets expression.</param>
    public PackageReferenceInfo(string packageId, string? version, string? privateAssets)
    {
        PackageId = packageId;
        Version = version;
        PrivateAssets = privateAssets;
    }

    /// <summary>
    /// Gets the package identifier.
    /// </summary>
    public string PackageId { get; }

    /// <summary>
    /// Gets the declared or evaluated package version.
    /// </summary>
    public string? Version { get; }

    /// <summary>
    /// Gets the optional private assets expression.
    /// </summary>
    public string? PrivateAssets { get; }
}
