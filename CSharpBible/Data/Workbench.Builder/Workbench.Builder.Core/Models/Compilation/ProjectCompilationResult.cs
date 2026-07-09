using System.Collections.Generic;
using Workbench.Builder.Core.Models.Diagnostics;
using Workbench.Builder.Core.Models.Inspection;

namespace Workbench.Builder.Core.Models.Compilation;

/// <summary>
/// Represents the structured outcome of a V1.2 compilation and emit attempt.
/// </summary>
public sealed class ProjectCompilationResult
{
    /// <summary>
    /// Initializes a new instance of <see cref="ProjectCompilationResult"/>.
    /// </summary>
    /// <param name="inspectionResult">The inspected project data used as compilation input.</param>
    /// <param name="emitSupport">The emit decision derived from the inspected project shape.</param>
    /// <param name="artifacts">The artifacts produced by compilation and emit.</param>
    /// <param name="diagnostics">The diagnostics produced during compilation and emit.</param>
    /// <param name="succeeded">A value indicating whether compilation and emit succeeded.</param>
    public ProjectCompilationResult(
        ProjectInspectionResult inspectionResult,
        ProjectEmitSupport emitSupport,
        IReadOnlyList<CompilationArtifactInfo> artifacts,
        IReadOnlyList<BuildDiagnostic> diagnostics,
        bool succeeded)
    {
        InspectionResult = inspectionResult;
        EmitSupport = emitSupport;
        Artifacts = artifacts;
        Diagnostics = diagnostics;
        Succeeded = succeeded;
    }

    /// <summary>
    /// Gets the inspected project data used as compilation input.
    /// </summary>
    public ProjectInspectionResult InspectionResult { get; }

    /// <summary>
    /// Gets the emit decision derived from the inspected project shape.
    /// </summary>
    public ProjectEmitSupport EmitSupport { get; }

    /// <summary>
    /// Gets the artifacts produced by compilation and emit.
    /// </summary>
    public IReadOnlyList<CompilationArtifactInfo> Artifacts { get; }

    /// <summary>
    /// Gets the diagnostics produced during compilation and emit.
    /// </summary>
    public IReadOnlyList<BuildDiagnostic> Diagnostics { get; }

    /// <summary>
    /// Gets a value indicating whether compilation and emit succeeded.
    /// </summary>
    public bool Succeeded { get; }
}
