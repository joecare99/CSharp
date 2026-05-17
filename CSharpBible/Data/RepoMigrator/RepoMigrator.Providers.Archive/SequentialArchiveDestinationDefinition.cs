using RepoMigrator.Core;

namespace RepoMigrator.Providers.Archive;

/// <summary>
/// Represents a future archive-output destination definition that can emit ordered snapshot archives.
/// </summary>
public sealed class SequentialArchiveDestinationDefinition
{
    private const string OutputDirectoryKey = "SequentialArchive.OutputDirectory";
    private const string ArchiveFileExtensionKey = "SequentialArchive.FileExtension";
    private const string OverwriteExistingArchivesKey = "SequentialArchive.OverwriteExistingArchives";

    /// <summary>
    /// Gets the output directory where generated archives should be written.
    /// </summary>
    public string OutputDirectory { get; init; } = string.Empty;

    /// <summary>
    /// Gets the archive file extension that should be emitted for generated snapshots.
    /// </summary>
    public string ArchiveFileExtension { get; init; } = ".zip";

    /// <summary>
    /// Gets a value indicating whether existing generated archives may be overwritten.
    /// </summary>
    public bool OverwriteExistingArchives { get; init; }

    /// <summary>
    /// Converts the destination definition into a normalized migration destination definition.
    /// </summary>
    public MigrationDestinationDefinition ToMigrationDestinationDefinition()
        => new()
        {
            Kind = MigrationDestinationKind.SequentialArchiveCollection,
            ProviderData = ToProviderData()
        };

    /// <summary>
    /// Converts the destination definition into provider-specific normalized values.
    /// </summary>
    public IReadOnlyDictionary<string, string> ToProviderData()
        => new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            [OutputDirectoryKey] = OutputDirectory,
            [ArchiveFileExtensionKey] = ArchiveFileExtension,
            [OverwriteExistingArchivesKey] = OverwriteExistingArchives.ToString()
        };

    /// <summary>
    /// Reconstructs a sequential-archive destination definition from provider-specific normalized values.
    /// </summary>
    public static SequentialArchiveDestinationDefinition FromProviderData(IReadOnlyDictionary<string, string> providerData)
    {
        ArgumentNullException.ThrowIfNull(providerData);

        return new SequentialArchiveDestinationDefinition
        {
            OutputDirectory = GetRequiredValue(providerData, OutputDirectoryKey),
            ArchiveFileExtension = GetOptionalValue(providerData, ArchiveFileExtensionKey) is { Length: > 0 } extension
                ? extension
                : ".zip",
            OverwriteExistingArchives = bool.TryParse(GetOptionalValue(providerData, OverwriteExistingArchivesKey), out var overwriteExistingArchives) && overwriteExistingArchives
        };
    }

    /// <summary>
    /// Reconstructs a sequential-archive destination definition from a normalized migration destination definition.
    /// </summary>
    public static SequentialArchiveDestinationDefinition FromMigrationDestinationDefinition(MigrationDestinationDefinition destination)
    {
        ArgumentNullException.ThrowIfNull(destination);
        if (destination.Kind != MigrationDestinationKind.SequentialArchiveCollection)
            throw new NotSupportedException("The supplied destination definition is not a sequential archive destination.");

        return FromProviderData(destination.ProviderData);
    }

    private static string GetRequiredValue(IReadOnlyDictionary<string, string> providerData, string key)
        => providerData.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value)
            ? value
            : throw new InvalidOperationException($"Missing sequential archive destination provider value '{key}'.");

    private static string GetOptionalValue(IReadOnlyDictionary<string, string> providerData, string key)
        => providerData.TryGetValue(key, out var value)
            ? value
            : string.Empty;
}
