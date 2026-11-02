using AA98_AvlnCodeStudio.Base.AI.Models;
using AA98_AvlnCodeStudio.Base.AI.Services;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Host.Mcp;

internal sealed class WorkbenchAIChatToolAdapter<TCommand>(TCommand command) : IAIChatTool
    where TCommand : class, IWorkbenchMcpCommandContribution
{
    private readonly TCommand _command = command ?? throw new ArgumentNullException(nameof(command));

    public AIChatToolDescriptor Descriptor => new(
        _command.McpDescriptor.CommandId,
        _command.McpDescriptor.Description,
        _command.McpDescriptor.InputSchema.GetRawText(),
        requiresConfirmation: !_command.McpDescriptor.IsReadOnly);

    public Task<string> ExecuteAsync(JsonElement arguments, CancellationToken cancellationToken)
    {
        return _command.ExecuteMcpAsync(arguments, cancellationToken);
    }
}
