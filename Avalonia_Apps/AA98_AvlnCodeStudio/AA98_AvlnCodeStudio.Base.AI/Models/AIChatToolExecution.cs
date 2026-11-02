using System;

namespace AA98_AvlnCodeStudio.Base.AI.Models;

/// <summary>
/// Records one tool call made during an AI chat turn.
/// </summary>
public sealed class AIChatToolExecution
{
    public AIChatToolExecution(AIChatToolCall call, string result, bool wasExecuted)
    {
        Call = call ?? throw new ArgumentNullException(nameof(call));
        ArgumentNullException.ThrowIfNull(result);
        Result = result;
        WasExecuted = wasExecuted;
    }

    public AIChatToolCall Call { get; }

    public string Result { get; }

    public bool WasExecuted { get; }
}
