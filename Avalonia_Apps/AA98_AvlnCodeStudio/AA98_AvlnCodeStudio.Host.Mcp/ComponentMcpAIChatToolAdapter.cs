using AA98_AvlnCodeStudio.Base.AI.Models;
using AA98_AvlnCodeStudio.Base.AI.Services;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Host.Mcp;

internal sealed class ComponentMcpAIChatToolAdapter<TCommand>(TCommand command) : IAIChatTool
    where TCommand : class, IComponentMcpCommand
{
    private readonly TCommand _command = command ?? throw new ArgumentNullException(nameof(command));

    public AIChatToolDescriptor Descriptor => new(
        _command.Descriptor.CommandId,
        _command.Descriptor.Description,
        _command.Descriptor.InputSchema.GetRawText(),
        requiresConfirmation: !_command.Descriptor.IsReadOnly);

    public Task<string> ExecuteAsync(JsonElement arguments, CancellationToken cancellationToken)
    {
        return _command.ExecuteAsync(arguments, cancellationToken);
    }
}
