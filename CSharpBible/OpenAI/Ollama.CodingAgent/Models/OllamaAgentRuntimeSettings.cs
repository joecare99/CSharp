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
    /// Gets the baseline maximum iteration cap used for one run.
    /// </summary>
    public static int DefaultMaxIterations => 80;

    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaAgentRuntimeSettings"/> class.
    /// </summary>
    /// <param name="stepTimeout">The timeout for one model step.</param>
    /// <param name="retryCount">The number of retries per model step.</param>
    /// <param name="maxIterations">The hard iteration cap for one run.</param>
    public OllamaAgentRuntimeSettings(
        TimeSpan stepTimeout,
        int retryCount,
        int maxIterations,
        AgentVerbosity verbosity = AgentVerbosity.Normal,
        bool showThinking = false)
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

        StepTimeout = stepTimeout;
        RetryCount = retryCount;
        MaxIterations = maxIterations;
        Verbosity = verbosity;
        ShowThinking = showThinking;
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
}
