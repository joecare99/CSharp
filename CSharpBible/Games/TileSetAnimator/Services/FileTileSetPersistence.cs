using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using TileSetAnimator.Persistence;

namespace TileSetAnimator.Services;

/// <summary>
/// Persists tile metadata and animations to companion JSON files.
/// </summary>
public sealed class FileTileSetPersistence : ITileSetPersistence
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    /// <inheritdoc />
    public async Task<TileSetState?> LoadStateAsync(string tileSheetPath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(tileSheetPath))
        {
            return null;
        }

        var targetFile = GetStateFilePath(tileSheetPath);
        if (!File.Exists(targetFile))
        {
            return null;
        }

        await using var stream = File.OpenRead(targetFile);
        return await JsonSerializer.DeserializeAsync<TileSetState>(stream, SerializerOptions, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task SaveStateAsync(TileSetState state, CancellationToken cancellationToken = default)
    {
        if (state == null)
        {
            throw new ArgumentNullException(nameof(state));
        }

        var targetFile = GetStateFilePath(state.TileSheetPath);
        Directory.CreateDirectory(Path.GetDirectoryName(targetFile)!);
        await using var stream = File.Open(targetFile, FileMode.Create, FileAccess.Write, FileShare.None);
        await JsonSerializer.SerializeAsync(stream, state, SerializerOptions, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TileSetClassMetadata?> LoadClassAsync(string classKey, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(classKey))
        {
            return null;
        }

        var targetFile = GetClassFilePath(classKey);
        if (!File.Exists(targetFile))
        {
            return null;
        }

        await using var stream = File.OpenRead(targetFile);
        return await JsonSerializer.DeserializeAsync<TileSetClassMetadata>(stream, SerializerOptions, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task SaveClassAsync(TileSetClassMetadata metadata, CancellationToken cancellationToken = default)
    {
        if (metadata == null)
        {
            throw new ArgumentNullException(nameof(metadata));
        }

        var targetFile = GetClassFilePath(metadata.Key);
        Directory.CreateDirectory(Path.GetDirectoryName(targetFile)!);
        await using var stream = File.Open(targetFile, FileMode.Create, FileAccess.Write, FileShare.None);
        await JsonSerializer.SerializeAsync(stream, metadata, SerializerOptions, cancellationToken).ConfigureAwait(false);
    }

    private static string GetStateFilePath(string tileSheetPath)
    {
        var directory = Path.GetDirectoryName(tileSheetPath);
        var fileName = Path.GetFileNameWithoutExtension(tileSheetPath);
        var companionName = fileName + ".tileset.json";
        return Path.Combine(directory ?? Environment.CurrentDirectory, companionName);
    }

    private static string GetClassFilePath(string classKey)
    {
        var baseFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TileSetAnimator", "TileClasses");
        return Path.Combine(baseFolder, classKey + ".tileclass.json");
    }
}
