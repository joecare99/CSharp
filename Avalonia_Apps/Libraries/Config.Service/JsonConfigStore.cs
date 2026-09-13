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
    private readonly IConfigSectionRegistry _registry;
    private readonly string _basePath;
    private readonly JsonSerializerOptions _options;

    public JsonConfigStore(IConfigSectionRegistry registry, string vendorName = "Vendor", string applicationName = "Application")
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));

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
        var keyPath = Path.Combine(_basePath, $"{sectionName}.json");

        if (!File.Exists(keyPath))
        {
            return fallbackValue;
        }

        try
        {
            var json = await File.ReadAllTextAsync(keyPath);
            var loaded = JsonSerializer.Deserialize<T>(json, _options)!;
            return loaded ?? fallbackValue;
        }
        catch (JsonException)
        {
            return fallbackValue;
        }
    }

    /// <inheritdoc/>
    public async Task SaveAsync<T>(string sectionName, T value)
    {
        var keyPath = Path.Combine(_basePath, $"{sectionName}.json");
        var json = JsonSerializer.Serialize(value, _options);
        await File.WriteAllTextAsync(keyPath, json);
    }

    /// <inheritdoc/>
    public async Task ResetAsync(string sectionName)
    {
        var keyPath = Path.Combine(_basePath, $"{sectionName}.json");

        if (File.Exists(keyPath))
        {
            try
            {
                await File.WriteAllTextAsync(keyPath, "");
            }
            catch
            {
                // Ignore deletion errors; the store remains consistent with its current state.
            }
        }
    }
}
