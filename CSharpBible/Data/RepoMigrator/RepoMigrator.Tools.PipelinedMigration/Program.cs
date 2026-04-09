using Microsoft.Extensions.DependencyInjection;
using RepoMigrator.Core;
using RepoMigrator.Core.Abstractions;
using RepoMigrator.Providers.Git;
using RepoMigrator.Providers.SvnCli;

namespace RepoMigrator.Tools.PipelinedMigration;

internal static class Program
{
    public static async Task<int> Main(string[] arrArgs)
    {
        PipelinedMigrationOptions? options;
        try
        {
            options = PipelinedMigrationOptions.Parse(arrArgs);
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
            .AddTransient<GitProvider>()
            .AddTransient<SvnCliProvider>()
            .AddSingleton<IProviderFactory>(sp => new ProviderSelectorFactory(new Dictionary<RepoType, Func<IVersionControlProvider>>
            {
                { RepoType.Git, () => sp.GetRequiredService<GitProvider>() },
                { RepoType.Svn, () => sp.GetRequiredService<SvnCliProvider>() }
            }))
            .AddSingleton<PipelinedMigrationService>()
            .AddSingleton<ConsoleMigrationProgress>()
            .BuildServiceProvider();

        try
        {
            var migrationService = serviceProvider.GetRequiredService<PipelinedMigrationService>();
            var progress = serviceProvider.GetRequiredService<ConsoleMigrationProgress>();
            await migrationService.RunAsync(options, progress, CancellationToken.None);
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
        Console.WriteLine("  dotnet run --project RepoMigrator\\RepoMigrator.Tools.PipelinedMigration -- --source <svn-url-or-path> --target <git-url-or-path> [--source-branch trunk] [--target-branch main] [--source-user user] [--source-password secret] [--target-user user] [--target-password secret] [--from 123] [--to 456] [--max-count 100] [--prefetch 3] [--export-workers 2] [--temp-root c:\\temp\\repo-pipeline] [--newest-first]");
    }
}
