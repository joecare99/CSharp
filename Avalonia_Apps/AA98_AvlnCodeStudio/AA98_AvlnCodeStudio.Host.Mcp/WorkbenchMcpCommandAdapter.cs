using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Host.Mcp;

internal sealed class WorkbenchMcpCommandAdapter<TCommand>(TCommand command) : IComponentMcpCommand
    where TCommand : class, IWorkbenchMcpCommandContribution
{
    private readonly TCommand _command = command ?? throw new ArgumentNullException(nameof(command));

    public ComponentMcpCommandDescriptor Descriptor => _command.McpDescriptor;

    public Task<string> ExecuteAsync(JsonElement arguments, CancellationToken cancellationToken)
    {
        return _command.ExecuteMcpAsync(arguments, cancellationToken);
    }
}
