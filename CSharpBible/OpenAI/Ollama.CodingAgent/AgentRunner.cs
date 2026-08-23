using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
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
        => await RunAsync(request, cancellationToken, null);

    /// <summary>
    /// Runs the agent loop and reports live runtime updates.
    /// </summary>
    public async Task<AgentRunResult> RunAsync(
        AgentRunRequest request,
        CancellationToken cancellationToken,
        Action<AgentRuntimeUpdate>? onUpdate)
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
            }, correlationId, iteration, onUpdate);
            string response = completion.Content;
            thinking.AddRange(completion.Thinking.Where(static fragment => !string.IsNullOrWhiteSpace(fragment)));
            foreach (string fragment in completion.Thinking.Where(static fragment => !string.IsNullOrWhiteSpace(fragment)))
            {
                _diagnosticsSink?.Record(new AgentDiagnosticEvent
                {
                    CorrelationId = correlationId,
                    EventName = "completion.thinking",
                    Iteration = iteration,
                    Detail = fragment,
                });
                onUpdate?.Invoke(new AgentRuntimeUpdate
                {
                    Kind = AgentRuntimeUpdateKind.Thinking,
                    Content = fragment,
                });
            }

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

        _diagnosticsSink?.Record(new AgentDiagnosticEvent
        {
            CorrelationId = correlationId,
            EventName = "run.failed",
            Error = $"The agent did not produce a response within {_runtimeSettings.MaxIterations} iterations.",
        });
        throw new InvalidOperationException($"The agent did not produce a response within {_runtimeSettings.MaxIterations} iterations.");
    }

    private async Task<AgentCompletion> CompleteWithRetryAsync(
        IReadOnlyList<AgentMessage> messages,
        CancellationToken cancellationToken,
        Action<int> onRetriesConsumed,
        string correlationId,
        int iteration,
        Action<AgentRuntimeUpdate>? onUpdate)
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
                AgentCompletion completion = _modelClient is IStreamingThinkingAgentModelClient streamingThinkingClient && onUpdate is not null
                    ? await streamingThinkingClient.CompleteDetailedAsync(messages, fragment => onUpdate(new AgentRuntimeUpdate
                    {
                        Kind = AgentRuntimeUpdateKind.Thinking,
                        Content = fragment,
                    }), timeoutCts.Token)
                    : _modelClient is IThinkingAgentModelClient thinkingClient
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
            catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex) when (IsTransient(ex))
            {
                retriesConsumed++;
                lastException = ex;
                RecordFailure(correlationId, iteration, attempt, stopwatch, ex);
                if (attempt < _runtimeSettings.RetryCount)
                {
                    TimeSpan backoffDelay = TimeSpan.FromTicks(_runtimeSettings.RetryBackoff.Ticks * (1L << attempt));
                    if (backoffDelay > TimeSpan.Zero)
                    {
                        await Task.Delay(backoffDelay, cancellationToken);
                    }
                }
            }
            catch (Exception ex)
            {
                lastException = ex;
                RecordFailure(correlationId, iteration, attempt, stopwatch, ex);
                break;
            }
        }

        System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(lastException!).Throw();
        throw new InvalidOperationException($"Model completion failed after {_runtimeSettings.RetryCount + 1} attempts.", lastException);
    }

    /// <summary>
    /// Determines whether the exception represents a transient provider failure worth retrying.
    /// </summary>
    /// <param name="exception">The observed exception.</param>
    /// <returns>True when the failure is classified as transient.</returns>
    internal static bool IsTransient(Exception exception)
        => exception is System.Net.Http.HttpRequestException
            or TimeoutException
            or System.IO.IOException
            or System.Net.Sockets.SocketException
            or OperationCanceledException;

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
