using System;

namespace Ollama.CodingAgent.Models;

/// <summary>
/// Defines runtime constraints for agent execution.
/// </summary>
public sealed class OllamaAgentRuntimeSettings
{
    /// <summary>
    /// Gets the baseline timeout used for one model call.
    /// </summary>
    public static TimeSpan DefaultStepTimeout => TimeSpan.FromMinutes(12);

    /// <summary>
    /// Gets the baseline retry count used for one step.
    /// </summary>
    public static int DefaultRetryCount => 3;

    /// <summary>
    /// Gets the baseline delay before the first retry of a transient failure.
    /// </summary>
    public static TimeSpan DefaultRetryBackoff => TimeSpan.FromSeconds(2);

    /// <summary>
    /// Gets the baseline maximum iteration cap used for one run.
    /// </summary>
    public static int DefaultMaxIterations => 80;

    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaAgentRuntimeSettings"/> class.
    /// </summary>
    /// <param name="stepTimeout">The timeout for one model step.</param>
    /// <param name="retryCount">The number of retries per model step.</param>
    /// <param name="maxIterations">The hard iteration cap for one run.</param>
    /// <param name="verbosity">The requested output verbosity.</param>
    /// <param name="showThinking">Whether model reasoning should be displayed.</param>
    /// <param name="retryBackoff">The base delay before the first retry; doubles for each subsequent retry. Defaults to <see cref="DefaultRetryBackoff"/>.</param>
    /// <param name="logToolCalls">When true, every delegated tool call is emitted to the console as a single structured line (name, validated parameters, status, duration, truncated result). Quiet verbosity always suppresses this output regardless of this flag.</param>
    public OllamaAgentRuntimeSettings(
        TimeSpan stepTimeout,
        int retryCount,
        int maxIterations,
        AgentVerbosity verbosity = AgentVerbosity.Normal,
        bool showThinking = false,
        TimeSpan? retryBackoff = null,
        bool logToolCalls = false)
    {
        if (stepTimeout <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(stepTimeout), "The step timeout must be greater than zero.");
        }

        if (retryCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(retryCount), "The retry count must be zero or greater.");
        }

        if (maxIterations <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxIterations), "The maximum iteration count must be greater than zero.");
        }

        if (retryBackoff.HasValue && retryBackoff.Value < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(retryBackoff), "The retry backoff must not be negative.");
        }

        StepTimeout = stepTimeout;
        RetryCount = retryCount;
        MaxIterations = maxIterations;
        Verbosity = verbosity;
        ShowThinking = showThinking;
        RetryBackoff = retryBackoff ?? DefaultRetryBackoff;
        LogToolCalls = logToolCalls;
    }

    /// <summary>
    /// Gets the timeout for one model step.
    /// </summary>
    public TimeSpan StepTimeout { get; }

    /// <summary>
    /// Gets the number of retries per model step.
    /// </summary>
    public int RetryCount { get; }

    /// <summary>
    /// Gets the base delay applied before the first retry of a transient failure.
    /// The delay doubles for each subsequent retry attempt.
    /// </summary>
    public TimeSpan RetryBackoff { get; }

    /// <summary>
    /// Gets the hard iteration cap for one run.
    /// </summary>
    public int MaxIterations { get; }

    /// <summary>
    /// Gets the requested output verbosity.
    /// </summary>
    public AgentVerbosity Verbosity { get; }

    /// <summary>
    /// Gets a value indicating whether model reasoning should be displayed.
    /// </summary>
    public bool ShowThinking { get; }

    /// <summary>
    /// Gets a value indicating whether every delegated tool call should be written
    /// to the console as a single structured line. Quiet verbosity suppresses output regardless.
    /// </summary>
    public bool LogToolCalls { get; }
}
