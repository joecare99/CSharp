using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Config.Service;

/// <summary>
/// Stores a versioned set of explicit configuration overrides in one JSON file.
/// This store does not establish operating-system ACLs or make policy files trusted.
/// </summary>
public sealed class JsonConfigLayerStore : IConfigLayerStore
{
    private const int CurrentSchemaVersion = 1;
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> s_pathLocks = new(
        OperatingSystem.IsWindows() ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
    private readonly string _filePath;
    private readonly SemaphoreSlim _pathLock;
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public JsonConfigLayerStore(ConfigLayer layer, string filePath)
    {
        if (!Enum.IsDefined(layer))
        {
            throw new ArgumentOutOfRangeException(nameof(layer));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        Layer = layer;
        _filePath = Path.GetFullPath(filePath);
        _pathLock = s_pathLocks.GetOrAdd(_filePath, static _ => new SemaphoreSlim(1, 1));
    }

    public ConfigLayer Layer { get; }

    /// <summary>Gets the explicitly configured file path for this layer.</summary>
    public string FilePath => _filePath;

    public async Task<ConfigLayerSnapshot> LoadAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureNoReparsePoints(_filePath);
        if (!File.Exists(_filePath))
        {
            return new ConfigLayerSnapshot(Layer, [], storageVersion: null);
        }

        byte[] content = await File.ReadAllBytesAsync(_filePath, cancellationToken).ConfigureAwait(false);
        using JsonDocument document = ParseDocument(content);
        return ParseSnapshot(document.RootElement, Convert.ToHexString(SHA256.HashData(content)));
    }

    public async Task<ConfigLayerSnapshot> SaveAsync(
        ConfigLayerSnapshot snapshot,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        if (snapshot.Layer != Layer)
        {
            throw new ArgumentException($"This store only accepts {Layer} layer snapshots.", nameof(snapshot));
        }

        await _pathLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            EnsureNoReparsePoints(_filePath);
            string directory = Path.GetDirectoryName(_filePath)
                ?? throw new InvalidOperationException("The configuration layer file must have a parent directory.");
            if (!Directory.Exists(directory))
            {
                throw new DirectoryNotFoundException($"Configuration layer directory '{directory}' does not exist.");
            }

            string? actualVersion = await GetCurrentVersionAsync(cancellationToken).ConfigureAwait(false);
            if (!string.Equals(actualVersion, snapshot.StorageVersion, StringComparison.Ordinal))
            {
                throw new IOException($"Configuration layer file '{_filePath}' changed since it was loaded. Reload it before saving.");
            }

            byte[] content = SerializeSnapshot(snapshot);
            string temporaryPath = Path.Combine(directory, $".{Path.GetFileName(_filePath)}.{Guid.NewGuid():N}.tmp");
            try
            {
                await File.WriteAllBytesAsync(temporaryPath, content, cancellationToken).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
                EnsureNoReparsePoints(_filePath);
                string finalPathVersion = await GetCurrentVersionAsync(cancellationToken).ConfigureAwait(false);
                if (!string.Equals(finalPathVersion, snapshot.StorageVersion, StringComparison.Ordinal))
                {
                    throw new IOException($"Configuration layer file '{_filePath}' changed while it was being saved. Reload it before retrying.");
                }

                File.Move(temporaryPath, _filePath, overwrite: true);
            }
            finally
            {
                if (File.Exists(temporaryPath))
                {
                    File.Delete(temporaryPath);
                }
            }

            return new ConfigLayerSnapshot(
                Layer,
                snapshot.Values,
                Convert.ToHexString(SHA256.HashData(content)));
        }
        finally
        {
            _pathLock.Release();
        }
    }

    private async Task<string?> GetCurrentVersionAsync(CancellationToken cancellationToken)
    {
        EnsureNoReparsePoints(_filePath);
        if (!File.Exists(_filePath))
        {
            return null;
        }

        byte[] bytes = await File.ReadAllBytesAsync(_filePath, cancellationToken).ConfigureAwait(false);
        return Convert.ToHexString(SHA256.HashData(bytes));
    }

