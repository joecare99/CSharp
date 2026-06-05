using CommandlineHelper;
using RepoMigrator.Core;
using CommandResources = global::RepoMigrator.Tools.PipelinedMigration.Properties.CommandResources;

namespace RepoMigrator.Tools.PipelinedMigration;

/// <summary>
/// Represents the command line options for the pipelined SVN-to-Git migration prototype.
/// </summary>
[CommandDescriptor(
    "pipelined-migration",
    ResourceType = typeof(CommandResources),
    DescriptionResourceName = nameof(CommandResources.PipelinedMigration_Description),
    HelpTextResourceName = nameof(CommandResources.PipelinedMigration_Help))]
public sealed class PipelinedMigrationOptions
{
    private const string SvnProviderKey = "svn";
    private const string GitProviderKey = "git";

    /// <summary>
    /// Gets or sets the SVN source URL or working copy path.
    /// </summary>
    [CommandOption(
        "--source",
        DescriptionResourceName = nameof(CommandResources.PipelinedMigration_Source_Description))]
    public string SourceUrl { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the Git target URL or repository path.
    /// </summary>
    [CommandOption(
        "--target",
        DescriptionResourceName = nameof(CommandResources.PipelinedMigration_Target_Description))]
    public string TargetUrl { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional SVN branch or trunk path.
    /// </summary>
    [CommandOption(
        "--source-branch",
        DescriptionResourceName = nameof(CommandResources.PipelinedMigration_SourceBranch_Description))]
    public string? SourceBranchOrTrunk { get; init; }

    /// <summary>
    /// Gets or sets the optional Git target branch.
    /// </summary>
    [CommandOption(
        "--target-branch",
        DescriptionResourceName = nameof(CommandResources.PipelinedMigration_TargetBranch_Description))]
    public string? TargetBranch { get; init; }

    /// <summary>
    /// Gets or sets the optional SVN user name.
    /// </summary>
    [CommandOption(
        "--source-user",
        DescriptionResourceName = nameof(CommandResources.PipelinedMigration_SourceUser_Description))]
    public string? SourceUser { get; init; }

    /// <summary>
    /// Gets or sets the optional SVN password.
    /// </summary>
    [CommandOption(
        "--source-password",
        DescriptionResourceName = nameof(CommandResources.PipelinedMigration_SourcePassword_Description))]
    public string? SourcePassword { get; init; }

    /// <summary>
    /// Gets or sets the optional Git user name.
    /// </summary>
    [CommandOption(
        "--target-user",
        DescriptionResourceName = nameof(CommandResources.PipelinedMigration_TargetUser_Description))]
    public string? TargetUser { get; init; }

    /// <summary>
    /// Gets or sets the optional Git password.
    /// </summary>
    [CommandOption(
        "--target-password",
        DescriptionResourceName = nameof(CommandResources.PipelinedMigration_TargetPassword_Description))]
    public string? TargetPassword { get; init; }

    /// <summary>
    /// Gets or sets the optional exclusive lower changeset boundary.
    /// </summary>
    [CommandOption(
        "--from",
        DescriptionResourceName = nameof(CommandResources.PipelinedMigration_From_Description))]
    public string? FromId { get; init; }

    /// <summary>
    /// Gets or sets the optional inclusive upper changeset boundary.
    /// </summary>
    [CommandOption(
        "--to",
        DescriptionResourceName = nameof(CommandResources.PipelinedMigration_To_Description))]
    public string? ToId { get; init; }

    /// <summary>
    /// Gets or sets the optional maximum number of revisions to process.
    /// </summary>
    [CommandOption(
        "--max-count",
        DescriptionResourceName = nameof(CommandResources.PipelinedMigration_MaxCount_Description))]
    public int? MaxCount { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether the oldest revisions are processed first.
    /// </summary>
    public bool OldestFirst { get; init; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the newest revisions are processed first.
    /// </summary>
    [CommandFlag(
        "--newest-first",
        DescriptionResourceName = nameof(CommandResources.PipelinedMigration_NewestFirst_Description))]
    public bool NewestFirst { get; init; }

    /// <summary>
    /// Gets or sets the maximum number of exported snapshots to keep ahead of the commit stage.
    /// </summary>
    [CommandOption(
        "--prefetch",
        DescriptionResourceName = nameof(CommandResources.PipelinedMigration_Prefetch_Description))]
    public int PrefetchCount { get; init; } = 3;

    /// <summary>
    /// Gets or sets the number of concurrent SVN export workers.
    /// </summary>
    [CommandOption(
        "--export-workers",
        DescriptionResourceName = nameof(CommandResources.PipelinedMigration_ExportWorkers_Description))]
    public int MaxExportWorkers { get; init; } = 2;

    /// <summary>
    /// Gets or sets the optional temporary working root.
    /// </summary>
    [CommandOption(
        "--temp-root",
        DescriptionResourceName = nameof(CommandResources.PipelinedMigration_TempRoot_Description))]
    public string? TempRoot { get; init; }

    /// <summary>
    /// Parses the command line arguments into a validated options instance.
    /// </summary>
    /// <param name="arrArgs">The raw command line arguments.</param>
    /// <returns>A validated options instance, or <see langword="null" /> when help should be shown.</returns>
    public static PipelinedMigrationOptions? Parse(string[] arrArgs)
    {
        if (arrArgs.Length == 0 || arrArgs.Any(sArg => string.Equals(sArg, "/?", StringComparison.OrdinalIgnoreCase)))
            return null;

        var parseResult = PipelinedMigrationOptionsCommand.Parse(arrArgs);
        if (parseResult.RequestHelp)
            return null;

        if (!parseResult.Success)
            throw new ArgumentException(parseResult.ErrorMessage);

        var generatedOptions = parseResult.Options!;
        var options = new PipelinedMigrationOptions
        {
            SourceUrl = generatedOptions.SourceUrl,
            TargetUrl = generatedOptions.TargetUrl,
            SourceBranchOrTrunk = generatedOptions.SourceBranchOrTrunk,
            TargetBranch = generatedOptions.TargetBranch,
            SourceUser = generatedOptions.SourceUser,
            SourcePassword = generatedOptions.SourcePassword,
            TargetUser = generatedOptions.TargetUser,
            TargetPassword = generatedOptions.TargetPassword,
            FromId = generatedOptions.FromId,
            ToId = generatedOptions.ToId,
            MaxCount = generatedOptions.MaxCount,
            OldestFirst = !generatedOptions.NewestFirst,
            NewestFirst = generatedOptions.NewestFirst,
            PrefetchCount = generatedOptions.PrefetchCount,
            MaxExportWorkers = generatedOptions.MaxExportWorkers,
            TempRoot = generatedOptions.TempRoot
        };

        options.Validate();
        return options;
    }

    /// <summary>
    /// Converts the source settings into a repository endpoint.
    /// </summary>
    /// <returns>The SVN source endpoint.</returns>
    public RepositoryEndpoint CreateSourceEndpoint()
        => new()
        {
            ProviderKey = SvnProviderKey,
            UrlOrPath = SourceUrl,
            BranchOrTrunk = SourceBranchOrTrunk,
            Credentials = new RepoCredentials { Username = SourceUser, Password = SourcePassword }
        };

    /// <summary>
    /// Converts the target settings into a repository endpoint.
    /// </summary>
    /// <returns>The Git target endpoint.</returns>
    public RepositoryEndpoint CreateTargetEndpoint()
        => new()
        {
            ProviderKey = GitProviderKey,
            UrlOrPath = TargetUrl,
            BranchOrTrunk = TargetBranch,
            Credentials = new RepoCredentials { Username = TargetUser, Password = TargetPassword }
        };

    /// <summary>
    /// Converts the selected boundaries into a changeset query.
    /// </summary>
    /// <returns>The changeset query.</returns>
    public ChangeSetQuery CreateQuery()
        => new()
        {
            FromExclusiveId = FromId,
            ToInclusiveId = ToId,
            MaxCount = MaxCount,
            OldestFirst = OldestFirst
        };

    /// <summary>
    /// Validates the options.
    /// </summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(SourceUrl))
            throw new ArgumentException("The source URL or path must be provided.");

        if (string.IsNullOrWhiteSpace(TargetUrl))
            throw new ArgumentException("The target URL or path must be provided.");

        if (PrefetchCount < 1)
            throw new ArgumentException("The prefetch count must be greater than zero.");

        if (MaxExportWorkers < 1)
            throw new ArgumentException("The export worker count must be greater than zero.");

        if (MaxCount is < 1)
            throw new ArgumentException("The max count must be greater than zero when specified.");
    }

}
