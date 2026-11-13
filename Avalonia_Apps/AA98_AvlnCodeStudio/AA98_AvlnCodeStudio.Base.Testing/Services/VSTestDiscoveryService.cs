using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AA98_AvlnCodeStudio.Base.Testing.Models;
using Microsoft.TestPlatform.VsTestConsole.TranslationLayer;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client.Interfaces;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace AA98_AvlnCodeStudio.Base.Testing.Services;

/// <summary>
/// Discovers tests from a built assembly using the supplied VSTest runner and adapters.
/// </summary>
public sealed class VSTestDiscoveryService
{
    public async Task<TestDiscoveryResult> DiscoverAsync(
        string runnerPath,
        string assemblyPath,
        IReadOnlyList<string> adapterPaths,
        CancellationToken cancellationToken = default)
    {
        var result = await ExecuteAsync(runnerPath, assemblyPath, adapterPaths, null, null, cancellationToken).ConfigureAwait(false);
        return result.Discovery;
    }

    public async Task<TestRunSummary> RunCaseAsync(
        string runnerPath,
        string assemblyPath,
        IReadOnlyList<string> adapterPaths,
        DiscoveredTestCase testCase,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(testCase);
        var result = await ExecuteAsync(runnerPath, assemblyPath, adapterPaths, testCase, null, cancellationToken).ConfigureAwait(false);
        return result.Run!;
    }

    public async Task<TestRunSummary> DebugCaseAsync(
        string runnerPath,
        string assemblyPath,
        IReadOnlyList<string> adapterPaths,
        DiscoveredTestCase testCase,
        Func<int, CancellationToken, Task<IAsyncDisposable>> attachDebugger,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(testCase);
        ArgumentNullException.ThrowIfNull(attachDebugger);
        var result = await ExecuteAsync(
            runnerPath, assemblyPath, adapterPaths, testCase, attachDebugger, cancellationToken).ConfigureAwait(false);
        return result.Run!;
    }

