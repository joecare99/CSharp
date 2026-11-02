using System;
using System.Collections.Generic;
using System.Linq;

namespace AA98_AvlnCodeStudio.Base.AI.Models;

/// <summary>
/// Represents a provider-agnostic text generation request.
/// </summary>
public sealed class AICompletionRequest
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AICompletionRequest"/> class.
    /// </summary>
    /// <param name="messages">The input messages.</param>
    /// <param name="options">The optional request options.</param>
    public AICompletionRequest(
        IEnumerable<AIMessage> messages,
        AICompletionOptions? options = null,
        IEnumerable<AIChatToolDescriptor>? tools = null)
    {
        if (messages is null)
        {
            throw new ArgumentNullException(nameof(messages));
        }

        Messages = messages.ToArray();
        if (Messages.Count == 0)
        {
            throw new ArgumentException("At least one message is required.", nameof(messages));
        }

        Options = options ?? new AICompletionOptions();
        AIChatToolDescriptor[] availableTools = tools?.ToArray() ?? [];
        if (availableTools.Any(static tool => tool is null))
        {
            throw new ArgumentException("AI completion tools cannot contain null entries.", nameof(tools));
        }

        string? duplicateToolName = availableTools
            .GroupBy(static tool => tool.Name, StringComparer.Ordinal)
            .Where(static group => group.Count() > 1)
            .Select(static group => group.Key)
            .FirstOrDefault();
        if (duplicateToolName is not null)
        {
            throw new ArgumentException($"The AI tool '{duplicateToolName}' is provided more than once.", nameof(tools));
        }

        Tools = availableTools;
    }

    /// <summary>
    /// Gets the input messages.
    /// </summary>
    public IReadOnlyList<AIMessage> Messages { get; }

    /// <summary>
    /// Gets the optional request options.
    /// </summary>
    public AICompletionOptions Options { get; }

    /// <summary>
    /// Gets the explicitly available structured tools.
    /// </summary>
    public IReadOnlyList<AIChatToolDescriptor> Tools { get; }
}