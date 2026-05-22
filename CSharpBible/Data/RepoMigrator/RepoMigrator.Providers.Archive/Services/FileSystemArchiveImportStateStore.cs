using System.Text.Json;
using RepoMigrator.Providers.Archive.Abstractions;

namespace RepoMigrator.Providers.Archive.Services;

/// <summary>
/// Persists archive import plans and execution state under a deterministic file-system layout.
/// </summary>
public sealed class FileSystemArchiveImportStateStore : IArchiveImportStateStore
{
    private const string RepoMigratorFolderName = "RepoMigrator";
    private const string ArchiveImportsFolderName = "ArchiveImports";
    private const string PlanFileName = "plan.json";
    private const string StateFileName = "state.json";

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly string _storageRootPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileSystemArchiveImportStateStore"/> class.
    /// </summary>
    /// <param name="storageRootPath">The runtime-defined root path for persisted archive import manifests.</param>
    public FileSystemArchiveImportStateStore(string storageRootPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(storageRootPath);
        _storageRootPath = Path.GetFullPath(storageRootPath);
    }

    /// <inheritdoc/>
    public async Task SavePlanAsync(ArchiveImportPlan plan, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(plan);
        await SaveAsync(plan, GetPlanPath(plan.PlanId), ct).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public Task<ArchiveImportPlan> LoadPlanAsync(string planId, CancellationToken ct)
        => LoadAsync<ArchiveImportPlan>(GetPlanPath(planId), ct);

    /// <inheritdoc/>
    public async Task SaveStateAsync(ArchiveImportState state, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(state);
        await SaveAsync(state, GetStatePath(state.PlanId), ct).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public Task<ArchiveImportState> LoadStateAsync(string planId, CancellationToken ct)
        => LoadAsync<ArchiveImportState>(GetStatePath(planId), ct);

    /// <inheritdoc/>
    public string GetPlanDirectoryPath(string planId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(planId);
        return Path.Combine(_storageRootPath, RepoMigratorFolderName, ArchiveImportsFolderName, SanitizePathSegment(planId));
    }

    private string GetPlanPath(string planId)
        => Path.Combine(GetPlanDirectoryPath(planId), PlanFileName);

    private string GetStatePath(string planId)
        => Path.Combine(GetPlanDirectoryPath(planId), StateFileName);

    private static async Task SaveAsync<T>(T value, string filePath, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        await using var stream = File.Create(filePath);
        await JsonSerializer.SerializeAsync(stream, value, SerializerOptions, ct).ConfigureAwait(false);
    }

    private static async Task<T> LoadAsync<T>(string filePath, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Archive import manifest '{filePath}' does not exist.", filePath);

        await using var stream = File.OpenRead(filePath);
        var value = await JsonSerializer.DeserializeAsync<T>(stream, SerializerOptions, ct).ConfigureAwait(false);
        return value ?? throw new InvalidDataException($"Archive import manifest '{filePath}' could not be deserialized.");
    }

    private static string SanitizePathSegment(string value)
    {
        var invalidCharacters = Path.GetInvalidFileNameChars();
        var sanitized = new string(value.Select(ch => invalidCharacters.Contains(ch) ? '-' : ch).ToArray()).Trim();
        return string.IsNullOrWhiteSpace(sanitized) ? "archive-import" : sanitized;
    }
}
