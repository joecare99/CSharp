using Microsoft.Extensions.DependencyInjection;
using RepoMigrator.Providers.Archive.Abstractions;
using RepoMigrator.Providers.Archive.Services;
using RepoMigrator.Providers.Compression.TarGz;
using RepoMigrator.Providers.Compression.Zip;

namespace RepoMigrator.Tools.ArchiveSmokeTest;

internal static class Program
{
    public static async Task<int> Main(string[] arrArgs)
    {
        if (arrArgs.Any(static arg => string.Equals(arg, "/?", StringComparison.OrdinalIgnoreCase)))
        {
            ArchiveSmokeTestOptionsCommand.WriteHelp(Console.Out);
            return 0;
        }

        var parseResult = ArchiveSmokeTestOptionsCommand.Parse(arrArgs);
        if (parseResult.RequestHelp)
        {
            ArchiveSmokeTestOptionsCommand.WriteHelp(Console.Out);
            return 0;
        }

        if (!parseResult.Success)
        {
            Console.Error.WriteLine(parseResult.ErrorMessage);
            ArchiveSmokeTestOptionsCommand.WriteUsage(Console.Out);
            return 1;
        }

        var parsedOptions = parseResult.Options!;
        var options = new ArchiveSmokeTestOptions
        {
            SourceDirectoryPath = Path.GetFullPath(parsedOptions.SourceDirectoryPath),
            WorkspaceRootPath = string.IsNullOrWhiteSpace(parsedOptions.WorkspaceRootPath)
                ? Directory.GetCurrentDirectory()
                : Path.GetFullPath(parsedOptions.WorkspaceRootPath),
            Recursive = parsedOptions.Recursive,
            AllowedExtensions = parsedOptions.AllowedExtensions
        };

        var serviceProvider = new ServiceCollection()
            .AddSingleton(new DirectoryArchiveSnapshotSourceProvider(options.WorkspaceRootPath))
            .AddSingleton<ArchiveSmokeTestProviderFactory>()
            .AddSingleton<ArchiveOrderingService>()
            .AddSingleton<ArchiveRefNamingService>()
            .AddSingleton<ArchiveExtractionRootDetectionService>()
            .AddSingleton<ArchiveExtractionRootConfigurationStore>()
            .AddSingleton<ZipArchiveDriver>()
            .AddSingleton<TarGzArchiveDriver>()
            .AddSingleton<IArchiveDriverRegistry>(sp => new ArchiveDriverRegistry(new IArchiveDriver[]
            {
                sp.GetRequiredService<TarGzArchiveDriver>(),
                sp.GetRequiredService<ZipArchiveDriver>()
            }))
            .AddSingleton<RepoMigrator.Core.Abstractions.IMigrationSourceProviderFactory>(sp => sp.GetRequiredService<ArchiveSmokeTestProviderFactory>())
            .AddSingleton<IArchiveImportPlanner, ArchiveImportPlanner>()
            .AddSingleton<IArchiveImportStateStore>(_ => new FileSystemArchiveImportStateStore(Path.Combine(options.WorkspaceRootPath, ".RepoMigratorRuntime")))
            .AddSingleton<ArchiveSmokeTestService>()
            .BuildServiceProvider();

        try
        {
            var service = serviceProvider.GetRequiredService<ArchiveSmokeTestService>();
            await service.RunAsync(options, CancellationToken.None);
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return 2;
        }
    }
}
