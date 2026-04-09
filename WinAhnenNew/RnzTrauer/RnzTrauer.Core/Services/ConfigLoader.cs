using System.Text.Json;

namespace RnzTrauer.Core;

/// <summary>
/// Provides JSON-based configuration loading for the ported tools.
/// </summary>
public static class ConfigLoader
{
    private static readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    /// <summary>
    /// Loads a configuration instance from the specified JSON file.
    /// </summary>
    public static T Load<T>(string sFilePath) where T : new()
    {
        if (!File.Exists(sFilePath))
        {
            throw new FileNotFoundException($"Configuration file was not found: {sFilePath}");
        }

        var xConfiguration = JsonSerializer.Deserialize<T>(File.ReadAllText(sFilePath), _options);
        return xConfiguration ?? new T();
    }
}
