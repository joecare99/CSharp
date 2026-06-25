using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Terminal.Core;

/// <summary>
/// Resolves robust shell and terminal defaults for POSIX platforms.
/// </summary>
public static class PosixTerminalEnvironment
{
    /// <summary>
    /// Resolves the shell executable that should be used for an interactive POSIX terminal session.
    /// </summary>
    /// <param name="preferredShell">The preferred shell path, typically from the <c>SHELL</c> environment variable.</param>
    /// <returns>The resolved shell executable path.</returns>
    public static string ResolveShell(string? preferredShell)
    {
        return ResolveShell(preferredShell, File.Exists);
    }

    /// <summary>
    /// Resolves the terminal type that should be used for an interactive POSIX terminal session.
    /// </summary>
    /// <param name="preferredTerm">The preferred terminal type, typically from the <c>TERM</c> environment variable.</param>
    /// <returns>The resolved terminal type.</returns>
    public static string ResolveTerm(string? preferredTerm)
    {
        return ResolveTerm(preferredTerm, Directory.Exists, File.Exists);
    }

    internal static string ResolveShell(string? preferredShell, Func<string, bool> fileExists)
    {
        ArgumentNullException.ThrowIfNull(fileExists);

        foreach (var candidate in GetShellCandidates(preferredShell))
        {
            if (fileExists(candidate))
            {
                return candidate;
            }
        }

        return "/bin/sh";
    }

    internal static string ResolveTerm(string? preferredTerm, Func<string, bool> directoryExists, Func<string, bool> fileExists)
    {
        ArgumentNullException.ThrowIfNull(directoryExists);
        ArgumentNullException.ThrowIfNull(fileExists);

        foreach (var candidate in GetTermCandidates(preferredTerm))
        {
            if (string.Equals(candidate, "dumb", StringComparison.Ordinal))
            {
                return candidate;
            }

            if (TerminfoEntryExists(candidate, directoryExists, fileExists))
            {
                return candidate;
            }
        }

        return "dumb";
    }

    private static IEnumerable<string> GetShellCandidates(string? preferredShell)
    {
        var normalizedPreferredShell = string.IsNullOrWhiteSpace(preferredShell)
            ? null
            : preferredShell.Trim();
        var isFishShell = normalizedPreferredShell is not null
            && string.Equals(Path.GetFileName(normalizedPreferredShell), "fish", StringComparison.OrdinalIgnoreCase);

        if (!isFishShell && normalizedPreferredShell is not null)
        {
            yield return normalizedPreferredShell;
        }

        yield return "/bin/bash";
        yield return "/usr/bin/bash";
        yield return "/bin/sh";
        yield return "/usr/bin/sh";

        if (isFishShell && normalizedPreferredShell is not null)
        {
            yield return normalizedPreferredShell;
        }
    }

    private static IEnumerable<string> GetTermCandidates(string? preferredTerm)
    {
        if (!string.IsNullOrWhiteSpace(preferredTerm))
        {
            yield return preferredTerm.Trim();
        }

        yield return "xterm-256color";
        yield return "xterm";
        yield return "vt100";
        yield return "ansi";
        yield return "dumb";
    }

    private static bool TerminfoEntryExists(string term, Func<string, bool> directoryExists, Func<string, bool> fileExists)
    {
        foreach (var directory in GetTerminfoDirectories())
        {
            if (!directoryExists(directory))
            {
                continue;
            }

            var firstCharacter = term[0].ToString();
            var lowerHexCharacter = ((int)term[0]).ToString("x");
            var upperHexCharacter = ((int)term[0]).ToString("X");

            if (fileExists(Path.Combine(directory, firstCharacter, term))
                || fileExists(Path.Combine(directory, lowerHexCharacter, term))
                || fileExists(Path.Combine(directory, upperHexCharacter, term)))
            {
                return true;
            }
        }

        return false;
    }

    private static IEnumerable<string> GetTerminfoDirectories()
    {
        var environmentTerminfo = Environment.GetEnvironmentVariable("TERMINFO");
        if (!string.IsNullOrWhiteSpace(environmentTerminfo))
        {
            yield return environmentTerminfo;
        }

        var environmentTerminfoDirectories = Environment.GetEnvironmentVariable("TERMINFO_DIRS");
        if (!string.IsNullOrWhiteSpace(environmentTerminfoDirectories))
        {
            foreach (var directory in environmentTerminfoDirectories
                .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(static path => !string.IsNullOrWhiteSpace(path)))
            {
                yield return directory;
            }
        }

        yield return "/etc/terminfo";
        yield return "/lib/terminfo";
        yield return "/usr/share/terminfo";
        yield return "/usr/local/share/terminfo";
        yield return "/usr/local/etc/terminfo";
    }
}