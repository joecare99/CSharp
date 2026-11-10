using System;
using System.IO;
using System.Linq;

namespace AA98_AvlnCodeStudio.Model.Workspaces;

/// <summary>
/// Captures an explicit project, configuration and optional framework for a workspace operation.
/// </summary>
public sealed class ProjectWorkflowContext
{
    private ProjectWorkflowContext(
        CSharpWorkspace workspace,
        CSharpWorkspaceProject project,
        string configuration,
        string? targetFramework)
    {
        Workspace = workspace;
        Project = project;
        Configuration = configuration;
        TargetFramework = targetFramework;
    }

    public CSharpWorkspace Workspace { get; }

    public CSharpWorkspaceProject Project { get; }

    public string Configuration { get; }

    public string? TargetFramework { get; }

    public static ProjectWorkflowContext Create(
        CSharpWorkspace workspace,
        string projectPath,
        string configuration,
        string? targetFramework = null)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        ArgumentException.ThrowIfNullOrWhiteSpace(projectPath);

        CSharpWorkspaceProject project = workspace.Projects.FirstOrDefault(candidate =>
            WorkspacePathIdentity.AreEqual(candidate.ProjectPath, projectPath))
            ?? throw new ArgumentException("The selected project does not belong to this workspace.", nameof(projectPath));
        if (!File.Exists(project.ProjectPath))
        {
            throw new FileNotFoundException("The selected project does not exist.", project.ProjectPath);
        }

        string normalizedConfiguration = ValidateValue(configuration, nameof(configuration));
        string? normalizedFramework = string.IsNullOrWhiteSpace(targetFramework)
            ? null
            : ValidateValue(targetFramework, nameof(targetFramework));
        return new ProjectWorkflowContext(workspace, project, normalizedConfiguration, normalizedFramework);
    }

    internal static string ValidateValue(string value, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
        string trimmed = value.Trim();
        if (trimmed.Length > 128 || trimmed.Any(static character =>
            !char.IsLetterOrDigit(character) && character is not ('.' or '-' or '_' or ' ')))
        {
            throw new ArgumentException("Only letters, digits, spaces, dots, underscores and hyphens are supported.", parameterName);
        }

        return trimmed;
    }
}
