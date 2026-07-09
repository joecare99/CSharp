using Workbench.Builder.Core.Models.Compilation;

namespace Workbench.Builder.Core.Abstractions;

/// <summary>
/// Compiles and emits a project from the stable V1.1 inspection baseline.
/// </summary>
public interface IProjectCompilationService
{
    /// <summary>
    /// Compiles the specified project request.
    /// </summary>
    /// <param name="request">The compilation request.</param>
    /// <returns>The structured compilation result.</returns>
    ProjectCompilationResult Compile(ProjectCompilationRequest request);
}
