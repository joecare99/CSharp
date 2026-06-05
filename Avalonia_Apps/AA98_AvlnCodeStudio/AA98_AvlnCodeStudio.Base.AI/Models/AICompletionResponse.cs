using System;

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
    public AICompletionResponse(AIMessage message)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
    }

    /// <summary>
    /// Gets the generated assistant message.
    /// </summary>
    public AIMessage Message { get; }
}