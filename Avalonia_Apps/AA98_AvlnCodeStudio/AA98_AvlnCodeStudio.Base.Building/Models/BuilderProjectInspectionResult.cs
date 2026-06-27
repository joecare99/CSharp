using System.Collections.Generic;

namespace AA98_AvlnCodeStudio.Base.Building.Models;

/// <summary>
/// Represents the structured result of a project inspection through the builder boundary.
/// </summary>
public sealed class BuilderProjectInspectionResult
{
    /// <summary>
    /// Gets or sets the inspected project path.
    /// </summary>
    public string ProjectPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional project display name.
    /// </summary>
    public string? ProjectName { get; set; }

    /// <summary>
    /// Gets or sets the optional target framework.
    /// </summary>
    public string? TargetFramework { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the project is treated as a test project.
    /// </summary>
    public bool IsTestProject { get; set; }

    /// <summary>
    /// Gets the compile items discovered for the project.
    /// </summary>
    public IList<string> CompileItems { get; } = new List<string>();

    /// <summary>
    /// Gets the declared project references.
    /// </summary>
    public IList<BuilderReferenceDescriptor> ProjectReferences { get; } = new List<BuilderReferenceDescriptor>();

    /// <summary>
    /// Gets the declared package references.
    /// </summary>
    public IList<BuilderReferenceDescriptor> PackageReferences { get; } = new List<BuilderReferenceDescriptor>();

    /// <summary>
    /// Gets the resolved references.
    /// </summary>
    public IList<BuilderReferenceDescriptor> ResolvedReferences { get; } = new List<BuilderReferenceDescriptor>();

    /// <summary>
    /// Gets the diagnostics produced during inspection.
    /// </summary>
    public IList<BuilderDiagnostic> Diagnostics { get; } = new List<BuilderDiagnostic>();
}