    private static async Task<(TestDiscoveryResult Discovery, TestRunSummary? Run)> ExecuteAsync(
        string runnerPath,
        string assemblyPath,
        IReadOnlyList<string> adapterPaths,
        DiscoveredTestCase? requestedCase,
        Func<int, CancellationToken, Task<IAsyncDisposable>>? attachDebugger,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(runnerPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(assemblyPath);
        ArgumentNullException.ThrowIfNull(adapterPaths);
        cancellationToken.ThrowIfCancellationRequested();
        runnerPath = Path.GetFullPath(runnerPath);
        assemblyPath = Path.GetFullPath(assemblyPath);
        if (!File.Exists(runnerPath))
        {
            throw new FileNotFoundException("The VSTest runner does not exist.", runnerPath);
        }
        if (!File.Exists(assemblyPath))
        {
            throw new FileNotFoundException("The test assembly does not exist.", assemblyPath);
        }
        string[] adapters = adapterPaths.Select(Path.GetFullPath).ToArray();
        foreach (string adapter in adapters)
        {
            if (!File.Exists(adapter))
            {
                throw new FileNotFoundException("The test adapter does not exist.", adapter);
            }
        }

        VsTestConsoleWrapper runner = new(runnerPath);
        DiscoveryHandler handler = new();
        bool started = false;
        int runningCase = 0;
        using DebugTestHostLauncher? launcher = attachDebugger is null ? null : new(attachDebugger, cancellationToken);
        try
        {
            runner.StartSession();
            started = true;
            cancellationToken.ThrowIfCancellationRequested();
            if (adapters.Length > 0)
            {
                await Task.Run(() => runner.InitializeExtensions(adapters)).ConfigureAwait(false);
            }
            cancellationToken.ThrowIfCancellationRequested();
            using CancellationTokenRegistration registration = cancellationToken.Register(() =>
            {
                launcher?.TerminateHosts();
                launcher?.RequestDetach();
                void CancelRunner()
                {
                    try
                    {
                        if (Volatile.Read(ref runningCase) == 0)
                        {
                            runner.CancelDiscovery();
                        }
                        else if (launcher is null)
                        {
                            runner.CancelTestRun();
                        }
                        else
                        {
                            runner.AbortTestRun();
                        }
                    }
                    catch (Exception)
                    {
                        // Cancellation is also checked after the request returns.
                    }
                }
                if (launcher is null)
                {
                    CancelRunner();
                }
                else
                {
                    _ = Task.Run(CancelRunner);
                }
            });
            await Task.Run(() => runner.DiscoverTests([assemblyPath], null!, new TestPlatformOptions(), handler)).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
            TestDiscoveryResult discovery = handler.GetResult();
            if (requestedCase is null)
            {
                return (discovery, null);
            }

            TestCase[] matching = handler.GetNativeCases()
                .Where(test => test.Id.ToString() == requestedCase.Id
                    && string.Equals(test.ExecutorUri?.ToString(), requestedCase.ExecutorUri, StringComparison.Ordinal)
                    && string.Equals(test.Source, requestedCase.Source, StringComparison.Ordinal)
                    && string.Equals(test.FullyQualifiedName, requestedCase.FullyQualifiedName, StringComparison.Ordinal))
                .ToArray();
            if (matching.Length != 1)
            {
                throw new InvalidOperationException("The selected adapter case is missing or ambiguous after rediscovery.");
            }

            RunHandler runHandler = new();
            Volatile.Write(ref runningCase, 1);
            cancellationToken.ThrowIfCancellationRequested();
            if (launcher is null)
            {
                await Task.Run(() => runner.RunTests(matching, null!, runHandler)).ConfigureAwait(false);
            }
            else
            {
                await Task.Run(() => runner.RunTestsWithCustomTestHost(matching, null!, runHandler, launcher)).ConfigureAwait(false);
                launcher.EnsureAttached();
            }
            cancellationToken.ThrowIfCancellationRequested();
            return (discovery, runHandler.GetResult());
        }
        catch (OperationCanceledException)
        {
            launcher?.TerminateHosts();
            throw;
        }
        catch (Exception exception)
        {
            launcher?.TerminateHosts();
            cancellationToken.ThrowIfCancellationRequested();
            throw new InvalidOperationException($"VSTest operation failed: {exception.Message}", exception);
        }
        finally
        {
            try
            {
                if (launcher is not null)
                {
                    await launcher.ReleaseDebuggerAsync().ConfigureAwait(false);
                }
            }
            finally
            {
                if (started)
                {
                    runner.EndSession();
                }
            }
        }
    }

    private sealed class DebugTestHostLauncher(
        Func<int, CancellationToken, Task<IAsyncDisposable>> attachDebugger,
        CancellationToken cancellationToken) : ITestHostLauncher2, IDisposable
    {
        private readonly object _gate = new();
        private readonly SemaphoreSlim _attachGate = new(1, 1);
        private readonly List<Process> _hosts = [];
        private Exception? _attachFailure;
        private bool _attached;
        private IAsyncDisposable? _debuggerLease;
        private Task? _releaseTask;

        public bool IsDebug => true;

        public int LaunchTestHost(TestProcessStartInfo startInfo) => LaunchTestHost(startInfo, CancellationToken.None);

        public int LaunchTestHost(TestProcessStartInfo startInfo, CancellationToken token)
        {
            throw new NotSupportedException("Test debugging requires a VSTest runner that launches its host and requests debugger attach.");
        }

        public bool AttachDebuggerToProcess(int pid) => AttachDebuggerToProcess(pid, CancellationToken.None);

        public bool AttachDebuggerToProcess(int pid, CancellationToken token)
        {
            try
            {
                lock (_gate)
                {
                    if (pid <= 0 || _hosts.Any(host => host.Id == pid))
                    {
                        throw new InvalidOperationException("VSTest requested an invalid or repeated test host attach.");
                    }
                    // Modern VSTest starts its own hosts and identifies them in this callback.
                    _hosts.Add(Process.GetProcessById(pid));
                }
                using CancellationTokenSource linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, token);
                _attachGate.Wait(linked.Token);
                try
                {
                    ReleaseDebuggerAsync().GetAwaiter().GetResult();
                    lock (_gate)
                    {
                        _debuggerLease = null;
                        _releaseTask = null;
                    }
                    linked.Token.ThrowIfCancellationRequested();
                    IAsyncDisposable lease = attachDebugger(pid, linked.Token).GetAwaiter().GetResult()
                        ?? throw new InvalidOperationException("The debugger attach callback did not return a session lease.");
                    lock (_gate)
                    {
                        _debuggerLease = lease;
                        _attached = true;
                    }
                    if (linked.IsCancellationRequested)
                    {
                        RequestDetach();
                        linked.Token.ThrowIfCancellationRequested();
                    }
                    return true;
                }
                finally
                {
                    _attachGate.Release();
                }
            }
            catch (Exception exception)
            {
                TerminateHosts();
                lock (_gate)
                {
                    _attachFailure = exception;
                }
                return false;
            }
        }

        public void RequestDetach() => _ = ReleaseDebuggerAsync();

        public Task ReleaseDebuggerAsync()
        {
            lock (_gate)
            {
                if (_debuggerLease is null)
                {
                    return Task.CompletedTask;
                }
                return _releaseTask ??= Task.Run(async () =>
                    await _debuggerLease.DisposeAsync().ConfigureAwait(false));
            }
        }

        public void EnsureAttached()
        {
            lock (_gate)
            {
                if (!_attached || _attachFailure is not null)
                {
                    throw new InvalidOperationException("The debugger was not attached to the VSTest host.", _attachFailure);
                }
            }
        }

        public void TerminateHosts(bool entireProcessTree = true)
        {
            lock (_gate)
            {
                foreach (Process host in _hosts)
                {
                    try
                    {
                        if (!host.HasExited)
                        {
                            host.Kill(entireProcessTree);
                        }
                    }
                    catch (InvalidOperationException) { }
                    catch (System.ComponentModel.Win32Exception) { }
                }
            }
        }

        public void Dispose()
        {
            TerminateHosts(entireProcessTree: false);
            lock (_gate)
            {
                foreach (Process host in _hosts)
                {
                    host.Dispose();
                }
                _hosts.Clear();
            }
        }
    }

