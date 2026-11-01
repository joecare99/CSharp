using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Host.Mcp;

public sealed class DelegateComponentMcpCommand(
    ComponentMcpCommandDescriptor descriptor,
    Func<JsonElement, CancellationToken, Task<string>> execute) : IComponentMcpCommand
{
    public ComponentMcpCommandDescriptor Descriptor { get; } =
        descriptor ?? throw new ArgumentNullException(nameof(descriptor));

    public Task<string> ExecuteAsync(JsonElement arguments, CancellationToken cancellationToken)
    {
        return execute(arguments, cancellationToken);
    }
}
