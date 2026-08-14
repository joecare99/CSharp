using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
    private readonly IAgentDiagnosticsSink? _diagnosticsSink;

    /// <summary>
    /// Initializes a new instance of the <see cref="AgentRunner"/> class.
    /// </summary>
    /// <param name="modelClient">The abstract model client.</param>
    /// <param name="runtimeSettings">The runtime settings.</param>
    public AgentRunner(
        IAgentModelClient modelClient,
        OllamaAgentRuntimeSettings runtimeSettings,
        IAgentDiagnosticsSink? diagnosticsSink = null)
    {
        _modelClient = modelClient ?? throw new ArgumentNullException(nameof(modelClient));
        _runtimeSettings = runtimeSettings ?? throw new ArgumentNullException(nameof(runtimeSettings));
        _diagnosticsSink = diagnosticsSink;
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
        string correlationId = Guid.NewGuid().ToString("N");
        _diagnosticsSink?.Record(new AgentDiagnosticEvent
        {
            CorrelationId = correlationId,
            EventName = "run.started",
        });

        List<AgentMessage> messages =
        [
            new AgentMessage("system", request.SystemPrompt),
            new AgentMessage("user", request.Prompt),
        ];

        int retriesUsed = 0;
        List<string> thinking = [];
        for (int iteration = 1; iteration <= _runtimeSettings.MaxIterations; iteration++)
        {
            AgentCompletion completion = await CompleteWithRetryAsync(messages, cancellationToken, retryAttempts =>
            {
                retriesUsed += retryAttempts;
            }, correlationId, iteration);
            string response = completion.Content;
            thinking.AddRange(completion.Thinking.Where(static fragment => !string.IsNullOrWhiteSpace(fragment)));

            if (string.IsNullOrWhiteSpace(response))
            {
                continue;
            }

            messages.Add(new AgentMessage("assistant", response));
            string normalizedResponse = AgentResponseNormalizer.Normalize(response, out bool finalizedWithMarker);
            if (!string.IsNullOrWhiteSpace(normalizedResponse))
            {
                AgentRunResult result = new()
                {
                    FinalResponse = normalizedResponse,
                    IterationsUsed = iteration,
                    RetryAttemptsUsed = retriesUsed,
                    FinalizedWithMarker = finalizedWithMarker,
                    Thinking = thinking,
                };
                _diagnosticsSink?.Record(new AgentDiagnosticEvent
                {
                    CorrelationId = correlationId,
                    EventName = "run.completed",
                    Iteration = iteration,
                });
                return result;
            }
        }

        throw new InvalidOperationException($"The agent did not produce a response within {_runtimeSettings.MaxIterations} iterations.");
    }

    private async Task<AgentCompletion> CompleteWithRetryAsync(
        IReadOnlyList<AgentMessage> messages,
        CancellationToken cancellationToken,
        Action<int> onRetriesConsumed,
        string correlationId,
        int iteration)
    {
        Exception? lastException = null;
        int retriesConsumed = 0;

        for (int attempt = 0; attempt <= _runtimeSettings.RetryCount; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
                using CancellationTokenSource timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                timeoutCts.CancelAfter(_runtimeSettings.StepTimeout);
                AgentCompletion completion = _modelClient is IThinkingAgentModelClient thinkingClient
                    ? await thinkingClient.CompleteDetailedAsync(messages, timeoutCts.Token)
                    : new AgentCompletion
                    {
                        Content = await _modelClient.CompleteAsync(messages, timeoutCts.Token),
                    };
                onRetriesConsumed(retriesConsumed);
                stopwatch.Stop();
                _diagnosticsSink?.Record(new AgentDiagnosticEvent
                {
                    CorrelationId = correlationId,
                    EventName = "completion.succeeded",
                    Iteration = iteration,
                    Attempt = attempt,
                    Duration = stopwatch.Elapsed,
                });
                return completion;
            }
            catch (OperationCanceledException ex) when (!cancellationToken.IsCancellationRequested && attempt < _runtimeSettings.RetryCount)
            {
                retriesConsumed++;
                lastException = ex;
                RecordFailure(correlationId, iteration, attempt, stopwatch, ex);
            }
            catch (Exception ex) when (attempt < _runtimeSettings.RetryCount)
            {
                retriesConsumed++;
                lastException = ex;
                RecordFailure(correlationId, iteration, attempt, stopwatch, ex);
            }
            catch (Exception ex)
            {
                lastException = ex;
                RecordFailure(correlationId, iteration, attempt, stopwatch, ex);
                break;
            }
        }

        throw new InvalidOperationException(
            $"Model completion failed after {_runtimeSettings.RetryCount + 1} attempts.",
            lastException);
    }

    private void RecordFailure(
        string correlationId,
        int iteration,
        int attempt,
        Stopwatch stopwatch,
        Exception exception)
    {
        stopwatch.Stop();
        _diagnosticsSink?.Record(new AgentDiagnosticEvent
        {
            CorrelationId = correlationId,
            EventName = "completion.failed",
            Iteration = iteration,
            Attempt = attempt,
            Duration = stopwatch.Elapsed,
            Error = exception.GetType().Name + ": " + exception.Message,
        });
    }
}
