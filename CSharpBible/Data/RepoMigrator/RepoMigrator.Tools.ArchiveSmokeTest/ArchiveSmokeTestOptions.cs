namespace RepoMigrator.Tools.ArchiveSmokeTest;

/// <summary>
/// Represents command-line options for the archive smoke-test tool.
/// </summary>
public sealed class ArchiveSmokeTestOptions
{
    /// <summary>
    /// Gets the source directory that contains archive snapshots.
    /// </summary>
    public required string SourceDirectoryPath { get; init; }

    /// <summary>
    /// Gets a value indicating whether subdirectories should be scanned recursively.
    /// </summary>
    public bool Recursive { get; init; }

    /// <summary>
    /// Gets the allowed archive extensions.
    /// </summary>
    public IReadOnlyList<string> AllowedExtensions { get; init; } = Array.Empty<string>();

    /// <summary>
    /// Gets the workspace root path used for DevOps plan persistence.
    /// </summary>
    public required string WorkspaceRootPath { get; init; }

    /// <summary>
    /// Parses the command-line options.
    /// </summary>
    public static ArchiveSmokeTestOptions? Parse(string[] arrArgs)
    {
        ArgumentNullException.ThrowIfNull(arrArgs);

        if (arrArgs.Length == 0 || arrArgs.Any(static arg => string.Equals(arg, "--help", StringComparison.OrdinalIgnoreCase) || string.Equals(arg, "-h", StringComparison.OrdinalIgnoreCase)))
            return null;

        string? sourceDirectoryPath = null;
        string workspaceRootPath = Directory.GetCurrentDirectory();
        var recursive = false;
        var allowedExtensions = new List<string>();

        for (var i = 0; i < arrArgs.Length; i++)
        {
            var arg = arrArgs[i];
            switch (arg)
            {
                case "--source":
                    sourceDirectoryPath = GetRequiredValue(arrArgs, ref i, arg);
                    break;
                case "--workspace":
                    workspaceRootPath = GetRequiredValue(arrArgs, ref i, arg);
                    break;
                case "--recursive":
                    recursive = true;
                    break;
                case "--extension":
                    allowedExtensions.Add(GetRequiredValue(arrArgs, ref i, arg));
                    break;
                default:
                    throw new ArgumentException($"Unknown argument '{arg}'.");
            }
        }

        if (string.IsNullOrWhiteSpace(sourceDirectoryPath))
            throw new ArgumentException("The --source argument is required.");

        return new ArchiveSmokeTestOptions
        {
            SourceDirectoryPath = Path.GetFullPath(sourceDirectoryPath),
            WorkspaceRootPath = Path.GetFullPath(workspaceRootPath),
            Recursive = recursive,
            AllowedExtensions = allowedExtensions
        };
    }

    private static string GetRequiredValue(string[] arrArgs, ref int i, string argumentName)
    {
        if (i + 1 >= arrArgs.Length || string.IsNullOrWhiteSpace(arrArgs[i + 1]))
            throw new ArgumentException($"Missing value for '{argumentName}'.");

        i++;
        return arrArgs[i];
    }
}
