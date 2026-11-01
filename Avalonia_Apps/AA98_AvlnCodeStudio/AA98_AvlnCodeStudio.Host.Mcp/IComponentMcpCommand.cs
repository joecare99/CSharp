using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Host.Mcp;

public interface IComponentMcpCommand
{
    ComponentMcpCommandDescriptor Descriptor { get; }

    Task<string> ExecuteAsync(JsonElement arguments, CancellationToken cancellationToken);
}
