using Microsoft.Extensions.DependencyInjection;

namespace RepoMigrator.Tools.GitBranchSplitter;

internal static class Program
{
    public static async Task<int> Main(string[] args)
    {
        if (args.Any(static arg => arg is "/?"))
        {
            GitBranchSplitOptionsCommand.WriteHelp(Console.Out);
            return 0;
        }

        var parseResult = GitBranchSplitOptionsCommand.Parse(args);
        if (parseResult.RequestHelp)
        {
            GitBranchSplitOptionsCommand.WriteHelp(Console.Out);
            return 0;
        }

        if (!parseResult.Success)
        {
            Console.Error.WriteLine(parseResult.ErrorMessage);
            GitBranchSplitOptionsCommand.WriteUsage(Console.Out);
            return 1;
        }

        var options = parseResult.Options!;

        var serviceProvider = new ServiceCollection()
            .AddSingleton<GitBranchSplitService>()
            .BuildServiceProvider();

        try
        {
            var splitter = serviceProvider.GetRequiredService<GitBranchSplitService>();
            await splitter.SplitAsync(options, CancellationToken.None);
            Console.WriteLine("Branch split completed.");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return 2;
        }
    }
}
