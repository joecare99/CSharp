using System;
using System.Threading;
using System.Threading.Tasks;
using AA98_AvlnCodeStudio.Base.Testing.Models;

namespace AA98_AvlnCodeStudio.Base.Testing.Services;

/// <summary>
/// Executes one discovered test while a caller attaches a debugger to its test host.
/// </summary>
public interface ITestCaseDebugExecutionService
{
    /// <summary>
    /// The callback must attach and configure the debugger before it completes,
    /// then return a lease that detaches the session when disposed. The runner may
    /// request successive process attaches; the previous lease is disposed before
    /// invoking the callback again. Ownership transfers to this service until the
    /// next attach, execution ends, or cancellation.
    /// </summary>
    Task<TestRunSummary> DebugCaseAsync(
        string assemblyPath,
        string projectDirectory,
        DiscoveredTestCase testCase,
        Func<int, CancellationToken, Task<IAsyncDisposable>> attachDebugger,
        CancellationToken cancellationToken = default);
}
