using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Host.Mcp;

public interface IComponentScreenshotSource
{
    string ComponentId { get; }

    Task<byte[]> CapturePngAsync(CancellationToken cancellationToken);
}
