using AA98_AvlnCodeStudio.Base.AI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Base.AI.Services;

/// <summary>
/// Runs provider-neutral AI chat turns and dispatches only explicitly registered tools.
/// </summary>
public sealed class AIChatService(IAIClient client, AIChatToolCatalog toolCatalog)
{
    private const int MaximumToolCallRounds = 8;

    public IReadOnlyList<AIChatToolDescriptor> AvailableTools => toolCatalog.Descriptors;

    public async Task<AIChatTurnResult> SendAsync(
        IEnumerable<AIMessage> messages,
        AICompletionOptions? options = null,
        Func<AIChatToolDescriptor, JsonElement, CancellationToken, Task<bool>>? approveToolExecution = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(messages);
        List<AIMessage> conversation = messages.ToList();
        if (conversation.Count == 0 || conversation.Any(static message => message is null))
        {
            throw new ArgumentException("A chat turn requires at least one non-null message.", nameof(messages));
        }

        List<AIChatToolExecution> executions = [];
        for (int round = 0; round <= MaximumToolCallRounds; round++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            AICompletionResponse response = await client.CompleteAsync(
                new AICompletionRequest(conversation, options, toolCatalog.Descriptors),
                cancellationToken).ConfigureAwait(false);
            if (response.ToolCalls.Count == 0)
            {
                conversation.Add(response.Message);
                return new AIChatTurnResult(conversation, executions);
            }

            if (round == MaximumToolCallRounds)
            {
                throw new InvalidOperationException(
                    $"The AI assistant exceeded the limit of {MaximumToolCallRounds} consecutive tool-call rounds.");
            }

            conversation.Add(new AIMessage(
                AIMessageRole.Assistant,
                response.Message.Content,
                toolCalls: response.ToolCalls));

            foreach (AIChatToolCall toolCall in response.ToolCalls)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (!toolCatalog.TryGetDescriptor(toolCall.Name, out AIChatToolDescriptor? descriptor))
                {
                    throw new KeyNotFoundException($"The AI requested the unregistered tool '{toolCall.Name}'.");
                }

                string result;
                bool wasExecuted = !descriptor!.RequiresConfirmation
                    || (approveToolExecution is not null
                        && await approveToolExecution(descriptor, toolCall.Arguments, cancellationToken).ConfigureAwait(false));
                if (wasExecuted)
                {
                    result = await toolCatalog.ExecuteAsync(toolCall.Name, toolCall.Arguments, cancellationToken)
                        .ConfigureAwait(false);
                }
                else
                {
                    result = "Execution was not approved by the user; no action was taken.";
                }

                executions.Add(new AIChatToolExecution(toolCall, result, wasExecuted));
                conversation.Add(new AIMessage(
                    AIMessageRole.Tool,
                    result,
                    toolCallId: toolCall.CallId,
                    toolName: toolCall.Name));
            }
        }

        throw new InvalidOperationException("The AI chat turn ended unexpectedly.");
    }
}
