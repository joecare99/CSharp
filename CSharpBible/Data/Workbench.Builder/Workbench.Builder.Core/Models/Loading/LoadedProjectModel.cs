using System.Collections.Generic;
using Workbench.Builder.Core.Models.Diagnostics;
using Workbench.Builder.Core.Models.Projects;

namespace Workbench.Builder.Core.Models.Loading;

/// <summary>
/// Represents the host-neutral result of loading and evaluating a project before reference resolution and final inspection mapping.
/// </summary>
public sealed class LoadedProjectModel
{
    /// <summary>
    /// Initializes a new instance of <see cref="LoadedProjectModel"/>.
    /// </summary>
    /// <param name="request">The original project load request.</param>
    /// <param name="projectFilePath">The full path to the project file.</param>
    /// <param name="projectDirectory">The full path to the project directory.</param>
    /// <param name="properties">The evaluated project properties.</param>
    /// <param name="compileItems">The compile items discovered for the project.</param>
    /// <param name="projectReferences">The project references declared by the project.</param>
    /// <param name="packageReferences">The package references declared by the project.</param>
    /// <param name="isSdkStyle">A value indicating whether the project appears to be SDK-style.</param>
    /// <param name="diagnostics">The diagnostics produced while loading the project.</param>
    public LoadedProjectModel(
        ProjectLoadRequest request,
        string projectFilePath,
        string projectDirectory,
        ProjectPropertySet properties,
        IReadOnlyList<CompileItemInfo> compileItems,
        IReadOnlyList<ProjectReferenceInfo> projectReferences,
        IReadOnlyList<PackageReferenceInfo> packageReferences,
        bool isSdkStyle,
        IReadOnlyList<BuildDiagnostic> diagnostics)
    {
        Request = request;
        ProjectFilePath = projectFilePath;
        ProjectDirectory = projectDirectory;
        Properties = properties;
        CompileItems = compileItems;
        ProjectReferences = projectReferences;
        PackageReferences = packageReferences;
        IsSdkStyle = isSdkStyle;
        Diagnostics = diagnostics;
    }

    /// <summary>
    /// Gets the original project load request.
    /// </summary>
    public ProjectLoadRequest Request { get; }

    /// <summary>
    /// Gets the full path to the project file.
    /// </summary>
    public string ProjectFilePath { get; }

    /// <summary>
    /// Gets the full path to the project directory.
    /// </summary>
    public string ProjectDirectory { get; }

    /// <summary>
    /// Gets the evaluated project properties.
    /// </summary>
    public ProjectPropertySet Properties { get; }

    /// <summary>
    /// Gets the compile items discovered for the project.
    /// </summary>
    public IReadOnlyList<CompileItemInfo> CompileItems { get; }

    /// <summary>
    /// Gets the project references declared by the project.
    /// </summary>
    public IReadOnlyList<ProjectReferenceInfo> ProjectReferences { get; }

    /// <summary>
    /// Gets the package references declared by the project.
    /// </summary>
    public IReadOnlyList<PackageReferenceInfo> PackageReferences { get; }

    /// <summary>
    /// Gets a value indicating whether the project appears to be SDK-style.
    /// </summary>
    public bool IsSdkStyle { get; }

    /// <summary>
    /// Gets the diagnostics produced while loading the project.
    /// </summary>
    public IReadOnlyList<BuildDiagnostic> Diagnostics { get; }
}
