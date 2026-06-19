namespace Workbench.Builder.Core.Models.Projects;

/// <summary>
/// Represents a project-to-project dependency declared in the inspected project file.
/// </summary>
public sealed class ProjectReferenceInfo
{
    /// <summary>
    /// Initializes a new instance of <see cref="ProjectReferenceInfo"/>.
    /// </summary>
    /// <param name="include">The original include value from the project reference item.</param>
    /// <param name="projectFilePath">The resolved project file path.</param>
    /// <param name="exists">A value indicating whether the referenced project file exists on disk.</param>
    public ProjectReferenceInfo(string include, string projectFilePath, bool exists)
    {
        Include = include;
        ProjectFilePath = projectFilePath;
        Exists = exists;
    }

    /// <summary>
    /// Gets the original include value from the project reference item.
    /// </summary>
    public string Include { get; }

    /// <summary>
    /// Gets the resolved project file path.
    /// </summary>
    public string ProjectFilePath { get; }

    /// <summary>
    /// Gets a value indicating whether the referenced project file exists on disk.
    /// </summary>
    public bool Exists { get; }
}
