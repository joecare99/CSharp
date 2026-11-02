using AA98_AvlnCodeStudio.Base.Versioning.Models;
using AA98_AvlnCodeStudio.Base.Versioning.Services;
using AA98_AvlnCodeStudio.Host.Mcp;
using AA98_AvlnCodeStudio.Model.Workspaces;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.UI.Commands;

/// <summary>
/// Inspects the explicitly supplied version-controlled workspace through MCP and the internal AI chat.
/// </summary>
public sealed class InspectVersionControlWorkspaceChatCommand(
    CSharpWorkspaceLoader workspaceLoader,
    IVersionControlService versionControlService) : IComponentMcpCommand
{
    private readonly CSharpWorkspaceLoader _workspaceLoader = workspaceLoader
        ?? throw new ArgumentNullException(nameof(workspaceLoader));
    private readonly IVersionControlService _versionControlService = versionControlService
        ?? throw new ArgumentNullException(nameof(versionControlService));

    public ComponentMcpCommandDescriptor Descriptor { get; } = new(
        "workspace.versioning.status",
        "Inspect workspace version-control status",
        "Reports the repository reference and local changes for the supplied C# workspace. Read-only.",
        """{"type":"object","properties":{"workspacePath":{"type":"string"}},"required":["workspacePath"],"additionalProperties":false}""",
        isReadOnly: true);

    public async Task<string> ExecuteAsync(JsonElement arguments, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string workspacePath = arguments.GetProperty("workspacePath").GetString()
            ?? throw new ArgumentException("The workspace path cannot be null.", nameof(arguments));
        CSharpWorkspace workspace = _workspaceLoader.Load(workspacePath);
        VersionControlStatus status = await _versionControlService.GetStatusAsync(
            new VersionControlStatusRequest
            {
                RepositoryContextPath = workspace.RootPath,
                IncludeChanges = true,
                IncludeCapabilities = true,
            },
            cancellationToken).ConfigureAwait(false);
        if (!status.IsRepositoryRootDiscovered
            || !status.Capabilities.Contains(VersionControlCapability.InspectStatus))
        {
            throw new ArgumentException(
                "The supplied workspace is not inside a supported version-control working copy.",
                nameof(arguments));
        }

        return JsonSerializer.Serialize(new
        {
            status.RepositoryRootPath,
            status.RepositoryName,
            status.ActiveReferenceName,
            ActiveReferenceKind = status.ActiveReferenceKind.ToString(),
            status.IsDetached,
            status.HasLocalChanges,
            Capabilities = status.Capabilities.Select(static capability => capability.ToString()),
            Changes = status.Changes.Select(static change => new
            {
                change.Path,
                change.PreviousPath,
                ChangeKind = change.ChangeKind.ToString(),
                change.IsStaged,
                change.IsIgnored,
            }),
        });
    }
}
