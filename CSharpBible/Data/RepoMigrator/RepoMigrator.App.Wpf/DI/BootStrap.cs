// RepoMigrator.App.Wpf/DI/Bootstrap.cs
using Microsoft.Extensions.DependencyInjection;
using RepoMigrator.App.Logic.Services;
using RepoMigrator.App.Wpf.ViewModels;
using RepoMigrator.App.State.Services;
using RepoMigrator.Core;
using RepoMigrator.Core.Abstractions;
using RepoMigrator.Core.Services;
using RepoMigrator.Providers.Archive.Abstractions;
using RepoMigrator.Providers.Archive.Services;
using RepoMigrator.Providers.Patch.Services;
using RepoMigrator.Providers.Compression.TarGz;
using RepoMigrator.Providers.Compression.Zip;
using RepoMigrator.Providers.Git;
using RepoMigrator.Providers.Svn;
using RepoMigrator.Providers.SvnCli;
using System.IO;

namespace RepoMigrator.App.Wpf;

public static class Bootstrap
{
    private const string GitProviderKey = "git";
    private const string SvnProviderKey = "svn";

    public static ServiceProvider Create()
    {
        var services = new ServiceCollection();
        var workspaceRootPath = ResolveWorkspaceRootPath();
        var archiveStorageRootPath = ResolveArchiveStorageRootPath(workspaceRootPath);

        // Core
        services.AddSingleton<IMigrationService, MigrationService>();

        // Providers als transiente Factories
        services.AddTransient<GitProvider>();
        services.AddTransient<SvnCliProvider>();
        services.AddSingleton<ProviderSelectorFactory>(sp =>
            new ProviderSelectorFactory(new Dictionary<string, Func<IVersionControlProvider>>(StringComparer.OrdinalIgnoreCase)
            {
                { GitProviderKey, () => sp.GetRequiredService<GitProvider>() },
                { SvnProviderKey, () => sp.GetRequiredService<SvnCliProvider>() }
            }));
        services.AddSingleton<IProviderFactory>(sp =>
            sp.GetRequiredService<ProviderSelectorFactory>());

        // Archive migration support
        services.AddSingleton(new DirectoryArchiveSnapshotSourceProvider(workspaceRootPath));
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
        services.AddSingleton<IArchiveImportStateStore>(_ => new FileSystemArchiveImportStateStore(archiveStorageRootPath));
        services.AddSingleton<IArchiveMigrationService, ArchiveMigrationService>();

        // Structured change sources
        services.AddSingleton<IMigrationChangeSetSource>(_ => new DirectoryPatchChangeSetSource(workspaceRootPath));
        services.AddSingleton<IMigrationChangeSetSink>(sp => new GitStructuredChangeSetSink(sp.GetRequiredService<GitProvider>(), workspaceRootPath));
        services.AddSingleton<IMigrationChangeSetSourceFactory, MigrationChangeSetSourceFactory>();
        services.AddSingleton<IMigrationChangeSetSinkFactory, MigrationChangeSetSinkFactory>();
        services.AddSingleton<MigrationExecutionPathSelector>();
        services.AddSingleton<IStructuredMigrationPlanner, StructuredMigrationPlanner>();

        // ViewModels
        services.AddSingleton<MigrationEndpointFactory>();
        services.AddSingleton<MigrationQueryService>();
        services.AddSingleton<RecentPathHistoryService>();
        services.AddSingleton<RepositorySelectionService>();
        services.AddSingleton<AppInputStateStore>();
        services.AddSingleton<MainViewModel>();

        return services.BuildServiceProvider();
    }

    private static string ResolveWorkspaceRootPath()
    {
        var baseDirectory = AppContext.BaseDirectory;
        var currentDirectory = Directory.GetCurrentDirectory();

        if (Directory.Exists(Path.Combine(currentDirectory, "DevOps")))
            return currentDirectory;

        var candidateDirectory = new DirectoryInfo(baseDirectory);
        while (candidateDirectory is not null)
        {
            if (Directory.Exists(Path.Combine(candidateDirectory.FullName, "DevOps")))
                return candidateDirectory.FullName;

            candidateDirectory = candidateDirectory.Parent;
        }

        return currentDirectory;
    }

    private static string ResolveArchiveStorageRootPath(string workspaceRootPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workspaceRootPath);
        return Path.Combine(workspaceRootPath, ".RepoMigratorRuntime");
    }
}
