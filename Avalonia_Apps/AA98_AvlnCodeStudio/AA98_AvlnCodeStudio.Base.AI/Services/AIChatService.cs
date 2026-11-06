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

    public Task<AIChatTurnResult> SendAsync(
        IEnumerable<AIMessage> messages,
        AICompletionOptions? options = null,
        Func<AIChatToolDescriptor, JsonElement, CancellationToken, Task<bool>>? approveToolExecution = null,
        CancellationToken cancellationToken = default) =>
        SendWithAllowedToolsAsync(messages, null, options, approveToolExecution, cancellationToken);

    public async Task<AIChatTurnResult> SendWithAllowedToolsAsync(
        IEnumerable<AIMessage> messages,
        IReadOnlyCollection<string>? allowedToolNames,
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

        IReadOnlyList<AIChatToolDescriptor> activeTools;
        if (allowedToolNames is null)
        {
            activeTools = toolCatalog.Descriptors;
        }
        else
        {
            HashSet<string> allowed = new(allowedToolNames, StringComparer.Ordinal);
            string[] unknownTools = allowed
                .Where(name => !toolCatalog.TryGetDescriptor(name, out _))
                .ToArray();
            if (unknownTools.Length > 0)
            {
                throw new ArgumentException(
                    $"The allowed tool list contains unregistered tools: {string.Join(", ", unknownTools)}.",
                    nameof(allowedToolNames));
            }

            activeTools = toolCatalog.Descriptors
                .Where(descriptor => allowed.Contains(descriptor.Name))
                .ToArray();
        }

        List<AIChatToolExecution> executions = [];
        for (int round = 0; round <= MaximumToolCallRounds; round++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            AICompletionResponse response = await client.CompleteAsync(
                new AICompletionRequest(conversation, options, activeTools),
                cancellationToken).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
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
                AIChatToolDescriptor? descriptor = activeTools.FirstOrDefault(
                    candidate => string.Equals(candidate.Name, toolCall.Name, StringComparison.Ordinal));
                if (descriptor is null)
                {
                    throw new KeyNotFoundException($"The AI requested a tool that is unavailable in this agent mode: '{toolCall.Name}'.");
                }

                string result;
                bool wasExecuted = !descriptor.RequiresConfirmation
                    || (approveToolExecution is not null
                        && await approveToolExecution(descriptor, toolCall.Arguments, cancellationToken).ConfigureAwait(false));
                cancellationToken.ThrowIfCancellationRequested();
                if (wasExecuted)
                {
                    result = await toolCatalog.ExecuteAsync(toolCall.Name, toolCall.Arguments, cancellationToken)
                        .ConfigureAwait(false);
                    cancellationToken.ThrowIfCancellationRequested();
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
