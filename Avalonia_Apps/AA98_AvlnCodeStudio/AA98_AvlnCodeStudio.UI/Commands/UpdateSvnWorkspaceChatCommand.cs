using AA98_AvlnCodeStudio.Base.Versioning.Models;
using AA98_AvlnCodeStudio.Base.Versioning.Services;
using AA98_AvlnCodeStudio.Host.Mcp;
using AA98_AvlnCodeStudio.Model.Workspaces;
using AA98_AvlnCodeStudio.Versioning.Svn.Services;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.UI.Commands;

/// <summary>
/// Updates the explicitly supplied SVN workspace through MCP and the internal AI chat.
/// </summary>
public sealed class UpdateSvnWorkspaceChatCommand(
    CSharpWorkspaceLoader workspaceLoader,
    SvnVersionControlService versionControlService,
    ISvnWorkingCopyOperations workingCopyOperations) : IComponentMcpCommand
{
    private readonly CSharpWorkspaceLoader _workspaceLoader = workspaceLoader
        ?? throw new ArgumentNullException(nameof(workspaceLoader));
    private readonly SvnVersionControlService _versionControlService = versionControlService
        ?? throw new ArgumentNullException(nameof(versionControlService));
    private readonly ISvnWorkingCopyOperations _workingCopyOperations = workingCopyOperations
        ?? throw new ArgumentNullException(nameof(workingCopyOperations));

    public ComponentMcpCommandDescriptor Descriptor { get; } = new(
        "workspace.svn.update",
        "Update SVN workspace",
        "Updates the supplied C# workspace from its Subversion repository. Requires explicit approval.",
        """{"type":"object","properties":{"workspacePath":{"type":"string"}},"required":["workspacePath"],"additionalProperties":false}""",
        isReadOnly: false);

    public async Task<string> ExecuteAsync(JsonElement arguments, CancellationToken cancellationToken)
    {
        string workspacePath = arguments.GetProperty("workspacePath").GetString()
            ?? throw new ArgumentException("The workspace path cannot be null.", nameof(arguments));
        CSharpWorkspace workspace = _workspaceLoader.Load(workspacePath);
        VersionControlStatus status = await _versionControlService.GetStatusAsync(
            new VersionControlStatusRequest
            {
                RepositoryContextPath = workspace.RootPath,
                IncludeChanges = false,
                IncludeCapabilities = true,
            },
            cancellationToken).ConfigureAwait(false);
        if (!status.IsRepositoryRootDiscovered
            || !status.Capabilities.Contains(VersionControlCapability.UpdateWorkingCopy))
        {
            throw new ArgumentException(
                "The supplied workspace is not inside a supported SVN working copy.",
                nameof(arguments));
        }

        string output = await _workingCopyOperations.UpdateAsync(
            workspace.RootPath,
            cancellationToken).ConfigureAwait(false);
        return JsonSerializer.Serialize(new
        {
            workspace.RootPath,
            status.RepositoryRootPath,
            Output = output,
        });
    }
}
