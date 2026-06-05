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
    public AICompletionRequest(IEnumerable<AIMessage> messages, AICompletionOptions? options = null)
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
    }

    /// <summary>
    /// Gets the input messages.
    /// </summary>
    public IReadOnlyList<AIMessage> Messages { get; }

    /// <summary>
    /// Gets the optional request options.
    /// </summary>
    public AICompletionOptions Options { get; }
}