    private sealed class DiscoveryHandler : ITestDiscoveryEventsHandler2
    {
        private readonly object _gate = new();
        private readonly List<TestCase> _cases = [];
        private readonly List<string> _warnings = [];
        private readonly List<string> _errors = [];
        private bool _completed;
        private bool _aborted;

        public void HandleDiscoveredTests(IEnumerable<TestCase>? discoveredTestCases)
        {
            if (discoveredTestCases is null)
            {
                return;
            }
            lock (_gate)
            {
                _cases.AddRange(discoveredTestCases);
            }
        }

        public void HandleDiscoveryComplete(DiscoveryCompleteEventArgs discoveryCompleteEventArgs, IEnumerable<TestCase>? lastChunk)
        {
            if (lastChunk is not null)
            {
                HandleDiscoveredTests(lastChunk);
            }
            lock (_gate)
            {
                _completed = true;
                _aborted = discoveryCompleteEventArgs.IsAborted;
            }
        }

        public void HandleLogMessage(TestMessageLevel level, string? message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }
            lock (_gate)
            {
                if (level == TestMessageLevel.Error)
                {
                    _errors.Add(message);
                }
                else if (level == TestMessageLevel.Warning)
                {
                    _warnings.Add(message);
                }
            }
        }

        public void HandleRawMessage(string rawMessage) { }

