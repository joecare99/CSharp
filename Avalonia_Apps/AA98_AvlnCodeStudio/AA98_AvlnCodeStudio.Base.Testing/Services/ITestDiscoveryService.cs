using System.Threading;
using System.Threading.Tasks;
using AA98_AvlnCodeStudio.Base.Testing.Models;

namespace AA98_AvlnCodeStudio.Base.Testing.Services;

/// <summary>
/// Discovers adapter test cases from a built assembly.
/// </summary>
public interface ITestDiscoveryService
{
    Task<TestDiscoveryResult> DiscoverAsync(
        string assemblyPath,
        string projectDirectory,
        CancellationToken cancellationToken = default);
}
