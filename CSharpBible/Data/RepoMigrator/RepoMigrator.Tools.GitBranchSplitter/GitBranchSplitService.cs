using LibGit2Sharp;

namespace RepoMigrator.Tools.GitBranchSplitter;

/// <summary>
/// Splits a local Git branch into path-based snapshot branches.
/// </summary>
public sealed class GitBranchSplitService
{
    /// <summary>
    /// Creates snapshot branches for first-level and second-level path groups.
    /// </summary>
    /// <param name="options">The split execution options.</param>
    /// <param name="ct">The cancellation token.</param>
    public Task SplitAsync(GitBranchSplitOptions options, CancellationToken ct)
    {
        var sRepositoryPath = Path.GetFullPath(options.RepositoryPath);
        if (!Repository.IsValid(sRepositoryPath))
            throw new DirectoryNotFoundException($"'{sRepositoryPath}' is not a valid Git repository.");

        using var gitRepository = new Repository(sRepositoryPath);
        if (gitRepository.RetrieveStatus().IsDirty)
            throw new InvalidOperationException("The repository contains local changes. Commit or discard them before splitting.");

        var gitSourceBranch = gitRepository.Branches[options.SourceBranch];
        if (gitSourceBranch is null || gitSourceBranch.IsRemote)
            throw new InvalidOperationException($"Local branch '{options.SourceBranch}' was not found.");

        if (gitSourceBranch.Tip is null)
            throw new InvalidOperationException($"Branch '{options.SourceBranch}' does not contain any commits.");

        Commands.Checkout(gitRepository, gitSourceBranch);

        var lstTrackedPaths = gitRepository.Index
            .Select(gitIndexEntry => gitIndexEntry.Path.Replace('\\', '/'))
            .Distinct(StringComparer.Ordinal)
            .ToList();

        var lstPlans = GitPathBranchSplitPlanner.BuildPlans(lstTrackedPaths, options.BranchPrefix);
        foreach (var plan in lstPlans)
        {
            ct.ThrowIfCancellationRequested();
            CreateOrReplaceBranch(gitRepository, gitSourceBranch, plan, options);
        }

        Commands.Checkout(gitRepository, gitSourceBranch);
        return Task.CompletedTask;
    }

    private static void CreateOrReplaceBranch(
        Repository gitRepository,
        Branch gitSourceBranch,
        BranchSplitPlan plan,
        GitBranchSplitOptions options)
    {
        Commands.Checkout(gitRepository, gitSourceBranch);
        gitRepository.Reset(ResetMode.Hard, gitSourceBranch.Tip);

        var gitExistingBranch = gitRepository.Branches[plan.BranchName];
        if (gitExistingBranch is not null)
        {
            if (!options.OverwriteExistingBranches)
                throw new InvalidOperationException($"Branch '{plan.BranchName}' already exists. Use --overwrite to replace it.");

            gitRepository.Branches.Remove(gitExistingBranch);
        }

        var gitTargetBranch = gitRepository.CreateBranch(plan.BranchName, gitSourceBranch.Tip);
        Commands.Checkout(gitRepository, gitTargetBranch);
        gitRepository.Reset(ResetMode.Hard, gitSourceBranch.Tip);

        ApplyPathFilter(gitRepository, plan.Paths);
        var repositoryStatus = gitRepository.RetrieveStatus();
        if (!repositoryStatus.IsDirty)
            return;

        var signature = new Signature(options.AuthorName, options.AuthorEmail, DateTimeOffset.UtcNow);
        gitRepository.Commit($"Split branch '{gitSourceBranch.FriendlyName}' into '{plan.BranchName}' by path.", signature, signature);
    }

    private static void ApplyPathFilter(Repository gitRepository, IReadOnlySet<string> hsKeepPaths)
    {
        var lstTrackedPaths = gitRepository.Index
            .Select(gitIndexEntry => gitIndexEntry.Path.Replace('\\', '/'))
            .Distinct(StringComparer.Ordinal)
            .ToList();

        foreach (var sTrackedPath in lstTrackedPaths)
        {
            if (hsKeepPaths.Contains(sTrackedPath))
                continue;

            var sAbsolutePath = Path.Combine(gitRepository.Info.WorkingDirectory, sTrackedPath.Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(sAbsolutePath))
                File.Delete(sAbsolutePath);
        }

        DeleteEmptyDirectories(gitRepository.Info.WorkingDirectory);

        var lstChangedPaths = gitRepository.RetrieveStatus()
            .Where(gitStatusEntry => gitStatusEntry.State != FileStatus.Unaltered && gitStatusEntry.State != FileStatus.Ignored)
            .Select(gitStatusEntry => gitStatusEntry.FilePath)
            .ToList();

        if (lstChangedPaths.Count > 0)
            Commands.Stage(gitRepository, lstChangedPaths);
    }

    private static void DeleteEmptyDirectories(string sWorkingDirectory)
    {
        var sGitDirectory = Path.GetFullPath(Path.Combine(sWorkingDirectory, ".git"));
        var lstDirectories = Directory
            .EnumerateDirectories(sWorkingDirectory, "*", SearchOption.AllDirectories)
            .Select(Path.GetFullPath)
            .Where(sDirectory => !sDirectory.StartsWith(sGitDirectory, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(static sDirectory => sDirectory.Length)
            .ToList();

        foreach (var sDirectory in lstDirectories)
        {
            if (!Directory.EnumerateFileSystemEntries(sDirectory).Any())
                Directory.Delete(sDirectory);
        }
    }
}
