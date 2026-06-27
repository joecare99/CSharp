using AA98_AvlnCodeStudio.Base.OS.Models;
using AA98_AvlnCodeStudio.Base.OS.Services;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace AA98.Terminal.Host.Services;

/// <summary>
/// Resolves a development shell for the terminal micro host.
/// </summary>
public sealed class DevelopmentTerminalShellResolver : ITerminalShellResolver
{
    /// <inheritdoc/>
    public Task<TerminalShellDescriptor> ResolveShellAsync(TerminalSessionStartRequest request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var shellPath = request.ShellPath;
        var shellDisplayName = request.ShellDisplayName;
        var isFallback = false;

        if (string.IsNullOrWhiteSpace(shellPath))
        {
            shellPath = ResolveDefaultShellPath();
            isFallback = true;
        }

        if (string.IsNullOrWhiteSpace(shellDisplayName))
        {
            shellDisplayName = Path.GetFileNameWithoutExtension(shellPath);
        }

        var descriptor = new TerminalShellDescriptor
        {
            DisplayName = shellDisplayName,
            ExecutablePath = shellPath,
            IsFallback = isFallback,
        };

        foreach (var argument in request.Arguments)
        {
            descriptor.Arguments.Add(argument);
        }

        return Task.FromResult(descriptor);
    }

    private static string ResolveDefaultShellPath()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var comSpec = Environment.GetEnvironmentVariable("ComSpec");
            if (!string.IsNullOrWhiteSpace(comSpec) && File.Exists(comSpec))
            {
                return comSpec;
            }

            return "cmd.exe";
        }

        const string bashPath = "/bin/bash";
        if (File.Exists(bashPath))
        {
            return bashPath;
        }

        return "/bin/sh";
    }
}