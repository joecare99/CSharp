// RepoMigrator.App.Wpf/DI/Bootstrap.cs
using Microsoft.Extensions.DependencyInjection;
using RepoMigrator.App.Wpf.ViewModels;
using RepoMigrator.Core;
using RepoMigrator.Core.Abstractions;
using RepoMigrator.Providers.Git;
using RepoMigrator.Providers.SvnCli;
using System;

namespace RepoMigrator.App.Wpf;

public static class Bootstrap
{
    public static ServiceProvider Create()
    {
        var services = new ServiceCollection();

        // Core
        services.AddSingleton<IMigrationService, MigrationService>();

        // Providers als transiente Factories
        services.AddTransient<GitProvider>();
        services.AddTransient<SvnCliProvider>();
        services.AddSingleton<IProviderFactory>(sp =>
        {
            return new ProviderSelectorFactory(new Dictionary<RepoType, Func<IVersionControlProvider>>
            {
                { RepoType.Git, () => sp.GetRequiredService<GitProvider>() },
                { RepoType.Svn, () => sp.GetRequiredService<SvnCliProvider>() }
            });
        });

        // ViewModels
        services.AddSingleton<MainViewModel>();

        return services.BuildServiceProvider();
    }
}
