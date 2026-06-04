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
    private readonly Process _process;

    /// <summary>
    /// Initializes a new instance of the <see cref="HostedProcess"/> class.
    /// </summary>
    /// <param name="process">The underlying process.</param>
    public HostedProcess(Process process)
    {
        _process = process;
        _process.OutputDataReceived += OnOutputDataReceived;
        _process.ErrorDataReceived += OnErrorDataReceived;
        _process.Exited += OnExited;
        _process.BeginOutputReadLine();
        _process.BeginErrorReadLine();
    }

    /// <inheritdoc/>
    public event EventHandler<string>? StandardOutputReceived;

    /// <inheritdoc/>
    public event EventHandler<string>? StandardErrorReceived;

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
        _process.OutputDataReceived -= OnOutputDataReceived;
        _process.ErrorDataReceived -= OnErrorDataReceived;
        _process.Exited -= OnExited;

        if (!_process.HasExited)
        {
            _process.Kill(true);
            await _process.WaitForExitAsync().ConfigureAwait(false);
        }

        _process.Dispose();
    }

    private void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (e.Data is not null)
        {
            StandardOutputReceived?.Invoke(this, e.Data);
        }
    }

    private void OnErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (e.Data is not null)
        {
            StandardErrorReceived?.Invoke(this, e.Data);
        }
    }

    private void OnExited(object? sender, EventArgs e)
        => Exited?.Invoke(this, _process.ExitCode);
}
