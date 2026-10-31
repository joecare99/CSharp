using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AA98_AvlnCodeStudio.Model.Workspaces;

/// <summary>
/// Describes a C# workspace loaded from a solution or project file.
/// </summary>
public sealed class CSharpWorkspace
{
    private readonly CSharpWorkspaceProject[] _projects;

    internal CSharpWorkspace(string entryPath, IEnumerable<CSharpWorkspaceProject> projects)
    {
        EntryPath = Path.GetFullPath(entryPath);
        RootPath = Path.GetDirectoryName(EntryPath) ?? Directory.GetCurrentDirectory();
        _projects = projects.ToArray();
    }

    /// <summary>
    /// Gets the solution or project file used to open this workspace.
    /// </summary>
    public string EntryPath { get; }

    /// <summary>
    /// Gets the directory containing the workspace entry file.
    /// </summary>
    public string RootPath { get; }

    /// <summary>
    /// Gets the C# projects discovered in the workspace.
    /// </summary>
    public IReadOnlyList<CSharpWorkspaceProject> Projects => _projects;
}

/// <summary>
/// Describes a C# project entry in a workspace.
/// </summary>
public sealed class CSharpWorkspaceProject
{
    internal CSharpWorkspaceProject(string projectPath)
    {
        ProjectPath = Path.GetFullPath(projectPath);
        Name = Path.GetFileNameWithoutExtension(ProjectPath);
    }

    /// <summary>
    /// Gets the normalized project file path.
    /// </summary>
    public string ProjectPath { get; }

    /// <summary>
    /// Gets the project name derived from its file name.
    /// </summary>
    public string Name { get; }
}
