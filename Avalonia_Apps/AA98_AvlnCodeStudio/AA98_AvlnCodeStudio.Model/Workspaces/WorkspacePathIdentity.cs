using System;
using System.IO;

namespace AA98_AvlnCodeStudio.Model.Workspaces;

/// <summary>
/// Provides normalized path identity using the current platform's path rules.
/// </summary>
public static class WorkspacePathIdentity
{
    public static StringComparer Comparer { get; } = OperatingSystem.IsWindows()
        ? StringComparer.OrdinalIgnoreCase
        : StringComparer.Ordinal;

    public static string Normalize(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        return Path.TrimEndingDirectorySeparator(Path.GetFullPath(path));
    }

    public static bool AreEqual(string firstPath, string secondPath)
    {
        return Comparer.Equals(Normalize(firstPath), Normalize(secondPath));
    }
}
