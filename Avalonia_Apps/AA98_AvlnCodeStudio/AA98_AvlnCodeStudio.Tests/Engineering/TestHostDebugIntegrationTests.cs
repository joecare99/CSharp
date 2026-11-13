using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AA98_AvlnCodeStudio.Base.Debugging.Models;
using AA98_AvlnCodeStudio.Base.Debugging.Services;
using AA98_AvlnCodeStudio.Base.Testing.Models;
using AA98_AvlnCodeStudio.Base.Testing.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class TestHostDebugIntegrationTests
{
    [TestMethod]
    public void DebuggerFixtureMethod()
    {
        int debugMarker = Environment.ProcessId;
        string? marker = Environment.GetEnvironmentVariable("AA98_TESTHOST_DEBUG_MARKER");
        if (!string.IsNullOrEmpty(marker))
        {
            string[] parts = marker.Split('|', 2);
            if (parts.Length == 2 && int.TryParse(parts[0], out int parentId) && parentId != debugMarker)
            {
                File.WriteAllText(parts[1], "executed");
            }
        }
        Assert.IsTrue(debugMarker > 0);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task DebugCase_AttachFailureDoesNotRunTest(bool cancelDuringAttach)
    {
        string source = SourcePath();
        string projectDirectory = Path.GetDirectoryName(Path.GetDirectoryName(source)!)!;
        string assembly = typeof(TestHostDebugIntegrationTests).Assembly.Location;
        DotNetTestDiscoveryService tests = new();
        TestDiscoveryResult discovery = await tests.DiscoverAsync(assembly, projectDirectory);
        DiscoveredTestCase selected = discovery.Cases.Single(item =>
            item.FullyQualifiedName.EndsWith(".TestHostDebugIntegrationTests.DebuggerFixtureMethod", StringComparison.Ordinal));
        string marker = Path.Combine(Path.GetTempPath(), $"AA98_DebugAttachFailure_{Guid.NewGuid():N}.txt");
        string? previous = Environment.GetEnvironmentVariable("AA98_TESTHOST_DEBUG_MARKER");
        try
        {
            Environment.SetEnvironmentVariable("AA98_TESTHOST_DEBUG_MARKER", $"{Environment.ProcessId}|{marker}");
            if (cancelDuringAttach)
            {
                using CancellationTokenSource cancellation = new(TimeSpan.FromSeconds(30));
                TaskCompletionSource entered = new(TaskCreationOptions.RunContinuationsAsynchronously);
                Task<TestRunSummary> run = tests.DebugCaseAsync(assembly, projectDirectory, selected, async (_, token) =>
                {
                    entered.TrySetResult();
                    await Task.Delay(Timeout.Infinite, token);
                    throw new InvalidOperationException("Attach should have been canceled.");
                }, cancellation.Token);
                await entered.Task.WaitAsync(TimeSpan.FromSeconds(20));
                await cancellation.CancelAsync();
                await Assert.ThrowsAsync<OperationCanceledException>(() => run.WaitAsync(TimeSpan.FromSeconds(20)));
            }
            else
            {
                InvalidOperationException error = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() =>
                    tests.DebugCaseAsync(assembly, projectDirectory, selected,
                        (_, _) => Task.FromException<IAsyncDisposable>(new InvalidOperationException("Attach failed deliberately."))));
                StringAssert.Contains(error.ToString(), "Attach failed deliberately.");
            }
            Assert.IsFalse(File.Exists(marker), "The test ran after debugger attach failed.");
        }
        finally
        {
            Environment.SetEnvironmentVariable("AA98_TESTHOST_DEBUG_MARKER", previous);
            if (File.Exists(marker))
            {
                File.Delete(marker);
            }
        }
    }

    [TestMethod]
    [TestCategory("Integration")]
    [DataRow(false)]
    [DataRow(true)]
    public async Task DebugCase_AttachesBeforeTheSelectedTestAndStopsAtItsBreakpoint(bool cancelWhilePaused)
    {
        string? adapter = Environment.GetEnvironmentVariable("AVLN_NETCOREDBG_PATH");
        if (!OperatingSystem.IsWindows() || string.IsNullOrWhiteSpace(adapter) || !File.Exists(adapter))
        {
            Assert.Inconclusive("Set AVLN_NETCOREDBG_PATH on Windows for this test.");
        }

        string source = SourcePath();
        string projectDirectory = Path.GetDirectoryName(Path.GetDirectoryName(source)!)!;
        string assembly = typeof(TestHostDebugIntegrationTests).Assembly.Location;
        DotNetTestDiscoveryService tests = new();
        TestDiscoveryResult discovery = await tests.DiscoverAsync(assembly, projectDirectory);
        DiscoveredTestCase selected = discovery.Cases.Single(item =>
            item.FullyQualifiedName.EndsWith(".TestHostDebugIntegrationTests.DebuggerFixtureMethod", StringComparison.Ordinal));
        int line = Array.FindIndex(File.ReadAllLines(source), text => text.Contains("int debugMarker =", StringComparison.Ordinal)) + 1;
        Assert.IsTrue(line > 0);
        NetCoreDbgDebugSessionService debugger = new(adapter);
        TaskCompletionSource<DebugSessionEventArgs> stopped = new(TaskCreationOptions.RunContinuationsAsynchronously);
        debugger.SessionEventReceived += (_, args) =>
        {
            if (args.EventName == "stopped" && args.Reason == "breakpoint")
            {
                stopped.TrySetResult(args);
            }
        };
        DebugSessionInfo? session = null;
        using CancellationTokenSource cancellation = new(TimeSpan.FromSeconds(60));
        Task<TestRunSummary> run = tests.DebugCaseAsync(assembly, projectDirectory, selected, async (pid, token) =>
        {
            session = await debugger.StartSessionAsync(new DebugLaunchRequest
            {
                AttachToExistingProcess = true,
                ProcessId = pid,
                WorkingDirectory = projectDirectory,
            }, token);
            try
            {
                await debugger.SetBreakpointsAsync(session.SessionId, source, [line], token);
                return new DebugSessionLease(debugger, session.SessionId);
            }
            catch
            {
                await debugger.StopSessionAsync(session.SessionId);
                session = null;
                throw;
            }
        }, cancellation.Token);
        try
        {
            Task first = await Task.WhenAny(stopped.Task, run).WaitAsync(cancellation.Token);
            if (ReferenceEquals(first, run))
            {
                await run;
                Assert.Fail("The test completed without stopping at its breakpoint.");
            }
            await stopped.Task;
            Assert.IsNotNull(session);
            Assert.IsTrue((await debugger.GetStackTraceAsync(session.SessionId, cancellation.Token)).Count > 0);
            if (cancelWhilePaused)
            {
                await Task.Run(cancellation.Cancel).WaitAsync(TimeSpan.FromSeconds(15));
                await Assert.ThrowsAsync<OperationCanceledException>(() => run.WaitAsync(TimeSpan.FromSeconds(15)));
            }
            else
            {
                await debugger.ContinueSessionAsync(session.SessionId, cancellation.Token);
                TestRunSummary result = await run.WaitAsync(cancellation.Token);
                Assert.AreEqual(1, result.PassedCount);
                Assert.AreEqual(selected.Id, result.Cases.Single().TestId);
            }
        }
        finally
        {
            Task cancel = cancellation.CancelAsync();
            await cancel.WaitAsync(TimeSpan.FromSeconds(15));
            try
            {
                await run.WaitAsync(TimeSpan.FromSeconds(15));
            }
            catch (Exception) when (cancellation.IsCancellationRequested)
            {
            }
        }
    }

    private static string SourcePath([CallerFilePath] string path = "") => path;

    private sealed class DebugSessionLease(IDebugSessionService debugger, string sessionId) : IAsyncDisposable
    {
        public async ValueTask DisposeAsync() => await debugger.StopSessionAsync(sessionId);
    }
}
