using AA98_AvlnCodeStudio.Base.Profiling.Models;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Base.Profiling.Services;

internal sealed class DotNetProfilingSession : IDotNetProfilingSession
{
    private readonly string _tool;
    private readonly IProfilingProcess _process;
    private readonly object _stopLock = new();
    private CancellationTokenRegistration _cancellationRegistration;
    private int _wasStopped;
    private int _disposed;

    public DotNetProfilingSession(
        int processId,
        DotNetProfilingOperation operation,
        string tool,
        string? artifactPath,
        IProfilingProcess process)
    {
        ProcessId = processId;
        Operation = operation;
        _tool = tool;
        ArtifactPath = artifactPath;
        _process = process;
        _process.OutputReceived += OnProcessOutputReceived;
        Completion = CompleteAsync();
    }

    public int ProcessId { get; }

    public DotNetProfilingOperation Operation { get; }

    public string? ArtifactPath { get; }

    public Task<DotNetProfilingResult> Completion { get; }

    public event EventHandler<DotNetProfilingOutputEventArgs>? OutputReceived;

    public void AttachCancellation(CancellationToken cancellationToken)
    {
        if (cancellationToken.CanBeCanceled)
        {
            _cancellationRegistration = cancellationToken.Register(
                static state => ((DotNetProfilingSession)state!).RequestStop(),
                this);
        }
    }

    public async Task StopAsync()
    {
        lock (_stopLock)
        {
            if (!_process.HasExited)
            {
                Interlocked.Exchange(ref _wasStopped, 1);
                try
                {
                    _process.Kill();
                }
                catch (InvalidOperationException) when (_process.HasExited)
                {
                }
            }
        }

        await Completion.ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }

        try
        {
            try
            {
                await StopAsync().ConfigureAwait(false);
            }
            catch (DotNetProfilingException)
            {
                _ = Completion.Exception;
            }
        }
        finally
        {
            _cancellationRegistration.Dispose();
            _process.OutputReceived -= OnProcessOutputReceived;
            _process.Dispose();
        }
    }

    private async Task<DotNetProfilingResult> CompleteAsync()
    {
        await _process.WaitForExitAsync(CancellationToken.None).ConfigureAwait(false);
        ProfilingOutputSnapshot standardOutputCapture = await _process.ReadStandardOutputAsync().ConfigureAwait(false);
        ProfilingOutputSnapshot standardErrorCapture = await _process.ReadStandardErrorAsync().ConfigureAwait(false);
        string standardOutput = standardOutputCapture.Text;
        string standardError = standardErrorCapture.Text;
        int exitCode = _process.ExitCode;
        bool wasStopped = Volatile.Read(ref _wasStopped) != 0;
        if (!wasStopped && exitCode != 0)
        {
            throw new DotNetProfilingException(
                $"Profiling tool '{_tool}' failed with exit code {exitCode}."
                + (string.IsNullOrWhiteSpace(standardError) ? string.Empty : $" {standardError.Trim()}"),
                _tool,
                exitCode,
                standardError);
        }

        if (!wasStopped && ArtifactPath is not null)
        {
            FileInfo artifact = new(ArtifactPath);
            if (!artifact.Exists || artifact.Length == 0)
            {
                throw new DotNetProfilingException(
                    $"Profiling tool '{_tool}' exited successfully but did not produce the expected artifact '{ArtifactPath}'.",
                    _tool,
                    exitCode,
                    standardError);
            }
        }

        return new DotNetProfilingResult(
            ProcessId,
            Operation,
            exitCode,
            wasStopped ? null : ArtifactPath,
            standardOutput,
            standardError,
            wasStopped,
            standardOutputCapture.WasTruncated,
            standardErrorCapture.WasTruncated);
    }

    private void RequestStop()
    {
        lock (_stopLock)
        {
            if (_process.HasExited)
            {
                return;
            }

            Interlocked.Exchange(ref _wasStopped, 1);
            try
            {
                _process.Kill();
            }
            catch (InvalidOperationException) when (_process.HasExited)
            {
            }
        }
    }

    private void OnProcessOutputReceived(object? sender, ProfilingProcessOutputEventArgs eventArgs)
    {
        OutputReceived?.Invoke(
            this,
            new DotNetProfilingOutputEventArgs(eventArgs.Text, eventArgs.IsError));
    }
}
