using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AA98_AvlnCodeStudio.Base.Testing.Models;

namespace AA98_AvlnCodeStudio.Base.Testing.Services;

/// <summary>
/// Discovers cases using the selected SDK and adapters copied beside a built assembly.
/// </summary>
public sealed class DotNetTestDiscoveryService : ITestDiscoveryService, ITestCaseExecutionService, ITestCaseDebugExecutionService
{
    public async Task<TestDiscoveryResult> DiscoverAsync(
        string assemblyPath,
        string projectDirectory,
        CancellationToken cancellationToken = default)
    {
        (string runner, string assembly, string[] adapters) =
            await ResolveAsync(assemblyPath, projectDirectory, cancellationToken).ConfigureAwait(false);
        return await new VSTestDiscoveryService().DiscoverAsync(runner, assembly, adapters, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<TestRunSummary> RunCaseAsync(
        string assemblyPath,
        string projectDirectory,
        DiscoveredTestCase testCase,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(testCase);
        (string runner, string assembly, string[] adapters) =
            await ResolveAsync(assemblyPath, projectDirectory, cancellationToken).ConfigureAwait(false);
        return await new VSTestDiscoveryService().RunCaseAsync(runner, assembly, adapters, testCase, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<TestRunSummary> DebugCaseAsync(
        string assemblyPath,
        string projectDirectory,
        DiscoveredTestCase testCase,
        Func<int, CancellationToken, Task<IAsyncDisposable>> attachDebugger,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(testCase);
        ArgumentNullException.ThrowIfNull(attachDebugger);
        (string runner, string assembly, string[] adapters) =
            await ResolveAsync(assemblyPath, projectDirectory, cancellationToken).ConfigureAwait(false);
        return await new VSTestDiscoveryService().DebugCaseAsync(
            runner, assembly, adapters, testCase, attachDebugger, cancellationToken).ConfigureAwait(false);
    }

    private static async Task<(string Runner, string Assembly, string[] Adapters)> ResolveAsync(
        string assemblyPath,
        string projectDirectory,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(assemblyPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(projectDirectory);
        cancellationToken.ThrowIfCancellationRequested();
        assemblyPath = Path.GetFullPath(assemblyPath);
        if (!File.Exists(assemblyPath))
        {
            throw new FileNotFoundException("The built test assembly does not exist.", assemblyPath);
        }

        string outputDirectory = Path.GetDirectoryName(assemblyPath)!;
        string[] adapters = Directory.GetFiles(outputDirectory, "*.dll")
            .Where(static path => path.EndsWith("TestAdapter.dll", StringComparison.OrdinalIgnoreCase))
            .ToArray();
        if (adapters.Length == 0)
        {
            throw new InvalidOperationException("No VSTest adapter was found beside the built test assembly.");
        }

        string runner = await new DotNetVSTestRunnerLocator().FindRunnerAsync(projectDirectory, cancellationToken)
            .ConfigureAwait(false);
        return (runner, assemblyPath, adapters);
    }
}
