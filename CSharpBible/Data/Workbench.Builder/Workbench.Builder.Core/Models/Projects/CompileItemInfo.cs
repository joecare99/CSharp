namespace Workbench.Builder.Core.Models.Projects;

/// <summary>
/// Represents a single source file that participates in compilation.
/// </summary>
public sealed class CompileItemInfo
{
    /// <summary>
    /// Initializes a new instance of <see cref="CompileItemInfo"/>.
    /// </summary>
    /// <param name="include">The original include value from the project.</param>
    /// <param name="filePath">The resolved file path for the compile item.</param>
    /// <param name="exists">A value indicating whether the compile item exists on disk.</param>
    public CompileItemInfo(string include, string filePath, bool exists)
    {
        Include = include;
        FilePath = filePath;
        Exists = exists;
    }

    /// <summary>
    /// Gets the original include value from the project.
    /// </summary>
    public string Include { get; }

    /// <summary>
    /// Gets the resolved file path for the compile item.
    /// </summary>
    public string FilePath { get; }

    /// <summary>
    /// Gets a value indicating whether the compile item exists on disk.
    /// </summary>
    public bool Exists { get; }
}
