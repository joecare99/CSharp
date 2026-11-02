using System;
using System.Collections.Generic;
using System.Linq;

namespace AA98_AvlnCodeStudio.Base.AI.Models;

/// <summary>
/// Contains the updated conversation and tool activity for one AI chat turn.
/// </summary>
public sealed class AIChatTurnResult
{
    public AIChatTurnResult(IEnumerable<AIMessage> messages, IEnumerable<AIChatToolExecution> toolExecutions)
    {
        ArgumentNullException.ThrowIfNull(messages);
        ArgumentNullException.ThrowIfNull(toolExecutions);
        Messages = messages.ToArray();
        ToolExecutions = toolExecutions.ToArray();
    }

    public IReadOnlyList<AIMessage> Messages { get; }

    public IReadOnlyList<AIChatToolExecution> ToolExecutions { get; }

    public AIMessage AssistantMessage => Messages.LastOrDefault(static message => message.Role == AIMessageRole.Assistant)
        ?? throw new InvalidOperationException("The AI chat turn contains no assistant response.");
}