    private byte[] SerializeSnapshot(ConfigLayerSnapshot snapshot)
    {
        using MemoryStream stream = new();
        using (Utf8JsonWriter writer = new(stream, new JsonWriterOptions { Indented = true }))
        {
            writer.WriteStartObject();
            writer.WriteNumber("schemaVersion", CurrentSchemaVersion);
            writer.WriteString("layer", Layer.ToString());
            writer.WriteStartArray("values");
            foreach (KeyValuePair<ConfigPropertyKey, ConfigLayerValue> entry in snapshot.Values)
            {
                writer.WriteStartObject();
                writer.WriteString("section", entry.Key.SectionName);
                writer.WriteString("property", entry.Key.PropertyName);
                writer.WritePropertyName("value");
                JsonSerializer.Serialize(writer, entry.Value.Value, entry.Value.Value?.GetType() ?? typeof(object), _serializerOptions);
                if (entry.Value.IsEnforced)
                {
                    writer.WriteBoolean("enforced", true);
                }

                writer.WriteEndObject();
            }

            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        return stream.ToArray();
    }

    private JsonDocument ParseDocument(byte[] content)
    {
        try
        {
            return JsonDocument.Parse(content);
        }
        catch (JsonException exception)
        {
            throw new InvalidDataException($"Configuration layer file '{_filePath}' contains invalid JSON.", exception);
        }
    }

    private ConfigLayerSnapshot ParseSnapshot(JsonElement root, string storageVersion)
    {
        if (root.ValueKind != JsonValueKind.Object
            || !root.TryGetProperty("schemaVersion", out JsonElement schemaVersion)
            || schemaVersion.ValueKind != JsonValueKind.Number
            || !schemaVersion.TryGetInt32(out int version)
            || version != CurrentSchemaVersion)
        {
            throw new InvalidDataException($"Configuration layer file '{_filePath}' has an unsupported or missing schema version.");
        }

        if (!root.TryGetProperty("layer", out JsonElement layerElement)
            || layerElement.ValueKind != JsonValueKind.String
            || !Enum.TryParse(layerElement.GetString(), ignoreCase: false, out ConfigLayer fileLayer)
            || fileLayer != Layer)
        {
            throw new InvalidDataException($"Configuration layer file '{_filePath}' does not contain the expected '{Layer}' layer.");
        }

        if (!root.TryGetProperty("values", out JsonElement valuesElement)
            || valuesElement.ValueKind != JsonValueKind.Array)
        {
            throw new InvalidDataException($"Configuration layer file '{_filePath}' does not contain a values array.");
        }

        List<KeyValuePair<ConfigPropertyKey, ConfigLayerValue>> values = [];
        foreach (JsonElement entry in valuesElement.EnumerateArray())
        {
            if (entry.ValueKind != JsonValueKind.Object
                || !entry.TryGetProperty("section", out JsonElement sectionElement)
                || sectionElement.ValueKind != JsonValueKind.String
                || !entry.TryGetProperty("property", out JsonElement propertyElement)
                || propertyElement.ValueKind != JsonValueKind.String
                || !entry.TryGetProperty("value", out JsonElement valueElement))
            {
                throw new InvalidDataException($"Configuration layer file '{_filePath}' contains an incomplete property entry.");
            }

            string section = sectionElement.GetString()!;
            string property = propertyElement.GetString()!;
            bool enforced = entry.TryGetProperty("enforced", out JsonElement enforcedElement)
                && enforcedElement.ValueKind == JsonValueKind.True;
            if (entry.TryGetProperty("enforced", out enforcedElement)
                && enforcedElement.ValueKind is not JsonValueKind.True and not JsonValueKind.False)
            {
                throw new InvalidDataException($"Configuration layer file '{_filePath}' contains a non-boolean enforced flag.");
            }

            values.Add(new KeyValuePair<ConfigPropertyKey, ConfigLayerValue>(
                new ConfigPropertyKey(section, property),
                new ConfigLayerValue(valueElement.Clone(), enforced)));
        }

        try
        {
            return new ConfigLayerSnapshot(Layer, values, storageVersion);
        }
        catch (ArgumentException exception)
        {
            throw new InvalidDataException($"Configuration layer file '{_filePath}' contains invalid property entries.", exception);
        }
    }

    private static void EnsureNoReparsePoints(string path)
    {
        string? currentPath = Path.GetDirectoryName(path);
        while (!string.IsNullOrEmpty(currentPath))
        {
            if (Directory.Exists(currentPath)
                && (File.GetAttributes(currentPath) & FileAttributes.ReparsePoint) != 0)
            {
                throw new IOException($"Configuration layer path '{currentPath}' must not contain a symbolic link or reparse point.");
            }

            string? parent = Path.GetDirectoryName(currentPath);
            if (string.Equals(parent, currentPath, OperatingSystem.IsWindows()
                    ? StringComparison.OrdinalIgnoreCase
                    : StringComparison.Ordinal))
            {
                break;
            }

            currentPath = parent;
        }

        if (File.Exists(path) && (File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0)
        {
            throw new IOException($"Configuration layer file '{path}' must not be a symbolic link or reparse point.");
        }
    }
}
