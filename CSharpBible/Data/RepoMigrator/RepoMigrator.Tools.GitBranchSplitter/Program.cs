using Microsoft.Extensions.DependencyInjection;

namespace RepoMigrator.Tools.GitBranchSplitter;

internal static class Program
{
    public static async Task<int> Main(string[] args)
    {
        GitBranchSplitOptions? options;

        try
        {
            options = GitBranchSplitOptions.Parse(args);
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

    private static void WriteUsage()
    {
        Console.WriteLine("Usage:");
        Console.WriteLine("  dotnet run --project RepoMigrator\\RepoMigrator.Tools.GitBranchSplitter -- --repo <repo-path> --source <branch-name> [--prefix split] [--overwrite] [--author-name \"RepoMigrator Tool\"] [--author-email \"tool@local\"]");
    }
}
