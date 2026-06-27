using System.Threading;
using System.Threading.Tasks;
using AA98_AvlnCodeStudio.Base.Building.Models;

namespace AA98_AvlnCodeStudio.Base.Building.Services;

/// <summary>
/// Provides a provider-neutral fallback builder service without backend integration.
/// </summary>
public sealed class NullCodeStudioBuilderService : ICodeStudioBuilderService
{
    /// <inheritdoc/>
    public Task<BuilderProjectInspectionResult> InspectProjectAsync(BuilderProjectInspectionRequest request, CancellationToken cancellationToken = default)
    {
        var result = new BuilderProjectInspectionResult
        {
            ProjectPath = request.ProjectPath,
            TargetFramework = request.TargetFramework,
            IsTestProject = false,
        };

        return Task.FromResult(result);
    }

    /// <inheritdoc/>
    public Task<BuilderBuildResult> BuildProjectAsync(BuilderBuildRequest request, CancellationToken cancellationToken = default)
    {
        var result = new BuilderBuildResult
        {
            ProjectPath = request.ProjectPath,
            Succeeded = false,
        };

        return Task.FromResult(result);
    }

    /// <inheritdoc/>
    public Task<BuilderTargetedTestResult> RunTargetedTestsAsync(BuilderTargetedTestRequest request, CancellationToken cancellationToken = default)
    {
        var result = new BuilderTargetedTestResult
        {
            ProjectPath = request.ProjectPath,
            Outcome = BuilderTargetedTestOutcome.Unknown,
            TotalCount = 0,
            PassedCount = 0,
            FailedCount = 0,
            SkippedCount = 0,
        };

        return Task.FromResult(result);
    }
}
