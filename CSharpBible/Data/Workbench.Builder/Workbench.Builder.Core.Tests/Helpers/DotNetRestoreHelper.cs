using System;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Workbench.Builder.Core.Tests.TestData;

/// <summary>
/// Restores copied SDK-style test projects once per path so MSBuild reference resolution can consume generated assets.
/// </summary>
internal static class DotNetRestoreHelper
{
    private static readonly ConcurrentDictionary<string, bool> RestoredProjects = new(StringComparer.OrdinalIgnoreCase);

    public static void EnsureRestored(string projectPath)
    {
        if (!RestoredProjects.TryAdd(projectPath, true))
        {
            return;
        }

        ProcessStartInfo startInfo = new("dotnet", $"restore \"{projectPath}\"")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        using Process process = Process.Start(startInfo)
            ?? throw new InvalidOperationException("Failed to start dotnet restore.");

        string standardOutput = process.StandardOutput.ReadToEnd();
        string standardError = process.StandardError.ReadToEnd();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"dotnet restore failed for '{projectPath}'.{Environment.NewLine}{standardOutput}{Environment.NewLine}{standardError}");
        }
    }
}
