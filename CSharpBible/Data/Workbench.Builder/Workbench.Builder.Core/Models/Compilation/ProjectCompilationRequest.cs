using Workbench.Builder.Core.Models.Inspection;

namespace Workbench.Builder.Core.Models.Compilation;

/// <summary>
/// Represents the caller-provided input for V1.2 compilation and emit.
/// </summary>
public sealed class ProjectCompilationRequest
{
    /// <summary>
    /// Initializes a new instance of <see cref="ProjectCompilationRequest"/>.
    /// </summary>
    /// <param name="inspectionResult">The inspected project data that acts as the input baseline for compilation.</param>
    /// <param name="outputDirectory">The optional output directory for emitted artifacts.</param>
    /// <param name="emitPortablePdb">A value indicating whether Portable PDB symbols should be emitted when supported.</param>
    public ProjectCompilationRequest(
        ProjectInspectionResult inspectionResult,
        string? outputDirectory = null,
        bool emitPortablePdb = true)
    {
        InspectionResult = inspectionResult;
        OutputDirectory = outputDirectory;
        EmitPortablePdb = emitPortablePdb;
    }

    /// <summary>
    /// Gets the inspected project data that acts as the input baseline for compilation.
    /// </summary>
    public ProjectInspectionResult InspectionResult { get; }

    /// <summary>
    /// Gets the optional output directory for emitted artifacts.
    /// </summary>
    public string? OutputDirectory { get; }

    /// <summary>
    /// Gets a value indicating whether Portable PDB symbols should be emitted when supported.
    /// </summary>
    public bool EmitPortablePdb { get; }
}
