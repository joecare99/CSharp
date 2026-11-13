using AA98_AvlnCodeStudio.Base.Profiling.Models;
using AA98_AvlnCodeStudio.Base.Profiling.DependencyInjection;
using AA98_AvlnCodeStudio.Base.Profiling.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Base.Profiling.Tests;

[TestClass]
public sealed class DotNetProfilingServiceTests
{
    [TestMethod]
    public async Task StartCounters_UsesExplicitProcessAndStopKillsOnlyProfiler()
    {
        FakeProfilingProcess process = new();
        FakeProfilingProcessLauncher launcher = new(process);
        DotNetProfilingService service = CreateService(launcher);

        await using IDotNetProfilingSession session = await service.StartAsync(
            new DotNetProfilingRequest(1234, DotNetProfilingOperation.Counters));

        CollectionAssert.AreEqual(
            new[] { "monitor", "--process-id", "1234" },
            launcher.Arguments);
        Assert.AreEqual("dotnet-counters", launcher.Executable);
        Assert.AreEqual(1234, session.ProcessId);
        Assert.IsNull(session.ArtifactPath);

        await session.StopAsync();

        Assert.IsTrue(process.WasKilled);
        DotNetProfilingResult result = await session.Completion;
        Assert.IsTrue(result.WasStopped);
        Assert.IsFalse(result.Succeeded);
        Assert.IsNull(result.ArtifactPath);
    }

