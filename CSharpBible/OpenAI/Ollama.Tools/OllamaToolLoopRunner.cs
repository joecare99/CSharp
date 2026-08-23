using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Ollama.Client.Interfaces;
using Ollama.Client.Models;
using Ollama.Client.Services;
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
        Ollama.Client.Models.ChatCompletionOptions options = new()
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
            Tools = BuildChatTools(),
        };

        Ollama.Client.Models.OllamaChatCompletion completion = await _chatRunner.CompleteChatAsync(options, cancellationToken);
        OllamaToolCall? toolCall = TryGetNativeToolCall(completion.ToolCalls);
        if (toolCall is null && (!TryParseToolCall(completion.Content, out toolCall) || toolCall is null))
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

    /// <summary>
    /// Runs repeated model/tool turns and reinjects each tool result into the conversation.
    /// </summary>
    public async Task<OllamaToolLoopResult> RunToCompletionAsync(
        string userPrompt,
        int maximumIterations = 8,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userPrompt);
        if (maximumIterations <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maximumIterations));
        }

        string instructions = OllamaToolPromptBuilder.BuildToolInstructions(_toolRegistry);
        List<Ollama.Client.Models.OllamaClientChatMessage> messages =
        [
            new()
            {
                Role = "system",
                Content = instructions,
            },
            new()
            {
                Role = "user",
                Content = userPrompt,
            },
        ];
        List<OllamaToolInvocationResult> invocations = [];

        for (int iteration = 0; iteration < maximumIterations; iteration++)
        {
            Ollama.Client.Models.OllamaChatCompletion completion = await _chatRunner.CompleteChatAsync(
                new Ollama.Client.Models.ChatCompletionOptions { Messages = messages, Tools = BuildChatTools() },
                cancellationToken);
            OllamaToolCall? toolCall = TryGetNativeToolCall(completion.ToolCalls);
            if (toolCall is null && (!TryParseToolCall(completion.Content, out toolCall) || toolCall is null))
            {
                return new OllamaToolLoopResult
                {
                    FinalResponse = completion.Content,
                    Invocations = invocations,
                    Completed = true,
                };
            }

            OllamaToolInvocationResult invocation = await _toolOrchestrator.ExecuteAsync(toolCall, cancellationToken);
            invocations.Add(invocation);
            messages.Add(new Ollama.Client.Models.OllamaClientChatMessage
            {
                Role = "assistant",
                Content = completion.Content,
            });
            messages.Add(new Ollama.Client.Models.OllamaClientChatMessage
            {
                Role = "tool",
                Content = invocation.Success
                    ? invocation.Output
                    : $"Tool execution failed: {invocation.Error}",
            });
        }

        return new OllamaToolLoopResult
        {
            FinalResponse = "The tool loop reached its iteration limit before producing a final response.",
            Invocations = invocations,
            Completed = false,
        };
    }

    private static bool TryParseToolCall(string content, out OllamaToolCall? toolCall)
    {
        try
        {
            toolCall = JsonSerializer.Deserialize<OllamaToolCall>(content);
            return toolCall is not null && !string.IsNullOrWhiteSpace(toolCall.ToolName);
        }
        catch (JsonException)
        {
            toolCall = null;
            return false;
        }
    }

    private IReadOnlyList<Ollama.Client.Models.OllamaChatTool> BuildChatTools()
        => _toolRegistry.GetDescriptors().Select(static descriptor => new Ollama.Client.Models.OllamaChatTool
        {
            Name = descriptor.Name,
            Description = descriptor.Description,
            Parameters = descriptor.Schema.Parameters.ToDictionary(
                static parameter => parameter.Name,
                static parameter => new Ollama.Client.Models.OllamaChatToolParameter
                {
                    Type = parameter.Type,
                    Description = parameter.Description,
                    Required = parameter.Required,
                }),
        }).ToArray();

    private static OllamaToolCall? TryGetNativeToolCall(IReadOnlyList<Ollama.Client.Models.OllamaChatToolCall> toolCalls)
    {
        Ollama.Client.Models.OllamaChatToolCall? call = toolCalls.FirstOrDefault();
        return call is null
            ? null
            : new OllamaToolCall
            {
                ToolName = call.Name,
                Input = call.Arguments,
            };
    }
}
