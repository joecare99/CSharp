using AA98_AvlnCodeStudio.Base.Versioning.Services;
using AA98_AvlnCodeStudio.Versioning.Git.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AA98_AvlnCodeStudio.Versioning.Git.DependencyInjection;

/// <summary>
/// Provides registration helpers for Git-backed versioning services.
/// </summary>
public static class GitVersioningServiceCollectionExtensions
{
    /// <summary>
    /// Adds the Git-backed Code Studio versioning services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection instance.</returns>
    public static IServiceCollection AddGitCodeStudioVersioning(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IGitCommandRunner, GitCommandRunner>();
        services.AddSingleton<IVersionControlService, GitVersionControlService>();
        return services;
    }
}
