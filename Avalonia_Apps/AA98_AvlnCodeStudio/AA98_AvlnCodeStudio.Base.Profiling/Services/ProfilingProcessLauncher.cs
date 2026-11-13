using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Base.Profiling.Services;

internal interface IProfilingProcessLauncher
{
    IProfilingProcess Start(string executable, IReadOnlyList<string> arguments);
}

internal interface IProfilingProcess : IDisposable
{
    bool HasExited { get; }

    int ExitCode { get; }

    event EventHandler<ProfilingProcessOutputEventArgs>? OutputReceived;

    Task WaitForExitAsync(CancellationToken cancellationToken);

    Task<ProfilingOutputSnapshot> ReadStandardOutputAsync();

    Task<ProfilingOutputSnapshot> ReadStandardErrorAsync();

    void Kill();
}

internal sealed class ProfilingProcessLauncher : IProfilingProcessLauncher
{
    public IProfilingProcess Start(string executable, IReadOnlyList<string> arguments)
    {
        ProcessStartInfo startInfo = new()
        {
            FileName = executable,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        foreach (string argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        Process process = new()
        {
            StartInfo = startInfo,
            EnableRaisingEvents = true,
        };
        try
        {
            if (!process.Start())
            {
                throw new InvalidOperationException($"Process '{executable}' did not start.");
            }

            return new ProfilingProcess(process);
        }
        catch
        {
            process.Dispose();
            throw;
        }
    }
}

internal sealed class ProfilingProcess : IProfilingProcess
{
    private readonly Process _process;
    private readonly Task<ProfilingOutputSnapshot> _standardOutput;
    private readonly Task<ProfilingOutputSnapshot> _standardError;

    public ProfilingProcess(Process process)
    {
        _process = process;
        _standardOutput = ReadLinesAsync(process.StandardOutput, isError: false);
        _standardError = ReadLinesAsync(process.StandardError, isError: true);
    }

    public bool HasExited => _process.HasExited;

    public int ExitCode => _process.ExitCode;

    public event EventHandler<ProfilingProcessOutputEventArgs>? OutputReceived;

    public Task WaitForExitAsync(CancellationToken cancellationToken)
    {
        return _process.WaitForExitAsync(cancellationToken);
    }

    public Task<ProfilingOutputSnapshot> ReadStandardOutputAsync()
    {
        return _standardOutput;
    }

    public Task<ProfilingOutputSnapshot> ReadStandardErrorAsync()
    {
        return _standardError;
    }

    public void Kill()
    {
        _process.Kill(entireProcessTree: true);
    }

    public void Dispose()
    {
        _process.Dispose();
    }

    private async Task<ProfilingOutputSnapshot> ReadLinesAsync(StreamReader reader, bool isError)
    {
        ProfilingOutputCapture capture = new(isError);
        capture.OutputReceived += (_, eventArgs) => OutputReceived?.Invoke(this, eventArgs);
        while (await reader.ReadLineAsync().ConfigureAwait(false) is { } line)
        {
            capture.AppendLine(line);
        }

        return capture.Complete();
    }
}

internal sealed class ProfilingOutputCapture
{
    internal const int MaximumRetainedCharacters = 64 * 1024;

    private readonly StringBuilder _retainedText = new();
    private readonly bool _isError;
    private bool _hasLine;
    private bool _isTruncated;

    public ProfilingOutputCapture(bool isError)
    {
        _isError = isError;
    }

    public event EventHandler<ProfilingProcessOutputEventArgs>? OutputReceived;

    public void AppendLine(string line)
    {
        if (!_isTruncated)
        {
            if (_hasLine)
            {
                if (_retainedText.Length + Environment.NewLine.Length <= MaximumRetainedCharacters)
                {
                    _retainedText.Append(Environment.NewLine);
                }
                else
                {
                    _isTruncated = true;
                }
            }

            if (!_isTruncated)
            {
                int charactersToRetain = Math.Min(
                    line.Length,
                    MaximumRetainedCharacters - _retainedText.Length);
                if (charactersToRetain > 0)
                {
                    _retainedText.Append(line, 0, charactersToRetain);
                }

                _isTruncated = charactersToRetain < line.Length;
            }
        }

        _hasLine = true;
        OutputReceived?.Invoke(this, new ProfilingProcessOutputEventArgs(line, _isError));
    }

    public ProfilingOutputSnapshot Complete()
    {
        string text = _retainedText.ToString();
        if (_isTruncated)
        {
            text += $"{Environment.NewLine}[output truncated after {MaximumRetainedCharacters} characters]";
        }

        return new ProfilingOutputSnapshot(text, _isTruncated);
    }
}

internal readonly record struct ProfilingOutputSnapshot(string Text, bool WasTruncated);

internal sealed class ProfilingProcessOutputEventArgs(string text, bool isError) : EventArgs
{
    public string Text { get; } = text;

    public bool IsError { get; } = isError;
}
