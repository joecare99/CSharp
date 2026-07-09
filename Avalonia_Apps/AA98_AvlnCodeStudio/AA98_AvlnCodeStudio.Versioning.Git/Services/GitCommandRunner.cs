using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Versioning.Git.Services;

/// <summary>
/// Runs local Git commands through the system process layer.
/// </summary>
public sealed class GitCommandRunner : IGitCommandRunner
{
    /// <inheritdoc/>
    public async Task<GitCommandResult> RunAsync(string workingDirectory, string arguments, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workingDirectory);
        ArgumentException.ThrowIfNullOrWhiteSpace(arguments);
        cancellationToken.ThrowIfCancellationRequested();

        using Process process = new()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = arguments,
                WorkingDirectory = workingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            },
        };

        process.Start();
        string standardOutput = await process.StandardOutput.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
        string standardError = await process.StandardError.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
        await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);

        return new GitCommandResult
        {
            ExitCode = process.ExitCode,
            StandardOutput = standardOutput,
            StandardError = standardError,
        };
    }
}
