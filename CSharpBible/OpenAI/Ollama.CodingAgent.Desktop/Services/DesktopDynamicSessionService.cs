using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Ollama.CodingAgent;
using Ollama.CodingAgent.Application.Diagnostics;
using Ollama.CodingAgent.Application.Extensions;
using Ollama.CodingAgent.Application.Interfaces;
using Ollama.CodingAgent.Application.Models;
using Ollama.CodingAgent.Application.Services;
using Ollama.CodingAgent.Application.ViewModels;
using Ollama.CodingAgent.Desktop.Models;

namespace Ollama.CodingAgent.Desktop.Services;

/// <summary>
/// Creates an isolated agent runtime for each prompt configuration snapshot.
/// </summary>
public sealed class DesktopDynamicSessionService : IStreamingAgentSessionService
{
    private readonly DesktopOptions _baseOptions;
    private readonly Func<DesktopConfiguration> _configurationProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="DesktopDynamicSessionService"/> class.
    /// </summary>
    public DesktopDynamicSessionService(
        DesktopOptions baseOptions,
        Func<DesktopConfiguration> configurationProvider)
    {
        _baseOptions = baseOptions ?? throw new ArgumentNullException(nameof(baseOptions));
        _configurationProvider = configurationProvider ?? throw new ArgumentNullException(nameof(configurationProvider));
    }

    /// <inheritdoc />
    public Task<AgentRunResult> RunAsync(string prompt, CancellationToken cancellationToken = default)
        => RunAsync(prompt, static _ => { }, cancellationToken);

    /// <inheritdoc />
    public Task<AgentRunResult> RunAsync(
        string prompt,
        Action<AgentRuntimeUpdate> onUpdate,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(prompt);
        ArgumentNullException.ThrowIfNull(onUpdate);
        DesktopConfiguration configuration = _configurationProvider().Normalize();
        return RunWithRuntimeAsync(configuration, prompt, onUpdate, cancellationToken);
    }

    private async Task<AgentRunResult> RunWithRuntimeAsync(
        DesktopConfiguration configuration,
        string prompt,
        Action<AgentRuntimeUpdate> onUpdate,
        CancellationToken cancellationToken)
    {
        ServiceCollection services = new();
        OllamaAgentCliOptions runtimeOptions = _baseOptions.ToRuntimeOptions(configuration);
        services.AddOllamaCodingAgent(runtimeOptions);
        services.AddAgentApplication(configuration.WorkspacePath, _baseOptions.SessionId);
        await using ServiceProvider provider = services.BuildServiceProvider(validateScopes: true);
        IAgentSessionService sessionService = provider.GetRequiredService<IAgentSessionService>();
        if (sessionService is not IStreamingAgentSessionService streamingService)
        {
            return await sessionService.RunAsync(prompt, cancellationToken);
        }

        return await streamingService.RunAsync(prompt, onUpdate, cancellationToken);
    }
}
