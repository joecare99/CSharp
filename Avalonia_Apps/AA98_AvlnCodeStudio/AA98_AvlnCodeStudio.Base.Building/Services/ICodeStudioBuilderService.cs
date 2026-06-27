using System.Threading;
using System.Threading.Tasks;
using AA98_AvlnCodeStudio.Base.Building.Models;

namespace AA98_AvlnCodeStudio.Base.Building.Services;

/// <summary>
/// Defines provider-neutral builder operations for studio components.
/// </summary>
public interface ICodeStudioBuilderService
{
    /// <summary>
    /// Inspects the requested project and returns a structured project view.
    /// </summary>
    /// <param name="request">The inspection request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The structured inspection result.</returns>
    Task<BuilderProjectInspectionResult> InspectProjectAsync(BuilderProjectInspectionRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Builds the requested project and returns a structured build result.
    /// </summary>
    /// <param name="request">The build request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The structured build result.</returns>
    Task<BuilderBuildResult> BuildProjectAsync(BuilderBuildRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes targeted tests for the requested project and returns a structured test result.
    /// </summary>
    /// <param name="request">The targeted test request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The structured targeted test result.</returns>
    Task<BuilderTargetedTestResult> RunTargetedTestsAsync(BuilderTargetedTestRequest request, CancellationToken cancellationToken = default);
}
