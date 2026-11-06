using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Config.Service;

/// <summary>
/// Imports a legacy section JSON file into the user layer without modifying its source.
/// </summary>
public sealed class ConfigLayerLegacyJsonMigrator
{
    private static readonly JsonSerializerOptions s_legacyOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private readonly IConfigSectionRegistry _sectionRegistry;

    public ConfigLayerLegacyJsonMigrator(IConfigSectionRegistry sectionRegistry)
    {
        _sectionRegistry = sectionRegistry ?? throw new ArgumentNullException(nameof(sectionRegistry));
    }

    /// <summary>
    /// Imports a legacy per-section JSON document into a user-layer store. Existing layer
    /// values are never replaced, and the source document is never modified or deleted.
    /// </summary>
    public async Task<ConfigLayerLegacyMigrationResult> MigrateSectionToUserLayerAsync(
        string legacyFilePath,
        string sectionName,
        IConfigLayerStore userStore,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(legacyFilePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(sectionName);
        ArgumentNullException.ThrowIfNull(userStore);
        if (userStore.Layer != ConfigLayer.User)
        {
            throw new ArgumentException("Legacy settings can only be imported into the user layer.", nameof(userStore));
        }

        if (!_sectionRegistry.TryGetSection(sectionName, out IConfigSectionProvider? section)
            || section is null)
        {
            throw new KeyNotFoundException($"Configuration section '{sectionName}' is not registered.");
        }

        string fullPath = Path.GetFullPath(legacyFilePath);
        cancellationToken.ThrowIfCancellationRequested();
        ConfigLayerSnapshot destination = await userStore.LoadAsync(cancellationToken).ConfigureAwait(false);
        ConfigPropertyKey migrationKey = CreateMigrationKey(fullPath, section.Name);
        if (destination.TryGetValue(migrationKey, out ConfigLayerValue? migrationState))
        {
            if (migrationState.Value is JsonElement { ValueKind: JsonValueKind.True })
            {
                return new ConfigLayerLegacyMigrationResult(
                    SourceFound: true,
                    DestinationChanged: false,
                    ImportedPropertyCount: 0,
                    ExistingDestinationPropertyCount: 0,
                    UnmappedPropertyCount: 0);
            }

            throw new InvalidDataException("The legacy configuration migration marker is invalid.");
        }

        if (!File.Exists(fullPath))
        {
            return new ConfigLayerLegacyMigrationResult(
                SourceFound: false,
                DestinationChanged: false,
                ImportedPropertyCount: 0,
                ExistingDestinationPropertyCount: 0,
                UnmappedPropertyCount: 0);
        }

        byte[] sourceBytes = await File.ReadAllBytesAsync(fullPath, cancellationToken).ConfigureAwait(false);
        Dictionary<string, string> propertyNames = CreateSerializedPropertyMap(section.ModelType);
        Dictionary<ConfigPropertyKey, ConfigLayerValue> importedValues = ParseValues(
            sourceBytes,
            section.Name,
            propertyNames,
            out int unmappedPropertyCount);

        Dictionary<ConfigPropertyKey, ConfigLayerValue> mergedValues = new(destination.Values);
        int existingDestinationPropertyCount = 0;
        int importedPropertyCount = 0;
        foreach (KeyValuePair<ConfigPropertyKey, ConfigLayerValue> entry in importedValues)
        {
            if (mergedValues.ContainsKey(entry.Key))
            {
                existingDestinationPropertyCount++;
                continue;
            }

            mergedValues.Add(entry.Key, entry.Value);
            importedPropertyCount++;
        }

        mergedValues.Add(migrationKey, new ConfigLayerValue(true));
        cancellationToken.ThrowIfCancellationRequested();
        ConfigLayerSnapshot updated = new(ConfigLayer.User, mergedValues, destination.StorageVersion);
        await userStore.SaveAsync(updated, cancellationToken).ConfigureAwait(false);
        return new ConfigLayerLegacyMigrationResult(
            SourceFound: true,
            DestinationChanged: true,
            ImportedPropertyCount: importedPropertyCount,
            ExistingDestinationPropertyCount: existingDestinationPropertyCount,
            UnmappedPropertyCount: unmappedPropertyCount);
    }

    private static ConfigPropertyKey CreateMigrationKey(string sourcePath, string sectionName)
    {
        string sourceHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(sourcePath)));
        return new ConfigPropertyKey("__ConfigMigration", $"legacy-json:{sectionName}:{sourceHash}");
    }

    private static Dictionary<ConfigPropertyKey, ConfigLayerValue> ParseValues(
        byte[] sourceBytes,
        string sectionName,
        IReadOnlyDictionary<string, string> propertyNames,
        out int unmappedPropertyCount)
    {
        JsonDocument document;
        try
        {
            document = JsonDocument.Parse(sourceBytes);
        }
        catch (JsonException exception)
        {
            throw new InvalidDataException("The legacy configuration section contains invalid JSON.", exception);
        }

        using (document)
        {
            if (document.RootElement.ValueKind != JsonValueKind.Object)
            {
                throw new InvalidDataException("The legacy configuration section must contain a JSON object.");
            }

            Dictionary<ConfigPropertyKey, ConfigLayerValue> values = [];
            unmappedPropertyCount = 0;
            foreach (JsonProperty property in document.RootElement.EnumerateObject())
            {
                bool isMapped = propertyNames.TryGetValue(property.Name, out string? modelPropertyName);
                string destinationPropertyName = isMapped ? modelPropertyName! : property.Name;
                ConfigPropertyKey key = new(sectionName, destinationPropertyName);
                if (!values.TryAdd(key, new ConfigLayerValue(property.Value.Clone())))
                {
                    throw new InvalidDataException(
                        $"The legacy configuration section contains duplicate values for '{sectionName}.{destinationPropertyName}'.");
                }

                if (!isMapped)
                {
                    unmappedPropertyCount++;
                }
            }

            return values;
        }
    }

    private static Dictionary<string, string> CreateSerializedPropertyMap(Type modelType)
    {
        Dictionary<string, string> propertyNames = new(StringComparer.Ordinal);
        foreach (PropertyInfo property in modelType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (property.GetIndexParameters().Length != 0 || property.GetMethod is null || !property.GetMethod.IsPublic)
            {
                continue;
            }

            JsonPropertyNameAttribute? customName = property.GetCustomAttribute<JsonPropertyNameAttribute>();
            string serializedName = customName?.Name
                ?? s_legacyOptions.PropertyNamingPolicy?.ConvertName(property.Name)
                ?? property.Name;
            if (!propertyNames.TryAdd(serializedName, property.Name))
            {
                throw new InvalidOperationException(
                    $"Configuration model '{modelType}' has multiple properties serialized as '{serializedName}'.");
            }
        }

        return propertyNames;
    }
}

/// <summary>Describes the result of importing one legacy configuration section.</summary>
public sealed record ConfigLayerLegacyMigrationResult(
    bool SourceFound,
    bool DestinationChanged,
    int ImportedPropertyCount,
    int ExistingDestinationPropertyCount,
    int UnmappedPropertyCount);
