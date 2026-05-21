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
        ArchiveSmokeTestOptions? options;
        try
        {
            options = ArchiveSmokeTestOptions.Parse(arrArgs);
        }
        catch (ArgumentException ex)
        {
            Console.Error.WriteLine(ex.Message);
            WriteUsage();
            return 1;
        }

        if (options is null)
        {
            WriteUsage();
            return 0;
        }

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
            .AddSingleton<IArchiveImportStateStore>(_ => new DevOpsArchiveImportStateStore(options.WorkspaceRootPath))
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

    private static void WriteUsage()
    {
        Console.WriteLine("Usage:");
        Console.WriteLine("  dotnet run --project RepoMigrator\\RepoMigrator.Tools.ArchiveSmokeTest -- --source <archive-directory> [--workspace <workspace-root>] [--recursive] [--extension .zip] [--extension .tar.gz]");
        Console.WriteLine();
        Console.WriteLine("Example:");
        Console.WriteLine("  dotnet run --project RepoMigrator\\RepoMigrator.Tools.ArchiveSmokeTest -- --source C:\\Projekte\\Cpp\\xpdf --workspace C:\\Projekte\\CSharp\\CSharpBible\\Data --recursive");
    }
}
