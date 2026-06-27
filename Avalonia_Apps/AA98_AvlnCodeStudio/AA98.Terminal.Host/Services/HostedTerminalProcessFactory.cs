using AA98_AvlnCodeStudio.Base.OS.Models;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace AA98.Terminal.Host.Services;

/// <summary>
/// Starts redirected shell processes for the terminal micro host.
/// </summary>
public sealed class HostedTerminalProcessFactory : IHostedTerminalProcessFactory
{
    /// <inheritdoc/>
    public Task<IHostedTerminalProcess> StartAsync(TerminalSessionStartRequest request, TerminalShellDescriptor shell, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var startInfo = new ProcessStartInfo
        {
            FileName = shell.ExecutablePath,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        if (!string.IsNullOrWhiteSpace(request.WorkingDirectory))
        {
            startInfo.WorkingDirectory = request.WorkingDirectory;
        }

        foreach (var argument in shell.Arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        foreach (var environmentVariable in request.EnvironmentVariables)
        {
            startInfo.Environment[environmentVariable.Key] = environmentVariable.Value;
        }

        var process = new Process
        {
            StartInfo = startInfo,
            EnableRaisingEvents = true,
        };

        process.Start();
        return Task.FromResult<IHostedTerminalProcess>(new HostedTerminalProcess(process));
    }
}