using AA98_AvlnCodeStudio.Base.AI.Models;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Base.AI.Services;

/// <summary>
/// Defines a tool explicitly made available to the internal AI chat.
/// </summary>
public interface IAIChatTool
{
    AIChatToolDescriptor Descriptor { get; }

    Task<string> ExecuteAsync(JsonElement arguments, CancellationToken cancellationToken);
}
