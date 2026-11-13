using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Base.Testing.Services;

/// <summary>
/// Resolves the VSTest runner in the SDK selected for a project directory.
/// </summary>
public sealed class DotNetVSTestRunnerLocator
{
    public async Task<string> FindRunnerAsync(string workingDirectory, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workingDirectory);
        workingDirectory = Path.GetFullPath(workingDirectory);
        if (!Directory.Exists(workingDirectory))
        {
            throw new DirectoryNotFoundException(workingDirectory);
        }

        string version = (await RunDotNetAsync(workingDirectory, "--version", cancellationToken).ConfigureAwait(false)).Trim();
        string installed = await RunDotNetAsync(workingDirectory, "--list-sdks", cancellationToken).ConfigureAwait(false);
        foreach (string line in installed.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries))
        {
            string prefix = version + " [";
            if (!line.StartsWith(prefix, StringComparison.Ordinal) || !line.EndsWith(']'))
            {
                continue;
            }

            string sdkRoot = line[prefix.Length..^1];
            string runner = Path.Combine(sdkRoot, version, "vstest.console.dll");
            if (File.Exists(runner))
            {
                return runner;
            }
        }

        throw new FileNotFoundException($"The VSTest runner for selected .NET SDK '{version}' was not found.");
    }

    private static async Task<string> RunDotNetAsync(string workingDirectory, string argument, CancellationToken cancellationToken)
    {
        ProcessStartInfo startInfo = new("dotnet")
        {
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        foreach (string name in new[]
        {
            "MSBuildSDKsPath", "MSBuildExtensionsPath", "MSBuildExtensionsPath32", "MSBuildToolsPath",
            "DOTNET_MSBUILD_SDK_RESOLVER_SDKS_DIR", "DOTNET_MSBUILD_SDK_RESOLVER_CLI_DIR",
        })
        {
            startInfo.Environment.Remove(name);
        }
        startInfo.ArgumentList.Add(argument);
        using Process process = Process.Start(startInfo)
            ?? throw new InvalidOperationException("The dotnet process could not be started.");
        Task<string> outputTask = process.StandardOutput.ReadToEndAsync();
        Task<string> errorTask = process.StandardError.ReadToEndAsync();
        try
        {
            await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            if (!process.HasExited)
            {
                process.Kill(entireProcessTree: true);
            }
            await process.WaitForExitAsync(CancellationToken.None).ConfigureAwait(false);
            throw;
        }
        string output = await outputTask.ConfigureAwait(false);
        string error = await errorTask.ConfigureAwait(false);
        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"dotnet {argument} failed: {error}{Environment.NewLine}{output}");
        }
        return output;
    }
}
