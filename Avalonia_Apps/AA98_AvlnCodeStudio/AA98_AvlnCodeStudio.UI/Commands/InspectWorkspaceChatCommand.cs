using AA98_AvlnCodeStudio.Base.Building.Models;
using AA98_AvlnCodeStudio.Base.Building.Services;
using AA98_AvlnCodeStudio.Host.Mcp;
using AA98_AvlnCodeStudio.Model.Workspaces;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.UI.Commands;

/// <summary>
/// Exposes read-only workspace project discovery to MCP and the internal AI chat.
/// </summary>
public sealed class InspectWorkspaceChatCommand(
    CSharpWorkspaceLoader workspaceLoader,
    ICodeStudioBuilderService builderService) : IComponentMcpCommand
{
    private readonly CSharpWorkspaceLoader _workspaceLoader = workspaceLoader
        ?? throw new ArgumentNullException(nameof(workspaceLoader));
    private readonly ICodeStudioBuilderService _builderService = builderService
        ?? throw new ArgumentNullException(nameof(builderService));

    public ComponentMcpCommandDescriptor Descriptor { get; } = new(
        "workspace.inspect",
        "Inspect C# workspace",
        "Inspects project metadata and diagnostics for the C# projects declared by a solution or project file. Does not build.",
        """{"type":"object","properties":{"workspacePath":{"type":"string"}},"required":["workspacePath"],"additionalProperties":false}""",
        isReadOnly: true);

    public async Task<string> ExecuteAsync(JsonElement arguments, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string path = arguments.GetProperty("workspacePath").GetString()
            ?? throw new ArgumentException("The workspace path cannot be null.", nameof(arguments));
        CSharpWorkspace workspace = _workspaceLoader.Load(path);
        var projects = new System.Collections.Generic.List<object>(workspace.Projects.Count);
        foreach (CSharpWorkspaceProject project in workspace.Projects)
        {
            cancellationToken.ThrowIfCancellationRequested();
            BuilderProjectInspectionResult inspection = await _builderService.InspectProjectAsync(
                new BuilderProjectInspectionRequest
                {
                    WorkspaceRootPath = workspace.RootPath,
                    ProjectPath = project.ProjectPath,
                },
                cancellationToken).ConfigureAwait(false);
            projects.Add(new
            {
                project.Name,
                project.ProjectPath,
                inspection.TargetFramework,
                inspection.IsTestProject,
                Diagnostics = inspection.Diagnostics.Select(static diagnostic => new
                {
                    diagnostic.Code,
                    diagnostic.Message,
                    Severity = diagnostic.Severity.ToString(),
                    diagnostic.FilePath,
                    diagnostic.LineNumber,
                    diagnostic.ColumnNumber,
                }),
            });
        }

        return JsonSerializer.Serialize(new
        {
            workspace.EntryPath,
            workspace.RootPath,
            Projects = projects,
        });
    }
}
