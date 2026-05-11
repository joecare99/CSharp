using System;
using System.Threading;
using System.Threading.Tasks;
using Ollama.Tools.Abstractions;

namespace Ollama.Tools;

/// <summary>
/// Resolves and invokes registered tools.
/// </summary>
public sealed class OllamaToolOrchestrator
{
    private readonly IOllamaToolRegistry _toolRegistry;

    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaToolOrchestrator"/> class.
    /// </summary>
    /// <param name="toolRegistry">The tool registry.</param>
    public OllamaToolOrchestrator(IOllamaToolRegistry toolRegistry)
    {
        _toolRegistry = toolRegistry ?? throw new ArgumentNullException(nameof(toolRegistry));
    }

    /// <summary>
    /// Executes the requested tool if it is registered.
    /// </summary>
    /// <param name="toolCall">The requested tool call.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The invocation outcome.</returns>
    public async Task<OllamaToolInvocationResult> ExecuteAsync(OllamaToolCall toolCall, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(toolCall);

        if (!_toolRegistry.TryGetTool(toolCall.ToolName, out IOllamaTool? tool) || tool is null)
        {
            return new OllamaToolInvocationResult
            {
                ToolName = toolCall.ToolName,
                Input = toolCall.Input,
                Success = false,
                Error = $"Tool '{toolCall.ToolName}' is not registered.",
            };
        }

        OllamaToolValidationResult validationResult = tool.Validate(toolCall.Input);
        if (!validationResult.IsValid)
        {
            return new OllamaToolInvocationResult
            {
                ToolName = toolCall.ToolName,
                Input = toolCall.Input,
                Success = false,
                Error = string.Join(" ", validationResult.Errors),
            };
        }

        OllamaToolResult result = await tool.ExecuteAsync(toolCall.Input, cancellationToken);
        return new OllamaToolInvocationResult
        {
            ToolName = toolCall.ToolName,
            Input = toolCall.Input,
            Success = result.Success,
            Output = result.Output,
            Error = result.Success ? null : result.Output,
        };
    }
}