    [TestMethod]
    public async Task StartTraceAndHeapDump_UsesGeneratedContainedArtifactsAndExactCommands()
    {
        string root = CreateTemporaryDirectory();
        try
        {
            foreach ((DotNetProfilingOperation operation, string executable, string artifactExtension, string[] command) in new[]
            {
                (DotNetProfilingOperation.Trace, "dotnet-trace", ".nettrace",
                    new[] { "collect", "--process-id", "42", "--output" }),
                (DotNetProfilingOperation.HeapDump, "dotnet-dump", ".dmp",
                    new[] { "collect", "--process-id", "42", "--type", "Heap", "--output" }),
            })
            {
                FakeProfilingProcess process = new();
                FakeProfilingProcessLauncher launcher = new(process);
                DotNetProfilingService service = CreateService(launcher);
                await using IDotNetProfilingSession session = await service.StartAsync(
                    new DotNetProfilingRequest(42, operation, root));

                Assert.AreEqual(executable, launcher.Executable);
                Assert.AreEqual(command.Length + 1, launcher.Arguments.Length);
                CollectionAssert.AreEqual(command, launcher.Arguments[..command.Length]);
                string artifactPath = session.ArtifactPath!;
                Assert.AreEqual(artifactExtension, Path.GetExtension(artifactPath));
                Assert.AreEqual("dotnet-profiling", Path.GetFileName(Path.GetDirectoryName(artifactPath)));
                Assert.IsTrue(artifactPath.StartsWith(
                    Path.Combine(Path.GetFullPath(root), "dotnet-profiling") + Path.DirectorySeparatorChar,
                    OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));
                Assert.AreEqual(artifactPath, launcher.Arguments[^1]);

                // This small marker is only a fake tool output; the tests never invoke a profiler or create a dump.
                File.WriteAllText(artifactPath, "fake artifact");
                process.Finish(0, "completed", string.Empty);
                DotNetProfilingResult result = await session.Completion;
                Assert.AreEqual(0, result.ExitCode);
                Assert.AreEqual(artifactPath, result.ArtifactPath);
                Assert.IsFalse(result.WasStopped);
            }
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task StartAsync_RejectsInvalidProcessAndReportsToolLaunchFailure()
    {
        FakeProfilingProcess process = new();
        FakeProfilingProcessLauncher launcher = new(process);
        DotNetProfilingService service = CreateService(launcher);
        await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(
            () => service.StartAsync(new DotNetProfilingRequest(0, DotNetProfilingOperation.Counters)));
        Assert.IsNull(launcher.Executable);

        launcher.LaunchException = new IOException("tool missing");
        DotNetProfilingException exception = await Assert.ThrowsExactlyAsync<DotNetProfilingException>(
            () => service.StartAsync(new DotNetProfilingRequest(12, DotNetProfilingOperation.Counters)));
        Assert.AreEqual("dotnet-counters", exception.ToolName);
        StringAssert.Contains(exception.Message, "tool missing");
    }

    [TestMethod]
    public async Task CompletedToolFailuresAndMissingArtifactsAreExplicit()
    {
        string root = CreateTemporaryDirectory();
        try
        {
            FakeProfilingProcess failedProcess = new();
            FakeProfilingProcessLauncher failedLauncher = new(failedProcess);
            IDotNetProfilingSession failedSession = await CreateService(failedLauncher).StartAsync(
                new DotNetProfilingRequest(77, DotNetProfilingOperation.Counters));
            failedProcess.Finish(5, string.Empty, "permission denied");
            DotNetProfilingException toolFailure = await Assert.ThrowsExactlyAsync<DotNetProfilingException>(
                async () => await failedSession.Completion);
            Assert.AreEqual(5, toolFailure.ExitCode);
            StringAssert.Contains(toolFailure.StandardError!, "permission denied");
            await failedSession.DisposeAsync();

            FakeProfilingProcess missingArtifactProcess = new();
            FakeProfilingProcessLauncher missingArtifactLauncher = new(missingArtifactProcess);
            IDotNetProfilingSession missingArtifactSession = await CreateService(missingArtifactLauncher).StartAsync(
                new DotNetProfilingRequest(77, DotNetProfilingOperation.Trace, root));
            missingArtifactProcess.Finish(0, string.Empty, string.Empty);
            DotNetProfilingException missingArtifact = await Assert.ThrowsExactlyAsync<DotNetProfilingException>(
                async () => await missingArtifactSession.Completion);
            StringAssert.Contains(missingArtifact.Message, "did not produce the expected artifact");
            await missingArtifactSession.DisposeAsync();
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task CancellationAfterStartupStopsProfilerProcess()
    {
        FakeProfilingProcess process = new();
        FakeProfilingProcessLauncher launcher = new(process);
        DotNetProfilingService service = CreateService(launcher);
        using CancellationTokenSource cancellation = new();

        IDotNetProfilingSession session = await service.StartAsync(
            new DotNetProfilingRequest(99, DotNetProfilingOperation.Counters),
            cancellation.Token);
        cancellation.Cancel();

        DotNetProfilingResult result = await session.Completion;
        Assert.IsTrue(process.WasKilled);
        Assert.IsTrue(result.WasStopped);
        await session.DisposeAsync();
    }

    [TestMethod]
    public async Task HighVolumeOutput_IsStreamedAndFinalCaptureIsBoundedAndMarked()
    {
        ProfilingOutputCapture standardOutputCapture = new(isError: false);
        ProfilingOutputCapture standardErrorCapture = new(isError: true);
        int streamedStandardOutputLineCount = 0;
        int streamedStandardErrorLineCount = 0;
        standardOutputCapture.OutputReceived += (_, eventArgs) =>
        {
            Assert.IsFalse(eventArgs.IsError);
            streamedStandardOutputLineCount++;
        };
        standardErrorCapture.OutputReceived += (_, eventArgs) =>
        {
            Assert.IsTrue(eventArgs.IsError);
            streamedStandardErrorLineCount++;
        };
        const int lineCount = 20_000;
        for (int line = 0; line < lineCount; line++)
        {
            string text = $"line-{line:D5}-12345678901234567890";
            standardOutputCapture.AppendLine(text);
            standardErrorCapture.AppendLine(text);
        }

        ProfilingOutputSnapshot standardOutput = standardOutputCapture.Complete();
        ProfilingOutputSnapshot standardError = standardErrorCapture.Complete();
        Assert.AreEqual(lineCount, streamedStandardOutputLineCount);
        Assert.AreEqual(lineCount, streamedStandardErrorLineCount);
        Assert.IsTrue(standardOutput.WasTruncated);
        Assert.IsTrue(standardError.WasTruncated);
        StringAssert.Contains(standardOutput.Text, "[output truncated after 65536 characters]");
        StringAssert.Contains(standardError.Text, "[output truncated after 65536 characters]");
        Assert.IsTrue(standardOutput.Text.Length < ProfilingOutputCapture.MaximumRetainedCharacters + 100);
        Assert.IsTrue(standardError.Text.Length < ProfilingOutputCapture.MaximumRetainedCharacters + 100);

        FakeProfilingProcess process = new();
        await using DotNetProfilingSession session = new(
            7,
            DotNetProfilingOperation.Counters,
            "fake-counters",
            artifactPath: null,
            process);
        process.Finish(
            0,
            standardOutput.Text,
            standardError.Text,
            standardOutputTruncated: standardOutput.WasTruncated,
            standardErrorTruncated: standardError.WasTruncated);

        DotNetProfilingResult result = await session.Completion;
        Assert.IsTrue(result.StandardOutputTruncated);
        Assert.IsTrue(result.StandardErrorTruncated);
        Assert.IsTrue(result.StandardOutput.Contains("[output truncated", StringComparison.Ordinal));
        Assert.IsTrue(result.StandardError.Contains("[output truncated", StringComparison.Ordinal));
    }

    [TestMethod]
    public async Task StoppingTrace_DoesNotReportPartialArtifactAsSuccessful()
    {
        string root = CreateTemporaryDirectory();
        try
        {
            FakeProfilingProcess process = new();
            IDotNetProfilingSession session = await CreateService(new FakeProfilingProcessLauncher(process)).StartAsync(
                new DotNetProfilingRequest(77, DotNetProfilingOperation.Trace, root));
            string intendedArtifactPath = session.ArtifactPath!;
            File.WriteAllText(intendedArtifactPath, "partial fake trace");

            await session.StopAsync();

            DotNetProfilingResult result = await session.Completion;
            Assert.IsTrue(result.WasStopped);
            Assert.IsFalse(result.Succeeded);
            Assert.IsNull(result.ArtifactPath);
            Assert.IsTrue(File.Exists(intendedArtifactPath));
            await session.DisposeAsync();
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public void AddDotNetProfiling_IsExplicitAndRegistersService()
    {
        ServiceCollection services = new();

        services.AddDotNetProfiling(new DotNetProfilingToolPaths("counters", "trace", "dump"));

        Assert.IsTrue(services.Any(descriptor => descriptor.ServiceType == typeof(IDotNetProfilingService)));
        Assert.IsTrue(services.Any(descriptor => descriptor.ServiceType == typeof(DotNetProfilingToolPaths)));
    }

    [TestMethod]
    public async Task ProcessLauncher_DrainsAnOwnedBenignChildProcess()
    {
        using IProfilingProcess process = new ProfilingProcessLauncher().Start("dotnet", ["--version"]);

        await process.WaitForExitAsync(CancellationToken.None).WaitAsync(TimeSpan.FromSeconds(15));
        ProfilingOutputSnapshot output = await process.ReadStandardOutputAsync().WaitAsync(TimeSpan.FromSeconds(5));
        ProfilingOutputSnapshot error = await process.ReadStandardErrorAsync().WaitAsync(TimeSpan.FromSeconds(5));

        Assert.AreEqual(0, process.ExitCode);
        Assert.IsFalse(string.IsNullOrWhiteSpace(output.Text));
        Assert.IsFalse(output.WasTruncated);
        Assert.AreEqual(string.Empty, error.Text);
    }

    private static DotNetProfilingService CreateService(FakeProfilingProcessLauncher launcher)
    {
        return new DotNetProfilingService(
            new DotNetProfilingToolPaths(),
            launcher);
    }

    private static string CreateTemporaryDirectory()
    {
        string path = Path.Combine(Path.GetTempPath(), $"AA98_DotNetProfiling_{Guid.NewGuid():N}");
        Directory.CreateDirectory(path);
        return path;
    }

    private sealed class FakeProfilingProcessLauncher(FakeProfilingProcess process) : IProfilingProcessLauncher
    {
        public string? Executable { get; private set; }

        public string[] Arguments { get; private set; } = [];

        public Exception? LaunchException { get; set; }

        public IProfilingProcess Start(string executable, IReadOnlyList<string> arguments)
        {
            if (LaunchException is not null)
            {
                throw LaunchException;
            }

            Executable = executable;
            Arguments = [.. arguments];
            return process;
        }
    }

    private sealed class FakeProfilingProcess : IProfilingProcess
    {
        private readonly TaskCompletionSource _exited = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly TaskCompletionSource<ProfilingOutputSnapshot> _standardOutput = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly TaskCompletionSource<ProfilingOutputSnapshot> _standardError = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private int _exitCode;

        public bool HasExited { get; private set; }

        public int ExitCode => _exitCode;

        public bool WasKilled { get; private set; }

        public event EventHandler<ProfilingProcessOutputEventArgs>? OutputReceived;

        public Task WaitForExitAsync(CancellationToken cancellationToken)
        {
            return _exited.Task.WaitAsync(cancellationToken);
        }

        public Task<ProfilingOutputSnapshot> ReadStandardOutputAsync()
        {
            return _standardOutput.Task;
        }

        public Task<ProfilingOutputSnapshot> ReadStandardErrorAsync()
        {
            return _standardError.Task;
        }

        public void Kill()
        {
            WasKilled = true;
            Finish(137, string.Empty, string.Empty);
        }

        public void Dispose()
        {
        }

        public void Finish(
            int exitCode,
            string standardOutput,
            string standardError,
            bool standardOutputTruncated = false,
            bool standardErrorTruncated = false)
        {
            _exitCode = exitCode;
            HasExited = true;
            if (standardOutput.Length > 0)
            {
                OutputReceived?.Invoke(this, new ProfilingProcessOutputEventArgs(standardOutput, isError: false));
            }

            if (standardError.Length > 0)
            {
                OutputReceived?.Invoke(this, new ProfilingProcessOutputEventArgs(standardError, isError: true));
            }

            _standardOutput.TrySetResult(new ProfilingOutputSnapshot(standardOutput, standardOutputTruncated));
            _standardError.TrySetResult(new ProfilingOutputSnapshot(standardError, standardErrorTruncated));
            _exited.TrySetResult();
        }
    }
}
