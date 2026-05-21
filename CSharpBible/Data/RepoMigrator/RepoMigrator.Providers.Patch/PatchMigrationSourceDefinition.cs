using RepoMigrator.Core;

namespace RepoMigrator.Providers.Patch;

/// <summary>
/// Represents the patch-specific source configuration used by the read-only patch driver.
/// </summary>
public sealed class PatchMigrationSourceDefinition
{
    private const string LocationKindKey = "Patch.LocationKind";
    private const string LocationKey = "Patch.Location";
    private const string AllowedExtensionsKey = "Patch.AllowedExtensions";
    private const string RecursiveDirectoryScanKey = "Patch.RecursiveDirectoryScan";
    private const string PathRewritePrefix = "Patch.PathRewrite.";

    /// <summary>
    /// Gets the location kind that identifies how the patch collection should be accessed.
    /// </summary>
    public PatchSourceLocationKind LocationKind { get; init; } = PatchSourceLocationKind.LocalDirectory;

    /// <summary>
    /// Gets the source location as a directory path.
    /// </summary>
    public string Location { get; init; } = string.Empty;

    /// <summary>
    /// Gets the allowed patch file extensions that discovery should consider.
    /// </summary>
    public IReadOnlyList<string> AllowedExtensions { get; init; } = Array.Empty<string>();

    /// <summary>
    /// Gets a value indicating whether local directory discovery should recurse into subdirectories.
    /// </summary>
    public bool RecursiveDirectoryScan { get; init; }

    /// <summary>
    /// Gets the explicit path rewrite rules that should be applied while normalizing patch paths.
    /// </summary>
    public IReadOnlyList<PathRewriteRule> PathRewrites { get; init; } = Array.Empty<PathRewriteRule>();

    /// <summary>
    /// Converts the patch source definition into a normalized migration source definition.
    /// </summary>
    public MigrationSourceDefinition ToMigrationSourceDefinition()
        => new()
        {
            Kind = MigrationSourceKind.PatchCollection,
            ProviderData = ToProviderData()
        };

    /// <summary>
    /// Converts the patch source definition into provider-specific normalized values.
    /// </summary>
    public IReadOnlyDictionary<string, string> ToProviderData()
    {
        var providerData = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            [LocationKindKey] = LocationKind.ToString(),
            [LocationKey] = Location,
            [AllowedExtensionsKey] = string.Join("|", AllowedExtensions),
            [RecursiveDirectoryScanKey] = RecursiveDirectoryScan.ToString()
        };

        for (var i = 0; i < PathRewrites.Count; i++)
        {
            var prefix = PathRewritePrefix + i + ".";
            providerData[prefix + nameof(PathRewriteRule.FromPrefix)] = PathRewrites[i].FromPrefix;
            providerData[prefix + nameof(PathRewriteRule.ToPrefix)] = PathRewrites[i].ToPrefix;
            providerData[prefix + nameof(PathRewriteRule.NormalizeDirectorySeparators)] = PathRewrites[i].NormalizeDirectorySeparators.ToString();
            providerData[prefix + nameof(PathRewriteRule.IgnoreCase)] = PathRewrites[i].IgnoreCase.ToString();
        }

        return providerData;
    }

    /// <summary>
    /// Reconstructs a patch source definition from provider-specific normalized values.
    /// </summary>
    public static PatchMigrationSourceDefinition FromProviderData(IReadOnlyDictionary<string, string> providerData)
    {
        ArgumentNullException.ThrowIfNull(providerData);

        return new PatchMigrationSourceDefinition
        {
            LocationKind = Enum.TryParse<PatchSourceLocationKind>(GetRequiredValue(providerData, LocationKindKey), ignoreCase: true, out var locationKind)
                ? locationKind
                : PatchSourceLocationKind.LocalDirectory,
            Location = GetRequiredValue(providerData, LocationKey),
            AllowedExtensions = GetOptionalValue(providerData, AllowedExtensionsKey)
                .Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
            RecursiveDirectoryScan = bool.TryParse(GetOptionalValue(providerData, RecursiveDirectoryScanKey), out var recursiveDirectoryScan) && recursiveDirectoryScan,
            PathRewrites = ReadPathRewrites(providerData)
        };
    }

    /// <summary>
    /// Reconstructs a patch source definition from a normalized migration source definition.
    /// </summary>
    public static PatchMigrationSourceDefinition FromMigrationSourceDefinition(MigrationSourceDefinition source)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (source.Kind != MigrationSourceKind.PatchCollection)
            throw new NotSupportedException("The supplied source definition is not patch-backed.");

        return FromProviderData(source.ProviderData);
    }

    private static IReadOnlyList<PathRewriteRule> ReadPathRewrites(IReadOnlyDictionary<string, string> providerData)
    {
        var rewrites = new List<PathRewriteRule>();
        for (var i = 0; ; i++)
        {
            var prefix = PathRewritePrefix + i + ".";
            if (!providerData.TryGetValue(prefix + nameof(PathRewriteRule.FromPrefix), out var fromPrefix))
                break;

            providerData.TryGetValue(prefix + nameof(PathRewriteRule.ToPrefix), out var toPrefix);
            providerData.TryGetValue(prefix + nameof(PathRewriteRule.NormalizeDirectorySeparators), out var normalizeSeparatorsValue);
            providerData.TryGetValue(prefix + nameof(PathRewriteRule.IgnoreCase), out var ignoreCaseValue);
            rewrites.Add(new PathRewriteRule
            {
                FromPrefix = fromPrefix ?? string.Empty,
                ToPrefix = toPrefix ?? string.Empty,
                NormalizeDirectorySeparators = bool.TryParse(normalizeSeparatorsValue, out var normalizeSeparators) && normalizeSeparators,
                IgnoreCase = bool.TryParse(ignoreCaseValue, out var ignoreCase) && ignoreCase
            });
        }

        return rewrites;
    }

    private static string GetRequiredValue(IReadOnlyDictionary<string, string> providerData, string key)
        => providerData.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value)
            ? value
            : throw new InvalidOperationException($"Missing patch source provider value '{key}'.");

    private static string GetOptionalValue(IReadOnlyDictionary<string, string> providerData, string key)
        => providerData.TryGetValue(key, out var value)
            ? value
            : string.Empty;
}
