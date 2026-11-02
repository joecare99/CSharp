using AA98_AvlnCodeStudio.Base.Components.Commands;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Host.Mcp;

/// <summary>
/// Defines a workbench command that is explicitly approved for MCP export.
/// </summary>
public interface IWorkbenchMcpCommandContribution : IWorkbenchCommandContribution
{
    /// <summary>
    /// Gets the MCP-specific metadata and input schema for the exported command.
    /// </summary>
    ComponentMcpCommandDescriptor McpDescriptor { get; }

    /// <summary>
    /// Executes the command through its explicitly defined MCP argument contract.
    /// </summary>
    Task<string> ExecuteMcpAsync(JsonElement arguments, CancellationToken cancellationToken);
}
