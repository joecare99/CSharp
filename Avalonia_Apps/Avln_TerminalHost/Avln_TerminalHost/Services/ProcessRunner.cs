using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Avln_TerminalHost.Services;

/// <summary>
/// Starts `%ComSpec%` as a redirected child process.
/// </summary>
public sealed class ProcessRunner(IComSpecLocator comSpecLocator) : IProcessRunner
{
    private readonly IComSpecLocator _comSpecLocator = comSpecLocator;

    /// <inheritdoc/>
    public Task<IHostedProcess> StartAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var startInfo = new ProcessStartInfo
        {
            FileName = _comSpecLocator.GetShellPath(),
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        var process = new Process
        {
            StartInfo = startInfo,
            EnableRaisingEvents = true,
        };

        process.Start();
        return Task.FromResult<IHostedProcess>(new HostedProcess(process));
    }
}
