using System;
using System.Collections.Generic;
using System.Linq;

namespace AA98_AvlnCodeStudio.Base.AI.Models;

/// <summary>
/// Represents a single provider-agnostic AI interaction message.
/// </summary>
public sealed class AIMessage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AIMessage"/> class.
    /// </summary>
    /// <param name="role">The message role.</param>
    /// <param name="content">The message content.</param>
    public AIMessage(
        AIMessageRole role,
        string content,
        string? toolCallId = null,
        string? toolName = null,
        IEnumerable<AIChatToolCall>? toolCalls = null)
    {
        ArgumentNullException.ThrowIfNull(content);
        if (!Enum.IsDefined(role))
        {
            throw new ArgumentOutOfRangeException(nameof(role));
        }

        if (role != AIMessageRole.Assistant && string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("Message content is required.", nameof(content));
        }

        if (role == AIMessageRole.Tool
            && (string.IsNullOrWhiteSpace(toolCallId) || string.IsNullOrWhiteSpace(toolName)))
        {
            throw new ArgumentException("Tool messages require a tool call identifier and name.");
        }

        if (role != AIMessageRole.Tool && (toolCallId is not null || toolName is not null))
        {
            throw new ArgumentException("Only tool messages can include a tool call identifier or name.");
        }

        AIChatToolCall[] calls = toolCalls?.ToArray() ?? [];
        if (calls.Any(static call => call is null))
        {
            throw new ArgumentException("AI messages cannot contain null tool calls.", nameof(toolCalls));
        }

        if (calls.Length > 0 && role != AIMessageRole.Assistant)
        {
            throw new ArgumentException("Only assistant messages can include tool calls.", nameof(toolCalls));
        }

        Role = role;
        Content = content;
        ToolCallId = toolCallId;
        ToolName = toolName;
        ToolCalls = calls;
    }

    /// <summary>
    /// Gets the message role.
    /// </summary>
    public AIMessageRole Role { get; }

    /// <summary>
    /// Gets the message content.
    /// </summary>
    public string Content { get; }

    public string? ToolCallId { get; }

    public string? ToolName { get; }

    public IReadOnlyList<AIChatToolCall> ToolCalls { get; }
}