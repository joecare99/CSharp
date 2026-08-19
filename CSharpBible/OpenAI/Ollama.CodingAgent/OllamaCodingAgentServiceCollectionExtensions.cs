using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Ollama.Client.Interfaces;
using Ollama.Client.Models;
using Ollama.Client.Services;
using Ollama.Tools;

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
        services.AddSingleton(_ => new HttpClient
        {
            Timeout = System.Threading.Timeout.InfiniteTimeSpan,
        });
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
        services.AddSingleton<CodingDelegationToolRegistryFactory>();
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
}
