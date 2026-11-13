using System;
using System.IO;
using System.Linq;

namespace AA98_AvlnCodeStudio.Model.Workspaces;

/// <summary>Finds the unique closest existing project containing a document in a workspace.</summary>
public static class AxamlResourceProjectResolver
{
    public static string? FindProjectRoot(CSharpWorkspace? workspace, string? workspaceRoot, string? documentPath)
    {
        if (workspace is null || string.IsNullOrWhiteSpace(workspaceRoot)
            || string.IsNullOrWhiteSpace(documentPath) || !IsWithin(workspaceRoot, documentPath))
        {
            return null;
        }

        string[] candidates = workspace.Projects
            .Where(project => File.Exists(project.ProjectPath))
            .Select(project => Path.GetDirectoryName(project.ProjectPath))
            .Where(directory => directory is not null
                && IsWithin(workspaceRoot, directory)
                && IsWithin(directory, documentPath))
            .Select(directory => directory!)
            .OrderByDescending(directory => directory.Length)
            .ToArray();
        return candidates.Length == 0
            || candidates.Length > 1 && candidates[0].Length == candidates[1].Length
                ? null
                : candidates[0];
    }

    private static bool IsWithin(string root, string path)
    {
        string relative = Path.GetRelativePath(root, path);
        return !Path.IsPathRooted(relative)
            && relative != ".."
            && !relative.StartsWith($"..{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
            && !relative.StartsWith($"..{Path.AltDirectorySeparatorChar}", StringComparison.Ordinal);
    }
}
