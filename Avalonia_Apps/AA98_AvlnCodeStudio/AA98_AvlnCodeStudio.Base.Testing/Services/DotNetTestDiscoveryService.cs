using System;
using System.IO;
using System.Linq;
using System.Text.Json;
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
        TestDiscoveryResult discovery = await new VSTestDiscoveryService().DiscoverAsync(runner, assembly, adapters, cancellationToken)
            .ConfigureAwait(false);
        return await AddPlatformWarningAsync(discovery, assembly, cancellationToken).ConfigureAwait(false);
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

        string[] adapters = await VSTestAdapterResolver.ResolveAsync(
            assemblyPath, projectDirectory, cancellationToken).ConfigureAwait(false);
        if (adapters.Length == 0)
        {
            if (await ReferencesTestingPlatformAsync(assemblyPath, cancellationToken).ConfigureAwait(false))
            {
                throw new InvalidOperationException(
                    "No VSTest adapter was found, and the assembly references Microsoft.Testing.Platform. "
                    + "Microsoft.Testing.Platform-only projects are not supported by this VSTest service. "
                    + "Run the project with its Microsoft.Testing.Platform runner, or configure a VSTest adapter "
                    + "for a VSTest-compatible project.");
            }

            throw new InvalidOperationException("No VSTest adapter was found beside the built test assembly.");
        }

        string runner = await new DotNetVSTestRunnerLocator().FindRunnerAsync(projectDirectory, cancellationToken)
            .ConfigureAwait(false);
        return (runner, assemblyPath, adapters);
    }

    internal static async Task<TestDiscoveryResult> AddPlatformWarningAsync(
        TestDiscoveryResult discovery,
        string assemblyPath,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (discovery.Cases.Count != 0
            || !await ReferencesTestingPlatformAsync(assemblyPath, cancellationToken).ConfigureAwait(false))
        {
            return discovery;
        }

        return discovery with
        {
            Warnings = discovery.Warnings.Append(
                "VSTest discovered no tests, and the assembly references Microsoft.Testing.Platform. "
                + "Microsoft.Testing.Platform-only projects are not supported by this VSTest service. "
                + "If this project uses Microsoft.Testing.Platform, run it with its own runner; "
                + "otherwise check that the correct VSTest adapter is configured.").ToArray(),
        };
    }

    private static async Task<bool> ReferencesTestingPlatformAsync(string assemblyPath, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string depsPath = Path.ChangeExtension(assemblyPath, ".deps.json");
        try
        {
            await using FileStream stream = new(depsPath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
            if (stream.Length > 16 * 1024 * 1024)
            {
                cancellationToken.ThrowIfCancellationRequested();
                return false;
            }

            using JsonDocument document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            JsonElement root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object
                || !root.TryGetProperty("runtimeTarget", out JsonElement runtimeTarget)
                || runtimeTarget.ValueKind != JsonValueKind.Object
                || !runtimeTarget.TryGetProperty("name", out JsonElement name)
                || name.ValueKind != JsonValueKind.String
                || !root.TryGetProperty("targets", out JsonElement targets)
                || targets.ValueKind != JsonValueKind.Object
                || !targets.TryGetProperty(name.GetString()!, out JsonElement target)
                || target.ValueKind != JsonValueKind.Object)
            {
                return false;
            }

            foreach (JsonProperty library in target.EnumerateObject())
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (!library.Name.StartsWith("Microsoft.Testing.Platform/", StringComparison.OrdinalIgnoreCase)
                    || library.Value.ValueKind != JsonValueKind.Object
                    || !library.Value.TryGetProperty("runtime", out JsonElement runtime)
                    || runtime.ValueKind != JsonValueKind.Object)
                {
                    continue;
                }

                if (runtime.EnumerateObject().Any(static asset =>
                    asset.Name.EndsWith("/Microsoft.Testing.Platform.dll", StringComparison.OrdinalIgnoreCase)))
                {
                    return true;
                }
            }
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or JsonException)
        {
            // Missing or unreadable dependency metadata does not establish the test platform.
        }

        cancellationToken.ThrowIfCancellationRequested();
        return false;
    }
}
