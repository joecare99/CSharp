using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;

namespace Ollama.CodingAgent.Application.Extensions;

/// <summary>
/// Registers reusable interactive agent application services.
/// </summary>
public static class AgentApplicationServiceCollectionExtensions
{
    /// <summary>
    /// Registers the shared application services for one workspace and session.
    /// </summary>
    public static IServiceCollection AddAgentApplication(
        this IServiceCollection services,
        string workspacePath,
        string sessionId)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(sessionId);

        string fullWorkspacePath = Path.GetFullPath(workspacePath);
        string sessionFilePath = Path.Combine(fullWorkspacePath, ".agent", "sessions", $"{sessionId}.json");
        services.AddSingleton<AgentDiagnosticsChannel>();
        services.AddSingleton<Ollama.CodingAgent.Interfaces.IAgentDiagnosticsSink>(
            provider => new AgentDiagnosticsSink(
                provider.GetRequiredService<AgentDiagnosticsChannel>(),
                provider.GetRequiredService<Ollama.CodingAgent.FileLlmTrafficLogger>()));
        services.AddSingleton<IAgentSessionStore>(_ => new JsonAgentSessionStore(sessionFilePath));
        services.AddSingleton<IAgentApprovalService, AgentApprovalService>();
        services.AddSingleton<IAgentSessionService>(provider => new AgentSessionService(
            provider.GetRequiredService<AgentRunner>(),
            provider.GetRequiredService<Ollama.CodingAgent.CodingTaskDelegationService>()));
        services.AddSingleton(provider => new AgentSessionViewModel(
            provider.GetRequiredService<IAgentSessionService>(),
            provider.GetRequiredService<IAgentSessionStore>(),
            provider.GetRequiredService<IAgentApprovalService>(),
            sessionId,
            fullWorkspacePath,
            provider.GetRequiredService<AgentDiagnosticsChannel>()));
        return services;
    }
}
