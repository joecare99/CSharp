using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AA98_AvlnCodeStudio.Base.Building.Models;

namespace AA98_AvlnCodeStudio.Base.Building.Services;

/// <summary>
/// Uses the .NET SDK to refresh executable output and dependency metadata without restoring packages.
/// </summary>
public sealed class DotNetProjectRuntimeBuilder : IProjectRuntimeBuilder
{
    public async Task BuildAsync(BuilderBuildRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.ProjectPath);
        string projectPath = Path.GetFullPath(request.ProjectPath);
        if (!File.Exists(projectPath))
        {
            throw new FileNotFoundException("The selected project does not exist.", projectPath);
        }

        ProcessStartInfo startInfo = new("dotnet")
        {
            WorkingDirectory = Path.GetDirectoryName(projectPath)!,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        startInfo.Environment.Remove("MSBuildSDKsPath");
        startInfo.Environment.Remove("MSBuildExtensionsPath");
        startInfo.Environment.Remove("MSBuildExtensionsPath32");
        startInfo.Environment.Remove("MSBuildToolsPath");
        startInfo.Environment.Remove("DOTNET_MSBUILD_SDK_RESOLVER_SDKS_DIR");
        startInfo.Environment.Remove("DOTNET_MSBUILD_SDK_RESOLVER_CLI_DIR");
        startInfo.ArgumentList.Add("build");
        startInfo.ArgumentList.Add(projectPath);
        startInfo.ArgumentList.Add("--no-restore");
        startInfo.ArgumentList.Add("--nologo");
        startInfo.ArgumentList.Add("--verbosity");
        startInfo.ArgumentList.Add("quiet");
        if (!string.IsNullOrWhiteSpace(request.Configuration))
        {
            startInfo.ArgumentList.Add("--configuration");
            startInfo.ArgumentList.Add(request.Configuration);
        }

        if (!string.IsNullOrWhiteSpace(request.TargetFramework))
        {
            startInfo.ArgumentList.Add("--framework");
            startInfo.ArgumentList.Add(request.TargetFramework);
        }

        cancellationToken.ThrowIfCancellationRequested();
        using Process process = Process.Start(startInfo)
            ?? throw new InvalidOperationException("The .NET runtime build process could not be started.");
        Task<string> outputTask = process.StandardOutput.ReadToEndAsync();
        Task<string> errorTask = process.StandardError.ReadToEndAsync();
        try
        {
            await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            try
            {
                if (!process.HasExited)
                {
                    process.Kill(entireProcessTree: true);
                }
            }
            catch (Exception exception) when (exception is InvalidOperationException or Win32Exception)
            {
            }

            await process.WaitForExitAsync(CancellationToken.None).ConfigureAwait(false);
            throw;
        }

        string output = await outputTask.ConfigureAwait(false);
        string error = await errorTask.ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();
        if (process.ExitCode != 0)
        {
            string detail = (output + Environment.NewLine + error).Trim();
            if (detail.Length > 8192)
            {
                detail = detail[..8192];
            }

            throw new InvalidOperationException($"The runtime build failed (exit code {process.ExitCode}): {detail}");
        }
    }
}
