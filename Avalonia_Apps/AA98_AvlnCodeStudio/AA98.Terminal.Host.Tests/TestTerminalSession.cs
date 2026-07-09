using AA98_AvlnCodeStudio.Base.OS.Models;
using AA98_AvlnCodeStudio.Base.OS.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AA98.Terminal.Host.Tests;

internal sealed class TestTerminalSession : ITerminalSession
{
    public string SessionId { get; set; } = string.Empty;

    public TerminalShellDescriptor Shell { get; set; } = new();

    public TerminalSessionState State { get; set; } = TerminalSessionState.Running;

    public event EventHandler<string>? OutputReceived;

    public event EventHandler<string>? OutputPartialReceived;

    public event EventHandler<string>? ErrorReceived;

    public event EventHandler<string>? ErrorPartialReceived;

    public event EventHandler<int>? Exited;

    public List<string> WrittenLines { get; } = [];

    public bool StopRequested { get; private set; }

    public bool IsDisposed { get; private set; }

    public Task WriteInputAsync(string text, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        WrittenLines.Add(text);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        StopRequested = true;
        State = TerminalSessionState.Stopped;
        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        IsDisposed = true;
        State = TerminalSessionState.Stopped;
        return ValueTask.CompletedTask;
    }

    public void EmitOutput(string text)
        => OutputReceived?.Invoke(this, text);

    public void EmitOutputPartial(string text)
        => OutputPartialReceived?.Invoke(this, text);

    public void EmitError(string text)
        => ErrorReceived?.Invoke(this, text);

    public void EmitErrorPartial(string text)
        => ErrorPartialReceived?.Invoke(this, text);

    public void EmitExited(int exitCode)
    {
        State = TerminalSessionState.Stopped;
        Exited?.Invoke(this, exitCode);
    }
}