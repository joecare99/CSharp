using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ollama.CodingAgent;

/// <summary>
/// Executes the provider-agnostic agent loop with timeout and retry controls.
/// </summary>
public sealed class AgentRunner
{
    private readonly IAgentModelClient _modelClient;
    private readonly OllamaAgentRuntimeSettings _runtimeSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="AgentRunner"/> class.
    /// </summary>
    /// <param name="modelClient">The abstract model client.</param>
    /// <param name="runtimeSettings">The runtime settings.</param>
    public AgentRunner(IAgentModelClient modelClient, OllamaAgentRuntimeSettings runtimeSettings)
    {
        _modelClient = modelClient ?? throw new ArgumentNullException(nameof(modelClient));
        _runtimeSettings = runtimeSettings ?? throw new ArgumentNullException(nameof(runtimeSettings));
    }

    /// <summary>
    /// Runs the agent loop.
    /// </summary>
    /// <param name="request">The run request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The run result.</returns>
    public async Task<AgentRunResult> RunAsync(AgentRunRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        request.Validate();

        List<AgentMessage> messages =
        [
            new AgentMessage("system", request.SystemPrompt),
            new AgentMessage("user", request.Prompt),
        ];

        int retriesUsed = 0;
        for (int iteration = 1; iteration <= _runtimeSettings.MaxIterations; iteration++)
        {
            string response = await CompleteWithRetryAsync(messages, cancellationToken, retryAttempts =>
            {
                retriesUsed += retryAttempts;
            });

            if (string.IsNullOrWhiteSpace(response))
            {
                continue;
            }

            messages.Add(new AgentMessage("assistant", response));
            string normalizedResponse = AgentResponseNormalizer.Normalize(response, out bool finalizedWithMarker);
            if (!string.IsNullOrWhiteSpace(normalizedResponse))
            {
                return new AgentRunResult
                {
                    FinalResponse = normalizedResponse,
                    IterationsUsed = iteration,
                    RetryAttemptsUsed = retriesUsed,
                    FinalizedWithMarker = finalizedWithMarker,
                };
            }
        }

        throw new InvalidOperationException($"The agent did not produce a response within {_runtimeSettings.MaxIterations} iterations.");
    }

    private async Task<string> CompleteWithRetryAsync(
        IReadOnlyList<AgentMessage> messages,
        CancellationToken cancellationToken,
        Action<int> onRetriesConsumed)
    {
        Exception? lastException = null;
        int retriesConsumed = 0;

        for (int attempt = 0; attempt <= _runtimeSettings.RetryCount; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                using CancellationTokenSource timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                timeoutCts.CancelAfter(_runtimeSettings.StepTimeout);
                string completion = await _modelClient.CompleteAsync(messages, timeoutCts.Token);
                onRetriesConsumed(retriesConsumed);
                return completion;
            }
            catch (OperationCanceledException ex) when (!cancellationToken.IsCancellationRequested && attempt < _runtimeSettings.RetryCount)
            {
                retriesConsumed++;
                lastException = ex;
            }
            catch (Exception ex) when (attempt < _runtimeSettings.RetryCount)
            {
                retriesConsumed++;
                lastException = ex;
            }
            catch (Exception ex)
            {
                lastException = ex;
                break;
            }
        }

        throw new InvalidOperationException(
            $"Model completion failed after {_runtimeSettings.RetryCount + 1} attempts.",
            lastException);
    }
}
