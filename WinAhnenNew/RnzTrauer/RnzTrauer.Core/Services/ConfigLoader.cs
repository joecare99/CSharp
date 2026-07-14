using RnzTrauer.Core.Services.Interfaces;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RnzTrauer.Core;

/// <summary>
/// Provides JSON-based configuration loading for the ported tools.
/// </summary>
public sealed class ConfigLoader : IConfigLoader
{
    private static readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly IFile _xFile;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigLoader"/> class.
    /// </summary>
    public ConfigLoader(IFile xFile)
    {
        _xFile = xFile ?? throw new ArgumentNullException(nameof(xFile));
    }

    /// <summary>
    /// Loads a configuration instance from the specified JSON file.
    /// </summary>
    public T Load<T>(string sFilePath) where T : new()
    {
        if (!_xFile.Exists(sFilePath))
        {
            throw new FileNotFoundException($"Configuration file was not found: {sFilePath}");
        }

        var xConfiguration = JsonSerializer.Deserialize<T>(_xFile.ReadAllText(sFilePath), _options);
        return xConfiguration ?? new T();
    }
}
