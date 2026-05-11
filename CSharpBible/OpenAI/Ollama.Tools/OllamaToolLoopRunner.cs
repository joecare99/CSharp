using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Ollama.Client;
using Ollama.Tools.Abstractions;

namespace Ollama.Tools;

/// <summary>
/// Runs a simple host-controlled tool loop using the chat client.
/// </summary>
public sealed class OllamaToolLoopRunner
{
    private readonly IOllamaToolChatRunner _chatRunner;
    private readonly OllamaToolOrchestrator _toolOrchestrator;
    private readonly IOllamaToolRegistry _toolRegistry;

    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaToolLoopRunner"/> class.
    /// </summary>
    /// <param name="chatRunner">The chat runner used to interpret tool requests.</param>
    /// <param name="toolRegistry">The registered tools.</param>
    /// <param name="toolOrchestrator">The tool orchestrator.</param>
    public OllamaToolLoopRunner(IOllamaToolChatRunner chatRunner, IOllamaToolRegistry toolRegistry, OllamaToolOrchestrator toolOrchestrator)
    {
        _chatRunner = chatRunner ?? throw new ArgumentNullException(nameof(chatRunner));
        _toolRegistry = toolRegistry ?? throw new ArgumentNullException(nameof(toolRegistry));
        _toolOrchestrator = toolOrchestrator ?? throw new ArgumentNullException(nameof(toolOrchestrator));
    }

    /// <summary>
    /// Executes one tool-loop round by asking the model for a tool call and invoking it.
    /// </summary>
    /// <param name="userPrompt">The original user prompt.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The tool invocation outcome.</returns>
    public async Task<OllamaToolInvocationResult> RunAsync(string userPrompt, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userPrompt);

        string instructions = OllamaToolPromptBuilder.BuildToolInstructions(_toolRegistry);
        Ollama.Client.ChatCompletionOptions options = new()
        {
            Messages =
            [
                new Ollama.Client.Models.OllamaClientChatMessage
                {
                    Role = "system",
                    Content = instructions,
                },
                new Ollama.Client.Models.OllamaClientChatMessage
                {
                    Role = "user",
                    Content = userPrompt,
                },
            ],
        };

        Ollama.Client.Models.OllamaChatCompletion completion = await _chatRunner.CompleteChatAsync(options, cancellationToken);
        OllamaToolCall? toolCall = JsonSerializer.Deserialize<OllamaToolCall>(completion.Content);
        if (toolCall is null || string.IsNullOrWhiteSpace(toolCall.ToolName))
        {
            return new OllamaToolInvocationResult
            {
                ToolName = string.Empty,
                Input = userPrompt,
                Success = false,
                Error = "The model did not return a valid tool call JSON object.",
            };
        }

        return await _toolOrchestrator.ExecuteAsync(toolCall, cancellationToken);
    }
}
