using Ollama.CodingAgent.Console.Configuration;
using Ollama.CodingAgent.Console.Commands;
using Ollama.CodingAgent.Console.Interfaces;
using Ollama.CodingAgent.Console.Infrastructure;
using Ollama.CodingAgent.Console.Presentation;
using Ollama.CodingAgent.Console.Services;
using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Ollama.CodingAgent;
using Ollama.CodingAgent.Application.Diagnostics;
using Ollama.CodingAgent.Application.Extensions;
using Ollama.CodingAgent.Application.Interfaces;
using Ollama.CodingAgent.Application.Models;
using Ollama.CodingAgent.Application.ViewModels;

namespace Ollama.CodingAgent.Console.Services;

/// <summary>
/// Creates a runtime snapshot for each console prompt.
/// </summary>
public sealed class ConsoleDynamicSessionService : IStreamingAgentSessionService
{
    private readonly ConsoleAgentCliOptions _baseOptions;
    private readonly ConsoleRuntimeConfiguration _configuration;

    public ConsoleDynamicSessionService(ConsoleAgentCliOptions baseOptions, ConsoleRuntimeConfiguration configuration)
    {
        _baseOptions = baseOptions ?? throw new ArgumentNullException(nameof(baseOptions));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _configuration.Set(baseOptions.Endpoint, baseOptions.Model, baseOptions.WorkspacePath);
    }

    public Task<AgentRunResult> RunAsync(string prompt, CancellationToken cancellationToken = default)
        => RunAsync(prompt, static _ => { }, cancellationToken);

    public Task<AgentRunResult> RunAsync(string prompt, Action<AgentRuntimeUpdate> onUpdate, CancellationToken cancellationToken = default)
    {
        OllamaAgentCliOptions options = OllamaAgentCliOptions.Parse([
            "--endpoint", _configuration.Endpoint,
            "--model", _configuration.Model,
            "--workspace-root", _configuration.WorkspacePath,
            "--session", _baseOptions.SessionId]);
        ServiceCollection services = new();
        services.AddOllamaCodingAgent(options);
        services.AddAgentApplication(_configuration.WorkspacePath, _baseOptions.SessionId);
        return RunAsync(services, prompt, onUpdate, cancellationToken);
    }

    private static async Task<AgentRunResult> RunAsync(ServiceCollection services, string prompt, Action<AgentRuntimeUpdate> onUpdate, CancellationToken cancellationToken)
    {
        await using ServiceProvider provider = services.BuildServiceProvider(validateScopes: true);
        IAgentSessionService sessionService = provider.GetRequiredService<IAgentSessionService>();
        return sessionService is IStreamingAgentSessionService streamingService
            ? await streamingService.RunAsync(prompt, onUpdate, cancellationToken)
            : await sessionService.RunAsync(prompt, cancellationToken);
    }
}