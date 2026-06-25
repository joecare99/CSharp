using System;
using System.IO;

namespace Terminal.Core;

/// <summary>
/// Creates ready-to-use terminal session options for the current operating system.
/// </summary>
public static class TerminalShellOptions
{
    /// <summary>
    /// Creates session options for the default interactive shell of the current platform.
    /// </summary>
    /// <param name="workingDirectory">The working directory to use. When omitted, the current directory is used.</param>
    /// <returns>A configured terminal session options instance.</returns>
    /// <exception cref="PlatformNotSupportedException">Thrown when the current platform is not supported.</exception>
    public static TerminalSessionOptions CreateDefault(string? workingDirectory = null)
    {
        var resolvedWorkingDirectory = string.IsNullOrWhiteSpace(workingDirectory)
            ? Environment.CurrentDirectory
            : Path.GetFullPath(workingDirectory);

        if (OperatingSystem.IsWindows())
        {
            return new TerminalSessionOptions
            {
                FileName = Environment.GetEnvironmentVariable("COMSPEC") ?? "cmd.exe",
                WorkingDirectory = resolvedWorkingDirectory,
                InitialSize = new TerminalSize(80, 25)
            };
        }

        if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
        {
            var resolvedShell = PosixTerminalEnvironment.ResolveShell(Environment.GetEnvironmentVariable("SHELL"));

            return new TerminalSessionOptions
            {
                FileName = resolvedShell,
                Arguments = "-i",
                WorkingDirectory = resolvedWorkingDirectory,
                InitialSize = new TerminalSize(80, 25)
            };
        }

        throw new PlatformNotSupportedException("No default terminal shell is configured for the current operating system.");
    }
}
