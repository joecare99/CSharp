using System.Threading;
using System.Threading.Tasks;
using AA98_AvlnCodeStudio.Base.Testing.Models;

namespace AA98_AvlnCodeStudio.Base.Testing.Services;

/// <summary>
/// Provides a provider-neutral fallback test execution service without runner integration.
/// </summary>
public sealed class NullTestExecutionService : ITestExecutionService
{
    /// <inheritdoc/>
    public Task<TestRunSummary> RunTestsAsync(TestRunRequest request, CancellationToken cancellationToken = default)
    {
        var summary = new TestRunSummary
        {
            Outcome = TestRunOutcome.Unknown,
            TotalCount = 0,
            PassedCount = 0,
            FailedCount = 0,
            SkippedCount = 0,
        };

        return Task.FromResult(summary);
    }
}
