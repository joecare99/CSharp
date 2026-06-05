using CommandlineHelper;
using CommandResources = global::RepoMigrator.Tools.ArchiveSmokeTest.Properties.CommandResources;

namespace RepoMigrator.Tools.ArchiveSmokeTest;

/// <summary>
/// Represents command-line options for the archive smoke-test tool.
/// </summary>
[CommandDescriptor(
    "archive-smoke-test",
    ResourceType = typeof(CommandResources),
    DescriptionResourceName = nameof(CommandResources.ArchiveSmokeTest_Description),
    HelpTextResourceName = nameof(CommandResources.ArchiveSmokeTest_Help))]
public sealed class ArchiveSmokeTestOptions
{
    /// <summary>
    /// Gets the source directory that contains archive snapshots.
    /// </summary>
    [CommandOption(
        "--source",
        DescriptionResourceName = nameof(CommandResources.ArchiveSmokeTest_Source_Description))]
    public required string SourceDirectoryPath { get; init; }

    /// <summary>
    /// Gets a value indicating whether subdirectories should be scanned recursively.
    /// </summary>
    [CommandFlag(
        "--recursive",
        DescriptionResourceName = nameof(CommandResources.ArchiveSmokeTest_Recursive_Description))]
    public bool Recursive { get; init; }

    /// <summary>
    /// Gets the allowed archive extensions.
    /// </summary>
    [CommandOption(
        "--extension",
        DescriptionResourceName = nameof(CommandResources.ArchiveSmokeTest_Extension_Description))]
    public IReadOnlyList<string> AllowedExtensions { get; init; } = Array.Empty<string>();

    /// <summary>
    /// Gets the workspace root path used for DevOps plan persistence.
    /// </summary>
    [CommandOption(
        "--workspace",
        DescriptionResourceName = nameof(CommandResources.ArchiveSmokeTest_Workspace_Description))]
    public string WorkspaceRootPath { get; init; } = string.Empty;

    /// <summary>
    /// Parses the command-line options.
    /// </summary>
    public static ArchiveSmokeTestOptions? Parse(string[] arrArgs)
    {
        if (arrArgs is null)
            throw new ArgumentNullException(nameof(arrArgs));

        if (arrArgs.Length == 0 || arrArgs.Any(static arg => string.Equals(arg, "/?", StringComparison.OrdinalIgnoreCase)))
            return null;

        var parseResult = ArchiveSmokeTestOptionsCommand.Parse(arrArgs);
        if (parseResult.RequestHelp)
            return null;

        if (!parseResult.Success)
            throw new ArgumentException(parseResult.ErrorMessage);

        var options = parseResult.Options!;
        return new ArchiveSmokeTestOptions
        {
            SourceDirectoryPath = Path.GetFullPath(options.SourceDirectoryPath),
            WorkspaceRootPath = string.IsNullOrWhiteSpace(options.WorkspaceRootPath)
                ? Directory.GetCurrentDirectory()
                : Path.GetFullPath(options.WorkspaceRootPath),
            Recursive = options.Recursive,
            AllowedExtensions = options.AllowedExtensions
        };
    }
}
