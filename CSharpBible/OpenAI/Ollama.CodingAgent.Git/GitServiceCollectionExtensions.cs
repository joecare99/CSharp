using System;
using Microsoft.Extensions.DependencyInjection;

namespace Ollama.CodingAgent.Git;

/// <summary>
/// Registers the approval-gated local Git workspace provider.
/// </summary>
public static class GitServiceCollectionExtensions
{
    /// <summary>
    /// Adds local Git inspection and approval-gated mutation services.
    /// </summary>
    public static IServiceCollection AddCodingAgentGit(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddSingleton<IGitWorkspaceService, GitWorkspaceService>();
        services.AddSingleton<IGitOperationExecutor, GitOperationExecutor>();
        return services;
    }
}
