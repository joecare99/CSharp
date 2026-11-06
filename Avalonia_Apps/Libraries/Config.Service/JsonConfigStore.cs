using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Config.Service;

/// <summary>
/// Implements <see cref="IConfigStore"/> by persisting each configuration section
/// as a JSON document under the registry's base directory. Missing files fall back
/// to <see cref="IConfigSectionProvider.CreateModel"/> for default values.
/// </summary>
public sealed class JsonConfigStore : IConfigStore
{
    private readonly string _basePath;
    private readonly JsonSerializerOptions _options;

    public JsonConfigStore(IConfigSectionRegistry registry, string vendorName = "Vendor", string applicationName = "Application")
    {
        ArgumentNullException.ThrowIfNull(registry);

        var normalizedVendorName = string.IsNullOrWhiteSpace(vendorName) ? "Vendor" : vendorName.Trim();
        var normalizedApplicationName = string.IsNullOrWhiteSpace(applicationName) ? "Application" : applicationName.Trim();

        var existingBasePath = Environment.GetEnvironmentVariable("CONFIG_ROOT");
        _basePath = !string.IsNullOrEmpty(existingBasePath)
            ? Path.Combine(existingBasePath, normalizedVendorName, normalizedApplicationName, "config")
            : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), normalizedVendorName, normalizedApplicationName, "config");

        if (!Directory.Exists(_basePath))
        {
            Directory.CreateDirectory(_basePath);
        }

        _options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    /// <inheritdoc/>
    public async Task<T> LoadAsync<T>(string sectionName, T fallbackValue)
    {
        string keyPath = ResolveKeyPath(sectionName);

        if (!File.Exists(keyPath))
        {
            return fallbackValue;
        }

        try
        {
            string json = await File.ReadAllTextAsync(keyPath).ConfigureAwait(false);
            T? loaded = JsonSerializer.Deserialize<T>(json, _options);
            return loaded ?? throw new InvalidDataException($"Configuration section '{sectionName}' contains JSON null instead of a configuration value.");
        }
        catch (JsonException exception)
        {
            throw new InvalidDataException($"Configuration section '{sectionName}' contains invalid JSON.", exception);
        }
    }

    /// <inheritdoc/>
    public async Task SaveAsync<T>(string sectionName, T value)
    {
        string keyPath = ResolveKeyPath(sectionName);
        string temporaryPath = Path.Combine(_basePath, $".{Guid.NewGuid():N}.tmp");
        string json = JsonSerializer.Serialize(value, _options);
        try
        {
            await File.WriteAllTextAsync(temporaryPath, json).ConfigureAwait(false);
            File.Move(temporaryPath, keyPath, overwrite: true);
        }
        finally
        {
            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }
        }
    }

    /// <inheritdoc/>
    public async Task ResetAsync(string sectionName)
    {
        string keyPath = ResolveKeyPath(sectionName);

        if (File.Exists(keyPath))
        {
            await Task.Run(() => File.Delete(keyPath)).ConfigureAwait(false);
        }
    }

    private string ResolveKeyPath(string sectionName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sectionName);
        if (sectionName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0
            || sectionName.Contains(Path.DirectorySeparatorChar)
            || sectionName.Contains(Path.AltDirectorySeparatorChar))
        {
            throw new ArgumentException("Configuration section names must be a single safe path segment.", nameof(sectionName));
        }

        string keyPath = Path.GetFullPath(Path.Combine(_basePath, $"{sectionName}.json"));
        string basePathPrefix = Path.GetFullPath(_basePath) + Path.DirectorySeparatorChar;
        if (!keyPath.StartsWith(basePathPrefix, OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
        {
            throw new ArgumentException("The configuration section path must remain inside the configured store.", nameof(sectionName));
        }

        return keyPath;
    }
}
