using System.Collections.Generic;
using Workbench.Builder.Core.Models.Diagnostics;
using Workbench.Builder.Core.Models.Projects;
using Workbench.Builder.Core.Models.References;

namespace Workbench.Builder.Core.Models.Inspection;

/// <summary>
/// Represents the complete structured result of a V1.1 project inspection.
/// </summary>
public sealed class ProjectInspectionResult
{
    /// <summary>
    /// Initializes a new instance of <see cref="ProjectInspectionResult"/>.
    /// </summary>
    /// <param name="project">The evaluated high-level project information.</param>
    /// <param name="compileItems">The compile items discovered for the project.</param>
    /// <param name="projectReferences">The project references declared by the project.</param>
    /// <param name="packageReferences">The package references declared by the project.</param>
    /// <param name="resolvedReferences">The framework, metadata, package, or project references resolved for the project.</param>
    /// <param name="diagnostics">The diagnostics produced during inspection.</param>
    /// <param name="isTestProject">A value indicating whether the project should be treated as a test project.</param>
    public ProjectInspectionResult(
        BuildProjectInfo project,
        IReadOnlyList<CompileItemInfo> compileItems,
        IReadOnlyList<ProjectReferenceInfo> projectReferences,
        IReadOnlyList<PackageReferenceInfo> packageReferences,
        IReadOnlyList<ResolvedReferenceInfo> resolvedReferences,
        IReadOnlyList<BuildDiagnostic> diagnostics,
        bool isTestProject)
    {
        Project = project;
        CompileItems = compileItems;
        ProjectReferences = projectReferences;
        PackageReferences = packageReferences;
        ResolvedReferences = resolvedReferences;
        Diagnostics = diagnostics;
        IsTestProject = isTestProject;
    }

    /// <summary>
    /// Gets the evaluated high-level project information.
    /// </summary>
    public BuildProjectInfo Project { get; }

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
    /// Gets the framework, metadata, package, or project references resolved for the project.
    /// </summary>
    public IReadOnlyList<ResolvedReferenceInfo> ResolvedReferences { get; }

    /// <summary>
    /// Gets the diagnostics produced during inspection.
    /// </summary>
    public IReadOnlyList<BuildDiagnostic> Diagnostics { get; }

    /// <summary>
    /// Gets a value indicating whether the project should be treated as a test project.
    /// </summary>
    public bool IsTestProject { get; }
}
