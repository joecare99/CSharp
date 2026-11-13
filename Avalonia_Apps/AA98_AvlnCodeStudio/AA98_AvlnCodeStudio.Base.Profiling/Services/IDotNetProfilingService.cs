using AA98_AvlnCodeStudio.Base.Profiling.Models;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Base.Profiling.Services;

/// <summary>
/// Starts opt-in diagnostics tools against an explicitly supplied process ID.
/// </summary>
public interface IDotNetProfilingService
{
    /// <summary>
    /// Starts a diagnostics tool. Cancellation after startup stops the tool process.
    /// </summary>
    Task<IDotNetProfilingSession> StartAsync(
        DotNetProfilingRequest request,
        CancellationToken cancellationToken = default);
}
