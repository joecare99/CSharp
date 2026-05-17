// RepoMigrator.App.Wpf/DI/Bootstrap.cs
using Microsoft.Extensions.DependencyInjection;
using RepoMigrator.App.Logic.Services;
using RepoMigrator.App.Wpf.ViewModels;
using RepoMigrator.App.State.Services;
using RepoMigrator.Core;
using RepoMigrator.Core.Abstractions;
using RepoMigrator.Providers.Archive.Abstractions;
using RepoMigrator.Providers.Archive.Services;
using RepoMigrator.Providers.Compression.TarGz;
using RepoMigrator.Providers.Compression.Zip;
using RepoMigrator.Providers.Git;
using RepoMigrator.Providers.SvnCli;

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

        // Archive migration support
        services.AddSingleton(new DirectoryArchiveSnapshotSourceProvider(AppContext.BaseDirectory));
        services.AddSingleton<ArchiveMigrationSourceProviderFactory>();
        services.AddSingleton<IMigrationSourceProviderFactory>(sp => sp.GetRequiredService<ArchiveMigrationSourceProviderFactory>());
        services.AddSingleton<ArchiveMigrationDestinationProviderFactory>();
        services.AddSingleton<IMigrationDestinationProviderFactory>(sp => sp.GetRequiredService<ArchiveMigrationDestinationProviderFactory>());
        services.AddSingleton<ArchiveOrderingService>();
        services.AddSingleton<ArchiveRefNamingService>();
        services.AddSingleton<ArchiveExtractionRootDetectionService>();
        services.AddSingleton<ArchiveExtractionRootConfigurationStore>();
        services.AddSingleton<ZipArchiveDriver>();
        services.AddSingleton<TarGzArchiveDriver>();
        services.AddSingleton<IArchiveDriverRegistry>(sp => new ArchiveDriverRegistry(new IArchiveDriver[]
        {
            sp.GetRequiredService<TarGzArchiveDriver>(),
            sp.GetRequiredService<ZipArchiveDriver>()
        }));
        services.AddSingleton<IArchiveImportPlanner, ArchiveImportPlanner>();
        services.AddSingleton<IArchiveImportStateStore>(_ => new DevOpsArchiveImportStateStore(AppContext.BaseDirectory));
        services.AddSingleton<IArchiveMigrationService, ArchiveMigrationService>();

        // ViewModels
        services.AddSingleton<MigrationEndpointFactory>();
        services.AddSingleton<MigrationQueryService>();
        services.AddSingleton<RecentPathHistoryService>();
        services.AddSingleton<RepositorySelectionService>();
        services.AddSingleton<AppInputStateStore>();
        services.AddSingleton<MainViewModel>();

        return services.BuildServiceProvider();
    }
}
