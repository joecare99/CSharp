using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Ollama.CodingAgent;

namespace Ollama.CodingAgent.Application.Services;

/// <summary>
/// Adapts the agent runtime for persistent interactive sessions.
/// </summary>
public sealed class AgentSessionService : IStreamingAgentSessionService
{
    private readonly AgentRunner _agentRunner;
    private readonly CodingTaskDelegationService? _delegationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AgentSessionService"/> class.
    /// </summary>
    public AgentSessionService(AgentRunner agentRunner)
        : this(agentRunner, null)
    {
    }

    /// <summary>
    /// Initializes a new instance with the optional delegated coding-task tool loop.
    /// </summary>
    public AgentSessionService(
        AgentRunner agentRunner,
        CodingTaskDelegationService? delegationService)
    {
        _agentRunner = agentRunner ?? throw new ArgumentNullException(nameof(agentRunner));
        _delegationService = delegationService;
    }

    /// <inheritdoc />
    public Task<AgentRunResult> RunAsync(string prompt, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(prompt);
        if (_delegationService is not null)
        {
            return _delegationService.RunDelegatedAsync(prompt, cancellationToken);
        }

        return _agentRunner.RunAsync(new AgentRunRequest
        {
            Prompt = prompt,
            SystemPrompt = AgentPromptBuilder.BuildDefaultSystemPrompt(),
        }, cancellationToken);
    }

    /// <inheritdoc />
    public Task<AgentRunResult> RunAsync(
        string prompt,
        Action<AgentRuntimeUpdate> onUpdate,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(prompt);
        ArgumentNullException.ThrowIfNull(onUpdate);
        if (_delegationService is not null)
        {
            return _delegationService.RunDelegatedAsync(prompt, onUpdate, cancellationToken);
        }

        return _agentRunner.RunAsync(new AgentRunRequest
        {
            Prompt = prompt,
            SystemPrompt = AgentPromptBuilder.BuildDefaultSystemPrompt(),
        }, cancellationToken, onUpdate);
    }
}
