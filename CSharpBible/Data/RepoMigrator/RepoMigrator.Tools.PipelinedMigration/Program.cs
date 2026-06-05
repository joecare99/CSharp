using Microsoft.Extensions.DependencyInjection;
using RepoMigrator.Core;
using RepoMigrator.Core.Abstractions;
using RepoMigrator.Providers.Git;
using RepoMigrator.Providers.SvnCli;

namespace RepoMigrator.Tools.PipelinedMigration;

internal static class Program
{
    private const string GitProviderKey = "git";
    private const string SvnProviderKey = "svn";

    public static async Task<int> Main(string[] arrArgs)
    {
        if (arrArgs.Any(static arg => string.Equals(arg, "/?", StringComparison.OrdinalIgnoreCase)))
        {
            PipelinedMigrationOptionsCommand.WriteHelp(Console.Out);
            return 0;
        }

        var parseResult = PipelinedMigrationOptionsCommand.Parse(arrArgs);
        if (parseResult.RequestHelp)
        {
            PipelinedMigrationOptionsCommand.WriteHelp(Console.Out);
            return 0;
        }

        if (!parseResult.Success)
        {
            Console.Error.WriteLine(parseResult.ErrorMessage);
            PipelinedMigrationOptionsCommand.WriteUsage(Console.Out);
            return 1;
        }

        PipelinedMigrationOptions options;
        try
        {
            options = new PipelinedMigrationOptions
            {
                SourceUrl = parseResult.Options!.SourceUrl,
                TargetUrl = parseResult.Options.TargetUrl,
                SourceBranchOrTrunk = parseResult.Options.SourceBranchOrTrunk,
                TargetBranch = parseResult.Options.TargetBranch,
                SourceUser = parseResult.Options.SourceUser,
                SourcePassword = parseResult.Options.SourcePassword,
                TargetUser = parseResult.Options.TargetUser,
                TargetPassword = parseResult.Options.TargetPassword,
                FromId = parseResult.Options.FromId,
                ToId = parseResult.Options.ToId,
                MaxCount = parseResult.Options.MaxCount,
                OldestFirst = !parseResult.Options.NewestFirst,
                NewestFirst = parseResult.Options.NewestFirst,
                PrefetchCount = parseResult.Options.PrefetchCount,
                MaxExportWorkers = parseResult.Options.MaxExportWorkers,
                TempRoot = parseResult.Options.TempRoot
            };
            options.Validate();
        }
        catch (ArgumentException ex)
        {
            Console.Error.WriteLine(ex.Message);
            PipelinedMigrationOptionsCommand.WriteUsage(Console.Out);
            return 1;
        }

        var serviceProvider = new ServiceCollection()
            .AddTransient<GitProvider>()
            .AddTransient<SvnCliProvider>()
            .AddSingleton<ProviderSelectorFactory>(sp => new ProviderSelectorFactory(new Dictionary<string, Func<IVersionControlProvider>>(StringComparer.OrdinalIgnoreCase)
            {
                { GitProviderKey, () => sp.GetRequiredService<GitProvider>() },
                { SvnProviderKey, () => sp.GetRequiredService<SvnCliProvider>() }
            }))
            .AddSingleton<IProviderFactory>(sp => sp.GetRequiredService<ProviderSelectorFactory>())
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
}
