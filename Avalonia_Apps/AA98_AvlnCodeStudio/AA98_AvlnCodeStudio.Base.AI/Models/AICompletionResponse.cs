using System;
using System.Collections.Generic;
using System.Linq;

namespace AA98_AvlnCodeStudio.Base.AI.Models;

/// <summary>
/// Represents a provider-agnostic text generation response.
/// </summary>
public sealed class AICompletionResponse
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AICompletionResponse"/> class.
    /// </summary>
    /// <param name="message">The generated assistant message.</param>
    public AICompletionResponse(AIMessage message, IEnumerable<AIChatToolCall>? toolCalls = null)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
        if (Message.Role != AIMessageRole.Assistant)
        {
            throw new ArgumentException("AI completion responses must contain an assistant message.", nameof(message));
        }

        AIChatToolCall[] calls = toolCalls?.ToArray() ?? [];
        if (calls.Any(static call => call is null))
        {
            throw new ArgumentException("AI completion tool calls cannot contain null entries.", nameof(toolCalls));
        }

        ToolCalls = calls;
    }

    /// <summary>
    /// Gets the generated assistant message.
    /// </summary>
    public AIMessage Message { get; }

    /// <summary>
    /// Gets the tool calls requested by the assistant.
    /// </summary>
    public IReadOnlyList<AIChatToolCall> ToolCalls { get; }
}