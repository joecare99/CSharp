using System;
using Workbench.Builder.Core.Abstractions;
using Workbench.Builder.Core.Models.Compilation;
using Workbench.Builder.Core.Models.Inspection;

namespace Workbench.Builder.Core.Services.Compilation;

/// <summary>
/// Evaluates the first-slice V1.2 emit support rules for inspected projects.
/// </summary>
public sealed class ProjectEmitSupportEvaluator : IProjectEmitSupportEvaluator
{
    /// <inheritdoc/>
    public ProjectEmitSupport Evaluate(ProjectInspectionResult inspectionResult)
    {
        ArgumentNullException.ThrowIfNull(inspectionResult);

        if (!inspectionResult.Project.IsSdkStyle)
        {
            return new ProjectEmitSupport(
                canEmit: false,
                emitKind: ProjectEmitKind.None,
                reason: "Only SDK-style projects are supported for V1.2 emit.");
        }

        if (inspectionResult.IsTestProject)
        {
            return new ProjectEmitSupport(
                canEmit: false,
                emitKind: ProjectEmitKind.None,
                reason: "Test projects are not emitted in the current V1.2 slice.");
        }

        if (string.IsNullOrWhiteSpace(inspectionResult.Project.OutputType)
            || string.Equals(inspectionResult.Project.OutputType, "Library", StringComparison.OrdinalIgnoreCase))
        {
            return new ProjectEmitSupport(
                canEmit: true,
                emitKind: ProjectEmitKind.Library,
                reason: "The inspected project will emit a library assembly.");
        }

        if (string.Equals(inspectionResult.Project.OutputType, "Exe", StringComparison.OrdinalIgnoreCase))
        {
            return new ProjectEmitSupport(
                canEmit: true,
                emitKind: ProjectEmitKind.Executable,
                reason: "The inspected project will emit an executable assembly.");
        }

        return new ProjectEmitSupport(
            canEmit: false,
            emitKind: ProjectEmitKind.None,
            reason: $"The output type '{inspectionResult.Project.OutputType}' is not supported in the current V1.2 slice.");
    }
}
