namespace Workbench.Builder.Core.Models.References;

/// <summary>
/// Describes the origin category of a resolved reference.
/// </summary>
public enum ReferenceKind
{
    /// <summary>
    /// The reference comes from the target framework or shared framework.
    /// </summary>
    Framework,

    /// <summary>
    /// The reference comes from a NuGet package.
    /// </summary>
    Package,

    /// <summary>
    /// The reference comes from another project in the build graph.
    /// </summary>
    Project,

    /// <summary>
    /// The reference comes from a direct metadata file path.
    /// </summary>
    Metadata,

    /// <summary>
    /// The reference is used for analyzers or source generators.
    /// </summary>
    Analyzer,
}
