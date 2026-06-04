using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Avln_TerminalHost.Services;

/// <summary>
/// Wraps a running process and forwards redirected streams as events.
/// </summary>
public sealed class HostedProcess : IHostedProcess
{
    private static readonly TimeSpan PartialFlushDelay = TimeSpan.FromMilliseconds(100);
    private readonly Process _process;
    private readonly CancellationTokenSource _readCancellationTokenSource = new();
    private readonly Task _standardOutputTask;
    private readonly Task _standardErrorTask;

    /// <summary>
    /// Initializes a new instance of the <see cref="HostedProcess"/> class.
    /// </summary>
    /// <param name="process">The underlying process.</param>
    public HostedProcess(Process process)
    {
        _process = process;
        _process.Exited += OnExited;

        var standardOutputReader = new OutputChunkReader(
            _process.StandardOutput,
            PartialFlushDelay,
            text => StandardOutputReceived?.Invoke(this, text),
            text => StandardOutputPartialReceived?.Invoke(this, text));
        var standardErrorReader = new OutputChunkReader(
            _process.StandardError,
            PartialFlushDelay,
            text => StandardErrorReceived?.Invoke(this, text),
            text => StandardErrorPartialReceived?.Invoke(this, text));

        _standardOutputTask = standardOutputReader.RunAsync(_readCancellationTokenSource.Token);
        _standardErrorTask = standardErrorReader.RunAsync(_readCancellationTokenSource.Token);
    }

    /// <inheritdoc/>
    public event EventHandler<string>? StandardOutputReceived;

    /// <inheritdoc/>
    public event EventHandler<string>? StandardOutputPartialReceived;

    /// <inheritdoc/>
    public event EventHandler<string>? StandardErrorReceived;

    /// <inheritdoc/>
    public event EventHandler<string>? StandardErrorPartialReceived;

    /// <inheritdoc/>
    public event EventHandler<int>? Exited;

    /// <inheritdoc/>
    public async Task WriteLineAsync(string text, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await _process.StandardInput.WriteLineAsync(text).ConfigureAwait(false);
        await _process.StandardInput.FlushAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_process.HasExited)
        {
            _process.Kill(true);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        _process.Exited -= OnExited;
        _readCancellationTokenSource.Cancel();

        if (!_process.HasExited)
        {
            _process.Kill(true);
            await _process.WaitForExitAsync().ConfigureAwait(false);
        }

        try
        {
            await Task.WhenAll(_standardOutputTask, _standardErrorTask).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
        }

        _readCancellationTokenSource.Dispose();
        _process.Dispose();
    }

    private void OnExited(object? sender, EventArgs e)
        => Exited?.Invoke(this, _process.ExitCode);
}
