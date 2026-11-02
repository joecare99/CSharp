using System;
using System.Text.Json;

namespace AA98_AvlnCodeStudio.Base.AI.Models;

/// <summary>
/// Represents a structured tool call returned by an AI provider.
/// </summary>
public sealed class AIChatToolCall
{
    public AIChatToolCall(string callId, string name, JsonElement arguments)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(callId);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (arguments.ValueKind != JsonValueKind.Object)
        {
            throw new ArgumentException("AI tool call arguments must be a JSON object.", nameof(arguments));
        }

        CallId = callId;
        Name = name;
        Arguments = arguments.Clone();
    }

    public string CallId { get; }

    public string Name { get; }

    public JsonElement Arguments { get; }
}
