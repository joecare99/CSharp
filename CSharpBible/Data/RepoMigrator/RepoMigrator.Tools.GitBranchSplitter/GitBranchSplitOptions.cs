using CommandlineHelper;
using CommandResources = RepoMigrator.Tools.GitBranchSplitter.Properties.CommandResources;

namespace RepoMigrator.Tools.GitBranchSplitter;

/// <summary>
/// Captures the command-line options for the Git branch splitter tool.
/// </summary>
[CommandDescriptor(
    "git-branch-splitter",
    ResourceType = typeof(CommandResources),
    DescriptionResourceName = nameof(CommandResources.GitBranchSplitter_Description),
    HelpTextResourceName = nameof(CommandResources.GitBranchSplitter_Help))]
public sealed class GitBranchSplitOptions
{
    /// <summary>
    /// Gets the repository working directory.
    /// </summary>
    [CommandOption(
        "--repo",
        DescriptionResourceName = nameof(CommandResources.GitBranchSplitter_Repo_Description))]
    public required string RepositoryPath { get; init; }

    /// <summary>
    /// Gets the local source branch that should be split.
    /// </summary>
    [CommandOption(
        "--source",
        DescriptionResourceName = nameof(CommandResources.GitBranchSplitter_Source_Description))]
    public required string SourceBranch { get; init; }

    /// <summary>
    /// Gets the prefix that is prepended to generated branch names.
    /// </summary>
    [CommandOption(
        "--prefix",
        DescriptionResourceName = nameof(CommandResources.GitBranchSplitter_Prefix_Description))]
    public string BranchPrefix { get; init; } = "split";

    /// <summary>
    /// Gets a value indicating whether existing generated branches may be replaced.
    /// </summary>
    [CommandFlag(
        "--overwrite",
        DescriptionResourceName = nameof(CommandResources.GitBranchSplitter_Overwrite_Description))]
    public bool OverwriteExistingBranches { get; init; }

    /// <summary>
    /// Gets the author name used for synthetic commits.
    /// </summary>
    [CommandOption(
        "--author-name",
        DescriptionResourceName = nameof(CommandResources.GitBranchSplitter_AuthorName_Description))]
    public string AuthorName { get; init; } = "RepoMigrator Tool";

    /// <summary>
    /// Gets the author email used for synthetic commits.
    /// </summary>
    [CommandOption(
        "--author-email",
        DescriptionResourceName = nameof(CommandResources.GitBranchSplitter_AuthorEmail_Description))]
    public string AuthorEmail { get; init; } = "tool@local";

    /// <summary>
    /// Parses the supported command-line arguments.
    /// </summary>
    /// <param name="args">The raw command-line arguments.</param>
    /// <returns>The parsed options, or <see langword="null"/> when help should be shown.</returns>
    public static GitBranchSplitOptions? Parse(string[] args)
    {
        if (args.Any(static sArgument => sArgument is "/?"))
            return null;
        var parseResult = GitBranchSplitOptionsCommand.Parse(args);
        if (parseResult.RequestHelp)
            return null;

        if (!parseResult.Success)
            throw new ArgumentException(parseResult.ErrorMessage);

        return parseResult.Options;
    }
}
