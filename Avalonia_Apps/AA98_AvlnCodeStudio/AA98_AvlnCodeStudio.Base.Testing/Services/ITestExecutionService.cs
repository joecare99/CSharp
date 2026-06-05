using System.Threading;
using System.Threading.Tasks;
using AA98_AvlnCodeStudio.Base.Testing.Models;

namespace AA98_AvlnCodeStudio.Base.Testing.Services;

/// <summary>
/// Defines provider-neutral test execution operations for studio components.
/// </summary>
public interface ITestExecutionService
{
    /// <summary>
    /// Executes a test run for the requested targets.
    /// </summary>
    /// <param name="request">The test run request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The summarized test run result.</returns>
    Task<TestRunSummary> RunTestsAsync(TestRunRequest request, CancellationToken cancellationToken = default);
}
