// Core/VersionManager.cs
using System.Text.Json;

namespace OllamaHarness.Core;

public sealed class VersionManager(string baseDir)
{
    private readonly string _versionsDir = Path.Combine(baseDir, "versions");
    private readonly string _metaFile = Path.Combine(baseDir, "version_meta.json");

    public sealed record VersionMeta(
        int Iteration,
        DateTime Timestamp,
        string[] Files,
        string? Notes);

    public void Initialize()
    {
        Directory.CreateDirectory(_versionsDir);
    }

    public int CurrentIteration => LoadMeta()?.Iteration ?? 0;

    public void SaveVersion(int iteration, Dictionary<string, string> sources, string? notes)
    {
        var dir = Path.Combine(_versionsDir, $"v{iteration:D4}");
        Directory.CreateDirectory(dir);

        foreach (var (file, content) in sources)
        {
            var path = Path.Combine(dir, file.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, content);
        }

        var meta = new VersionMeta(iteration, DateTime.UtcNow, [.. sources.Keys], notes);
        File.WriteAllText(_metaFile, JsonSerializer.Serialize(meta,
            new JsonSerializerOptions { WriteIndented = true }));

        Console.WriteLine($"  ✓ Version {iteration} saved → {dir}");
    }

    public Dictionary<string, string>? LoadVersion(int iteration)
    {
        var dir = Path.Combine(_versionsDir, $"v{iteration:D4}");
        if (!Directory.Exists(dir)) return null;

        return Directory.GetFiles(dir, "*.cs", SearchOption.AllDirectories)
            .ToDictionary(
                f => Path.GetRelativePath(dir, f),
                File.ReadAllText);
    }

    public void Rollback(int targetIteration, string activeSourceDir)
    {
        var sources = LoadVersion(targetIteration)
            ?? throw new InvalidOperationException($"Version {targetIteration} not found.");

        Directory.Delete(activeSourceDir, recursive: true);
        Directory.CreateDirectory(activeSourceDir);

        foreach (var (file, content) in sources)
        {
            var path = Path.Combine(activeSourceDir, file);
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, content);
        }

        Console.WriteLine($"  ↩ Rolled back to v{targetIteration}");
    }

    private VersionMeta? LoadMeta()
    {
        if (!File.Exists(_metaFile)) return null;
        return JsonSerializer.Deserialize<VersionMeta>(File.ReadAllText(_metaFile));
    }
}