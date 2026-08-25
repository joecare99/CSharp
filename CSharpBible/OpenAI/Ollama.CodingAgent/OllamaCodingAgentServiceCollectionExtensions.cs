using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Ollama.Client.Interfaces;
using Ollama.Client.Models;
using Ollama.Client.Services;
using Ollama.Tools;
using Polly;

namespace Ollama.CodingAgent;

/// <summary>
/// Registers the reusable Ollama coding-agent runtime composition.
/// </summary>
public static class OllamaCodingAgentServiceCollectionExtensions
{
    /// <summary>
    /// Registers provider, runtime, workspace, and delegated tool services for the supplied CLI options.
    /// </summary>
    public static IServiceCollection AddOllamaCodingAgent(
        this IServiceCollection services,
        OllamaAgentCliOptions cliOptions)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(cliOptions);

        services.AddSingleton(cliOptions.RuntimeSettings);
        services.AddSingleton(cliOptions);
        services.AddSingleton<FileLlmTrafficLogger>(provider =>
            new FileLlmTrafficLogger(cliOptions.SessionId));
        services.AddSingleton<ILlmTrafficLogger>(provider =>
            provider.GetRequiredService<FileLlmTrafficLogger>());
        services.AddSingleton<BaseLib.Models.Interfaces.ILog>(provider =>
            provider.GetRequiredService<FileLlmTrafficLogger>());
        services.AddSingleton(new OllamaClientOptions(new Uri(cliOptions.Endpoint)));

        // Provider traffic can stream for many minutes; the AgentRunner owns step timeouts.
        // Keep retries without the standard handler's bounded timeout strategies.
        // The standard resilience handler validates timeouts against [10ms, 1 day] and rejects
        // Timeout.InfiniteTimeSpan, so derive finite bounds from the configured runtime settings.
        TimeSpan attemptTimeout = cliOptions.RuntimeSettings.StepTimeout;
        TimeSpan totalRequestTimeout = ComputeBoundedTotalRequestTimeout(
            cliOptions.RuntimeSettings.StepTimeout,
            cliOptions.RuntimeSettings.RetryCount,
            cliOptions.RuntimeSettings.MaxIterations);
        services.AddHttpClient(OllamaHttpClientNames.Agent, client =>
            {
                client.Timeout = totalRequestTimeout;
            })
            .AddResilienceHandler("ollama-agent-resilience", builder =>
            {
                options.TotalRequestTimeout.Timeout = totalRequestTimeout;
                options.AttemptTimeout.Timeout = attemptTimeout;
                // The default circuit-breaker sampling duration (30s) must be >= 2 * AttemptTimeout
                // to be statistically meaningful. Scale it up while keeping it within the
                // validator's [500ms, 1 day] ceiling and at least double the attempt timeout.
                options.CircuitBreaker.SamplingDuration = ComputeBoundedCircuitBreakerSamplingDuration(attemptTimeout);
                builder.AddRetry(new Microsoft.Extensions.Http.Resilience.HttpRetryStrategyOptions());
            });

        // Web lookups are short-lived and bounded; keep the standard resilience defaults.
        services.AddHttpClient(OllamaHttpClientNames.WebLookup, client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
            });

        services.AddSingleton(provider =>
            provider.GetRequiredService<IHttpClientFactory>().CreateClient(OllamaHttpClientNames.Agent));
        services.AddSingleton(provider =>
        {
            HttpClient httpClient = provider.GetRequiredService<HttpClient>();
            OllamaClientOptions options = provider.GetRequiredService<OllamaClientOptions>();
            return new OllamaClient(httpClient, options);
        });
        services.AddSingleton(provider =>
        {
            OllamaClient client = provider.GetRequiredService<OllamaClient>();
            return client.GetChatClient(cliOptions.Model);
        });
        services.AddSingleton<IOllamaBaselineClient>(provider =>
            new OllamaClientBaselineAdapter(
                provider.GetRequiredService<OllamaClient>(),
                cliOptions.Model));
        services.AddSingleton<OllamaBaselineService>(provider =>
            new OllamaBaselineService(
                provider.GetRequiredService<IOllamaBaselineClient>(),
                cliOptions.Model));
        services.AddSingleton<IAgentModelClient>(provider =>
            new OllamaChatModelClient(
                provider.GetRequiredService<OllamaChatClient>(),
                new Uri(cliOptions.Endpoint),
                provider.GetRequiredService<ILlmTrafficLogger>()));
        services.AddSingleton<AgentRunner>();
        services.AddSingleton(provider => new WorkspacePathPolicy(cliOptions.WorkspaceRoot));
        services.AddSingleton(provider => new CodingDelegationToolRegistryFactory(
            provider.GetRequiredService<WorkspacePathPolicy>(),
            provider.GetRequiredService<IHttpClientFactory>()));
        services.AddSingleton<IOllamaToolRegistry>(provider =>
            provider.GetRequiredService<CodingDelegationToolRegistryFactory>().CreateRegistry());
        services.AddSingleton<OllamaToolOrchestrator>();
        services.AddSingleton(provider => new OllamaToolChatRunnerAdapter(
            provider.GetRequiredService<OllamaChatClient>(),
            null,
            provider.GetRequiredService<ILlmTrafficLogger>(),
            new Uri(cliOptions.Endpoint)));
        services.AddSingleton<CodingTaskDelegationService>();
        return services;
    }

    /// <summary>
    /// Returns a total-request timeout sized to StepTimeout * RetryCount * MaxIterations,
    /// clamped into the [10 ms, 1 day] range required by the standard resilience handler.
    /// </summary>
    private static TimeSpan ComputeBoundedTotalRequestTimeout(TimeSpan stepTimeout, int retryCount, int maxIterations)
    {
        const long oneDayTicks = TimeSpan.TicksPerDay;
        long candidateTicks = stepTimeout.Ticks;
        try
        {
            candidateTicks = checked(stepTimeout.Ticks * Math.Max(1, retryCount + 1) * Math.Max(1, maxIterations));
        }
        catch (OverflowException)
        {
            candidateTicks = oneDayTicks;
        }

        if (candidateTicks <= 0)
        {
            candidateTicks = TimeSpan.TicksPerMillisecond * 10; // minimum allowed by the validator.
        }
        else if (candidateTicks > oneDayTicks)
        {
            candidateTicks = oneDayTicks;
        }

        return TimeSpan.FromTicks(candidateTicks);
    }

    /// <summary>
    /// Returns a circuit-breaker sampling duration that is at least twice the supplied
    /// attempt timeout (required by the standard resilience validator) and stays within
    /// the validator's [500 ms, 1 day] ceiling.
    /// </summary>
    private static TimeSpan ComputeBoundedCircuitBreakerSamplingDuration(TimeSpan attemptTimeout)
    {
        const long oneDayTicks = TimeSpan.TicksPerDay;
        const long minSamplingTicks = TimeSpan.TicksPerMillisecond * 500; // validator minimum.
        long candidateTicks;
        try
        {
            candidateTicks = checked(attemptTimeout.Ticks * 2);
        }
        catch (OverflowException)
        {
            candidateTicks = oneDayTicks;
        }

        if (candidateTicks < minSamplingTicks)
        {
            candidateTicks = minSamplingTicks;
        }
        else if (candidateTicks > oneDayTicks)
        {
            candidateTicks = oneDayTicks;
        }

        return TimeSpan.FromTicks(candidateTicks);
    }
}
