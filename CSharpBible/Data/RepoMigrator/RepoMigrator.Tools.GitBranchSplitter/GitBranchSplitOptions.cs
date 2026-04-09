namespace RepoMigrator.Tools.GitBranchSplitter;

/// <summary>
/// Captures the command-line options for the Git branch splitter tool.
/// </summary>
public sealed class GitBranchSplitOptions
{
    /// <summary>
    /// Gets the repository working directory.
    /// </summary>
    public required string RepositoryPath { get; init; }

    /// <summary>
    /// Gets the local source branch that should be split.
    /// </summary>
    public required string SourceBranch { get; init; }

    /// <summary>
    /// Gets the prefix that is prepended to generated branch names.
    /// </summary>
    public string BranchPrefix { get; init; } = "split";

    /// <summary>
    /// Gets a value indicating whether existing generated branches may be replaced.
    /// </summary>
    public bool OverwriteExistingBranches { get; init; }

    /// <summary>
    /// Gets the author name used for synthetic commits.
    /// </summary>
    public string AuthorName { get; init; } = "RepoMigrator Tool";

    /// <summary>
    /// Gets the author email used for synthetic commits.
    /// </summary>
    public string AuthorEmail { get; init; } = "tool@local";

    /// <summary>
    /// Parses the supported command-line arguments.
    /// </summary>
    /// <param name="args">The raw command-line arguments.</param>
    /// <returns>The parsed options, or <see langword="null"/> when help should be shown.</returns>
    public static GitBranchSplitOptions? Parse(string[] args)
    {
        if (args.Length == 0 || args.Any(static sArgument => sArgument is "--help" or "-h" or "/?"))
            return null;

        string? sRepositoryPath = null;
        string? sSourceBranch = null;
        var sBranchPrefix = "split";
        var sAuthorName = "RepoMigrator Tool";
        var sAuthorEmail = "tool@local";
        var xOverwriteExistingBranches = false;

        for (var iArgument = 0; iArgument < args.Length; iArgument++)
        {
            var sArgument = args[iArgument];
            switch (sArgument)
            {
                case "--repo":
                    sRepositoryPath = ReadNextValue(args, ref iArgument, sArgument);
                    break;

                case "--source":
                    sSourceBranch = ReadNextValue(args, ref iArgument, sArgument);
                    break;

                case "--prefix":
                    sBranchPrefix = ReadNextValue(args, ref iArgument, sArgument);
                    break;

                case "--author-name":
                    sAuthorName = ReadNextValue(args, ref iArgument, sArgument);
                    break;

                case "--author-email":
                    sAuthorEmail = ReadNextValue(args, ref iArgument, sArgument);
                    break;

                case "--overwrite":
                    xOverwriteExistingBranches = true;
                    break;

                default:
                    throw new ArgumentException($"Unknown argument: {sArgument}");
            }
        }

        if (string.IsNullOrWhiteSpace(sRepositoryPath))
            throw new ArgumentException("Missing required argument --repo.");

        if (string.IsNullOrWhiteSpace(sSourceBranch))
            throw new ArgumentException("Missing required argument --source.");

        return new GitBranchSplitOptions
        {
            RepositoryPath = sRepositoryPath,
            SourceBranch = sSourceBranch,
            BranchPrefix = sBranchPrefix,
            OverwriteExistingBranches = xOverwriteExistingBranches,
            AuthorName = sAuthorName,
            AuthorEmail = sAuthorEmail
        };
    }

    private static string ReadNextValue(string[] args, ref int iArgument, string sCurrentArgument)
    {
        if (iArgument + 1 >= args.Length)
            throw new ArgumentException($"Missing value for argument {sCurrentArgument}.");

        iArgument++;
        return args[iArgument];
    }
}
