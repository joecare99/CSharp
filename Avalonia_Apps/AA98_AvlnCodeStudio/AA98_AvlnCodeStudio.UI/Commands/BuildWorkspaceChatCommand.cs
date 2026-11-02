using AA98_AvlnCodeStudio.Base.Building.Models;
using AA98_AvlnCodeStudio.Base.Building.Services;
using AA98_AvlnCodeStudio.Host.Mcp;
using AA98_AvlnCodeStudio.Model.Workspaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.UI.Commands;

/// <summary>
/// Builds one or all projects contained in the explicitly supplied C# workspace.
/// </summary>
public sealed class BuildWorkspaceChatCommand(
    CSharpWorkspaceLoader workspaceLoader,
    ICodeStudioBuilderService builderService) : IComponentMcpCommand
{
    public ComponentMcpCommandDescriptor Descriptor { get; } = new(
        "workspace.build",
        "Build C# workspace",
        "Builds one project in the supplied C# workspace, or all projects when projectPath is omitted. Requires explicit approval.",
        """{"type":"object","properties":{"workspacePath":{"type":"string"},"projectPath":{"type":"string"},"configuration":{"type":"string"},"targetFramework":{"type":"string"}},"required":["workspacePath"],"additionalProperties":false}""",
        isReadOnly: false);

    public async Task<string> ExecuteAsync(JsonElement arguments, CancellationToken cancellationToken)
    {
        string workspacePath = arguments.GetProperty("workspacePath").GetString()
            ?? throw new ArgumentException("The workspace path cannot be null.", nameof(arguments));
        CSharpWorkspace workspace = workspaceLoader.Load(workspacePath);
        CSharpWorkspaceProject[] projects;
        if (arguments.TryGetProperty("projectPath", out JsonElement projectPathElement))
        {
            string projectPath = projectPathElement.GetString()
                ?? throw new ArgumentException("The project path cannot be null.", nameof(arguments));
            string normalizedProjectPath = Path.GetFullPath(projectPath);
            CSharpWorkspaceProject? project = workspace.Projects.FirstOrDefault(candidate =>
                string.Equals(
                    Path.GetFullPath(candidate.ProjectPath),
                    normalizedProjectPath,
                    OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));
            if (project is null)
            {
                throw new ArgumentException("The requested project is not part of the supplied workspace.", nameof(arguments));
            }

            projects = [project];
        }
        else
        {
            projects = workspace.Projects.ToArray();
        }

        List<ProjectBuildResponse> results = new(projects.Length);
        foreach (CSharpWorkspaceProject project in projects)
        {
            cancellationToken.ThrowIfCancellationRequested();
            BuilderBuildResult result = await builderService.BuildProjectAsync(
                new BuilderBuildRequest
                {
                    WorkspaceRootPath = workspace.RootPath,
                    ProjectPath = project.ProjectPath,
                    Configuration = GetOptionalString(arguments, "configuration"),
                    TargetFramework = GetOptionalString(arguments, "targetFramework"),
                },
                cancellationToken).ConfigureAwait(false);
            results.Add(new ProjectBuildResponse(
                result.ProjectPath,
                result.Succeeded,
                result.Diagnostics.Select(static diagnostic => new BuildDiagnosticResponse(
                    diagnostic.Code,
                    diagnostic.Message,
                    diagnostic.Severity.ToString(),
                    diagnostic.FilePath,
                    diagnostic.LineNumber,
                    diagnostic.ColumnNumber)).ToArray()));
        }

        return results.Count == 1
            ? JsonSerializer.Serialize(results[0])
            : JsonSerializer.Serialize(new
            {
                WorkspacePath = workspace.EntryPath,
                Projects = results,
                Succeeded = results.All(static result => result.Succeeded),
            });
    }

    private static string? GetOptionalString(JsonElement arguments, string propertyName)
    {
        return arguments.TryGetProperty(propertyName, out JsonElement value)
            ? value.GetString()
            : null;
    }

    private sealed record ProjectBuildResponse(
        string ProjectPath,
        bool Succeeded,
        IReadOnlyList<BuildDiagnosticResponse> Diagnostics);

    private sealed record BuildDiagnosticResponse(
        string Code,
        string Message,
        string Severity,
        string? FilePath,
        int? LineNumber,
        int? ColumnNumber);
}
