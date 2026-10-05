using System;

namespace AhnWin52Backup.Cli.Composition;

internal static class ServiceProviderExtensions
{
    public static TService GetRequiredService<TService>(this IServiceProvider provider)
        where TService : class
    {
        ArgumentNullException.ThrowIfNull(provider);
        return provider.GetService(typeof(TService)) as TService
            ?? throw new InvalidOperationException($"Service {typeof(TService).FullName} is not registered.");
    }
}
