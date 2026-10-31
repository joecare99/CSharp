using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace AA98_AvlnCodeStudio.Model.Workspaces;

/// <summary>
/// Loads C# project references from solution and project files.
/// </summary>
public sealed class CSharpWorkspaceLoader
{
    private static readonly Regex s_solutionProjectPattern = new(
        "^Project\\(\"[^\"]+\"\\)\\s*=\\s*\"[^\"]*\",\\s*\"(?<path>[^\"]+\\.csproj)\"",
        RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

    /// <summary>
    /// Loads a workspace from a Visual Studio solution or C# project file.
    /// </summary>
    public CSharpWorkspace Load(string entryPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entryPath);

        string normalizedEntryPath = Path.GetFullPath(entryPath);
        if (!File.Exists(normalizedEntryPath))
        {
            throw new FileNotFoundException("The workspace entry file does not exist.", normalizedEntryPath);
        }

        string extension = Path.GetExtension(normalizedEntryPath);
        IEnumerable<string> projectPaths = extension.ToLowerInvariant() switch
        {
            ".csproj" => [normalizedEntryPath],
            ".sln" => LoadVisualStudioSolution(normalizedEntryPath),
            ".slnx" => LoadXmlSolution(normalizedEntryPath),
            _ => throw new NotSupportedException($"Workspace entry type '{extension}' is not supported."),
        };

        CSharpWorkspaceProject[] projects = projectPaths
            .Select(path => NormalizePath(path, Path.GetDirectoryName(normalizedEntryPath)!))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(static path => new CSharpWorkspaceProject(path))
            .ToArray();

        if (projects.Length == 0)
        {
            throw new InvalidDataException($"No C# project entries were found in '{normalizedEntryPath}'.");
        }

        return new CSharpWorkspace(normalizedEntryPath, projects);
    }

    private static IEnumerable<string> LoadVisualStudioSolution(string solutionPath)
    {
        foreach (string line in File.ReadLines(solutionPath))
        {
            Match match = s_solutionProjectPattern.Match(line);
            if (match.Success)
            {
                yield return match.Groups["path"].Value;
            }
        }
    }

    private static IEnumerable<string> LoadXmlSolution(string solutionPath)
    {
        XDocument solution = XDocument.Load(solutionPath);
        return solution
            .Descendants()
            .Where(static element => element.Name.LocalName == "Project")
            .Select(static element => (string?)element.Attribute("Path"))
            .Where(static path => !string.IsNullOrWhiteSpace(path)
                && string.Equals(Path.GetExtension(path), ".csproj", StringComparison.OrdinalIgnoreCase))
            .Select(static path => path!);
    }

    private static string NormalizePath(string path, string workspaceRoot)
    {
        string platformPath = path
            .Replace('\\', Path.DirectorySeparatorChar)
            .Replace('/', Path.DirectorySeparatorChar);
        return Path.GetFullPath(Path.IsPathRooted(platformPath)
            ? platformPath
            : Path.Combine(workspaceRoot, platformPath));
    }
}