        public TestDiscoveryResult GetResult()
        {
            lock (_gate)
            {
                if (!_completed || _aborted || _errors.Count > 0)
                {
                    throw new InvalidOperationException("VSTest discovery did not complete successfully. "
                        + string.Join(Environment.NewLine, _errors));
                }
                return new TestDiscoveryResult(_cases.Select(test => new DiscoveredTestCase(
                    test.Id.ToString(),
                    test.ExecutorUri?.ToString() ?? string.Empty,
                    test.Source,
                    test.FullyQualifiedName,
                    test.DisplayName,
                    test.CodeFilePath,
                    test.LineNumber > 0 ? test.LineNumber : null)).ToArray(), _warnings.ToArray());
            }
        }

        public TestCase[] GetNativeCases()
        {
            lock (_gate)
            {
                return _cases.ToArray();
            }
        }
    }

    private sealed class RunHandler : ITestRunEventsHandler
    {
        private readonly object _gate = new();
        private readonly List<TestResult> _results = [];
        private readonly List<string> _errors = [];
        private TestRunCompleteEventArgs? _completed;

        public void HandleTestRunStatsChange(TestRunChangedEventArgs? args)
        {
            if (args?.NewTestResults is not null)
            {
                lock (_gate)
                {
                    _results.AddRange(args.NewTestResults);
                }
            }
        }

        public void HandleTestRunComplete(
            TestRunCompleteEventArgs args,
            TestRunChangedEventArgs? lastChunk,
            ICollection<AttachmentSet>? runContextAttachments,
            ICollection<string>? executorUris)
        {
            HandleTestRunStatsChange(lastChunk);
            lock (_gate)
            {
                _completed = args;
            }
        }

        public void HandleLogMessage(TestMessageLevel level, string? message)
        {
            if (level == TestMessageLevel.Error && !string.IsNullOrWhiteSpace(message))
            {
                lock (_gate)
                {
                    _errors.Add(message);
                }
            }
        }

        public void HandleRawMessage(string rawMessage) { }

        public int LaunchProcessWithDebuggerAttached(TestProcessStartInfo testProcessStartInfo)
            => throw new NotSupportedException("Per-case debugging is not supported.");

        public TestRunSummary GetResult()
        {
            lock (_gate)
            {
                if (_completed is null || _completed.IsAborted || _completed.IsCanceled
                    || _completed.Error is not null)
                {
                    throw new InvalidOperationException("VSTest case execution did not complete successfully. "
                        + _completed?.Error?.Message + " " + string.Join(Environment.NewLine, _errors));
                }
                if (_results.Count != 1)
                {
                    throw new InvalidOperationException($"Expected one adapter case result but received {_results.Count}. "
                        + string.Join(Environment.NewLine, _errors));
                }

                TestResult result = _results[0];
                TestRunSummary summary = new() { TotalCount = 1 };
                foreach (string error in _errors)
                {
                    summary.Messages.Add(error);
                }
                summary.Cases.Add(new TestCaseResult(
                    result.DisplayName ?? result.TestCase.DisplayName,
                    result.Outcome.ToString(),
                    result.Duration,
                    null,
                    result.TestCase.FullyQualifiedName,
                    result.TestCase.Id.ToString(),
                    null));
                switch (result.Outcome)
                {
                    case TestOutcome.Passed:
                        summary.PassedCount = 1;
                        summary.Outcome = TestRunOutcome.Passed;
                        break;
                    case TestOutcome.Failed:
                        summary.FailedCount = 1;
                        summary.Outcome = TestRunOutcome.Failed;
                        summary.Failures.Add(new TestRunFailure
                        {
                            TestName = result.DisplayName ?? result.TestCase.DisplayName,
                            Message = result.ErrorMessage ?? string.Empty,
                            StackTrace = result.ErrorStackTrace,
                            SourcePath = result.TestCase.CodeFilePath,
                            LineNumber = result.TestCase.LineNumber > 0 ? result.TestCase.LineNumber : null,
                        });
                        break;
                    case TestOutcome.Skipped:
                        summary.SkippedCount = 1;
                        summary.Outcome = TestRunOutcome.Partial;
                        break;
                    default:
                        throw new InvalidOperationException($"VSTest returned an unsupported case outcome: {result.Outcome}.");
                }
                return summary;
            }
        }
    }
}
