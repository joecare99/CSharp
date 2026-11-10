using System.Collections.Generic;

namespace AA98_AvlnCodeStudio.Base.Building.Models;

/// <summary>
/// Represents the structured result of a build execution through the builder boundary.
/// </summary>
public sealed class BuilderBuildResult
{
    /// <summary>
    /// Gets or sets the built project path.
    /// </summary>
    public string ProjectPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the build succeeded.
    /// </summary>
    public bool Succeeded { get; set; }

    /// <summary>
    /// Gets the compilation artifacts produced by the build.
    /// These artifacts do not guarantee that runtime dependency metadata is current or that an executable can be launched.
    /// </summary>
    public IList<BuilderCompilationArtifact> Artifacts { get; } = new List<BuilderCompilationArtifact>();

    /// <summary>
    /// Gets the diagnostics produced during the build.
    /// </summary>
    public IList<BuilderDiagnostic> Diagnostics { get; } = new List<BuilderDiagnostic>();
}
