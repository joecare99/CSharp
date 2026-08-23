using System;
using System.Collections.Generic;
using System.Text;
using Ollama.Client.Models;

namespace Ollama.Client.Services;

/// <summary>
/// Accumulates streamed <see cref="OllamaStreamingChatUpdate"/> fragments into a buffered <see cref="OllamaChatCompletion"/>.
/// </summary>
public sealed class OllamaStreamingChatAggregator
{
    private readonly StringBuilder _content = new();
    private readonly List<string> _thinking = [];
    private readonly List<OllamaChatToolCall> _toolCalls = [];
    private readonly Action<string>? _onThinkingFragment;

    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaStreamingChatAggregator"/> class.
    /// </summary>
    /// <param name="onThinkingFragment">Optional callback invoked for every non-empty thinking fragment as it arrives.</param>
    public OllamaStreamingChatAggregator(Action<string>? onThinkingFragment = null)
    {
        _onThinkingFragment = onThinkingFragment;
    }

    /// <summary>
    /// Adds one streamed update to the aggregation.
    /// </summary>
    /// <param name="update">The streamed update.</param>
    public void Add(OllamaStreamingChatUpdate update)
    {
        ArgumentNullException.ThrowIfNull(update);

        if (!string.IsNullOrWhiteSpace(update.Thinking))
        {
            _thinking.Add(update.Thinking!);
            _onThinkingFragment?.Invoke(update.Thinking!);
        }

        if (!string.IsNullOrWhiteSpace(update.Content))
        {
            _content.Append(update.Content);
        }

        _toolCalls.AddRange(update.ToolCalls);
    }

    /// <summary>
    /// Builds the buffered completion from all updates added so far.
    /// </summary>
    /// <returns>The buffered chat completion.</returns>
    public OllamaChatCompletion ToCompletion()
        => new()
        {
            Content = _content.ToString(),
            Thinking = _thinking,
            ToolCalls = _toolCalls,
        };
}
