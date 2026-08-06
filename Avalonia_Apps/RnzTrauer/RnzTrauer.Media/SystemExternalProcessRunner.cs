using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace RnzTrauer.Media;

/// <summary>Default OS process runner with timeout and cancellation handling.</summary>
public sealed class SystemExternalProcessRunner : IExternalProcessRunner
{
    /// <inheritdoc />
    public async Task<ExternalProcessResult> RunAsync(
        ExternalProcessRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (string.IsNullOrWhiteSpace(request.ExecutablePath))
            throw new ArgumentException("An executable path is required.", nameof(request));
        if (request.Timeout <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(request.Timeout));

        using var process = new Process
        {
            StartInfo = CreateStartInfo(request),
            EnableRaisingEvents = true,
        };
        if (!process.Start())
            throw new InvalidOperationException(
                $"The process '{request.ExecutablePath}' could not be started.");

        using var timeoutSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutSource.CancelAfter(request.Timeout);
        try
        {
            var outputTask = process.StandardOutput.ReadToEndAsync(timeoutSource.Token);
            var errorTask = process.StandardError.ReadToEndAsync(timeoutSource.Token);
            await process.WaitForExitAsync(timeoutSource.Token).ConfigureAwait(false);
            return new ExternalProcessResult(
                process.ExitCode,
                await outputTask.ConfigureAwait(false),
                await errorTask.ConfigureAwait(false));
        }
        catch (OperationCanceledException)
        {
            if (!process.HasExited)
                process.Kill(entireProcessTree: true);
            throw;
        }
    }

    private static ProcessStartInfo CreateStartInfo(ExternalProcessRequest request)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = request.ExecutablePath,
            WorkingDirectory = request.WorkingDirectory ?? string.Empty,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
        };
        foreach (var argument in request.Arguments)
            startInfo.ArgumentList.Add(argument);
        return startInfo;
    }
}
