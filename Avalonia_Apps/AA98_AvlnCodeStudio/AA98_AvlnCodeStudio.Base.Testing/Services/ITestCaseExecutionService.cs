using System.Threading;
using System.Threading.Tasks;
using AA98_AvlnCodeStudio.Base.Testing.Models;

namespace AA98_AvlnCodeStudio.Base.Testing.Services;

/// <summary>
/// Executes one previously discovered adapter case after validating it against fresh discovery.
/// </summary>
public interface ITestCaseExecutionService
{
    Task<TestRunSummary> RunCaseAsync(
        string assemblyPath,
        string projectDirectory,
        DiscoveredTestCase testCase,
        CancellationToken cancellationToken = default);
}
