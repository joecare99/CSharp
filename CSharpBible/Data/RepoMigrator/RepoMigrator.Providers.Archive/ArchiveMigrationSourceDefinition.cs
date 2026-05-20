using RepoMigrator.Core;

namespace RepoMigrator.Providers.Archive;

/// <summary>
/// Represents the archive-specific source configuration used by archive-backed migration source providers.
/// </summary>
public sealed class ArchiveMigrationSourceDefinition
{
    private const string LocationKindKey = "Archive.LocationKind";
    private const string LocationKey = "Archive.Location";
    private const string AllowedExtensionsKey = "Archive.AllowedExtensions";
    private const string AllowRelativeLinksKey = "Archive.AllowRelativeLinks";
    private const string RecursiveDirectoryScanKey = "Archive.RecursiveDirectoryScan";

    /// <summary>
    /// Gets the location kind that identifies how the archive collection should be accessed.
    /// </summary>
    public ArchiveSourceLocationKind LocationKind { get; init; } = ArchiveSourceLocationKind.LocalDirectory;

    /// <summary>
    /// Gets the source location as a URL or directory path.
    /// </summary>
    public string Location { get; init; } = string.Empty;

    /// <summary>
    /// Gets the allowed archive file extensions that discovery should consider.
    /// </summary>
    public IReadOnlyList<string> AllowedExtensions { get; init; } = Array.Empty<string>();

    /// <summary>
    /// Gets a value indicating whether relative links should be followed for HTTP-based discovery.
    /// </summary>
    public bool AllowRelativeLinks { get; init; }

    /// <summary>
    /// Gets a value indicating whether local directory discovery should recurse into subdirectories.
    /// </summary>
    public bool RecursiveDirectoryScan { get; init; }

    /// <summary>
    /// Converts the archive source definition into a normalized migration source definition.
    /// </summary>
    public MigrationSourceDefinition ToMigrationSourceDefinition()
        => new()
        {
            Kind = MigrationSourceKind.ArchiveCollection,
            ProviderData = ToProviderData()
        };

    /// <summary>
    /// Converts the archive source definition into provider-specific normalized values.
    /// </summary>
    public IReadOnlyDictionary<string, string> ToProviderData()
        => new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            [LocationKindKey] = LocationKind.ToString(),
            [LocationKey] = Location,
            [AllowedExtensionsKey] = string.Join("|", AllowedExtensions),
            [AllowRelativeLinksKey] = AllowRelativeLinks.ToString(),
            [RecursiveDirectoryScanKey] = RecursiveDirectoryScan.ToString()
        };

    /// <summary>
    /// Reconstructs an archive source definition from provider-specific normalized values.
    /// </summary>
    public static ArchiveMigrationSourceDefinition FromProviderData(IReadOnlyDictionary<string, string> providerData)
    {
        ArgumentNullException.ThrowIfNull(providerData);

        return new ArchiveMigrationSourceDefinition
        {
            LocationKind = Enum.TryParse<ArchiveSourceLocationKind>(GetRequiredValue(providerData, LocationKindKey), ignoreCase: true, out var locationKind)
                ? locationKind
                : ArchiveSourceLocationKind.LocalDirectory,
            Location = GetRequiredValue(providerData, LocationKey),
            AllowedExtensions = GetOptionalValue(providerData, AllowedExtensionsKey)
                .Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
            AllowRelativeLinks = bool.TryParse(GetOptionalValue(providerData, AllowRelativeLinksKey), out var allowRelativeLinks) && allowRelativeLinks,
            RecursiveDirectoryScan = bool.TryParse(GetOptionalValue(providerData, RecursiveDirectoryScanKey), out var recursiveDirectoryScan) && recursiveDirectoryScan
        };
    }

    /// <summary>
    /// Reconstructs an archive source definition from a normalized migration source definition.
    /// </summary>
    public static ArchiveMigrationSourceDefinition FromMigrationSourceDefinition(MigrationSourceDefinition source)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (source.Kind != MigrationSourceKind.ArchiveCollection)
            throw new NotSupportedException("The supplied source definition is not archive-backed.");

        return FromProviderData(source.ProviderData);
    }

    private static string GetRequiredValue(IReadOnlyDictionary<string, string> providerData, string key)
        => providerData.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value)
            ? value
            : throw new InvalidOperationException($"Missing archive source provider value '{key}'.");

    private static string GetOptionalValue(IReadOnlyDictionary<string, string> providerData, string key)
        => providerData.TryGetValue(key, out var value)
            ? value
            : string.Empty;
}
