using Workbench.Builder.Core.Models.Compilation;
using Workbench.Builder.Core.Models.Inspection;

namespace Workbench.Builder.Core.Abstractions;

/// <summary>
/// Evaluates whether an inspected project can be emitted in the current V1.2 slice.
/// </summary>
public interface IProjectEmitSupportEvaluator
{
    /// <summary>
    /// Evaluates the emit support for the specified inspected project.
    /// </summary>
    /// <param name="inspectionResult">The inspected project result.</param>
    /// <returns>The emit decision for the inspected project.</returns>
    ProjectEmitSupport Evaluate(ProjectInspectionResult inspectionResult);
}
