using AA98.Terminal.Host.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AA98.Terminal.Host.Tests;

internal sealed class TestHostedTerminalProcess : IHostedTerminalProcess
{
    public event EventHandler<string>? StandardOutputReceived;

    public event EventHandler<string>? StandardOutputPartialReceived;

    public event EventHandler<string>? StandardErrorReceived;

    public event EventHandler<string>? StandardErrorPartialReceived;

    public event EventHandler<int>? Exited;

    public List<string> WrittenLines { get; } = [];

    public bool StopRequested { get; private set; }

    public bool IsDisposed { get; private set; }

    public Task WriteLineAsync(string text, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        WrittenLines.Add(text);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        StopRequested = true;
        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        IsDisposed = true;
        return ValueTask.CompletedTask;
    }

    public void EmitStandardOutput(string text)
        => StandardOutputReceived?.Invoke(this, text);

    public void EmitStandardOutputPartial(string text)
        => StandardOutputPartialReceived?.Invoke(this, text);

    public void EmitStandardError(string text)
        => StandardErrorReceived?.Invoke(this, text);

    public void EmitStandardErrorPartial(string text)
        => StandardErrorPartialReceived?.Invoke(this, text);

    public void EmitExited(int exitCode)
        => Exited?.Invoke(this, exitCode);
}