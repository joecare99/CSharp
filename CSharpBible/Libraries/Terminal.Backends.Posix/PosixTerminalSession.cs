using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Terminal.Core;

namespace Terminal.Backends.Posix;

/// <summary>
/// Hosts a shell process for Linux and macOS platforms.
/// </summary>
/// <remarks>
/// The first slice uses a redirected process host while preserving the session contract and resize model.
/// A native PTY transport can replace the process wiring later without changing the public API.
/// </remarks>
public sealed class PosixTerminalSession : ITerminalSession
{
    private Process? _process;
    private StreamWriter? _inputWriter;
    private CancellationTokenSource? _readerCancellationTokenSource;
    private Task? _stdoutTask;
    private Task? _stderrTask;

    /// <inheritdoc/>
    public event EventHandler<string>? OutputReceived;

    /// <inheritdoc/>
    public bool IsRunning { get; private set; }

    /// <inheritdoc/>
    public TerminalSize Size { get; private set; } = new(80, 25);

    /// <inheritdoc/>
    public Task StartAsync(TerminalSessionOptions options, CancellationToken cancellationToken = default)
    {
        if (!(OperatingSystem.IsLinux() || OperatingSystem.IsMacOS()))
        {
            throw new PlatformNotSupportedException("The Posix terminal backend is only available on Linux and macOS.");
        }

        if (IsRunning)
        {
            throw new InvalidOperationException("The terminal session is already running.");
        }

        if (string.IsNullOrWhiteSpace(options.FileName))
        {
            throw new ArgumentException("A terminal executable is required.", nameof(options));
        }

        cancellationToken.ThrowIfCancellationRequested();
        Size = options.InitialSize.Normalize();

        var resolvedShell = PosixTerminalEnvironment.ResolveShell(options.FileName);
        var startInfo = new ProcessStartInfo
        {
            FileName = resolvedShell,
            Arguments = options.Arguments,
            WorkingDirectory = string.IsNullOrWhiteSpace(options.WorkingDirectory) ? Environment.CurrentDirectory : options.WorkingDirectory,
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8
        };

        foreach (var pair in options.EnvironmentVariables)
        {
            startInfo.Environment[pair.Key] = pair.Value;
        }

        resolvedShell = PosixTerminalEnvironment.ResolveShell(startInfo.Environment.TryGetValue("SHELL", out var shell)
            ? shell
            : resolvedShell);
        startInfo.Environment["SHELL"] = resolvedShell;
        startInfo.Environment["TERM"] = PosixTerminalEnvironment.ResolveTerm(startInfo.Environment.TryGetValue("TERM", out var term)
            ? term
            : null);
        startInfo.Environment["COLUMNS"] = Size.Columns.ToString();
        startInfo.Environment["LINES"] = Size.Rows.ToString();

        _process = new Process { StartInfo = startInfo, EnableRaisingEvents = true };
        _process.Start();
        _inputWriter = _process.StandardInput;
        _readerCancellationTokenSource = new CancellationTokenSource();
        _stdoutTask = Task.Run(() => PumpReaderAsync(_process.StandardOutput, _readerCancellationTokenSource.Token));
        _stderrTask = Task.Run(() => PumpReaderAsync(_process.StandardError, _readerCancellationTokenSource.Token));
        IsRunning = true;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task WriteAsync(string input, CancellationToken cancellationToken = default)
    {
        if (!IsRunning || _inputWriter is null || string.IsNullOrEmpty(input))
        {
            return;
        }

        await _inputWriter.WriteAsync(input.AsMemory(), cancellationToken).ConfigureAwait(false);
        await _inputWriter.FlushAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public Task ResizeAsync(TerminalSize size, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Size = size.Normalize();
        if (_process is not null && !_process.HasExited)
        {
            _process.StartInfo.Environment["COLUMNS"] = Size.Columns.ToString();
            _process.StartInfo.Environment["LINES"] = Size.Rows.ToString();
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!IsRunning)
        {
            return;
        }

        IsRunning = false;

        if (_readerCancellationTokenSource is not null)
        {
            _readerCancellationTokenSource.Cancel();
        }

        try
        {
            if (_process is not null && !_process.HasExited)
            {
                _process.Kill(entireProcessTree: true);
                await _process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
            }
        }
        catch
        {
        }

        if (_stdoutTask is not null)
        {
            try
            {
                await _stdoutTask.ConfigureAwait(false);
            }
            catch
            {
            }
        }

        if (_stderrTask is not null)
        {
            try
            {
                await _stderrTask.ConfigureAwait(false);
            }
            catch
            {
            }
        }

        _inputWriter?.Dispose();
        _readerCancellationTokenSource?.Dispose();
        _process?.Dispose();
        _inputWriter = null;
        _readerCancellationTokenSource = null;
        _process = null;
        _stdoutTask = null;
        _stderrTask = null;
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        await StopAsync().ConfigureAwait(false);
    }

    private async Task PumpReaderAsync(StreamReader reader, CancellationToken cancellationToken)
    {
        var buffer = new char[1024];
        while (!cancellationToken.IsCancellationRequested)
        {
            var read = await reader.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken).ConfigureAwait(false);
            if (read <= 0)
            {
                break;
            }

            OutputReceived?.Invoke(this, new string(buffer, 0, read));
        }
    }
}
