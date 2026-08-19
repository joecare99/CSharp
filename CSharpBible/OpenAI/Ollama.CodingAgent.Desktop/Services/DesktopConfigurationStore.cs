using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Ollama.CodingAgent.Desktop.Models;

namespace Ollama.CodingAgent.Desktop.Services;

/// <summary>
/// Persists recently used Ollama endpoint configurations in the per-user application data folder.
/// </summary>
public sealed class DesktopConfigurationStore
{
    private const int MaximumEntries = 10;
    private readonly string _filePath;
    private readonly JsonSerializerOptions _serializerOptions = new() { WriteIndented = true };

    /// <summary>
    /// Initializes a store using the default per-user application data location.
    /// </summary>
    public DesktopConfigurationStore()
        : this(Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Ollama",
            "CodingAgent",
            "desktop-configurations.json"))
    {
    }

    public DesktopConfigurationStore(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        _filePath = filePath;
    }

    /// <summary>
    /// Loads recently used configurations.
    /// </summary>
    public async Task<IReadOnlyList<DesktopConfiguration>> LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_filePath))
        {
            return [];
        }

        try
        {
            await using FileStream stream = File.OpenRead(_filePath);
            List<DesktopConfiguration>? configurations = await JsonSerializer.DeserializeAsync<List<DesktopConfiguration>>(
                stream,
                _serializerOptions,
                cancellationToken);
            return configurations?.Where(IsValid).Take(MaximumEntries).ToArray() ?? [];
        }
        catch (JsonException)
        {
            return [];
        }
        catch (IOException)
        {
            return [];
        }
    }

    /// <summary>
    /// Adds a configuration to the front of the MRU list.
    /// </summary>
    public async Task<IReadOnlyList<DesktopConfiguration>> RememberAsync(
        DesktopConfiguration configuration,
        CancellationToken cancellationToken = default)
    {
        DesktopConfiguration normalized = configuration.Normalize();
        List<DesktopConfiguration> configurations = (await LoadAsync(cancellationToken)).ToList();
        configurations.RemoveAll(item => string.Equals(item.Endpoint, normalized.Endpoint, StringComparison.OrdinalIgnoreCase)
            && string.Equals(item.WorkspacePath, normalized.WorkspacePath, StringComparison.OrdinalIgnoreCase));
        configurations.Insert(0, normalized);
        configurations = configurations.Take(MaximumEntries).ToList();

        string? directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await using FileStream stream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(stream, configurations, _serializerOptions, cancellationToken);
        return configurations;
    }

    private static bool IsValid(DesktopConfiguration configuration)
    {
        try
        {
            configuration.Normalize();
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }
}
