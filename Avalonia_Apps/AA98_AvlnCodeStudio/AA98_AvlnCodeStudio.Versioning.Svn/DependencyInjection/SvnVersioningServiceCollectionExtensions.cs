using AA98_AvlnCodeStudio.Versioning.Svn.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AA98_AvlnCodeStudio.Versioning.Svn.DependencyInjection;

/// <summary>
/// Provides registration helpers for Subversion-backed versioning services.
/// </summary>
public static class SvnVersioningServiceCollectionExtensions
{
    public static IServiceCollection AddSvnCodeStudioVersioning(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<ISvnCommandRunner, SvnCommandRunner>();
        services.AddSingleton<SvnVersionControlService>();
        services.AddSingleton<ISvnWorkingCopyOperations, SvnWorkingCopyOperations>();
        return services;
    }
}
