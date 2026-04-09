using RepoMigrator.Core;

namespace RepoMigrator.Tools.PipelinedMigration;

/// <summary>
/// Represents the command line options for the pipelined SVN-to-Git migration prototype.
/// </summary>
public sealed class PipelinedMigrationOptions
{
    /// <summary>
    /// Gets or sets the SVN source URL or working copy path.
    /// </summary>
    public string SourceUrl { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the Git target URL or repository path.
    /// </summary>
    public string TargetUrl { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional SVN branch or trunk path.
    /// </summary>
    public string? SourceBranchOrTrunk { get; init; }

    /// <summary>
    /// Gets or sets the optional Git target branch.
    /// </summary>
    public string? TargetBranch { get; init; }

    /// <summary>
    /// Gets or sets the optional SVN user name.
    /// </summary>
    public string? SourceUser { get; init; }

    /// <summary>
    /// Gets or sets the optional SVN password.
    /// </summary>
    public string? SourcePassword { get; init; }

    /// <summary>
    /// Gets or sets the optional Git user name.
    /// </summary>
    public string? TargetUser { get; init; }

    /// <summary>
    /// Gets or sets the optional Git password.
    /// </summary>
    public string? TargetPassword { get; init; }

    /// <summary>
    /// Gets or sets the optional exclusive lower changeset boundary.
    /// </summary>
    public string? FromId { get; init; }

    /// <summary>
    /// Gets or sets the optional inclusive upper changeset boundary.
    /// </summary>
    public string? ToId { get; init; }

    /// <summary>
    /// Gets or sets the optional maximum number of revisions to process.
    /// </summary>
    public int? MaxCount { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether the oldest revisions are processed first.
    /// </summary>
    public bool OldestFirst { get; init; } = true;

    /// <summary>
    /// Gets or sets the maximum number of exported snapshots to keep ahead of the commit stage.
    /// </summary>
    public int PrefetchCount { get; init; } = 3;

    /// <summary>
    /// Gets or sets the number of concurrent SVN export workers.
    /// </summary>
    public int MaxExportWorkers { get; init; } = 2;

    /// <summary>
    /// Gets or sets the optional temporary working root.
    /// </summary>
    public string? TempRoot { get; init; }

    /// <summary>
    /// Parses the command line arguments into a validated options instance.
    /// </summary>
    /// <param name="arrArgs">The raw command line arguments.</param>
    /// <returns>A validated options instance, or <see langword="null" /> when help should be shown.</returns>
    public static PipelinedMigrationOptions? Parse(string[] arrArgs)
    {
        if (arrArgs.Length == 0 || arrArgs.Any(sArg => string.Equals(sArg, "--help", StringComparison.OrdinalIgnoreCase) || string.Equals(sArg, "-h", StringComparison.OrdinalIgnoreCase)))
            return null;

        var dctArgs = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        for (var iIndex = 0; iIndex < arrArgs.Length; iIndex++)
        {
            var sArg = arrArgs[iIndex];
            if (!sArg.StartsWith("--", StringComparison.Ordinal))
                throw new ArgumentException($"Unexpected argument '{sArg}'.");

            if (string.Equals(sArg, "--newest-first", StringComparison.OrdinalIgnoreCase))
            {
                dctArgs[sArg] = bool.FalseString;
                continue;
            }

            if (iIndex + 1 >= arrArgs.Length || arrArgs[iIndex + 1].StartsWith("--", StringComparison.Ordinal))
                throw new ArgumentException($"Missing value for '{sArg}'.");

            dctArgs[sArg] = arrArgs[++iIndex];
        }

        if (!dctArgs.TryGetValue("--source", out var sSourceUrl) || string.IsNullOrWhiteSpace(sSourceUrl))
            throw new ArgumentException("Missing required '--source' value.");

        if (!dctArgs.TryGetValue("--target", out var sTargetUrl) || string.IsNullOrWhiteSpace(sTargetUrl))
            throw new ArgumentException("Missing required '--target' value.");

        var options = new PipelinedMigrationOptions
        {
            SourceUrl = sSourceUrl,
            TargetUrl = sTargetUrl,
            SourceBranchOrTrunk = GetOptionalValue(dctArgs, "--source-branch"),
            TargetBranch = GetOptionalValue(dctArgs, "--target-branch"),
            SourceUser = GetOptionalValue(dctArgs, "--source-user"),
            SourcePassword = GetOptionalValue(dctArgs, "--source-password"),
            TargetUser = GetOptionalValue(dctArgs, "--target-user"),
            TargetPassword = GetOptionalValue(dctArgs, "--target-password"),
            FromId = GetOptionalValue(dctArgs, "--from"),
            ToId = GetOptionalValue(dctArgs, "--to"),
            MaxCount = TryParseOptionalInt(dctArgs, "--max-count"),
            OldestFirst = !dctArgs.ContainsKey("--newest-first"),
            PrefetchCount = TryParseOptionalInt(dctArgs, "--prefetch") ?? 3,
            MaxExportWorkers = TryParseOptionalInt(dctArgs, "--export-workers") ?? 2,
            TempRoot = GetOptionalValue(dctArgs, "--temp-root")
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
            Type = RepoType.Svn,
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
            Type = RepoType.Git,
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

    private static string? GetOptionalValue(IReadOnlyDictionary<string, string?> dctArgs, string sKey)
        => dctArgs.TryGetValue(sKey, out var sValue) ? sValue : null;

    private static int? TryParseOptionalInt(IReadOnlyDictionary<string, string?> dctArgs, string sKey)
    {
        if (!dctArgs.TryGetValue(sKey, out var sValue) || string.IsNullOrWhiteSpace(sValue))
            return null;

        return int.TryParse(sValue, out var iValue)
            ? iValue
            : throw new ArgumentException($"Value '{sValue}' for '{sKey}' is not a valid integer.");
    }
}
